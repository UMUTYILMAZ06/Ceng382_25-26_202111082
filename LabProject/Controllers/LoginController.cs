using Microsoft.AspNetCore.Mvc;

namespace LabProject.Controllers
{
    public class LoginController : Controller
    {
        // GET: /Login
        [HttpGet]
        public IActionResult Index()
        {
            return View(); // Views/Login/Index.cshtml dosyasını çağırır
        }

        // POST: /Login
        [HttpPost]
        public IActionResult Index(string username, string password)
        {
            // Örnek: basit bir kullanıcı doğrulama (gerçek projede veritabanı kullanılır)
            if (username == "admin" && password == "1234")
            {
                ViewBag.Message = "Giriş başarılı!";
                return RedirectToAction("Welcome");
            }
            else
            {
                ViewBag.Message = "Hatalı kullanıcı adı veya şifre.";
                return View();
            }
        }

        // GET: /Login/Welcome
        public IActionResult Welcome()
        {
            return View(); // Views/Login/Welcome.cshtml sayfası varsa onu gösterir
        }
    }
}
