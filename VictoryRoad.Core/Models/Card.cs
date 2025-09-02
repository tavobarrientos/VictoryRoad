namespace VictoryRoad.Core.Models;

public class Card
{
    public int Quantity { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SetCode { get; set; } = string.Empty;
    public string CollectorNumber { get; set; } = string.Empty;
    
    public string GetDisplayName()
    {
        if (!string.IsNullOrEmpty(SetCode) && !string.IsNullOrEmpty(CollectorNumber))
            return $"{Name} {SetCode} {CollectorNumber}";
        return Name;
    }
    
    public Card Clone()
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