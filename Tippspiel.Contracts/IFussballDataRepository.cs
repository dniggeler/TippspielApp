using System.Collections.Generic;
using System.Threading.Tasks;
using Tippspiel.Contracts.Models;

namespace Tippspiel.Contracts
{
    public interface IFussballDataRepository
    {
        Task<bool> IsSpieltagCompleteAsync(bool disableCache = false);

        Task<List<TeamModel>> GetTeamsAsync(bool disableCache = false);

        Task<GroupInfoModel> GetCurrentGroupAsync(bool disableCache = false);

        Task<List<GroupInfoModel>> GetAllGroupsAsync(bool disableCache = false);

        Task<MatchDataModel> GetNextMatchAsync(bool disableCache = false);

        Task<MatchDataModel> GetLastMatchAsync(bool disableCache = false);

        Task<MatchDataModel> GetMatchDataAsync(int matchId, bool disableCache = false);

        Task<List<MatchDataModel>> GetMatchesByCurrentGroupAsync(bool disableCache = false);

        Task<List<MatchDataModel>> GetMatchesByGroupAsync(int groupId, bool disableCache = false);

        Task<List<MatchDataModel>> GetAllMatchesAsync(bool disableCache = false);
    }
}
