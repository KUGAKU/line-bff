using LineBff.Wrappers;

namespace LineBff.DataAccess.DataSource.Remote
{
	public class APIDataSource : IHttpClientWrapper
	{
		private readonly IHttpClientWrapper _httpClientWrapper;

		public APIDataSource(IHttpClientWrapper httpClientWrapper)
		{
			_httpClientWrapper = httpClientWrapper;
		}
		public Task<HttpResponseMessage> GetAsync(string requestUri)
		{
			return _httpClientWrapper.GetAsync(requestUri);
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return _httpClientWrapper.SendAsync(request);
        }
    }
}

