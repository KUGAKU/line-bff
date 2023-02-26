using System.Text.Json;
using LineBff.DataAccess;
using LineBff.RequestDTO;
using LineBff.ResponseDTO;
using LineBff.Utils;

namespace LineBff.BusinessLogic
{
    public interface ILineService
    {
        GenerateAuthURLResponse GenerateAuthURL();
        Task<GenerateAccesstokenResponse> GenerateAccesstoken(GenerateAccesstokenRequest generateAccesstokenRequest);

    }

	public class LineService: ILineService
    {

        private readonly HttpClient _httpClient;
        private readonly ILineRepository _lineRepository;

        public LineService(IHttpClientFactory httpClientFactory, ILineRepository lineRepository)
        {
            _httpClient = httpClientFactory.CreateClient();
            _lineRepository = lineRepository;
        }

        public GenerateAuthURLResponse GenerateAuthURL()
		{
            var responseType = "?response_type=code";
            var clientId = $"&client_id={EnvVarUtil.GetEnvVarByKeyStr("LINE_CLIENT_ID")}";
            var callbackURL = System.Web.HttpUtility.UrlEncode(EnvVarUtil.GetEnvVarByKeyStr("LINE_CALLBACK_URL"));
            var redirectUri = $"&redirect_uri={callbackURL}";
            var stateValue = SecureRandomGenerator.GenerateRandomString(15);
            var state = $"&state={stateValue}";
            var scope = "&scope=profile%20openid";

            //TODO: リプレイアタック防止の為のnonceを追加する
            var uriBuilder = new UriBuilder("https", "access.line.me", 443, "/oauth2/v2.1/authorize", responseType + clientId + redirectUri + state + scope);
            var responseDto = new GenerateAuthURLResponse
            {
                AuthURL = uriBuilder.Uri.ToString()
            };

            if (!_lineRepository.AddLineState(stateValue)) {
                throw new SystemException();
            }
            return responseDto;
        }

        public async Task<GenerateAccesstokenResponse> GenerateAccesstoken(GenerateAccesstokenRequest generateAccesstokenRequest)
        {
            var state = _lineRepository.GetLineState();
            if (state != generateAccesstokenRequest.State) {
                throw new SystemException();
            }

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

            var response = await _httpClient.SendAsync(request) ?? throw new ArgumentNullException();
            var data = await response.Content.ReadAsByteArrayAsync();
            var body =  JsonSerializer.Deserialize<GenerateAccesstokenResponse>(data) ?? throw new ArgumentNullException();

            _lineRepository.AddLineAccessToken(body);
            return body;
        }
    }
}

