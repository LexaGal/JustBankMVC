using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BankModel.Entities.Account
{
    public class AccountState
    {
        [DisplayName("Bank Id")]
        public string BankId { get; set; }
        
        [DisplayName("Bank $")]
        public int BankMoney { get; set; }
        
        [DisplayName("Bank %")]
        public int BankProcents { get; set; }

        [DisplayName("Card Id")]
        public string CardId { get; set; }

        [DisplayName("Pin")]
        public string PinCode { get; set; }

        [DisplayName("Card $")]
        public int CardMoney { get; set; }

        public AccountState()
        {
            BankId = Bank.NewId;
            BankProcents = Bank.BankProcents;
            CardId = Bank.NewId;
            PinCode = Bank.NewPin;
        }

        public AccountState(AccountState accountState)
        {
            BankId = accountState.BankId;
            BankMoney = accountState.BankMoney;
            BankProcents = accountState.BankProcents;
            CardId = accountState.CardId;
            PinCode = accountState.PinCode;
            CardMoney = accountState.CardMoney;
         }
    }
}