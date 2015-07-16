using System;
using System.Collections.Generic;
using System.Linq;
using BankModel.Abstract;
using BankModel.Entities.Credits;

namespace BankModel.Entities.Operations
{
    public class MoneyOperations : IMoneyOperations
    {
        private Client CurrentClient { get; set; }

        public MoneyOperations(Client client)
        {
            CurrentClient = client;
        }

        public void PutMoney(int money)
        {
            CurrentClient.State.BankMoney += money;
        }

        public void TrasferMoneyToCard(int money)
        {
            CurrentClient.State.CardMoney += money;
            CurrentClient.State.BankMoney -= money;
        }

        public void TrasferMoneyFromCard(int money)
        {
            CurrentClient.State.CardMoney -= money;
            CurrentClient.State.BankMoney += money;
        }

        public void TransferMoneyFromBankIdToBankId(int money, Client client)
        {
            CurrentClient.State.BankMoney -= money;
            client.State.BankMoney += money;
        }

        public void TakeCredit(Credit credit)
        {
            credit.DateTime = DateTime.Now;
            credit.ClientId = CurrentClient.Id;
        }

        public void PayCredit(int money, Credit credit)
        {
            CurrentClient.State.BankMoney -= money;
            credit.Money -= money;
        }
    
        public void UpdateClientBankMoney()
        {
            CurrentClient.State.BankMoney += (CurrentClient.State.BankMoney*CurrentClient.State.BankProcents/100);
        }

        public void UpdateClientCredits(List<Credit> credits)
        {
            credits.ForEach(credit => credit.Money += (credit.Money*credit.Procents/100));
        }
    }
}