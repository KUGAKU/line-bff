using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;

namespace LineBff.Middleware
{
    public class AuthenticationMiddleware : IFunctionsWorkerMiddleware
    {
        private readonly string[] allowedPublicAbsolutePaths = new string[]
        {
            "/api/generate-authurl",
            "/api/generate-accesstoken",
            "/api/get-user-profile"
        };

        private readonly string[] allowedPrivateAbsolutePaths = new string[]
        {
            "/api/generate-accesstoken"
        };



        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            var requestData = await context.GetHttpRequestDataAsync();
            if (requestData == null) return;
            if (!IsAllowedPublicAbsolutePaths(requestData.Url)) return;

            await next(context);
        }

        private bool IsAllowedPublicAbsolutePaths(Uri uri)
        {
            if (allowedPublicAbsolutePaths.Contains(uri.AbsolutePath)) return true;
            return false;
        }

        private bool IsAllowedPrivateAbsolutePaths(Uri uri)
        {
            if (allowedPrivateAbsolutePaths.Contains(uri.AbsolutePath)) return true;
            return false;
        }
    }
}

