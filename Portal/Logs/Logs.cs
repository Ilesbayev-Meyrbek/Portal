using NLog;
using Portal.Models;

namespace Portal.Logs
{
    public class Logs
    {
        private readonly User _currentUser;
        private readonly string _currentUserForStr;
        private readonly string _callsite;
        private readonly string _data;
        private readonly string _message;

        Logger logger = LogManager.GetCurrentClassLogger();

        public Logs(User currentUser, string callsite, string data, string message)
        {
            _currentUser = currentUser;
            _callsite = callsite;
            _data = data;
            _message = message;
        }

        public void WriteInfoLogs()
        {
            logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("Callsite", _callsite).WithProperty("IdentityUser", _currentUser.Login).WithProperty("Data", _data).Info(_message);
        }

        public void WriteErrorLogs()
        {
            logger.WithProperty("MarketID", _currentUser.MarketID).WithProperty("Callsite", _callsite).WithProperty("IdentityUser", _currentUser.Login).WithProperty("Data", _data).Error(_message);
        }
    }
}
