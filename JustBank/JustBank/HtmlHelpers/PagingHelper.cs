using System;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using JustBank.Models;

namespace JustBank.HtmlHelpers
{
    public static class PagingHelper
    {
        public static MvcHtmlString PageLinks(PagingInfo pagingInfo, Func<int, string> pageUrl, string tagId)
        {
            StringBuilder result = new StringBuilder();

            for (int i = 1; i <= pagingInfo.TotalPages; i++)
            {
                TagBuilder tag = new TagBuilder("a");
                tag.MergeAttribute("data-ajax", "true");
                tag.MergeAttribute("data-ajax-mode", "replace");
                tag.MergeAttribute("data-ajax-update", string.Format("{0}{1}", '#', tagId));
                tag.MergeAttribute("href", pageUrl(i));
                tag.InnerHtml = i.ToString();

                if (i == pagingInfo.CurrentPage)
                {
                    tag.AddCssClass("selected");
                }
                result.Append(tag);
            }
            return MvcHtmlString.Create(result.ToString());
        }
    }
}