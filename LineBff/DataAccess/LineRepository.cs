using LineBff.DataAccess.Datasource;
using LineBff.ResponseDTO;
using LineBff.RequestDTO;
using LineBff.Wrappers;
using LineBff.Utils;
using Newtonsoft.Json;

namespace LineBff.DataAccess
{
	public interface ILineRepository
	{
		bool AddLineState(string state);
		bool AddLineAccessToken(GenerateAccesstokenResponse dto);
		string GetLineState();
		GenerateAccesstokenResponse GetLineAccessToken();
		Task<GenerateAccesstokenResponse> GenerateAccesstoken(GenerateAccesstokenRequest generateAccesstokenRequest);
    }

	public class LineRepository: ILineRepository
	{
		private readonly ICacheDataSource _cacheDataSource;
		private readonly IHttpClientWrapper _apiDataSource;
		private readonly string _lineStateKey = "line_state";
		private readonly string _lineAccessTokenKey = "line_access_token";

        public LineRepository(ICacheDataSource cacheDataSource, IHttpClientWrapper apiDataSource)
		{
			_cacheDataSource = cacheDataSource;
			_apiDataSource = apiDataSource;
		}

        public bool AddLineState(string state)
        {
			return _cacheDataSource.SetStringValue(_lineStateKey, state, 30); //30分でstateの検証はできなくなる
        }

        public bool AddLineAccessToken(GenerateAccesstokenResponse dto)
        {
			var json = JsonConvert.SerializeObject(dto);
            return _cacheDataSource.SetStringValue(_lineAccessTokenKey, json, 10080); //1週間でアクセストークンがなくなる
        }

        public string GetLineState()
        {
			var value = _cacheDataSource.GetValue(_lineStateKey);
			return value.ToString();
        }

        public GenerateAccesstokenResponse GetLineAccessToken()
        {
			var value = _cacheDataSource.GetValue(_lineAccessTokenKey);
			var response = JsonConvert.DeserializeObject<GenerateAccesstokenResponse>(value.ToString()) ?? throw new SystemException();
            return response;
        }

        public async Task<GenerateAccesstokenResponse> GenerateAccesstoken(GenerateAccesstokenRequest generateAccesstokenRequest)
        {
			var parameters = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", generateAccesstokenRequest.AuthorizationCode },
                { "redirect_uri", EnvVarUtil.GetEnvVarByKeyStr("LINE_CALLBACK_URL")},
                { "client_id", EnvVarUtil.GetEnvVarByKeyStr("LINE_CLIENT_ID")},
                { "client_secret", EnvVarUtil.GetEnvVarByKeyStr("LINE_CLIENT_SECRET")},
                //{ "code_verifier", "" } //TODO: PKCE対応する 
            };

            var content = new FormUrlEncodedContent(parameters);
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.line.me/oauth2/v2.1/token")
            {
                Content = content
            };

            var response = await _apiDataSource.SendAsync(request) ?? throw new ArgumentNullException();

            if (response.StatusCode != System.Net.HttpStatusCode.OK) {
                throw new SystemException();
            }

			var data = await response.Content.ReadAsStringAsync();
            var body = JsonConvert.DeserializeObject<GenerateAccesstokenResponse>(data) ?? throw new ArgumentNullException();
			return body;
        }
    }
}

