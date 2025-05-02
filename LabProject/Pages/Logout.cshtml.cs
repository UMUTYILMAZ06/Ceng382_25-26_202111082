using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LabProject.Pages
{
    public class LogoutModel : PageModel
    {
        public void OnGet()
        {
            // Oturum temizle
            HttpContext.Session.Clear();

            // Çerezleri temizle
            Response.Cookies.Delete("username");
            Response.Cookies.Delete("token");
            Response.Cookies.Delete("session_id");
        }
    }
}
