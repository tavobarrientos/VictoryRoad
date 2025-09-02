namespace VictoryRoad.Core.Models;

public class Deck
{
    public string Name { get; set; } = string.Empty;
    public string PlayerName { get; set; } = string.Empty;
    public string PlayerId { get; set; } = string.Empty;
    public DateTime? Date { get; set; } = null;
    public string Division { get; set; } = string.Empty;
    public string Format { get; set; } = "Standard";
    
    public List<Card> Pokemon { get; set; } = [];
    public List<Card> Trainers { get; set; } = [];
    public List<Card> Energy { get; set; } = [];
    
    public int TotalCards => 
        Pokemon.Sum(c => c.Quantity) + 
        Trainers.Sum(c => c.Quantity) + 
        Energy.Sum(c => c.Quantity);
    
    public bool IsValid => TotalCards == 60;
    
    public string GetValidationMessage()
    {
        if (TotalCards < 60)
            return $"Deck has only {TotalCards} cards. Need {60 - TotalCards} more cards.";
        if (TotalCards > 60)
            return $"Deck has {TotalCards} cards. Remove {TotalCards - 60} cards.";
        return "Deck is valid with exactly 60 cards.";
    }
}