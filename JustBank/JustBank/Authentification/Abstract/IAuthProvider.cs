using BankModel.Entities;

namespace JustBank.Authentification.Abstract
{
    public interface IAuthProvider
    {
        Client Authenticate(string username, string password);
    }
}