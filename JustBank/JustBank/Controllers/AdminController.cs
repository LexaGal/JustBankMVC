using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Services.Description;
using BankModel.Abstract;
using BankModel.Entities;
using BankModel.Entities.Credits;
using BankModel.Entities.Errors;
using BankModel.Entities.Operations;
using BankModel.Serialization;
using JustBank.HtmlHelpers;
using JustBank.Models;
using JustBank.SessionInfo;
using Ninject.Infrastructure.Language;
using Message = JustBank.Messaging.Message;
using Operation = BankModel.Entities.Operations.Operation;

namespace JustBank.Controllers
{
    public class AdminController : Controller
    {
        private IRepository<Client> _clientsRepository;
        private IRepository<Operation> _operationsRepository;
        private IRepository<Credit> _creditsRepository;
        private const int PageSize = 7;

        public AdminController(IRepository<Client> clientsRepository, IRepository<Operation> operationsRepository,
            IRepository<Credit> creditsRepository)
        {
            _clientsRepository = clientsRepository;
            _operationsRepository = operationsRepository;
            _creditsRepository = creditsRepository;
        }

        public ActionResult Search(FormCollection form, string filter = ",", int page = 1)
        {
            List<Client> clients = _clientsRepository.Objects.ToList();

            IQueryable<string> namesQuery = from d in _clientsRepository.Objects
                orderby d.SecondName
                select d.SecondName;

            List<string> namesList = new List<string>();
            namesList.AddRange(namesQuery.Distinct());

            string name = form["Name"] ?? filter.Split(',')[0];
            string surname = form["Surname"] ?? filter.Split(',')[1];

            ViewBag.Name = name;
            ViewBag.Surname = new SelectList(namesList);
            
            ListViewModel<Client> emptyviewModel = new ListViewModel<Client>
            {
                Filter = string.Format("{0}{1}{2}", name, ',', surname),
                List = new List<Client>(),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = 0
                }
            };

            ListViewModel<Client> viewModel = new ListViewModel<Client>
            {
                List = clients.Skip((page - 1) * PageSize).Take(PageSize),
                Filter = string.Format("{0}{1}{2}", name, ',', surname),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = clients.Count()
                }
            };

            if (!String.IsNullOrEmpty(name))
            {
                clients = clients.Where(s => s.FirstName.Contains(name)).ToList();
            }

            if (string.IsNullOrEmpty(surname))
            {
                if (!clients.Any())
                {
                    ViewBag.Error = Message.ClientsNotFound;
                    return View(emptyviewModel);
                }
                viewModel.List = clients.Skip((page - 1)*PageSize).Take(PageSize);
                viewModel.PagingInfo.TotalItems = clients.Count();
                return View(viewModel);
            }

            clients = clients.Where(x => x.SecondName == surname).ToList();

