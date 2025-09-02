using VictoryRoad.Core.Models;

namespace VictoryRoad.Core.Services;

public interface IPdfService
{
    Task<byte[]> FillDeckListPdf(string templatePath, Deck deck);
    Task SaveDeckListPdf(string templatePath, string outputPath, Deck deck);
}