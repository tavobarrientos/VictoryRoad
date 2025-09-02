using System.Text.RegularExpressions;
using VictoryRoad.Core.Models;

namespace VictoryRoad.Core.Services;

public class TcgLiveParser : ITcgLiveParser
{
    private readonly Regex _cardLineRegex = new(@"^\s*(\d+)\s+(.+?)\s+([A-Z]{2,})\s+(\d+)\s*$");
    // Matches section headers like "Pokémon: 16" or "Trainer: 33"
    private readonly Regex _sectionHeaderRegex = new(@"^\s*(Pokémon|Trainer|Energy):\s*(\d+)\s*$");
    
    public Deck ParseDeckList(string deckListText)
    {
        var deck = new Deck();
        var lines = deckListText.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);
        
        var currentSection = "";
        
        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            
            if (string.IsNullOrWhiteSpace(trimmedLine))
                continue;
            
            // Check if it's a section header
            var sectionMatch = _sectionHeaderRegex.Match(trimmedLine);
            if (sectionMatch.Success)
            {
                currentSection = sectionMatch.Groups[1].Value;
                continue;
            }
            
            // Try to parse as a card line with set code and collector number
            var cardMatch = _cardLineRegex.Match(trimmedLine);
            if (cardMatch.Success)
            {
                var card = new Card
                {
                    Quantity = int.Parse(cardMatch.Groups[1].Value),
                    Name = cardMatch.Groups[2].Value.Trim(),
                    SetCode = cardMatch.Groups[3].Value,
                    CollectorNumber = cardMatch.Groups[4].Value
                };
                
                AddCardToSection(deck, card, currentSection);
            }
            else
            {
                var simpleParse = ParseSimpleLine(trimmedLine);
                if (simpleParse != null)
                {
                    AddCardToSection(deck, simpleParse, currentSection);
                }
            }
        }
        
        return deck;
    }
    
    private void AddCardToSection(Deck deck, Card card, string section)
    {
        switch (section)
        {
            case "Pokémon":
                deck.Pokemon.Add(card);
                break;
            case "Trainer":
                deck.Trainers.Add(card);
                break;
            case "Energy":
                deck.Energy.Add(card);
                break;
        }
    }
    
    private Card? ParseSimpleLine(string line)
    {
        // Try to match lines like "4 Card Name" or "4 Card Name SET 123"
        var match = Regex.Match(line, @"^\s*(\d+)\s+(.+)$");
        if (match.Success)
        {
            var quantity = int.Parse(match.Groups[1].Value);
            var rest = match.Groups[2].Value.Trim();
            
            // Try to extract set code and collector number from the end
            var lastSpaceIndex = rest.LastIndexOf(' ');
            if (lastSpaceIndex > 0)
            {
                var possibleNumber = rest[(lastSpaceIndex + 1)..];
                if (int.TryParse(possibleNumber, out var collectorNumber))
                {
                    var beforeNumber = rest[..lastSpaceIndex].Trim();
                    var secondLastSpace = beforeNumber.LastIndexOf(' ');
                    
                    if (secondLastSpace > 0)
                    {
                        var possibleSetCode = beforeNumber[(secondLastSpace + 1)..];
                        // Check if it looks like a set code (2+ uppercase letters, optionally with numbers)
                        if (Regex.IsMatch(possibleSetCode, @"^[A-Z]{2,}[A-Z0-9]*$"))
                        {
                            return new Card
                            {
                                Quantity = quantity,
                                Name = beforeNumber[..secondLastSpace].Trim(),
                                SetCode = possibleSetCode,
                                CollectorNumber = possibleNumber
                            };
                        }
                    }
                }
            }
            
            // If we couldn't parse set code and collector number, just use the whole name
            return new Card
            {
                Quantity = quantity,
                Name = rest,
                SetCode = "",
                CollectorNumber = ""
            };
        }
        
        return null;
    }
}