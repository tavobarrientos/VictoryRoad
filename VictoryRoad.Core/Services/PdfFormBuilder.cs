using iTextSharp.text.pdf;
using VictoryRoad.Core.Models;

namespace VictoryRoad.Core.Services;

public abstract class PdfFormBuilder
{
    // Player information coordinates
    protected abstract float PlayerNameX { get; }
    protected abstract float PlayerNameY { get; }
    protected abstract float PlayerIdX { get; }
    protected abstract float PlayerIdY { get; }
    protected abstract float DateX { get; }
    protected abstract float DateY { get; }
    protected abstract float DateDayX { get; }
    protected abstract float DateMonthX { get; }
    protected abstract float DateYearX { get; }
    protected abstract float DivisionX { get; }
    protected abstract float JuniorDivisionY { get; }
    protected abstract float SeniorDivisionY { get; }
    protected abstract float MasterDivisionY { get; }
    
    // Format coordinates
    protected abstract float FormatY { get; }
    protected abstract float StandardFormatX { get; }
    protected abstract float ExpandedFormatX { get; }
    
    // Card section starting Y coordinates
    protected abstract float PokemonStartY { get; }
    protected abstract float TrainerStartY { get; }
    protected abstract float EnergyStartY { get; }
    
    // Column X positions for quantity and card names
    protected abstract float LeftQuantityX { get; }
    protected abstract float LeftCardNameX { get; }
    protected abstract float RightQuantityX { get; }
    protected abstract float RightCardNameX { get; }
    
    // Row spacing
    protected abstract float RowHeight { get; }
    protected virtual int MaxCardsPerColumn => 10;
    
    protected virtual float GetCorrectDivisionYCoordinate(string division)
    {
        return division?.ToLower() switch
        {
            "junior" => JuniorDivisionY,
            "senior" => SeniorDivisionY,
            "masters" => MasterDivisionY,
            _ => JuniorDivisionY // Default to junior if division is null or unknown
        };
    }
    
    protected virtual float GetCorrectFormatXCoordinate(string format)
    {
        return format?.ToLower() switch
        {
            "standard" => StandardFormatX,
            "expanded" => ExpandedFormatX,
            _ => StandardFormatX // Default to standard if format is null or unknown
        };
    }
    
    public byte[] BuildPdf(string templatePath, Deck deck)
    {
        using var reader = new PdfReader(templatePath);
        using var output = new MemoryStream();
        using var stamper = new PdfStamper(reader, output);
        
        var canvas = stamper.GetOverContent(1);
        var baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
        
        canvas.BeginText();
        canvas.SetFontAndSize(baseFont, 9); // Slightly larger font for readability
        
        // Fill player information
        FillPlayerInfo(canvas, deck);
        
        // Fill card sections
        FillCardSection(canvas, deck.Pokemon, PokemonStartY, false);
        FillCardSection(canvas, deck.Trainers, TrainerStartY, true); // true = trainer cards (name only)
        FillCardSection(canvas, deck.Energy, EnergyStartY, false);
        
        canvas.EndText();
        
        stamper.Close();
        reader.Close();
        
        return output.ToArray();
    }
    
    private void FillPlayerInfo(PdfContentByte canvas, Deck deck)
    {
        if (!string.IsNullOrEmpty(deck.PlayerName))
        {
            canvas.ShowTextAligned(PdfContentByte.ALIGN_LEFT, deck.PlayerName, PlayerNameX, PlayerNameY, 0);
        }
        
        if (!string.IsNullOrEmpty(deck.PlayerId))
        {
            canvas.ShowTextAligned(PdfContentByte.ALIGN_LEFT, deck.PlayerId, PlayerIdX, PlayerIdY, 0);
        }
        
        // Fill date as separate Month, Day, Year fields only if date is set
        if (deck.Date.HasValue)
        {
            var month = deck.Date.Value.ToString("MM");
            var day = deck.Date.Value.ToString("dd");
            var year = deck.Date.Value.ToString("yyyy");
            
            canvas.ShowTextAligned(PdfContentByte.ALIGN_LEFT, month, DateMonthX, DateY, 0);
            canvas.ShowTextAligned(PdfContentByte.ALIGN_LEFT, day, DateDayX, DateY, 0);
            canvas.ShowTextAligned(PdfContentByte.ALIGN_LEFT, year, DateYearX, DateY, 0);
        }
        
        if (!string.IsNullOrEmpty(deck.Division))
        {
            // Place 'x' mark for the selected division
            var coordinateY = GetCorrectDivisionYCoordinate(deck.Division);
            canvas.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "x", DivisionX, coordinateY, 0);
        }
        
        if (!string.IsNullOrEmpty(deck.Format))
        {
            // Place 'x' mark for the selected format
            var coordinateX = GetCorrectFormatXCoordinate(deck.Format);
            canvas.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "x", coordinateX, FormatY, 0);
        }
    }
    
    private void FillCardSection(PdfContentByte canvas, List<Card> cards, float startY, bool nameOnly = false)
    {
        int cardIndex = 0;
        
        foreach (var card in cards)
        {
            if (cardIndex >= MaxCardsPerColumn * 2) // Max 20 cards per section (10 per column)
                break;
            
            bool isRightColumn = cardIndex >= MaxCardsPerColumn;
            int rowInColumn = cardIndex % MaxCardsPerColumn;
            
            float quantityX = isRightColumn ? RightQuantityX : LeftQuantityX;
            float cardNameX = isRightColumn ? RightCardNameX : LeftCardNameX;
            float y = startY - (rowInColumn * RowHeight);
            
            // Draw quantity (centered in the quantity column)
            canvas.ShowTextAligned(PdfContentByte.ALIGN_CENTER, card.Quantity.ToString(), quantityX, y, 0);
            
            // Draw card name (left aligned)
            var displayName = nameOnly ? card.Name : card.GetDisplayName();
            if (displayName.Length > 32)
            {
                displayName = displayName.Substring(0, 29) + "...";
            }
            canvas.ShowTextAligned(PdfContentByte.ALIGN_LEFT, displayName, cardNameX, y, 0);
            
            cardIndex++;
        }
    }
}