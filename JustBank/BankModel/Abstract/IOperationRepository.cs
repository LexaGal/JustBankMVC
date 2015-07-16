using System.Linq;
using BankModel.Entities;
using BankModel.Entities.Operations;

namespace BankModel.Abstract
{
    public interface IOperationRepository
    {
        IQueryable<Operation> Operations { get; }
        void SaveOperation(Operation operation);
        Operation DeleteOperation(int operationId);
    }
}