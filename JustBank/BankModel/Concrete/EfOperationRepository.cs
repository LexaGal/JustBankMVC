using System.Data.Entity;
using System.Linq;
using BankModel.Abstract;
using BankModel.Entities;
using BankModel.Entities.Operations;

namespace BankModel.Concrete
{
    public class EfOperationRepository : IRepository<Operation>
    {
        private EfDbContext _context = new EfDbContext();
        public IQueryable<Operation> Objects
        {
            get { return _context.Operations; }
        }

        public void SaveObject(Operation operation)
        {
            if (operation.Id == 0)
            {
                _context.Operations.Add(operation);
                _context.SaveChanges();
                Bank.OperationRepository = this;
            }
        }

        public Operation DeleteObject(int operationId)
        {
            Operation dbEntry = _context.Operations.Find(operationId);
            if (dbEntry != null)
            {
                _context.Operations.Remove(dbEntry);
                _context.SaveChanges();
                Bank.OperationRepository = this;
            }
            return dbEntry;
        }
    }
}