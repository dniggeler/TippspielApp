using System;

namespace Data.SqlClient
{
    public class MatchInfoItem
    {
        public string LeagueIdentifier { get; set; }
        public string SeasonIdentifier { get; set; }
        public int Id { get; set; }
        public int MatchId { get; set; }
        public int GroupOrderId { get; set; }
        public int MatchNr { get; set; }
        public DateTime KickoffTime { get; set; }
        public DateTime KickoffTimeUtc { get; set; }
        public bool IsFinished { get; set; }
        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public string HomeTeamIcon { get; set; }
        public string AwayTeamIcon { get; set; }
        public int HomeTeamScore { get; set; }
        public int AwayTeamScore { get; set; }
        public bool IsInProlongation { get; set; }
    }
}