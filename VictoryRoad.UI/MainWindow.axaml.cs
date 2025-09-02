using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using VictoryRoad.UI.ViewModels;

namespace VictoryRoad.UI;

public partial class MainWindow : Window
{
    private MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext!;
    
    public MainWindow()
    {
        InitializeComponent();
        
        var viewModel = new MainWindowViewModel();
        DataContext = viewModel;
        
        viewModel.ShowSaveFileDialog = ShowSaveFileDialog;
        viewModel.ShowMessageBox = ShowMessageBox;
    }
    
    private void ImportDeck_Click(object? sender, RoutedEventArgs e)
    {
        ViewModel.ImportDeck();
    }
    
    private void AddPokemon_Click(object? sender, RoutedEventArgs e)
    {
        ViewModel.AddPokemon();
    }
    
    private void AddTrainer_Click(object? sender, RoutedEventArgs e)
    {
        ViewModel.AddTrainer();
    }
    
    private void AddEnergy_Click(object? sender, RoutedEventArgs e)
    {
        ViewModel.AddEnergy();
    }
    
    private void RemoveCard_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is CardViewModel card)
        {
            ViewModel.RemoveCard(card);
        }
    }
    
    private void ClearAll_Click(object? sender, RoutedEventArgs e)
    {
        ViewModel.ClearAll();
    }
    
    private void Validate_Click(object? sender, RoutedEventArgs e)
    {
        ViewModel.Validate();
    }
    
    private async void ExportPdf_Click(object? sender, RoutedEventArgs e)
    {
        await ViewModel.ExportPdf();
    }
    
    private async Task<IStorageFile?> ShowSaveFileDialog()
    {
        var options = new FilePickerSaveOptions
        {
            Title = "Save Deck List PDF",
            DefaultExtension = "pdf",
            SuggestedFileName = "deck-list.pdf",
            FileTypeChoices =
            [
                new FilePickerFileType("PDF Files")
                {
                    Patterns = ["*.pdf"]
                }
            ]
        };
        
        return await StorageProvider.SaveFilePickerAsync(options);
    }
    
    private async Task ShowMessageBox(string title, string message)
    {
        var messageBox = new Window
        {
            Title = title,
            Width = 400,
            Height = 200,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new StackPanel
            {
                Margin = new Avalonia.Thickness(20),
                Children =
                {
                    new TextBlock 
                    { 
                        Text = message, 
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                        Margin = new Avalonia.Thickness(0, 0, 0, 20)
                    },
                    new Button 
                    { 
                        Content = "OK",
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                        Width = 100
                    }
                }
            }
        };
        
        var button = (messageBox.Content as StackPanel)?.Children.OfType<Button>().First();
        if (button != null)
        {
            button.Click += (_, _) => messageBox.Close();
        }
        
        await messageBox.ShowDialog(this);
    }
}