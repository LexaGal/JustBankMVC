using System.ComponentModel.DataAnnotations;

namespace BankModel.Entities.Operations
{
    public class Convertation
    {
        public static readonly string[] Currencies = { "USD", "BYR", "EUR", "RUB" };
        
        public string ConvertFrom { get; set; }
        
        public string ConvertTo { get; set; }
        
        public double CoeffValue { get; set; }
    }
}