using Microsoft.Azure.Functions.Worker.Http;

namespace LineBff.Interceptor
{
    public static class HttpRequestIntercepter
    {
        public static async Task<string> GetResponseBody(this HttpResponseData response)
        {
            using var reader = new StreamReader(response.Body);
            return await reader.ReadToEndAsync();
        }
    }
}

