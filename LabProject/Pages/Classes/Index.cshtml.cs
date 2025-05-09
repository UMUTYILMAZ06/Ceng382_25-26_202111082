using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LabProject.Data;
using LabProject.Models;
using System.Text.Json;

namespace LabProject.Pages.Classes
{
    public class IndexModel : PageModel
    {
        private readonly SchoolDbContext _context;

        public IndexModel(SchoolDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Class NewClass { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }

        public IList<Class> ClassList { get; set; } = new List<Class>();

        [BindProperty]
        public bool IsEditing { get; set; }

        private bool IsUserLoggedIn() => Request.Cookies["loggedUser"] != null;

        public async Task<IActionResult> OnGetAsync()
        {
            if (!IsUserLoggedIn())
                return RedirectToPage("/Login");

            // ✅ Otomatik veri ekleme
            if (!await _context.Classes.AnyAsync())
            {
                var autoClasses = Enumerable.Range(1, 100).Select(i => new Class
                {
                    Name = $"Class {i}",
                    PersonCount = i * 10,
                    Description = $"Auto generated description {i}",
                    IsActive = true
                }).ToList();

                await _context.Classes.AddRangeAsync(autoClasses);
                await _context.SaveChangesAsync();
            }

            await LoadDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAddAsync()
        {
            if (!IsUserLoggedIn()) return RedirectToPage("/Login");

            if (!ModelState.IsValid)
            {
                await LoadDataAsync();
                return Page();
            }

            NewClass.IsActive = true;
            _context.Classes.Add(NewClass);
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateAsync()
        {
            if (!IsUserLoggedIn()) return RedirectToPage("/Login");

            if (!ModelState.IsValid)
            {
                await LoadDataAsync();
                return Page();
            }

            _context.Classes.Update(NewClass);
            await _context.SaveChangesAsync();
            IsEditing = false;
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetEditAsync(int id)
        {
            if (!IsUserLoggedIn()) return RedirectToPage("/Login");

            var item = await _context.Classes.FindAsync(id);
            if (item != null)
            {
                NewClass = item;
                IsEditing = true;
            }

            await LoadDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            if (!IsUserLoggedIn()) return RedirectToPage("/Login");

            var existing = await _context.Classes.FindAsync(id);
            if (existing != null)
            {
                existing.IsActive = false;
                _context.Classes.Update(existing);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostExportJsonAsync()
        {
            if (!IsUserLoggedIn()) return RedirectToPage("/Login");

            var query = _context.Classes.Where(c => c.IsActive);

            if (!string.IsNullOrWhiteSpace(SearchTerm))
                query = query.Where(c => c.Name.Contains(SearchTerm));

            var exportData = await query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .Select(c => new { c.Name, c.PersonCount, c.Description })
                .ToListAsync();

            var json = JsonSerializer.Serialize(exportData, new JsonSerializerOptions { WriteIndented = true });
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);

            return File(bytes, "application/json", $"class_export_{DateTime.Now:yyyyMMdd_HHmmss}.json");
        }

        private async Task LoadDataAsync()
        {
            var query = _context.Classes.Where(c => c.IsActive);

            if (!string.IsNullOrWhiteSpace(SearchTerm))
                query = query.Where(c => c.Name.Contains(SearchTerm));

            var totalItems = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            ClassList = await query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }
    }
}
