using System.Data.Entity;
using BankModel.Entities;
using BankModel.Entities.Credits;
using BankModel.Entities.Operations;

namespace BankModel.Concrete
{
    public class EfDbContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<Operation> Operations { get; set; }
        public DbSet<Credit> Credits { get; set; }
    }
}