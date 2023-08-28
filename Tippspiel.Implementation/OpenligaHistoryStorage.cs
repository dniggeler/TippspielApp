using Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using Tippspiel.Contracts;
using Tippspiel.Contracts.Models;

namespace Tippspiel.Implementation
{
    public class OpenligaHistoryStorage : IMatchHistory
    {
        private string _seasonIdentifier;
        private string _leagueIdentifier;

        public OpenligaHistoryStorage(SportsdataConfigInfo info)
            : this(info.LeagueShortcut, info.LeagueSaison)
        {
        }

        public OpenligaHistoryStorage(string leagueShortcut, string leagueSaison)
        {
            _leagueIdentifier = leagueShortcut;
            _seasonIdentifier = leagueSaison;
        }

        public List<GroupInfoModel> GetAllGroups()
        {
            using (var ctxt = new MatchInfoContext())
            {
                return ctxt
                    .Items
                    .Where(r => r.LeagueIdentifier == _leagueIdentifier &&
                                r.SeasonIdentifier == _seasonIdentifier)
                    .Select(r => new GroupInfoModel
                    {
                        Id = r.GroupOrderId,
                        Text = r.GroupOrderId.ToString()
                    })
                    .ToList();
            }
        }

        public List<MatchDataModel> GetAllMatches()
        {
            using (var ctxt = new MatchInfoContext())
            {
                return ctxt
                    .Items
                    .Where(r => r.LeagueIdentifier == _leagueIdentifier &&
                            r.SeasonIdentifier == _seasonIdentifier)
                    .Select(Create)
                    .ToList();
            }
        }

        public MatchDataModel GetMatchData(int matchId)
        {
            using (var ctxt = new MatchInfoContext())
            {
                return ctxt
                    .Items
                    .Where(r => r.MatchId == matchId &&
                            r.LeagueIdentifier == _leagueIdentifier &&
                            r.SeasonIdentifier == _seasonIdentifier)
                    .Select(Create)
                    .SingleOrDefault();
            }
        }

        public List<MatchDataModel> GetMatchesByGroup(int groupOrderId)
        {
            using (var ctxt = new MatchInfoContext())
            {
                return ctxt
                    .Items
                    .Where(r => r.GroupOrderId == groupOrderId &&
                            r.LeagueIdentifier == _leagueIdentifier &&
                            r.SeasonIdentifier == _seasonIdentifier)
                    .Select(Create)
                    .ToList();
            }
        }

        public static MatchDataModel Create(MatchInfoItem match)
        {
            var matchModelObj = new MatchDataModel();
            matchModelObj.MatchId = match.MatchId;
            matchModelObj.Group = new GroupInfoModel
            {
                Id = match.GroupOrderId,
            };
            matchModelObj.KickoffTime = match.KickoffTime;
            matchModelObj.KickoffTimeUTC = match.KickoffTimeUtc;
            matchModelObj.MatchResults = new[]
            {
                new MatchResultModel
                {
                    ResultOrderId = 1,
                    ResultTypeId = 2,
                    HomeTeamScore = match.HomeTeamScore,
                    AwayTeamScore = match.AwayTeamScore,
                    ResultName = "Endergebnis"
                }
            };
            matchModelObj.HomeTeam = new TeamModel
                { ShortName = match.HomeTeam, IconUrl = match.HomeTeamIcon, Id = match.HomeTeamId };
            matchModelObj.AwayTeam = new TeamModel
                { ShortName = match.AwayTeam, IconUrl = match.AwayTeamIcon, Id = match.AwayTeamId };
            matchModelObj.IsFinished = match.IsFinished;
            matchModelObj.LeagueShortcut = match.LeagueIdentifier;

            return matchModelObj;
        }
    }
}