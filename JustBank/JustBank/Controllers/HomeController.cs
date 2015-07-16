using System;

using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using BankModel.Entities;
using BankModel.Entities.Operations;

namespace JustBank.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult CurrencyConverter(FormCollection convertation)
        {
            CurrencyConverter currencyConverter = new CurrencyConverter();

            var fromList = Convertation.Currencies;
            var toList = Convertation.Currencies;

            ViewBag.ConvertFrom = new SelectList(fromList);
            ViewBag.ConvertTo = new SelectList(toList);

            string from = convertation["ConvertFrom"];
            string to = convertation["ConvertTo"];

            foreach (var item in currencyConverter.Convertations)
            {
                if (item.ConvertFrom == from && item.ConvertTo == to)
                {
                    try
                    {
                        var sumFrom = Convert.ToDouble(convertation["SumFrom"]);
                        var sumTo = sumFrom*item.CoeffValue;

                        ViewBag.ConvertFrom = from;
                        ViewBag.ConvertTo = to;
                        ViewBag.SumTo = sumTo;
                        ViewBag.SumFrom = sumFrom;
                    }
                    catch (FormatException)
                    {}
                    break;
                }
            }
            return View();
        }
    }
}