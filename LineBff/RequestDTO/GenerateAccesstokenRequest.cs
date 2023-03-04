using Newtonsoft.Json;

namespace LineBff.RequestDTO
{
	public class GenerateAccesstokenRequest
	{
		[JsonProperty("authorization_code")]
		public string AuthorizationCode { get; set; }
		[JsonProperty("state")]
		public string State { get; set; }
	}
}

