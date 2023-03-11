using LineBff.BusinessLogic;
using LineBff.DataAccess;
using LineBff.DataAccess.Datasource;
using LineBff.Middleware;
using LineBff.Utils;
using LineBff.Wrappers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace LineBff
{
    class Program
    {
        static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults(app => app.UseMiddleware<AuthenticationMiddleware>())
                .ConfigureServices(s =>
                {
                    var redis = ConnectionMultiplexer.Connect(EnvVarUtil.GetEnvVarByKeyStr("REDIS_CONNECTION_STRING")); //TODO: この書き方じゃなくてもかけるから直す
                    s.AddSingleton<IHttpClientWrapper>(new HttpClientWrapper(new HttpClient()));
                    s.AddSingleton(redis.GetDatabase());
                    s.AddSingleton<ICacheDataSource, CacheDataSource>();
                    s.AddSingleton<ILineRepository, LineRepository>();
                    s.AddSingleton<ISessionRepository, SessionRepository>();
                    s.AddSingleton<ILineService, LineService>();
                    s.AddSingleton<ISessionService, SessionService>();
                })
                .Build();
            host.Run();
        }
    }
}