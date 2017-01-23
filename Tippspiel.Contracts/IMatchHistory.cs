using System.Collections.Generic;
using Tippspiel.Contracts.Models;

namespace Tippspiel.Contracts
{
    public interface IMatchHistory
    {
        List<GroupInfoModel> GetAllGroups();

        MatchDataModel GetMatchData(int matchId);

        List<MatchDataModel> GetMatchesByGroup(int groupId);
        List<MatchDataModel> GetAllMatches();
    }
}
