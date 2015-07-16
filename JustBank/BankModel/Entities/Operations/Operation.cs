using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BankModel.Entities.Operations
{
    public class Operation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("Date/Time")]
        public DateTime DateTime { get; set; }

        [Required]
        [DisplayName("Client Id")]
        public int ClientId { get; set; }

        [Required]
        [DisplayName("Type")]
        public string Type { get; set; }

        [DisplayName("Source Bank Id")]
        public string SourceBankId { get; set; }

        [DisplayName("Source Card Id")]
        public string SourceCardId { get; set; }

        [DisplayName("Dest. Bank Id")]
        public string DestinationBankId { get; set; }

        [DisplayName("Dest. Card Id")]
        public string DestinationCardId { get; set; }

        [Required]
        [DisplayName("Money $")]
        public int Money { get; set; }
        
        public Operation(){}

        public Operation(DateTime logDateTime, int clientId , string operationType, string sourceBankId, string sourceCardId,
            string destinationBankId, string destinationCardId, int money)
        {
            DateTime = logDateTime;
            ClientId = clientId;
            Type = operationType;
            SourceBankId = sourceBankId;
            SourceCardId = sourceCardId;
            DestinationBankId = destinationBankId;
            DestinationCardId = destinationCardId;
            Money = money;
        }
    }
}