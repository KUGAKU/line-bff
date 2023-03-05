using StackExchange.Redis;

namespace LineBff.DataAccess.Datasource
{
    public interface ICacheDataSource
    {
        bool SetStringValue(string key, string value, int minutes);
        string GetValue(string key);
    }


    public class CacheDataSource : ICacheDataSource
    {

        private readonly IDatabase _database;

        public CacheDataSource(IDatabase database)
        {
            _database = database;
        }

        public string GetValue(string key)
        {
            return _database.StringGet(key).ToString();
        }

        public bool SetStringValue(string key, string value, int minutes)
        {
            var expirationTime = TimeSpan.FromMinutes(minutes);
            return _database.StringSet(key, value, expirationTime);
        }
    }
}

