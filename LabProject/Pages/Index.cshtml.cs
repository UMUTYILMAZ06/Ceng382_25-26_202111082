using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LabProject.Models;
using System.Text.Json;

namespace LabProject.Pages
{
    public class IndexModel : PageModel
    {
        public static List<ClassInformationModel> ClassList { get; set; } = GenerateFakeData();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

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
        public string ExportMode { get; set; } = "All";

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
            if (Request.Form.TryGetValue("SelectedColumns", out var selected))
            {
                SelectedColumns = selected.ToString().Split(",", StringSplitOptions.RemoveEmptyEntries);
            }

            var query = ClassList.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
                query = query.Where(c => c.ClassName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));

            var pageData = query
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

            if (SelectedColumns == null || SelectedColumns.Length == 0)
                SelectedColumns = new[] { "ClassName", "StudentCount", "Description" };

            var exportData = pageData.Select(item =>
            {
                var dict = new Dictionary<string, object>();
                if (SelectedColumns.Contains("ClassName")) dict["ClassName"] = item.ClassName;
                if (SelectedColumns.Contains("StudentCount")) dict["StudentCount"] = item.StudentCount;
                if (SelectedColumns.Contains("Description")) dict["Description"] = item.Description;
                return dict;
            }).ToList();

            var json = JsonSerializer.Serialize(exportData, new JsonSerializerOptions { WriteIndented = true });
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            return File(bytes, "application/json", $"filtered_export_{DateTime.Now:yyyyMMdd_HHmmss}.json");
        }
    }
}
