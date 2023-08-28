using System;
using System.Linq;
using Newtonsoft.Json;

namespace Tippspiel.Contracts.Models
{
    public class MatchDataModel
    {
        [JsonProperty("matchID")]
        public int MatchId { get; set; }

        [JsonProperty("group")]
        public GroupInfoModel Group { get; set; }

        public int MatchNr { get; set; }

        [JsonProperty("leagueShortcut")]
        public string LeagueShortcut { get; set; }

        [JsonProperty("matchDateTime")]
        public DateTime KickoffTime { get; set; }

        [JsonProperty("matchDateTimeUTC")]
        public DateTime KickoffTimeUTC { get; set; }

        [JsonProperty("matchIsFinished")]
        public bool IsFinished { get; set; }

        [JsonProperty("team1")]
        public TeamModel HomeTeam { get; set; }

        [JsonProperty("team2")]
        public TeamModel AwayTeam { get; set; }

        [JsonProperty("matchResults")]
        public MatchResultModel[] MatchResults { get; set; }

        public int HomeTeamScore => CurrentScore().homeTeamScore;
        public int AwayTeamScore => CurrentScore().awayTeamScore;
        public bool HasVerlaengerung { get; set; }

        public int? ResultType
        {
            get
            {
                if (!HasStarted) return null;

                return CurrentResult()?.ResultType;
            }
        }

        public bool HasStarted => !(KickoffTime > DateTime.Now);

        public (int homeTeamScore, int awayTeamScore) CurrentScore()
        {
            MatchResultModel currentResult = CurrentResult();

            if (currentResult == null)
            {
                return (0, 0);
            }

            return (currentResult.HomeTeamScore, currentResult.AwayTeamScore);
        }

        private MatchResultModel CurrentResult()
        {
            // get the result with the highest resultOrderID
            if (MatchResults.Length == 0 && !HasStarted)
            {
                return null;
            }

            if (MatchResults.Length == 0 && HasStarted)
            {
                return new MatchResultModel();
            }

            return MatchResults.OrderByDescending(r => r.ResultOrderId)
                .First();
        }
    }
}
