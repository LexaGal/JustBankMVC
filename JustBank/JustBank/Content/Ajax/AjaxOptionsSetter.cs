using System;
using System.Security.Policy;
using System.Web.Mvc.Ajax;

namespace JustBank.Content.Ajax
{
    public static class AjaxOptionsSetter
    {
        private static AjaxOptions _ajaxOpts;

        public static AjaxOptions Set(string id, string url)
        {
            return _ajaxOpts = new AjaxOptions
            {
                HttpMethod = "Post",
                InsertionMode = InsertionMode.Replace,
                UpdateTargetId = id,
                Url = url,
                LoadingElementId = "loading",
                LoadingElementDuration = 2000
            };
        }
    }
}