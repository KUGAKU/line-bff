using LineBff.DataAccess.Datasource;

namespace LineBff.DataAccess
{
    public interface ISessionRepository
    {
        bool AddSessionId(string sessionId);
        bool AddStringValueWithSessionId(string stringValue);
        string GetSessionId();
        string GetStringValueBySessionId(string sessionId);
    }

    public class SessionRepository : ISessionRepository
    {
        private readonly ICacheDataSource _cacheDataSource;
        private readonly string _sessionIdKey = "session_id";

        public SessionRepository(ICacheDataSource cacheDataSource)
        {
            _cacheDataSource = cacheDataSource;
        }
        public bool AddSessionId(string sessionId)
        {
            return _cacheDataSource.SetStringValue(_sessionIdKey, sessionId, 10080); //1週間でsessionIdがなくなる
        }

        public bool AddStringValueWithSessionId(string stringValue)
        {
            var sessionId = _cacheDataSource.GetValue(_sessionIdKey);
            if (sessionId == null) throw new SystemException();
            return _cacheDataSource.SetStringValue(sessionId, stringValue, 10080); //1週間でsessionIdがなくなる
        }

        public string GetSessionId()
        {
            return _cacheDataSource.GetValue(_sessionIdKey);
        }

        public string GetStringValueBySessionId(string sessionId)
        {
            return _cacheDataSource.GetValue(sessionId);
        }
    }
}