using Newtonsoft.Json;

namespace Tippspiel.Contracts.Models
{
    public class TeamModel
    {
        [JsonProperty("teamId")]
        public int Id { get; set; }

        [JsonProperty("teamName")]
        public string Name { get; set; }

        [JsonProperty("shortName")]
        public string ShortName { get; set; }

        [JsonProperty("teamIconUrl")]
        public string IconUrl { get; set; }
    }
}