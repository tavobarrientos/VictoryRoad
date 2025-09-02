using VictoryRoad.Core.Models;

namespace VictoryRoad.Core.Services;

public interface IDeckValidator
{
    ValidationResult ValidateDeck(Deck deck);
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    
    public string GetSummary()
    {
        if (IsValid)
            return "Deck is valid!";
        
        return string.Join("\n", Errors);
    }
}