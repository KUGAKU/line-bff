using System;
using Newtonsoft.Json;

namespace LineBff.Entity
{
	public class IntrospectAccessTokenEntity
	{
        [JsonProperty("scope")]
        public string Scope { get; set; }
        [JsonProperty("client_id")]
        public string ClientId { get; set; }
        [JsonProperty("expires_in")]
        public int expiresIn { get; set; }
    }
}

