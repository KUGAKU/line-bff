﻿using LineBff.DataAccess;
using LineBff.RequestDTO;
using LineBff.ResponseDTO;
using LineBff.Utils;
using Newtonsoft.Json;

namespace LineBff.BusinessLogic
{
    public interface ILineService
    {
        GenerateAuthURLResponse GenerateAuthURL();
        Task<GenerateAccesstokenResponse> GenerateAccesstoken(GenerateAccesstokenRequest generateAccesstokenRequest);

        Task<UserProfileResponse> GetUserProfile();
        Task<IntrospectAccessTokenResponse> IntrospectAccessToken();
    }

    public class LineService : ILineService
    {
        private readonly ILineRepository _lineRepository;
        private readonly ISessionRepository _sessionRepository;

        public LineService(ILineRepository lineRepository, ISessionRepository sessionRepository)
        {
            _lineRepository = lineRepository;
            _sessionRepository = sessionRepository;
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

            if (!_lineRepository.AddLineState(stateValue))
            {
                throw new SystemException();
            }
            return responseDto;
        }

        public async Task<GenerateAccesstokenResponse> GenerateAccesstoken(GenerateAccesstokenRequest generateAccesstokenRequest)
        {
            var state = _lineRepository.GetLineState();
            if (state != generateAccesstokenRequest.State)
            {
                throw new SystemException();
            }
            var response = await _lineRepository.GenerateAccesstoken(generateAccesstokenRequest);
            var json = JsonConvert.SerializeObject(response);
            if (!_sessionRepository.AddStringValueWithSessionId(json))
            {
                throw new SystemException();
            }
            _lineRepository.AddLineAccessToken(response);
            return response;
        }

        public Task<UserProfileResponse> GetUserProfile()
        {
            var sessionId = _sessionRepository.GetSessionId();
            var value = _sessionRepository.GetStringValueBySessionId(sessionId);
            if (value == null)
            {
                throw new ArgumentNullException();
            }
            var accessToken = JsonConvert.DeserializeObject<GenerateAccesstokenResponse>(value);
            if (accessToken == null)
            {
                throw new ArgumentNullException();
            }
            return _lineRepository.GetUserProfile(accessToken.AccessToken);
        }

        public async Task<IntrospectAccessTokenResponse> IntrospectAccessToken()
        {
            var sessionId = _sessionRepository.GetSessionId();
            var value = _sessionRepository.GetStringValueBySessionId(sessionId);
            if (value == null)
            {
                throw new ArgumentNullException();
            }
            var accessToken = JsonConvert.DeserializeObject<GenerateAccesstokenResponse>(value);
            if (accessToken == null)
            {
                throw new ArgumentNullException();
            }
            var response = new IntrospectAccessTokenResponse();
            var entity = await _lineRepository.IntrospectAccessToken(accessToken.AccessToken);
            response.IsValid = entity != null ? true : false;
            return response;
        }
    }
}

