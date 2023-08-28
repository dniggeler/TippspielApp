using Newtonsoft.Json;

namespace Tippspiel.Contracts.Models
{
    public class GroupInfoModel
    {
        [JsonProperty("groupOrderID")]
        public int Id { get; set; }

        [JsonProperty("groupName")]
        public string Text { get; set; }
    }
}
