using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using BankModel.Abstract;
using BankModel.Concrete;
using BankModel.Entities.Credits;
using BankModel.Entities.Operations;
using BankModel.Serialization;

namespace BankModel.Entities
{
    public class Bank
    {
        public static int AllMoney { get; private set; }
        public static int BankProcents { get; private set; }
        public static IRepository<Client> ClientRepository { get; set; }
        public static IRepository<Operation> OperationRepository { get; set; }
        public static IRepository<Credit> CreditRepository { get; set; }
        public static List<Credit> Credits { get; set; }

        public static string NewId
        {
            get { return Guid.NewGuid().ToString().Substring(0, 8); }
        }

        public static string NewPin
        {
            get
            {
                return (((Guid.NewGuid().ToString())[0]) % 10).ToString() + ((Guid.NewGuid().ToString())[0]) % 10
                    + ((Guid.NewGuid().ToString())[0]) % 10 + ((Guid.NewGuid().ToString())[0]) % 10;
            }
        }

        public static List<Credit> CreditsList()
        {
            MemoryStream stream = new MemoryStream();
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, Credits);
            stream.Position = 0;
            List<Credit> credits = (List<Credit>) formatter.Deserialize(stream);
            foreach (var credit in credits)
            {
                credit.Period = DateTime.Now.AddYears(credit.Period.Year).Date;
            }
            return credits.ToList();
        }

        public static void Initialize()
        {
            AllMoney = int.MaxValue;
            BankProcents = 5;        
            ClientRepository = new EfClientRepository();
            OperationRepository = new EfOperationRepository();
            CreditRepository = new EfCreditRepository();
            Credits = new List<Credit>
            {
                new Credit(5, "House", new DateTime(10, 1, 1)),
                new Credit(7, "Car", new DateTime(7, 1, 1)),
                new Credit(4, "Education", new DateTime(5, 1, 1)),
                new Credit(10, "Travelling", new DateTime(1, 1, 1))
            };
        }
    }
}
