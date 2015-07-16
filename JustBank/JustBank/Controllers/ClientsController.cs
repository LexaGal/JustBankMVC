using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI.WebControls;
using BankModel.Abstract;
using BankModel.Entities;
using BankModel.Entities.Account;
using BankModel.Entities.Credits;
using BankModel.Entities.Errors;
using BankModel.Entities.Operations;
using BankModel.Serialization;
using JustBank.Attributes;
using JustBank.Models;
using JustBank.SessionInfo;
using Ninject.Infrastructure.Language;
using WebGrease.Css.Extensions;
using Message = JustBank.Messaging.Message;

namespace JustBank.Controllers
{
    [BankAuth(true)]
    public class ClientsController : Controller
    {
        private IRepository<Client> _clientsRepository;
        private IRepository<Operation> _operationsRepository;
        private IRepository<Credit> _creditsRepository;
        private const int PageSize = 7;

        public ClientsController(IRepository<Client> clientsRepository, IRepository<Operation> operationsRepository,
            IRepository<Credit> creditsRepository)
        {
            _clientsRepository = clientsRepository;
            _operationsRepository = operationsRepository;
            _creditsRepository = creditsRepository;
        }


        public ActionResult Index()
        {
            if (LogInInfo.LoggedInClient() == null)
            {
                return RedirectToAction("LogIn", "Account");
            }
            return View(LogInInfo.LoggedInClient());
        }
        

        [AllowAnonymous]
        public ActionResult CreateNew()
        {
            return View(new Client());
        }


        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNew(Client client)
        {
            if (ModelState.IsValid)
            {
                if (client.State.BankMoney < 0 || client.State.BankMoney > 1000000)
                {
                    ModelState.AddModelError("State.BankMoney", Message.SumIntervalMismatch);
                    return View(client);
                }
                
                List<Client> list = _clientsRepository.Objects.ToList();
                bool isClient = false;
                
                if (!String.IsNullOrWhiteSpace(client.Email))
                {
                    isClient = list.Any(p => p.Id != client.Id && String.CompareOrdinal(p.Email, client.Email) == 0);
                }
                if (isClient)
                {
                    ModelState.AddModelError("Email", Message.EmailIsUsed);
                    return View(client);
                }

                LogInInfo.LogClientIn(client);
                _clientsRepository.SaveObject(client);
                
                if (Session["url"] != null)
                {
                    string url = Session["url"].ToString();
                    Session["url"] = null;
                    return RedirectPermanent(url);
                }
                return RedirectToAction("Index", "Clients");
            }
            return View(client);
        }


        public ActionResult Edit()
        {
            return View(LogInInfo.LoggedInClient());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Client client)
        {
           client.State = (AccountState) Session["state"];
           Session["state"] = null;
           return CreateNew(client);
        }


        public ActionResult DeleteClient()
        {
            return View(LogInInfo.LoggedInClient());
        }


