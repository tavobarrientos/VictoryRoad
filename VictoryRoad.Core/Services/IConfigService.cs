using VictoryRoad.Core.Models;

namespace VictoryRoad.Core.Services;

public interface IConfigService
{
    UserConfig LoadUserConfig();
    void SaveUserConfig(UserConfig config);
}
