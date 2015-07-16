using System.Collections.Generic;
using System.Web.Mvc;
using BankModel.Entities;

namespace JustBank.Models
{
    public class ListViewModel<T> where T: class 
    {
        public IEnumerable<T> List { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public string Filter { get; set; }
        public MvcHtmlString PagesString { get; set; }
        public bool IsForAllIds { get; set; }
    }
}