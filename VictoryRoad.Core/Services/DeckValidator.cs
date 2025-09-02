using VictoryRoad.Core.Models;

namespace VictoryRoad.Core.Services;

public class DeckValidator : IDeckValidator
{
    private static readonly HashSet<string> BasicEnergyNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Fire Energy", "Water Energy", "Grass Energy", "Lightning Energy",
        "Psychic Energy", "Fighting Energy", "Darkness Energy", "Metal Energy",
        "Fairy Energy", "Dragon Energy", "Colorless Energy"
    };
    
    public ValidationResult ValidateDeck(Deck deck)
    {
        var result = new ValidationResult { IsValid = true };
        
        var totalCards = deck.TotalCards;
        
        if (totalCards != 60)
        {
            result.IsValid = false;
            if (totalCards < 60)
            {
                result.Errors.Add($"Deck has only {totalCards} cards. Need {60 - totalCards} more cards.");
            }
            else
            {
                result.Errors.Add($"Deck has {totalCards} cards. Remove {totalCards - 60} cards.");
            }
        }
        
        if (deck.Pokemon.Count == 0)
        {
            result.Warnings.Add("Deck has no Pokémon cards. This is unusual but allowed.");
        }
        
        var pokemonCount = deck.Pokemon.Sum(c => c.Quantity);
        var trainerCount = deck.Trainers.Sum(c => c.Quantity);
        var energyCount = deck.Energy.Sum(c => c.Quantity);
        
        if (pokemonCount > 0 && pokemonCount < 6)
        {
            result.Warnings.Add($"Deck has only {pokemonCount} Pokémon cards. Most decks have at least 6-12.");
        }
        
        if (energyCount > 20)
        {
            result.Warnings.Add($"Deck has {energyCount} Energy cards. Most competitive decks use 8-15.");
        }
        
        // Validate Pokemon and Trainer cards (4-copy limit applies to all)
        foreach (var card in deck.Pokemon.Concat(deck.Trainers))
        {
            ValidateCardQuantity(card, result, maxCopies: 4);
        }
        
        // Validate Energy cards (different rules for basic vs special energy)
        foreach (var energy in deck.Energy)
        {
            if (energy.Quantity < 1)
            {
                result.IsValid = false;
                result.Errors.Add($"Card '{energy.Name}' has invalid quantity: {energy.Quantity}");
                continue;
            }
            
            bool isBasicEnergy = IsBasicEnergy(energy.Name);
            
            if (!isBasicEnergy && energy.Quantity > 4)
            {
                result.IsValid = false;
                result.Errors.Add($"Special Energy '{energy.Name}' has {energy.Quantity} copies. Maximum allowed is 4.");
            }
            // Basic energy cards have no limit, so we don't check them
        }
        
        return result;
    }
    
    private void ValidateCardQuantity(Card card, ValidationResult result, int maxCopies)
    {
        if (card.Quantity > maxCopies)
        {
            result.IsValid = false;
            result.Errors.Add($"Card '{card.Name}' has {card.Quantity} copies. Maximum allowed is {maxCopies}.");
        }
        
        if (card.Quantity < 1)
        {
            result.IsValid = false;
            result.Errors.Add($"Card '{card.Name}' has invalid quantity: {card.Quantity}");
        }
    }
    
    private bool IsBasicEnergy(string cardName)
    {
        // Check if the card name exactly matches or contains a basic energy name
        return BasicEnergyNames.Any(basicName => 
            cardName.Equals(basicName, StringComparison.OrdinalIgnoreCase) ||
            cardName.Contains(basicName, StringComparison.OrdinalIgnoreCase));
    }
}