namespace VictoryRoad.Core.Services;

public class LetterFormBuilder : PdfFormBuilder
{
    // Player information coordinates (adjusted for iTextSharp coordinate system)
    protected override float PlayerNameX => 92f;
    protected override float PlayerNameY => 714f;
    
    protected override float PlayerIdX => 279f;
    protected override float PlayerIdY => 714f;
    
    // Date Coordinates
    protected override float DateX => 494f;
    protected override float DateY => 714f;
    protected override float DateDayX => 522f;
    protected override float DateMonthX => 494f;
    protected override float DateYearX => 549f;
    protected override float DivisionX => 376f;
    protected override float JuniorDivisionY => 676f;
    protected override float SeniorDivisionY => 662f;
    protected override float MasterDivisionY => 649f;
    
    // Format coordinates
    protected override float FormatY => 730f;
    protected override float StandardFormatX => 157f;
    protected override float ExpandedFormatX => 207f;
    
    // Card section starting Y coordinates (measured from bottom of page)
    // Pokemon section starts near the top
    protected override float PokemonStartY => 587f;
    
    // Trainer section in the middle
    protected override float TrainerStartY => 410f;
    
    // Energy section at the bottom
    protected override float EnergyStartY => 129f;
    
    // Column X positions for card lists
    // Left column - POKEMON needs more space to the right
    protected override float LeftQuantityX => 276f;
    protected override float LeftCardNameX => 299f;  // Pushed WAY right from 105f to avoid Pokemon symbol overlap
    
    // Right column - also pushed right
    protected override float RightQuantityX => 430f;
    protected override float RightCardNameX => 445f;  // Pushed WAY right from 375f for Pokemon
    
    // Row spacing between cards
    protected override float RowHeight => 16.5f;
}