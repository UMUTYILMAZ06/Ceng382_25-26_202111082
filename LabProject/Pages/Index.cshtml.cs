using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LabProject.Models;

namespace LabProject.Pages
{
    public class IndexModel : PageModel
    {
        // Static list to store class data (temporary storage)
        public static List<ClassInformationModel> ClassList { get; set; } = new List<ClassInformationModel>();

        [BindProperty]
        public ClassInformationModel NewClass { get; set; } = new ClassInformationModel();

        // Track editing status
        [BindProperty]
        public bool IsEditing { get; set; } = false;

        public void OnGet()
        {
        }

        // POST: Add New Class
        public IActionResult OnPostAdd()
        {
            if (ModelState.IsValid)
            {
                NewClass.Id = ClassList.Count + 1; // Auto-increment Id
                ClassList.Add(NewClass);
                return RedirectToPage("/Index"); // Sayfayı yenile
            }
            return Page();
        }

        // Handle Delete Action
        public IActionResult OnGetDelete(int id)
        {
            var classToDelete = ClassList.FirstOrDefault(c => c.Id == id);
            if (classToDelete != null)
            {
                ClassList.Remove(classToDelete);
            }
            return RedirectToPage("/Index");
        }

        // Handle Edit Action
        public IActionResult OnGetEdit(int id)
        {
            var classToEdit = ClassList.FirstOrDefault(c => c.Id == id);
            if (classToEdit != null)
            {
                NewClass = classToEdit;
                IsEditing = true; // Set editing mode to true
            }
            return Page();
        }

        // POST: Update Class
        public IActionResult OnPostUpdate()
        {
            if (ModelState.IsValid)
            {
                var existingClass = ClassList.FirstOrDefault(c => c.Id == NewClass.Id);
                if (existingClass != null)
                {
                    ClassList.Remove(existingClass);
                }
                ClassList.Add(NewClass);
                IsEditing = false; // Turn off editing mode after update
                return RedirectToPage("/Index");
            }
            return Page();
        }
    }
}
