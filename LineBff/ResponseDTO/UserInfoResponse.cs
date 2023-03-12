using Newtonsoft.Json;

namespace LineBff.ResponseDTO
{
    public class UserInfoResponse
    {
        [JsonProperty("sub")]
        public string sub { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("picture")]
        public string Picture { get; set; }
    }
}