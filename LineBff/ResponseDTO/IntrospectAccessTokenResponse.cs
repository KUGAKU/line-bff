using System;
using Newtonsoft.Json;

namespace LineBff.ResponseDTO
{
	public class IntrospectAccessTokenResponse
	{
		[JsonProperty("is_valid")]
        public bool IsValid { get; set; }
	}
}

