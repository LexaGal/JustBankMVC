using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BankModel.Entities.Credits
{   
    [Serializable]
    public class Credit
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
        [DisplayName("Credit $")]
        public int Money { get; set; }

        [Required]
        [DisplayName("Credit %")]
        public int Procents { get; set; }

        [Required]
        [DisplayName("Type")]
        public string Type { get; set; }

        [Required]
        [DisplayName("Period")]
        public DateTime Period { get; set; }

        public Credit(int procents, string type, DateTime period)
        {
            Procents = procents;
            Type = type;
            Period = period;
        }

        public Credit() {}

        public void CopyFieldsFrom(Credit credit)
        {
            ClientId = credit.ClientId;
            Money = credit.Money;
            Procents = credit.Procents;
            Type = credit.Type;
            Period = credit.Period;
        }
        
    }
}
