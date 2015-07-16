using System.Linq;
using System.Web.Providers.Entities;
using System.Web.Security;
using System.Web.SessionState;
using BankModel.Abstract;
using BankModel.Entities;
using JustBank.Authentification.Abstract;

namespace JustBank.Authentification.Concrete
{
    public class AuthProvider : IAuthProvider
    {
        public Client Authenticate(string username, string password)
        {
            bool result = FormsAuthentication.Authenticate(username, password);

            if (result)
            {
                FormsAuthentication.SetAuthCookie(username, false);
                return new Client {FirstName = username, Password = password};
            }

            Client client = Bank.ClientRepository.Objects.SingleOrDefault(c => c.FirstName == username && c.Password == password);
            return client;
        }
    }
}