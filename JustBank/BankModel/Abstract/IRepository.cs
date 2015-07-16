using System.Linq;
using BankModel.Entities;
using BankModel.Entities.Operations;

namespace BankModel.Abstract
{
    public interface IRepository<T> where T: class 
    {
        IQueryable<T> Objects { get; }
        void SaveObject(T obj);
        T DeleteObject(int objId);
    }
}