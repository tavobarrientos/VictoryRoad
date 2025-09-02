namespace VictoryRoad.Core.Models;

public class DeckListFormData
{
    public string Name { get; set; } = string.Empty;
    public string PlayerName { get; set; } = string.Empty;
    public string PlayerId { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.Now;
    public string Division { get; set; } = string.Empty;
    
    public class DeckListEntry
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public int Quantity { get; set; }
        public string CardName { get; set; } = string.Empty;
    }
    
    public List<DeckListEntry> Entries { get; set; } = [];
}