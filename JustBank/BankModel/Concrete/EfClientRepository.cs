using System.Data.Entity;
using System.Linq;
using BankModel.Abstract;
using BankModel.Entities;

namespace BankModel.Concrete
{
    public class EfClientRepository :  IRepository<Client>
    {
        private EfDbContext _context = new EfDbContext();
        public IQueryable<Client> Objects
        {
            get { return _context.Clients; }
        }

        public void SaveObject(Client client)
        {
            if (client.Id == 0)
            {
                _context.Clients.Add(client);
                _context.SaveChanges();
                return;
            }
            
            Client dbEntry = _context.Clients.Find(client.Id);
            if (dbEntry != null)
            {
                dbEntry.CopyFieldsFrom(client);
                _context.SaveChanges();
                Bank.ClientRepository = this;
            }
        }

        public Client DeleteObject(int clientId)
        {
            Client dbEntry = _context.Clients.Find(clientId);
            if (dbEntry != null)
            {
                _context.Clients.Remove(dbEntry);
                _context.SaveChanges();
                Bank.ClientRepository = this;
            }
            return dbEntry;
        }
    }
}