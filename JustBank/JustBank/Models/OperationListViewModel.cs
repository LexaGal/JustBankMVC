using System.Collections.Generic;
using BankModel.Entities;
using BankModel.Entities.Operations;

namespace JustBank.Models
{
    public class OperationListViewModel
    {
        public IEnumerable<Operation> Operations{ get; set; }
        public PagingInfo PagingInfo { get; set; }
        public string CurrentCategory { get; set; }
    }
}