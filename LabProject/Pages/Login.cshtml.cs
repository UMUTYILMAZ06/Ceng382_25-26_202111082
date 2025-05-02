using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using LabProject.Models;

namespace LabProject.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet() { }

        public IActionResult OnPost()
        {
            var filePath = Path.Combine("wwwroot", "data", "users.json");
            if (!System.IO.File.Exists(filePath))
            {
                ErrorMessage = "User data not found.";
                return Page();
            }

            var json = System.IO.File.ReadAllText(filePath);
            var users = JsonSerializer.Deserialize<List<User>>(json);

            var matchedUser = users?.FirstOrDefault(u =>
                u.Username == Username &&
                u.Password == Password &&
                u.IsActive);

            if (matchedUser != null)
            {
                var token = Guid.NewGuid().ToString();
                var sessionId = HttpContext.Session.Id;

                // Session
                HttpContext.Session.SetString("username", matchedUser.Username);
                HttpContext.Session.SetString("token", token);
                HttpContext.Session.SetString("session_id", sessionId);

                // Cookie
                var options = new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddMinutes(30),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                };
                Response.Cookies.Append("username", matchedUser.Username, options);
                Response.Cookies.Append("token", token, options);
                Response.Cookies.Append("session_id", sessionId, options);

                return RedirectToPage("/Index");
            }
            else
            {
                ErrorMessage = "Invalid username or password.";
                return Page();
            }
        }
    }
}