        [HttpPost, ActionName("DeleteClient")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteClientConfirmed(int id)
        {
            _clientsRepository.DeleteObject(id);
            return RedirectToAction("Login", "Account");
        }


        public ActionResult Operations()
        {
            if (LogInInfo.LoggedInClient() != null)
            {
                return View();
            }
            return RedirectToAction("Login", "Account");
        }


        public ActionResult PutMoney()
        {
            if (LogInInfo.LoggedInClient() != null)
            {
                return View();
            }
            return RedirectToAction("LogIn", "Account");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PutMoney(FormCollection form)
        {
            Client client = LogInInfo.LoggedInClient();
            
            string bankId = form["BankId"];
            int sum;
            try
            {
                sum = int.Parse(form["Sum"]);
            }
            catch (FormatException)
            {
                ViewBag.Result = Message.WrongSum;
                return View();
            }
            if (client == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (client.State.BankId != bankId)
            {
                ViewBag.Result = Message.WrongId;
                return View();
            }
            if (sum <= 0 || sum > 100000000)
            {
                ViewBag.Result = Message.WrongSum;
                return View();
            }

            MoneyOperations moneyOperation = new MoneyOperations(client);
            moneyOperation.PutMoney(sum);

            Operation operation = new Operation(DateTime.Now, client.Id, "Put", client.State.BankId,
                String.Empty, String.Empty, String.Empty, sum);

            _clientsRepository.SaveObject(client);
            LogInInfo.UpdateLoggedInClient();
            _operationsRepository.SaveObject(operation);

            ViewBag.Result = Message.OperationSuccessed;
            return View();
        }


        public ActionResult TransferMoneyToCard()
        {
            if (LogInInfo.LoggedInClient() != null)
            {
                return View();
            }
            return RedirectToAction("LogIn", "Account");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TransferMoneyToCard(FormCollection form)
        {
            Client client = LogInInfo.LoggedInClient();
            
            string bankId = form["BankId"];
            string cardId = form["CardId"];
            int sum;
            try
            {
                sum = int.Parse(form["Sum"]);
            }
            catch (FormatException)
            {
                ViewBag.Result = Message.WrongSum;
                return View();
            }
            if (client == null)
            {
                return RedirectToAction("LogIn", "Account");
            }
            if (client.State.CardId != cardId || client.State.BankId != bankId)
            {
                ViewBag.Result = Message.WrongId;
                return View();
            }
            if (sum <= 0 || sum > 100000000)
            {
                ViewBag.Result = Message.SumIntervalMismatch;
                return View();
            }
            if (sum > client.State.BankMoney)
            {
                ViewBag.Result = Message.NotEnoughBankMoney;
                return View();
            }

            MoneyOperations moneyOperation = new MoneyOperations(client);
            
            moneyOperation.TrasferMoneyToCard(sum);

            Operation operation = new Operation(DateTime.Now, client.Id, "Transfer", client.State.BankId,
                String.Empty, String.Empty, client.State.CardId, sum);

            _clientsRepository.SaveObject(client);
            LogInInfo.UpdateLoggedInClient();
            _operationsRepository.SaveObject(operation);
            
            ViewBag.Result = Message.OperationSuccessed;
            return View();
        }


        public ActionResult TransferMoneyFromCard()
        {
            if (LogInInfo.LoggedInClient() != null)
            {
                return View();
            }
            return RedirectToAction("LogIn", "Account");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TransferMoneyFromCard(FormCollection form)
        {
            Client client = LogInInfo.LoggedInClient();

            string bankId = form["BankId"];
            string cardId = form["CardId"];
            int sum;
            try
            {
                sum = int.Parse(form["Sum"]);
            }
            catch (FormatException)
            {
                ViewBag.Result = Message.WrongSum;
                return View();
            }
            if (client == null)
            {
                return RedirectToAction("LogIn", "Account");
            }
            if (client.State.CardId != cardId || client.State.BankId != bankId)
            {
                ViewBag.Result = Message.WrongId;
                return View();
            }
            if (sum <= 0 || sum > 100000000)
            {
                ViewBag.Result = Message.SumIntervalMismatch;
                return View();
            }
            if (sum > client.State.BankMoney)
            {
                ViewBag.Result = Message.NotEnoughBankMoney;
                return View();
            }

            MoneyOperations moneyOperation = new MoneyOperations(client);
            moneyOperation.TrasferMoneyFromCard(sum);

            Operation operation = new Operation(DateTime.Now, client.Id, "Transfer", String.Empty,
                client.State.CardId, client.State.BankId, String.Empty, sum);

            _clientsRepository.SaveObject(client);
            LogInInfo.UpdateLoggedInClient();
            _operationsRepository.SaveObject(operation);
            
            ViewBag.Result = Message.OperationSuccessed; 
            return View();
        }

        
        public ActionResult TransferMoneyFromBankIdToBankId()
        {
            Client client = LogInInfo.LoggedInClient();
            if (client != null && client.AccountType != "Premium")
            {
                Session.Add("error", new ErrorNotifier
                {
                    Source = "transfer",
                    ActionLink = new Tuple<string, string>("Edit", Url.Action("Edit"))
                });
                return RedirectToAction("Error", "Clients", (ErrorNotifier) Session["error"]);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TransferMoneyFromBankIdToBankId(FormCollection form)
        {
            Client client = LogInInfo.LoggedInClient();
            string bankId = form["BankId"];
            string bankIdTo = form["BankIdTo"];
            int sum;
            try
            {
                sum = int.Parse(form["Sum"]);
            }
            catch (FormatException)
            {
                ViewBag.Result = Message.WrongSum;
                return View();
            }

            List<Client> list = _clientsRepository.Objects.ToList();
            Client clientTo = null;
            try
            {
                clientTo = list.First(p => String.CompareOrdinal(p.State.BankId, bankIdTo) == 0);
            }
            catch (InvalidOperationException)
            {
                if (client == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                if (client.State.BankId != bankId)
                {
                    ViewBag.Result = string.Format("Bank Id {0}", Message.WrongField);
                    return View();
                }
                if (clientTo == null)
                {
                    ViewBag.Result = string.Format("Bank Id To {0}", Message.WrongField);
                    return View();
                }
            }
            if (sum <= 0 || sum > 100000000)
            {
                ViewBag.Result = Message.SumIntervalMismatch;
                return View();
            }
            if (sum > client.State.BankMoney)
            {
                ViewBag.Result = Message.NotEnoughBankMoney;
                return View();
            }

            MoneyOperations moneyOperation = new MoneyOperations(client);
            moneyOperation.TransferMoneyFromBankIdToBankId(sum, clientTo);

            Operation operation = new Operation(DateTime.Now, client.Id, "Transfer", client.State.BankId,
                String.Empty, clientTo.State.BankId, String.Empty, sum);
            
            _clientsRepository.SaveObject(client);
            LogInInfo.UpdateLoggedInClient();
            _clientsRepository.SaveObject(clientTo);
            _operationsRepository.SaveObject(operation);

            ViewBag.Result = Message.OperationSuccessed;
            return View();
        }


        public ActionResult TakeCredit()
        {
            Client client = LogInInfo.LoggedInClient();
            if (client != null)
            {
                if (client.AccountType != "Premium")
                {
                    Session.Add("error", new ErrorNotifier
                    {
                        Source = "take",
                        ActionLink = new Tuple<string, string>("Edit", Url.Action("Edit"))
                    });
                    return RedirectToAction("Error", "Clients", (ErrorNotifier)Session["error"]);
                }

                ListViewModel<Credit> viewModel = new ListViewModel<Credit> {List = Bank.CreditsList()};
                return View(viewModel);
            }
            return new EmptyResult();
        }


        public ActionResult ShowCredit(string type = null)
        {
            Credit credit = Bank.CreditsList().Find(cr => cr.Type == type);
            Session["credit"] = credit;

            Client client = LogInInfo.LoggedInClient();
            if (client != null && client.GetObjects(_creditsRepository).Any(cr => cr.Type == credit.Type))
            {
                Session.Add("error", new ErrorNotifier
                {
                    Source = "takeCredit",
                    ActionLink = new Tuple<string, string>("Pay", Url.Action("PayCredit"))
                });
                return RedirectToAction("Error", "Clients", (ErrorNotifier)Session["error"]);
            }
            return View(credit);
         }


        [HttpPost, ActionName("ShowCredit")]
        [ValidateAntiForgeryToken]
        public ActionResult TakeCreditConfirmed(FormCollection form)
        {
            Client client = LogInInfo.LoggedInClient();
            Credit credit = (Credit)Session["credit"];

            if (credit == null)
            {
                return RedirectToAction("TakeCredit");
            }

            int creditSum = int.Parse(form["Sum"]);
            
            if (client.State.BankId != form["BankId"])
            {
                ViewBag.Result = Message.WrongId;
                return View(credit);
            }
            
            credit.Money = creditSum;
            credit.ClientId = client.Id;
         
            MoneyOperations moneyOperation = new MoneyOperations(client);
            
            moneyOperation.TakeCredit(credit);
            _creditsRepository.SaveObject(credit);

            Operation operation = new Operation(DateTime.Now, client.Id, string.Format("Take for {0}", credit.Type),
                String.Empty, String.Empty, client.State.BankId, String.Empty, creditSum);
            _operationsRepository.SaveObject(operation);

            _clientsRepository.SaveObject(client);
            LogInInfo.UpdateLoggedInClient();

            ViewBag.Result = Message.OperationSuccessed;
            return View(credit);
        }


        public ActionResult PayCredit()
        {
            Client client = LogInInfo.LoggedInClient();
            if (client != null)
            {
                if (client.AccountType != "Premium")
                {
                    Session.Add("error", new ErrorNotifier
                    {
                        Source = "pay",
                        ActionLink = new Tuple<string, string>("Edit", Url.Action("Edit"))
                    });
                    return RedirectToAction("Error", "Clients", (ErrorNotifier) Session["error"]);
                }

                List<Credit> credits = client.GetObjects(_creditsRepository);
                ListViewModel<Credit> viewModel = new ListViewModel<Credit>();
                if (credits.Any())
                {
                    viewModel.List = credits;
                    return View(viewModel);
                }

                viewModel.List = new List<Credit>();
                return View(viewModel);
            }
            return RedirectToAction("Login", "Account");
        }


        public ActionResult CreditInfo(int id = 0)
        {
            Credit credit = LogInInfo.LoggedInClient().GetObjects(_creditsRepository).Find(cr => cr.Id == id);
            Session["credit"] = credit;
            return View(credit);
        }


        [HttpPost, ActionName("CreditInfo")]
        [ValidateAntiForgeryToken]
        public ActionResult PayCredit(FormCollection form)
        {
            Client client = LogInInfo.LoggedInClient();
            Credit credit = (Credit)Session["credit"];

            if (credit == null)
            {
                return RedirectToAction("PayCredit");
            }

            string bankId = form["BankId"];
            int creditSum = int.Parse(form["Sum"]);
            
            if (client == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (client.State.BankId != bankId)
            {
                ViewBag.Result = Message.WrongId;
                return View(credit);
            }
            if (creditSum <= 0 || creditSum > 100000000)
            {
                ViewBag.Result = Message.SumIntervalMismatch;
                return View(credit);
            }
            if (creditSum > client.State.BankMoney)
            {
                ViewBag.Result = Message.NotEnoughBankMoney;
                return View(credit);
            }
            if (creditSum > credit.Money)
            {
                ViewBag.Result = Message.WrongSumForCredit;
                return View(credit);
            }

            MoneyOperations moneyOperation = new MoneyOperations(client);

            moneyOperation.PayCredit(creditSum, credit);
            _creditsRepository.SaveObject(credit);

            Operation operation = new Operation(DateTime.Now, client.Id, string.Format("Pay for {0}", credit.Type),
                client.State.BankId, String.Empty, String.Empty, String.Empty, creditSum);
            _operationsRepository.SaveObject(operation);

            _clientsRepository.SaveObject(client);
            LogInInfo.UpdateLoggedInClient();

            ViewBag.Result = Message.OperationSuccessed;
            return View(credit);
        }


        public ActionResult Error()
        {
            ErrorNotifier notifier = (ErrorNotifier) Session["error"];

            if (notifier != null)
            {
                notifier.SetDescription();
                return View(notifier);
            }
            return RedirectToAction("TakeCredit", "Clients");
        }


        public ActionResult OperationsLog(FormCollection form, string filter, int page = 1)
        {
            Client client = LogInInfo.LoggedInClient();
            List<Operation> operations = client.GetObjects(_operationsRepository);

            ListViewModel<Operation> emptyviewModel = new ListViewModel<Operation>
            {
                Filter = null,
                List = new List<Operation>(),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = 0
                }
            };

            ListViewModel<Operation> viewModel = new ListViewModel<Operation>
            {
                List = operations.Skip((page - 1)*PageSize).Take(PageSize),
                Filter = filter,
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = operations.Count()
                }
            };

            if (!operations.Any())
            {
                ViewBag.Error = Message.OperationsNotFound;
                return View(emptyviewModel);
            }

            string sortBy = form["SortBy"] ?? filter ?? "Date";

            operations = ListFilter<Operation>.Filter(operations, sortBy);

            viewModel.List = operations.Skip((page - 1)*PageSize).Take(PageSize);
            viewModel.Filter = sortBy;

            if (form["Load"] == "Xml" && form["LoadFile"] == "true,false")
            {
                Serializer.GetXml(operations, form["File"]);
                ViewBag.IsLoaded = Message.FileLoaded;
                return View(viewModel);
            }

            if (form["Load"] == "Json" && form["LoadFile"] == "true,false")
            {
                Serializer.GetJson(operations, form["File"]);
                ViewBag.IsLoaded = Message.FileLoaded;
                return View(viewModel);
            }
            return View(viewModel);
        }
    
    }
}