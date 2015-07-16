using System.Collections.Generic;
using BankModel.Abstract;
using BankModel.Entities.Credits;

namespace BankModel.Entities.Operations
{
    interface IMoneyOperations
    {
        void PutMoney(int money);
        void TrasferMoneyFromCard(int money);
        void TrasferMoneyToCard(int money);
        void TransferMoneyFromBankIdToBankId(int money, Client client);
        void TakeCredit(Credit credit);
        void PayCredit(int money, Credit credit);
        void UpdateClientBankMoney();
        void UpdateClientCredits(List<Credit> credit);
    }
}
