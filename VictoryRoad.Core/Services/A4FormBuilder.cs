namespace VictoryRoad.Core.Services;

// Preserved for future use - currently the app defaults to Letter format
public class A4FormBuilder : PdfFormBuilder
{
    // Player information coordinates (adjusted for iTextSharp coordinate system)
    protected override float PlayerNameX => 120f;
    protected override float PlayerNameY => 715f;
    
    protected override float PlayerIdX => 415f;
    protected override float PlayerIdY => 715f;
    
    // Date Coordinates (A4 format)
    protected override float DateX => 490f;
    protected override float DateY => 695f;
    protected override float DateDayX => 518f;
    protected override float DateMonthX => 490f;
    protected override float DateYearX => 545f;
    
    protected override float DivisionX => 415f;
    protected override float JuniorDivisionY => 695f;
    protected override float SeniorDivisionY => 680f;
    protected override float MasterDivisionY => 665f;
    
    // Format coordinates
    protected override float FormatY => 650f;
    protected override float StandardFormatX => 415f;
    protected override float ExpandedFormatX => 480f;
    
    // Card section starting Y coordinates (A4 is taller than Letter)
    // Pokemon section starts near the top
    protected override float PokemonStartY => 640f;
    
    // Trainer section in the middle
    protected override float TrainerStartY => 445f;
    
    // Energy section at the bottom
    protected override float EnergyStartY => 250f;
    
    // Column X positions for card lists
    // Left column - shifted right to avoid expansion symbols
    protected override float LeftQuantityX => 50f;
    protected override float LeftCardNameX => 105f;  // Moved right to avoid symbol column
    
    // Right column - also shifted right
    protected override float RightQuantityX => 320f;
    protected override float RightCardNameX => 375f;  // Moved right to avoid symbol column
    
    // Row spacing between cards
    protected override float RowHeight => 16.5f;
}