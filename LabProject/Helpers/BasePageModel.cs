using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LabProject.Helpers
{
    public class BasePageModel : PageModel
    {
        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            var username = HttpContext.Session.GetString("username");
            var currentPage = context.HttpContext.Request.Path.Value?.ToLower();

            if (string.IsNullOrEmpty(username) && !currentPage!.Contains("login"))
            {
                context.Result = new RedirectToPageResult("/Login");
            }

            base.OnPageHandlerExecuting(context);
        }
    }
}