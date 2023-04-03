using System.Net;
using System.Text;
using LineBff.DataAccess;
using LineBff.ResponseDTO;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Newtonsoft.Json;

namespace LineBff.Middleware
{
    public class AuthenticationMiddleware : IFunctionsWorkerMiddleware
    {

        private readonly ISessionRepository _sessionRepository;

        public AuthenticationMiddleware(ISessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public readonly string[] allowedPublicAbsolutePaths = new string[]
        {
            "/api/generate-authurl",
        };

        public readonly string[] allowedPrivateAbsolutePaths = new string[]
        {
            "/api/introspect-accesstoken",
            "/api/generate-accesstoken",
            "/api/get-user-profile"
        };


        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            var requestData = await context.GetHttpRequestDataAsync();
            if (requestData == null)
            {
                return;
            }

            if (IsAllowedPublicAbsolutePaths(requestData.Url))
            {
                await next(context);
                return;
            }

            var cookies = requestData!.Cookies;
            var session = cookies.FirstOrDefault(cookie => cookie.Name == "session");

            if (session == null)
            { //Cookie not sent from the browser.
                var response = requestData.CreateResponse();
                response.StatusCode = HttpStatusCode.Unauthorized;
                var body = new IntrospectAccessTokenResponse();
                body.IsValid = false;
                var json = JsonConvert.SerializeObject(body);
                response.Body = new MemoryStream(Encoding.UTF8.GetBytes(json));
                context.GetInvocationResult().Value = response;
                return;
            }

            var sessionId = _sessionRepository.GetSessionId();
            if (!IsValidSession(session.Value, sessionId)) //Session ID mismatch, indicating an invalid or tampered session ID.
            {
                var response = requestData.CreateResponse();
                response.StatusCode = HttpStatusCode.Unauthorized;
                context.GetInvocationResult().Value = response;
                return;
            }

            if (!IsAllowedPrivateAbsolutePaths(requestData.Url))
            {
                var response = requestData.CreateResponse();
                response.StatusCode = HttpStatusCode.Forbidden;
                context.GetInvocationResult().Value = response;
                return;
            }

            await next(context);
        }

        public bool IsValidSession(string sessionValue, string sessionId)
        {
            if (sessionValue == sessionId)
            {
                return true;
            }
            return false;
        }

        public bool IsAllowedPublicAbsolutePaths(Uri uri)
        {
            if (allowedPublicAbsolutePaths.Contains(uri.AbsolutePath)) return true;
            return false;
        }

        public bool IsAllowedPrivateAbsolutePaths(Uri uri)
        {
            if (allowedPrivateAbsolutePaths.Contains(uri.AbsolutePath)) return true;
            return false;
        }
    }
}

