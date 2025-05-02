// Helpers/BasePageModel.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LabProject.Helpers
{
    public class BasePageModel : PageModel
    {
        public override void OnPageHandlerExecuting(Microsoft.AspNetCore.Mvc.Filters.PageHandlerExecutingContext context)
        {
            var username = HttpContext.Session.GetString("username");

            // Eğer kullanıcı login değilse ve sayfa login değilse, login sayfasına yönlendir
            var currentPage = context.HttpContext.Request.Path.Value?.ToLower();
            if (string.IsNullOrEmpty(username) && !currentPage!.Contains("login"))
            {
                context.Result = new RedirectToPageResult("/Login");
            }

            base.OnPageHandlerExecuting(context);
        }
    }
}
