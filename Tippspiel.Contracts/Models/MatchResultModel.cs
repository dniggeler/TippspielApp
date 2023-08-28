using Newtonsoft.Json;

namespace Tippspiel.Contracts.Models
{
    public class MatchResultModel
    {
        [JsonProperty("resultID")]
        public int ResultId { get; set; }

        [JsonProperty("resultName")]
        public string ResultName { get; set; }

        [JsonProperty("pointsTeam1")]
        public int HomeTeamScore { get; set; }

        [JsonProperty("pointsTeam2")]
        public int AwayTeamScore { get; set; }

        [JsonProperty("resultOrderID")]
        public int ResultOrderId { get; set; }

        [JsonProperty("resultTypeID")]
        public int ResultTypeId { get; set; }

        public int? ResultType => HomeTeamScore > AwayTeamScore
            ? 1
            : HomeTeamScore < AwayTeamScore ? 2 : 0;
    }
}
