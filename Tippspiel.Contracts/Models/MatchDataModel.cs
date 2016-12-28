using System;

namespace Tippspiel.Contracts.Models
{
    public class MatchDataModel
    {
        public int MatchId { get; set; }
        public int GroupId { get; set; }
        public int MatchNr { get; set; }
        public string LeagueShortcut { get; set; }
        public DateTime KickoffTime { get; set; }
        public DateTime KickoffTimeUTC { get; set; }
        public bool IsFinished { get; set; }
        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public string HomeTeamIcon { get; set; }
        public string AwayTeamIcon { get; set; }
        public int HomeTeamScore { get; set; }
        public int AwayTeamScore { get; set; }
        public bool HasVerlaengerung { get; set; }

        public int? ResultType
        {
            get
            {
                if (HasStarted == true)
                {
                    return (HomeTeamScore > AwayTeamScore) ?
                                                    1 : (HomeTeamScore < AwayTeamScore) ?
                                                    2 : 0;
                }
                else
                {
                    return null;
                }
            }
        }

        public bool HasStarted
        {
            get
            {
                return !(KickoffTime > DateTime.Now);
            }
        }
    }
}
