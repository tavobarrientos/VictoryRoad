using VictoryRoad.Core.Models;

namespace VictoryRoad.Core.Services;

public interface ITcgLiveParser
{
    Deck ParseDeckList(string deckListText);
}