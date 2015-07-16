using System.Web;
using System.Web.Mvc;
using System.Web.Providers.Entities;
using BankModel.Entities;

namespace JustBank.Attributes
{
 public class BankAuthAttribute : AuthorizeAttribute
 {
     private bool _localAllowed;

     public BankAuthAttribute(bool allowedParam)
     {
         _localAllowed = allowedParam;
     }

     protected override bool AuthorizeCore(HttpContextBase httpContext)
     {
         if (httpContext.Request.IsLocal)
         {
             return _localAllowed;
         }
         return true;
     }
 }
}
