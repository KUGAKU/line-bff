using LineBff.DataAccess;

namespace LineBff.BusinessLogic
{
    public interface ISessionService
    {
        bool SaveSessionId(string sessionId);
        string RetrieveSessionId();
    }

    public class SessionService : ISessionService
    {
        private readonly ISessionRepository _sessionRepository;

        public SessionService(ISessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public string RetrieveSessionId()
        {
            return _sessionRepository.GetSessionId();
        }

        public bool SaveSessionId(string sessionId)
        {
            return _sessionRepository.AddSessionId(sessionId);
        }
    }
}