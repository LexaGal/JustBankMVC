using System.Linq;
using BankModel.Entities;

namespace BankModel.Abstract
{
    public interface IClientRepository
    {
        IQueryable<Client> Clients { get; }
        void SaveClient(Client client);
        Client DeleteClient(int clientId);
    }
}