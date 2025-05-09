using LabProject.Data; // DbContext erişimi için
using LabProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LabProject.Pages
{
    public class LoginModel : PageModel
    {
        private readonly SchoolDbContext _context;

        public LoginModel(SchoolDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Username and password are required.";
                return Page();
            }

            // Veritabanındaki Users tablosundan kullanıcıyı ara
            var matchedUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == Username && u.Password == Password && u.IsActive);

            if (matchedUser == null)
            {
                ErrorMessage = "Invalid username or password.";
                return Page();
            }

            // Session
            var token = Guid.NewGuid().ToString();
            HttpContext.Session.SetString("username", matchedUser.Username);
            HttpContext.Session.SetString("token", token);

            // Cookie
            var options = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddMinutes(30),
                HttpOnly = true,
                SameSite = SameSiteMode.Strict
            };

            Response.Cookies.Append("loggedUser", matchedUser.Username, options);

            return RedirectToPage("/Classes/Index");
        }
    }
}
