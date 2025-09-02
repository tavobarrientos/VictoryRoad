using ReactiveUI;
using VictoryRoad.Core.Models;

namespace VictoryRoad.UI.ViewModels;

public class CardViewModel : ViewModelBase
{
    private int _quantity;
    private string _name = string.Empty;
    private string _setCode = string.Empty;
    private string _collectorNumber = string.Empty;
    
    public int Quantity
    {
        get => _quantity;
        set => this.RaiseAndSetIfChanged(ref _quantity, value);
    }
    
    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }
    
    public string SetCode
    {
        get => _setCode;
        set => this.RaiseAndSetIfChanged(ref _setCode, value);
    }
    
    public string CollectorNumber
    {
        get => _collectorNumber;
        set => this.RaiseAndSetIfChanged(ref _collectorNumber, value);
    }
    
    public string DisplayName => 
        !string.IsNullOrEmpty(SetCode) && !string.IsNullOrEmpty(CollectorNumber)
            ? $"{Name} {SetCode} {CollectorNumber}"
            : Name;
    
    public CardViewModel()
    {
    }
    
    public CardViewModel(Card card)
    {
        Quantity = card.Quantity;
        Name = card.Name;
        SetCode = card.SetCode;
        CollectorNumber = card.CollectorNumber;
    }
    
    public Card ToCard()
    {
        return new Card
        {
            Quantity = Quantity,
            Name = Name,
            SetCode = SetCode,
            CollectorNumber = CollectorNumber
        };
    }
}