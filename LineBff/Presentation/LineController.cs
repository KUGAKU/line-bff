using System.Net;
using System.Text;
using LineBff.BusinessLogic;
using LineBff.RequestDTO;
using LineBff.Utils;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;

namespace LineBff
{
    public class LineController
    {
        private readonly ILineService _lineService;
        private readonly ISessionService _sessionService;

        public LineController(ILineService lineService, ISessionService sessionService)
        {
            _lineService = lineService;
            _sessionService = sessionService;
        }

        [Function("generate-authurl")]
        public HttpResponseData RunGenerateAuthURL([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            try
            {
                var dto = _lineService.GenerateAuthURL();
                var json = JsonConvert.SerializeObject(dto);

                var sessionId = SecureRandomGenerator.GenerateRandomString(16);
                if (!_sessionService.SaveSessionId(sessionId)) throw new SystemException();

                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Cookies.Append("session", sessionId);
                response.Headers.Add("Content-Type", "application/json");
                response.Body = new MemoryStream(Encoding.UTF8.GetBytes(json));
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Function("generate-accesstoken")]
        public async Task<HttpResponseData> RunGenerateAccesstoken([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                //TODO: バリデーション
                var request = JsonConvert.DeserializeObject<GenerateAccesstokenRequest>(requestBody) ?? throw new ArgumentNullException();
                var dto = await _lineService.GenerateAccesstoken(request);
                var json = JsonConvert.SerializeObject(dto);
                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "application/json");
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Function("get-user-profile")]
        public async Task<HttpResponseData> RunGetUserProfile([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            try
            {
                var cookies = req.Cookies.ToDictionary(cookie => cookie.Name, cookie => cookie.Value);
                var cookie = cookies.ContainsKey("session") ? cookies["session"] : throw new InvalidOperationException();
                var dto = await _lineService.GetUserProfile();
                var json = JsonConvert.SerializeObject(dto);
                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "application/json");
                response.Body = new MemoryStream(Encoding.UTF8.GetBytes(json));
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

