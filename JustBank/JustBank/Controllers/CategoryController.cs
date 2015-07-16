using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BankModel.Abstract;
using BankModel.Entities;

namespace JustBank.Controllers
{
    public class CategoryController : Controller
    {
        private IRepository<Client> _repository;

        public CategoryController(IRepository<Client> repo)
        {
            _repository = repo;
        }

        public PartialViewResult Categories(string category = null)
        {
            ViewBag.SelectedCategory = category;
            IEnumerable<string> categories = _repository.Objects
                .Select(x => x.AccountType)
                .Distinct()
                .OrderBy(x => x);
            return PartialView(categories);

        }
    }
}