using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using BankModel.Abstract;
using BankModel.Entities.Account;
using BankModel.Entities.Credits;
using BankModel.Entities.Operations;

namespace BankModel.Entities
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("Name")]
        public string FirstName { get; set; }

        [Required]
        [DisplayName("Surname")]
        public string SecondName { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayName("Account")]
        public string AccountType { get; set; }

        public AccountState State { get; set; }

        public void CopyFieldsFrom(Client client)
        {
            FirstName = client.FirstName;
            SecondName = client.SecondName;
            Email = client.Email;
            Password = client.Password;
            AccountType = client.AccountType;
            State = new AccountState(client.State);
        }

        public Client()
        {
            State = new AccountState();
        }

        public List<T> GetObjects<T>(IRepository<T> repository) where T : class
        {
            if (typeof (T) == typeof (Operation))
            {
                List<Operation> clientOperations = ((IRepository<Operation>) repository).Objects.ToList()
                    .Where(o => o.ClientId == Id).ToList();

                return clientOperations as List<T>;
            }

            if (typeof(T) == typeof(Credit))
            {
                List<Credit> clientCredits = ((IRepository<Credit>) repository).Objects.ToList()
                    .Where(c => c.ClientId == Id).ToList();

                return clientCredits as List<T>;
            }
            return null;
        }
    }
}