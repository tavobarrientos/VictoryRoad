using VictoryRoad.Core.Models;

namespace VictoryRoad.Core.Services;

public class PdfService : IPdfService
{
    public async Task<byte[]> FillDeckListPdf(string templatePath, Deck deck)
    {
        return await Task.Run(() =>
        {
            var formBuilder = new LetterFormBuilder();
            return formBuilder.BuildPdf(templatePath, deck);
        });
    }
    
    public async Task SaveDeckListPdf(string templatePath, string outputPath, Deck deck)
    {
        var pdfBytes = await FillDeckListPdf(templatePath, deck);
        await File.WriteAllBytesAsync(outputPath, pdfBytes);
    }
}