            if (!clients.Any())
            {
               ViewBag.Error = Message.ClientsNotFound;
               return View(emptyviewModel);
            }
            viewModel.List = clients.Skip((page - 1)*PageSize).Take(PageSize);
            viewModel.PagingInfo.TotalItems = clients.Count();
            return View(viewModel);
        }


        public PartialViewResult ShowClients(FormCollection form, string filter, int page = 1)
        {
            string sortBy = form["SortBy"] ?? filter ?? "Id";

            List<Client> clients = ListFilter<Client>.Filter(_clientsRepository.Objects.ToList(), sortBy);

            ListViewModel<Client> viewModel = new ListViewModel<Client>
            {
                List = clients.Skip((page - 1)*PageSize).Take(PageSize),
                Filter = sortBy,
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = clients.Count()
                }
            };
            viewModel.PagesString = PagingHelper.PageLinks(viewModel.PagingInfo,
                x => Url.Action("ShowClients", new {filter = viewModel.Filter, page = x}), "table");

            if (form["Load"] == "Xml" && form["LoadFile"] == "true,false")
            {
                Serializer.GetXml(clients, form["File"]);
                ViewBag.IsLoaded = Message.FileLoaded;
                return PartialView(viewModel);
            }
            if (form["Load"] == "Json" && form["LoadFile"] == "true,false")
            {
                Serializer.GetJson(clients, form["File"]);
                ViewBag.IsLoaded = Message.FileLoaded;
                return PartialView(viewModel);
            }

            if (clients.Count() != 0)
            {
                return PartialView(viewModel);
            }
            viewModel.List = ListFilter<Client>.Filter(clients, "Id");
            return PartialView(viewModel);
        }


        public ActionResult Index()
        {
            ListViewModel<Client> viewModel = new ListViewModel<Client>
            {
                Filter = "FirstName",
                PagingInfo = new PagingInfo
                {
                    CurrentPage = 1,
                    ItemsPerPage = PageSize,
                    TotalItems = _clientsRepository.Objects.Count()
                }
            };
            return View(viewModel);
        }


        public ActionResult Create()
        {
            Session["url"] = Url.Action("Index", "Admin");
            return RedirectToAction("CreateNew", "Clients");
        }


        public ActionResult DeleteClient(int id = 0)
        {
            Client client = _clientsRepository.Objects.SingleOrDefault(c => c.Id == id);
            if (client == null)
            {
                return new EmptyResult();
            }
            return View(client);
        }


        [HttpPost, ActionName("DeleteClient")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteClientConfirmed(int id)
        {
            Client client = _clientsRepository.Objects.First(c => c.Id == id);
            _clientsRepository.DeleteObject(id);            
            foreach (var o in client.GetObjects(_operationsRepository))
            {
                _operationsRepository.DeleteObject(o.Id);
            }
            foreach (var c in client.GetObjects(_creditsRepository))
            {
                _creditsRepository.DeleteObject(c.Id);
            }
            return RedirectToAction("Index", "Admin");
        }


        public PartialViewResult ShowOperations(FormCollection form, string filter, int page = 1, int id = 0)
        {
            List<Operation> operations;
            if (id == 0)
            {
                operations = ListFilter<Operation>.Filter(_operationsRepository.Objects.ToList(), "Date");
            }
            else if (id > 0)
            {
                operations = ListFilter<Operation>.Filter(_clientsRepository.Objects.
                    First(c => c.Id == id).GetObjects(_operationsRepository).ToList(), "Date");
            }
            else
            {
                operations = new List<Operation>();
            }
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

            viewModel.PagesString = PagingHelper.PageLinks(viewModel.PagingInfo,
                x => Url.Action("ShowOperations", new 
                { 
                    filter = viewModel.Filter,
                    page = x,
                    id = viewModel.List.First().ClientId
                }), "table");

            if (!operations.Any())
            {
                ViewBag.Error = Message.OperationsNotFound;
                return PartialView(emptyviewModel);
            }

            string sortBy = form["SortBy"] ?? filter ?? "Date";

            operations = ListFilter<Operation>.Filter(operations, sortBy);

            viewModel.List = operations.Skip((page - 1)*PageSize).Take(PageSize);
            viewModel.Filter = sortBy;
            viewModel.PagesString = PagingHelper.PageLinks(viewModel.PagingInfo,
               x => Url.Action("ShowOperations", new
               {
                   filter = viewModel.Filter,
                   page = x,
                   id = viewModel.List.First().ClientId
               }), "table");


            if (form["Load"] == "Xml" && form["LoadFile"] == "true,false")
            {
                Serializer.GetXml(operations, form["File"]);
                ViewBag.IsLoaded = Message.FileLoaded;
                return PartialView(viewModel);
            }

            if (form["Load"] == "Json" && form["LoadFile"] == "true,false")
            {
                Serializer.GetJson(operations, form["File"]);
                ViewBag.IsLoaded = Message.FileLoaded;
                return PartialView(viewModel);
            }
            return PartialView(viewModel);
        }


        public ActionResult OperationsLog(int id = 0)
        {
            ListViewModel<Operation> viewModel = new ListViewModel<Operation>
            {
                Filter = "Date",
                PagingInfo = new PagingInfo
                {
                    CurrentPage = 1,
                    ItemsPerPage = PageSize,
                }
            };
            if (id == 0)
            {
                viewModel.List = _operationsRepository.Objects;
                viewModel.PagingInfo.TotalItems = _operationsRepository.Objects.Count();
                return View(viewModel);
            }
            viewModel.List = _operationsRepository.Objects.Where(op => op.ClientId == id);
            viewModel.PagingInfo.TotalItems = _operationsRepository.Objects.Count(op => op.ClientId == id);
            return View(viewModel);
        }


        public ActionResult DeleteLog(int id = 0)
        {
            Operation log = _operationsRepository.Objects.SingleOrDefault(c => c.Id == id);
            if (log == null)
            {
                return new EmptyResult();
            }
            return View(log);
        }


        [HttpPost, ActionName("DeleteLog")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteLogConfirmed(int id)
        {
            int clientId = _operationsRepository.Objects.First(o => o.Id == id).ClientId;
            _operationsRepository.DeleteObject(id);
            return RedirectToAction("OperationsLog", "Admin", new { id = clientId });
        }


        public ActionResult Update(int id = 0)
        {
            Client client = _clientsRepository.Objects.SingleOrDefault(c => c.Id == id);
            return View(client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(FormCollection form)
        {
            string button = form["Update"];

            int id = int.Parse(form["Id"]);
            Client client = _clientsRepository.Objects.SingleOrDefault(c => c.Id == id);

            if (client == null)
            {
                return View();
            }

            if (button == "CreditsMoney")
            {
                List<Credit> credits = client.GetObjects(_creditsRepository);

                if (client.AccountType == "Usual" || credits.Count == 0)
                {
                    ErrorNotifier notifier = new ErrorNotifier
                    {
                        Source = "updateCredits"
                    };
                    notifier.SetDescription();
                    return RedirectToAction("Error", "Admin", notifier);
                }

                MoneyOperations moneyOperation = new MoneyOperations(client);
                moneyOperation.UpdateClientCredits(credits);

                foreach (Credit credit in credits)
                {
                    Operation operation = new Operation(DateTime.Now, client.Id,
                        string.Format("Update credit for {0}", credit.Type),
                        String.Empty, String.Empty, client.State.BankId, String.Empty, credit.Money*credit.Procents/100);
                    _operationsRepository.SaveObject(operation);
                }
                _clientsRepository.SaveObject(client);

            }

            if (button == "BankMoney")
            {
                if (client.State.BankMoney == 0)
                {
                    ErrorNotifier notifier = new ErrorNotifier
                    {
                        Source = "updateBank"
                    };
                    notifier.SetDescription();
                    return RedirectToAction("Error", "Admin", notifier);
                }

                int procents = client.State.BankMoney * client.State.BankProcents / 100;
                MoneyOperations moneyOperation = new MoneyOperations(client);
                moneyOperation.UpdateClientBankMoney();

                Operation operation = new Operation(DateTime.Now, client.Id,
                    string.Format("Update bank money"),
                    String.Empty, String.Empty, client.State.BankId, String.Empty, procents);
                _operationsRepository.SaveObject(operation);

                _clientsRepository.SaveObject(client);
            }
            return RedirectToAction("OperationsLog", "Admin");
        }
        

        public ActionResult Error(ErrorNotifier notifier)
        {
            return View(notifier);
        }


        public PartialViewResult ShowCredits(FormCollection form, string filter, int page = 1, int id = 0)
        {
            List<Credit> credits;
            if (id == 0)
            {
                credits = ListFilter<Credit>.Filter(_creditsRepository.Objects.ToList(), "Date");
            }
            else if (id > 0)
            {
                credits = ListFilter<Credit>.Filter(_creditsRepository.Objects
                    .Where(cr => cr.ClientId == id).ToList(), "Date");
            }
            else
            {
                credits = new List<Credit>();
            }
            ListViewModel<Credit> emptyviewModel = new ListViewModel<Credit>
            {
                Filter = null,
                List = new List<Credit>(),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = 0
                }
            };

            ListViewModel<Credit> viewModel = new ListViewModel<Credit>
            {
                List = credits.Skip((page - 1)*PageSize).Take(PageSize),
                Filter = filter,
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = credits.Count()
                }
            };

            viewModel.PagesString = PagingHelper.PageLinks(viewModel.PagingInfo,
               x => Url.Action("ShowCredits", new
               {
                   filter = viewModel.Filter,
                   page = x,
                   id = viewModel.List.First().ClientId
               }), "table");

            if (!credits.Any())
            {
                ViewBag.Error = Message.CreditsNotFound;
                return PartialView(emptyviewModel);
            }

            string sortBy = form["SortBy"] ?? filter ?? "Date";

            credits = ListFilter<Credit>.Filter(credits, sortBy);

            viewModel.List = credits.Skip((page - 1)*PageSize).Take(PageSize);
            viewModel.Filter = sortBy;
            viewModel.PagesString = PagingHelper.PageLinks(viewModel.PagingInfo,
               x => Url.Action("ShowCredits", new
               {
                   filter = viewModel.Filter,
                   page = x,
                   id = viewModel.List.First().ClientId
               }), "table");

            if (form["Load"] == "Xml" && form["LoadFile"] == "true,false")
            {
                Serializer.GetXml(credits, form["File"]);
                ViewBag.IsLoaded = Message.FileLoaded;
                return PartialView(viewModel);
            }

            if (form["Load"] == "Json" && form["LoadFile"] == "true,false")
            {
                Serializer.GetJson(credits, form["File"]);
                ViewBag.IsLoaded = Message.FileLoaded;
                return PartialView(viewModel);
            }
            return PartialView(viewModel);
        }


        public ActionResult CreditsInfo(int id = 0)
        {
            ListViewModel<Credit> viewModel = new ListViewModel<Credit>
            {
                Filter = "Date",
                PagingInfo = new PagingInfo
                {
                    CurrentPage = 1,
                    ItemsPerPage = PageSize,
                }
            };
            if (id == 0)
            {
                viewModel.List = _creditsRepository.Objects;
                viewModel.PagingInfo.TotalItems = _creditsRepository.Objects.Count();
                return View(viewModel);
            }
            viewModel.List = _creditsRepository.Objects.Where(op => op.ClientId == id);
            viewModel.PagingInfo.TotalItems = _creditsRepository.Objects.Count(op => op.ClientId == id);
            return View(viewModel);
        }


        public ActionResult DeleteCredit(int id = 0)
        {
            Credit credit = _creditsRepository.Objects.SingleOrDefault(c => c.Id == id);
            if (credit == null)
            {
                return new EmptyResult();
            }
            return View(credit);
        }


        [HttpPost, ActionName("DeleteCredit")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCreditConfirmed(int id)
        {
            int clientId = _creditsRepository.Objects.First(cr => cr.Id == id).ClientId;
            _creditsRepository.DeleteObject(id);
            return RedirectToAction("CreditsInfo", "Admin", new {id = clientId});
        }
    }
}