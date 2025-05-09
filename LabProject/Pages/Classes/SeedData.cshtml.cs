using LabProject.Data;
using LabProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LabProject.Pages.Classes
{
    public class SeedDataModel : PageModel
    {
        private readonly SchoolDbContext _context;

        public SeedDataModel(SchoolDbContext context)
        {
            _context = context;
        }

        public void OnGet() { }

        public IActionResult OnPost()
        {
            for (int i = 1; i <= 100; i++)
            {
                if (!_context.Classes.Any(c => c.Name == $"Class {i}"))
                {
                    _context.Classes.Add(new Class
                    {
                        Name = $"Class {i}",
                        PersonCount = i * 10,
                        Description = $"Auto generated class {i}",
                        IsActive = true
                    });
                }
            }

            _context.SaveChanges();
            return RedirectToPage("/Classes/Index");
        }
    }
}
