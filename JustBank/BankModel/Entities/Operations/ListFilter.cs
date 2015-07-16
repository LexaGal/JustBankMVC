using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankModel.Abstract;
using BankModel.Entities.Credits;

namespace BankModel.Entities.Operations
{
    public class ListFilter<T> where T : class
    {
        public static List<T> Filter(List<T> source, string field)
        {
            if (typeof (T) == typeof (Client))
            {
                List<Client> clients = source as List<Client>;

                if (clients != null)
                {
                    switch (field)
                    {
                        case "Id":
                            clients = clients.OrderBy((client => client.Id)).ToList();
                            break;

                        case "FirstName":
                            clients = clients.OrderBy((client => client.FirstName)).ToList();
                            break;

                        case "SecondName":
                            clients = clients.OrderBy((client => client.SecondName)).ToList();
                            break;

                        case "AccountType":
                            clients = clients.OrderBy((client => client.AccountType)).ToList();
                            break;

                        case "BankMoney":
                            clients = clients.OrderByDescending((client => client.State.BankMoney)).ToList();
                            break;
                            
                    }
                    return clients as List<T>;
                }
            }

            if (typeof (T) == typeof (Operation))
            {
                List<Operation> operations = source as List<Operation>;

                if (operations != null)
                {
                    switch (field)
                    {
                        case "Type":
                            operations = operations.OrderBy((op => op.Type)).ToList();
                            break;

                        case "Date":
                            operations = operations.OrderByDescending((op => op.DateTime)).ToList();
                            break;

                        case "BankId":
                            operations = operations.OrderBy((op => op.SourceBankId)).ToList();
                            break;

                        case "Money":
                            operations = operations.OrderByDescending((op => op.Money)).ToList();
                            break;
                    }
                    return operations as List<T>;
                }
            }
            
            if (typeof(T) == typeof(Credit))
            {
                List<Credit> credits = source as List<Credit>;

                if (credits != null)
                {
                    switch (field)
                    {
                        case "Type":
                            credits = credits.OrderBy((cr => cr.Type)).ToList();
                            break;

                        case "Date":
                            credits = credits.OrderByDescending((cr => cr.DateTime)).ToList();
                            break;

                        case "ClientId":
                            credits = credits.OrderBy((cr => cr.ClientId)).ToList();
                            break;

                        case "Money":
                            credits = credits.OrderByDescending((cr => cr.Money)).ToList();
                            break;

                        case "Period":
                            credits = credits.OrderBy((cr => cr.Period)).ToList();
                            break;
                    }
                    return credits as List<T>;
                }
            }
            return null;
        }
    }
}