using System.Web.Mvc;
using System.Web.UI.WebControls;
using BankModel.Abstract;
using BankModel.Entities;
using JustBank.Authentification.Abstract;
using JustBank.Models;
using JustBank.SessionInfo;

namespace JustBank.Controllers
{
    public class AccountController : Controller
    {
        private IAuthProvider _authProvider;
        
        public AccountController(IAuthProvider auth)
        {
            _authProvider = auth;
        }
        
        public ViewResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                Client client;
                if ((client = _authProvider.Authenticate(model.UserName, model.Password)) != null)
                {
                    LogInInfo.LogClientIn(client);
                    
                    if (client.FirstName == "a")
                    {
                        return Redirect(returnUrl ?? Url.Action("Index", "Admin"));                           
                    }
                    return Redirect(returnUrl ?? Url.Action("Index", "Clients"));
                }

                ModelState.AddModelError("", "Incorrect username or password");
                return View();
            }

            return View();
        }

        public ActionResult Register()
        {
            return View("Login");
        }

        public ActionResult ExternalLoginsList(object returnurl)
        {
            return View("Error");
        }
        
        public ActionResult Manage()
        {
            return View("Error");
        }

        public ActionResult LogOff()
        {
            LogInInfo.LogClientOut();
            return RedirectToAction("Login", "Account");
        }
    }
}