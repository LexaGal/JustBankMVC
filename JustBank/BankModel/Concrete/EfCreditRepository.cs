using System.Data.Entity;
using System.Linq;
using BankModel.Abstract;
using BankModel.Entities;
using BankModel.Entities.Credits;

namespace BankModel.Concrete
{
    public class EfCreditRepository : IRepository<Credit>
    {
        private EfDbContext _context = new EfDbContext();
        public IQueryable<Credit> Objects
        {
            get { return _context.Credits; }
        }

        public void SaveObject(Credit credit)
        {
            if (credit.Id == 0)
            {
                _context.Credits.Add(credit);
                _context.SaveChanges();
                return;
            }

            if (credit.Money == 0)
            {
                DeleteObject(credit.Id);
            }

            Credit dbEntry = _context.Credits.Find(credit.Id);
            if (dbEntry != null)
            {
                dbEntry.CopyFieldsFrom(credit);
                _context.SaveChanges();
                Bank.CreditRepository = this;
            }
        }

        public Credit DeleteObject(int creditId)
        {
            Credit dbEntry = _context.Credits.Find(creditId);
            if (dbEntry != null)
            {
                _context.Credits.Remove(dbEntry);
                _context.SaveChanges();
                Bank.CreditRepository = this;
            }
            return dbEntry;
        }
    }
}