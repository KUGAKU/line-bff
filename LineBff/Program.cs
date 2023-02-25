using LineBff.BusinessLogic;
using LineBff.DataAccess;
using LineBff.DataAccess.Datasource;
using LineBff.Utils;
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
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(s =>
                {
                    var redis = ConnectionMultiplexer.Connect(EnvVarUtil.GetEnvVarByKeyStr("REDIS_CONNECTION_STRING")); //TODO: この書き方じゃなくてもかけるから直す
                    s.AddHttpClient();
                    s.AddSingleton(redis.GetDatabase());
                    s.AddSingleton<ICacheDataSource, CacheDataSource>();
                    s.AddSingleton<ILineRepository, LineRepository>();
                    s.AddSingleton<ILineService, LineService>();
                })
                .Build();
            host.Run();
        }
    }
}