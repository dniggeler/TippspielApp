using System.Collections.Generic;
using Tippspiel.Contracts.Models;

namespace Tippspiel.Contracts
{
    public interface IFussballDataRepository
    {
        bool IsSpieltagComplete { get; }

        bool Exist(string leagueShortcut, string leagueSeason);

        GroupInfoModel GetCurrentGroup();
        List<GroupInfoModel> GetAllGroups();

        MatchDataModel GetNextMatch();
        MatchDataModel GetLastMatch();

        MatchDataModel GetMatchData(int matchId);

        List<MatchDataModel> GetMatchesByCurrentGroup();
        List<MatchDataModel> GetMatchesByGroup(int groupId);
        List<MatchDataModel> GetAllMatches();
    }
}
