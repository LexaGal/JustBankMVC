using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace BankModel.Entities.Operations
{
    public class CurrencyConverter
    {
        public Convertation Course { get; set; }

        public List<Convertation> Convertations { get; private set; }

        public List<Convertation> GetMoneyCourse()
        {
            List<Convertation> convertations = new List<Convertation>();
            using (TextReader textReader = new StreamReader(ConfigurationManager.AppSettings["Course"]))
            {
                while (true)
                {
                    string line = textReader.ReadLine();
                    if (line != null)
                    {
                        string[] items = line.Split(',');
                        convertations.Add(new Convertation()
                        {
                            ConvertFrom = items[0],
                            ConvertTo = items[1],
                            CoeffValue = double.Parse(items[2])
                        });

                        continue;
                    }
                    return convertations;
                }
            }
        }

        public CurrencyConverter()
        {
            Convertations = GetMoneyCourse();
        }
    }
}