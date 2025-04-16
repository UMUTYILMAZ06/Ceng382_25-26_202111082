using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LabProject.Models;
using System.Linq;
using System.Text.Json;

namespace LabProject.Pages
{
    public class IndexModel : PageModel
    {
        public static List<ClassInformationModel> ClassList { get; set; } = GenerateFakeData();

        // Filtreleme
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        // Sayfalama
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }

        public List<ClassInformationTable> PaginatedData { get; set; } = new();

        [BindProperty]
        public ClassInformationModel NewClass { get; set; } = new();

        [BindProperty]
        public bool IsEditing { get; set; }

        [BindProperty]
        public string[] SelectedColumns { get; set; } = Array.Empty<string>();

        [BindProperty]
        public string ExportMode { get; set; } = "All"; // "All" or "Filtered"

        public void OnGet()
        {
            ApplyFilteringAndPaging();
        }

        public IActionResult OnPostAdd()
        {
            if (!ModelState.IsValid)
            {
                ApplyFilteringAndPaging();
                return Page();
            }

            // Add new class to list dynamically
            NewClass.Id = ClassList.Any() ? ClassList.Max(c => c.Id) + 1 : 1;
            ClassList.Add(NewClass);

            return RedirectToPage();
        }

        public IActionResult OnPostUpdate()
        {
            if (!ModelState.IsValid)
            {
                ApplyFilteringAndPaging();
                return Page();
            }

            // Update existing class in the list dynamically
            var existing = ClassList.FirstOrDefault(c => c.Id == NewClass.Id);
            if (existing != null)
            {
                existing.ClassName = NewClass.ClassName;
                existing.StudentCount = NewClass.StudentCount;
                existing.Description = NewClass.Description;
            }

            IsEditing = false;
            return RedirectToPage();
        }

        public IActionResult OnGetDelete(int id)
        {
            var item = ClassList.FirstOrDefault(c => c.Id == id);
            if (item != null)
                ClassList.Remove(item);

            return RedirectToPage();
        }

        public IActionResult OnGetEdit(int id)
        {
            var item = ClassList.FirstOrDefault(c => c.Id == id);
            if (item != null)
            {
                NewClass = new ClassInformationModel
                {
                    Id = item.Id,
                    ClassName = item.ClassName,
                    StudentCount = item.StudentCount,
                    Description = item.Description
                };
                IsEditing = true;
            }

            ApplyFilteringAndPaging();
            return Page();
        }

        private void ApplyFilteringAndPaging()
        {
            var query = ClassList.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
                query = query.Where(c => c.ClassName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));

            var totalItems = query.Count();
            TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            PaginatedData = query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .Select(c => new ClassInformationTable
                {
                    Id = c.Id,
                    ClassName = c.ClassName,
                    StudentCount = c.StudentCount,
                    Description = c.Description
                })
                .ToList();
        }

        private static List<ClassInformationModel> GenerateFakeData()
        {
            var list = new List<ClassInformationModel>();
            for (int i = 1; i <= 100; i++)
            {
                list.Add(new ClassInformationModel
                {
                    Id = i,
                    ClassName = $"Class {i}",
                    StudentCount = i % 30 + 1,
                    Description = $"Description {i}"
                });
            }
            return list;
        }

        public IActionResult OnPostExportJson()
        {
            List<ClassInformationTable> sourceData;

            if (ExportMode == "Filtered")
            {
                var query = ClassList.AsQueryable();
                if (!string.IsNullOrWhiteSpace(SearchTerm))
                    query = query.Where(c => c.ClassName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));

                sourceData = query.Select(c => new ClassInformationTable
                {
                    Id = c.Id,
                    ClassName = c.ClassName,
                    StudentCount = c.StudentCount,
                    Description = c.Description
                }).ToList();
            }
            else
            {
                sourceData = ClassList.Select(c => new ClassInformationTable
                {
                    Id = c.Id,
                    ClassName = c.ClassName,
                    StudentCount = c.StudentCount,
                    Description = c.Description
                }).ToList();

                // ExportMode "All" ise tüm sütunları dahil et
                SelectedColumns = new[] { "ClassName", "StudentCount", "Description" };
            }

            var columns = SelectedColumns.Length == 0
                ? new[] { "ClassName", "StudentCount", "Description" }
                : SelectedColumns;

            var exportData = sourceData.Select(item =>
            {
                var dict = new Dictionary<string, object>();
                if (columns.Contains("ClassName")) dict["ClassName"] = item.ClassName;
                if (columns.Contains("StudentCount")) dict["StudentCount"] = item.StudentCount;
                if (columns.Contains("Description")) dict["Description"] = item.Description;
                return dict;
            }).ToList();

            var json = JsonSerializer.Serialize(exportData, new JsonSerializerOptions { WriteIndented = true });
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            return File(bytes, "application/json", $"export_{DateTime.Now:yyyyMMdd_HHmmss}.json");
        }
    }
}
