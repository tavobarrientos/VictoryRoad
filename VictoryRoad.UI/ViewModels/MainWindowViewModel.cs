using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using VictoryRoad.Core.Models;
using VictoryRoad.Core.Services;

namespace VictoryRoad.UI.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly ITcgLiveParser _parser;
    private readonly IPdfService _pdfService;
    private readonly IDeckValidator _validator;
    
    private string _playerName = string.Empty;
    private string _playerId = string.Empty;
    private DateTime? _date = null;
    private string _division = string.Empty;
    private string _format = "Standard";
    private string _importText = string.Empty;
    private string _validationMessage = string.Empty;
    private bool _isValid;
    private int _totalCardCount;
    
    
    public string PlayerName
    {
        get => _playerName;
        set => SetProperty(ref _playerName, value);
    }
    
    public string PlayerId
    {
        get => _playerId;
        set => SetProperty(ref _playerId, value);
    }
    
    public DateTime? Date
    {
        get => _date;
        set => SetProperty(ref _date, value);
    }
    
    public string Division
    {
        get => _division;
        set => SetProperty(ref _division, value);
    }
    
    public string Format
    {
        get => _format;
        set => SetProperty(ref _format, value);
    }
    
    public string ImportText
    {
        get => _importText;
        set => SetProperty(ref _importText, value);
    }
    
    public string ValidationMessage
    {
        get => _validationMessage;
        set => SetProperty(ref _validationMessage, value);
    }
    
    public bool IsValid
    {
        get => _isValid;
        set => SetProperty(ref _isValid, value);
    }
    
    public int TotalCardCount
    {
        get => _totalCardCount;
        private set => SetProperty(ref _totalCardCount, value);
    }
    
    public ObservableCollection<CardViewModel> PokemonCards { get; }
    public ObservableCollection<CardViewModel> TrainerCards { get; }
    public ObservableCollection<CardViewModel> EnergyCards { get; }
    
    public Func<Task<IStorageFile?>>? ShowSaveFileDialog { get; set; }
    public Func<string, string, Task>? ShowMessageBox { get; set; }
    
    public MainWindowViewModel()
    {
        _parser = new TcgLiveParser();
        _pdfService = new PdfService();
        _validator = new DeckValidator();
        
        PokemonCards = [];
        TrainerCards = [];
        EnergyCards = [];
        
        PokemonCards.CollectionChanged += (_, _) => UpdateCardCount();
        TrainerCards.CollectionChanged += (_, _) => UpdateCardCount();
        EnergyCards.CollectionChanged += (_, _) => UpdateCardCount();
    }
    
    public void ImportDeck()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ImportText))
                return;
            
            var deck = _parser.ParseDeckList(ImportText);
            
            // Only import the cards, preserve user-entered player information
            PokemonCards.Clear();
            foreach (var card in deck.Pokemon)
                PokemonCards.Add(new CardViewModel(card));
            
            TrainerCards.Clear();
            foreach (var card in deck.Trainers)
                TrainerCards.Add(new CardViewModel(card));
            
            EnergyCards.Clear();
            foreach (var card in deck.Energy)
                EnergyCards.Add(new CardViewModel(card));
            
            ImportText = string.Empty;
        }
        catch (Exception ex)
        {
            ValidationMessage = $"Error importing deck: {ex.Message}";
            IsValid = false;
        }
    }
    
    public void AddPokemon()
    {
        PokemonCards.Add(new CardViewModel 
        { 
            Quantity = 1, 
            Name = "New Pokemon" 
        });
    }
    
    public void AddTrainer()
    {
        TrainerCards.Add(new CardViewModel 
        { 
            Quantity = 1, 
            Name = "New Trainer" 
        });
    }
    
    public void AddEnergy()
    {
        EnergyCards.Add(new CardViewModel 
        { 
            Quantity = 1, 
            Name = "New Energy" 
        });
    }
    
    public void RemoveCard(CardViewModel card)
    {
        PokemonCards.Remove(card);
        TrainerCards.Remove(card);
        EnergyCards.Remove(card);
    }
    
    public void ClearAll()
    {
        PokemonCards.Clear();
        TrainerCards.Clear();
        EnergyCards.Clear();
        PlayerName = string.Empty;
        PlayerId = string.Empty;
        Division = string.Empty;
        Format = "Standard";
        Date = null;
        ValidationMessage = string.Empty;
        IsValid = false;
    }
    
    public void Validate()
    {
        try
        {
            var deck = BuildDeck();
            var result = _validator.ValidateDeck(deck);
            
            IsValid = result.IsValid;
            ValidationMessage = result.GetSummary();
            
            if (result.Warnings.Any())
            {
                ValidationMessage += "\n\nWarnings:\n" + string.Join("\n", result.Warnings);
            }
        }
        catch (Exception ex)
        {
            ValidationMessage = $"Validation error: {ex.Message}";
            IsValid = false;
        }
    }
    
    private void UpdateCardCount()
    {
        try
        {
            TotalCardCount = 
                PokemonCards.Sum(c => c.Quantity) + 
                TrainerCards.Sum(c => c.Quantity) + 
                EnergyCards.Sum(c => c.Quantity);
            
            Validate();
        }
        catch (Exception ex)
        {
            ValidationMessage = $"Error updating card count: {ex.Message}";
            IsValid = false;
        }
    }
    
    public async Task ExportPdf()
    {
        IStorageFile? file = null;
        if (ShowSaveFileDialog != null)
            file = await ShowSaveFileDialog();
        
        if (file == null)
            return;
        
        try
        {
            var deck = BuildDeck();
            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "play-pokemon-deck-list-85x11.pdf");
            
            if (!File.Exists(templatePath))
            {
                templatePath = Path.Combine(Directory.GetCurrentDirectory(), "play-pokemon-deck-list-85x11.pdf");
            }
            
            if (!File.Exists(templatePath))
            {
                if (ShowMessageBox != null)
                    await ShowMessageBox("Error", "Template PDF not found. Please ensure play-pokemon-deck-list-85x11.pdf is in the application directory.");
                return;
            }
            
            await _pdfService.SaveDeckListPdf(templatePath, file.Path.LocalPath, deck);
            
            if (ShowMessageBox != null)
                await ShowMessageBox("Success", $"Deck list exported to {file.Name}");
        }
        catch (Exception ex)
        {
            if (ShowMessageBox != null)
                await ShowMessageBox("Error", $"Failed to export PDF: {ex.Message}");
        }
    }
    
    private Deck BuildDeck()
    {
        return new Deck
        {
            Name = string.Empty,
            PlayerName = PlayerName,
            PlayerId = PlayerId,
            Date = Date,
            Division = Division,
            Format = Format,
            Pokemon = PokemonCards.Select(vm => vm.ToCard()).ToList(),
            Trainers = TrainerCards.Select(vm => vm.ToCard()).ToList(),
            Energy = EnergyCards.Select(vm => vm.ToCard()).ToList()
        };
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}