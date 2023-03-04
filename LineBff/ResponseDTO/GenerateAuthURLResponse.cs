using Newtonsoft.Json;

namespace LineBff.ResponseDTO
{
	public class GenerateAuthURLResponse
	{
        [JsonProperty("auth_url")]
        public string AuthURL { get; set; }
    }
}

