using LineBff.DataAccess.Datasource;
using LineBff.ResponseDTO;
using LineBff.RequestDTO;
using LineBff.Utils;
using Newtonsoft.Json;
using Flurl.Http;

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

    public class LineRepository : ILineRepository
    {
        private readonly ICacheDataSource _cacheDataSource;
        private readonly string _lineStateKey = "line_state";
        private readonly string _lineAccessTokenKey = "line_access_token";

        private readonly string _lineTokenApiEndpoint = "https://api.line.me/oauth2/v2.1/token";

        public LineRepository(ICacheDataSource cacheDataSource)
        {
            _cacheDataSource = cacheDataSource;
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
            return _cacheDataSource.GetValue(_lineStateKey);
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

            var response = await _lineTokenApiEndpoint
                .WithHeaders(new { Accept = "application/json" })
                .PostUrlEncodedAsync(parameters) ?? throw new ArgumentNullException();

            if (response.StatusCode != 200)
            {
                throw new SystemException();
            }

            var data = await response.GetStringAsync();
            var body = JsonConvert.DeserializeObject<GenerateAccesstokenResponse>(data) ?? throw new ArgumentNullException();
            return body;
        }
    }
}

