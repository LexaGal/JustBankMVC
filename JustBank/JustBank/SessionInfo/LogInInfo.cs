using System.Linq;
using BankModel.Entities;

namespace JustBank.SessionInfo
{
    public class LogInInfo
    {
        private static Client _client;

        public static bool AnyClientIsLogedIn()
        {
            return _client != null;
        }

        public static Client LoggedInClient()
        {
            return _client;
        }

        public static void LogClientIn(Client client)
        {
            _client = client;
        }

        public static void LogClientOut()
        {
            _client = null;
        }

        public static void UpdateLoggedInClient()
        {
            if (LoggedInClient() != null)
            {
                _client = Bank.ClientRepository.Objects.First(c => c.Id == _client.Id);
            }
        }
    }
}