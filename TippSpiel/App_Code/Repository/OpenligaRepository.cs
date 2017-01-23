using System;
using System.Linq;
using System.Collections.Generic;
using Tippspiel.Contracts;
using Tippspiel.Contracts.Models;
using TippSpiel.SvcOpenData;

namespace Tippspiel.Implementation
{
    public class OpenligaRepository : IFussballDataRepository, IDisposable
    {
        bool _disposed = false;
        private string _leagueSaison;
        private string _leagueShortcut;
        private readonly SportsdataSoapClient _client;

        public OpenligaRepository(SportsdataConfigInfo info)
            : this(info.LeagueShortcut, info.LeagueSaison, new SportsdataSoapClient())
        {
        }

        public OpenligaRepository(string leagueShortcut, string leagueSaison, SportsdataSoapClient client)
        {
            _leagueShortcut = leagueShortcut;
            _leagueSaison = leagueSaison;

            _client = client;
        }

        public bool IsSpieltagComplete
        {
            get
            {
                var mNext = GetNextMatch();
                var dataLast = GetLastMatch();

                if (mNext == null)
                {
                    return true;
                }
                else if (dataLast == null)
                {
                    return false;
                }
                else
                {
                    if (dataLast.GroupId < mNext.GroupId)
                    {
                        return true;
                    }

                    return false;
                }
            }
        }

        public bool Exist(string leagueShortcut, string leagueSeason)
        {
            var leagues = _client.GetAvailLeagues();

            var count = (from e in leagues
                         where e.leagueShortcut == leagueShortcut &&
                               e.leagueSaison == leagueSeason
                         select e)
                             .Count();

            return count == 1 ? true : false;
        }

        public List<GroupInfoModel> GetAllGroups()
        {
            var groupList = new List<GroupInfoModel>();
            foreach (var g in _client.GetAvailGroups(_leagueShortcut, _leagueSaison))
            {
                groupList.Add(new GroupInfoModel
                {
                    Id = g.groupOrderID,
                    Text = g.groupName
                });
            }

            return groupList;
        }

        public List<MatchDataModel> GetAllMatches()
        {
            var mList = new List<MatchDataModel>();

            foreach (var m in _client.GetMatchdataByLeagueSaison(_leagueShortcut, _leagueSaison))
            {
                mList.Add(Create(m));
            }

            return mList;
        }

        public GroupInfoModel GetCurrentGroup()
        {
            var g = _client.GetCurrentGroup(_leagueShortcut);

            return new GroupInfoModel
            {
                Id = g.groupOrderID,
                Text = g.groupName
            };
        }

        public MatchDataModel GetNextMatch()
        {
            Matchdata m = _client.GetNextMatch(_leagueShortcut);

            return Create(m);
        }

        public MatchDataModel GetLastMatch()
        {
            Matchdata m = _client.GetLastMatch(_leagueShortcut);

            return Create(m);
        }
        public MatchDataModel GetMatchData(int matchId)
        {
            Matchdata m = _client.GetMatchByMatchID(matchId);

            return Create(m);
        }

        public List<MatchDataModel> GetMatchesByCurrentGroup()
        {
            return GetMatchesByGroup(GetCurrentGroup().Id);
        }

        public List<MatchDataModel> GetMatchesByGroup(int groupId)
        {
            Matchdata[] matches = _client.GetMatchdataByGroupLeagueSaison(groupId, _leagueShortcut, _leagueSaison);

            // check if data has been found at all
            if (matches.Count() == 1)
            {
                if (matches.First().matchID == -1)
                {
                    return new List<MatchDataModel>();
                }
            }

            var mList = new List<MatchDataModel>();

            foreach (var m in matches)
            {
                mList.Add(Create(m));
            }

            return mList;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _client.Close();
            }

            // Free any unmanaged objects here. 
            //
            _disposed = true;
        }

        public static MatchDataModel Create(Matchdata match)
        {
            var matchModelObj = new MatchDataModel();
            matchModelObj.MatchId = match.matchID;
            matchModelObj.GroupId = match.groupOrderID;
            matchModelObj.KickoffTime = match.matchDateTime;
            matchModelObj.KickoffTimeUTC = match.matchDateTimeUTC;
            matchModelObj.HomeTeamId = match.idTeam1;
            matchModelObj.AwayTeamId = match.idTeam2;
            matchModelObj.HomeTeamScore = match.pointsTeam1;
            matchModelObj.AwayTeamScore = match.pointsTeam2;
            matchModelObj.HomeTeamIcon = match.iconUrlTeam1;
            matchModelObj.AwayTeamIcon = match.iconUrlTeam2;
            matchModelObj.HomeTeam = match.nameTeam1;
            matchModelObj.AwayTeam = match.nameTeam2;
            matchModelObj.IsFinished = match.matchIsFinished;
            matchModelObj.LeagueShortcut = match.leagueShortcut;

            if (match.matchResults != null && match.matchResults.Count > 0)
            {
                var result = (from r in match.matchResults orderby r.resultTypeId descending select r).FirstOrDefault();

                if (result == null) return matchModelObj;

                matchModelObj.HomeTeamScore = result.pointsTeam1;
                matchModelObj.AwayTeamScore = result.pointsTeam2;
            }

            return matchModelObj;
        }
    }
}