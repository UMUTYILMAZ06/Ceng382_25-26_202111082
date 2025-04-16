namespace LabProject.Models
{
    public class ClassInformationTable
    {
        public int Id { get; set; } // Bu sadece arkaplanda kullanılacak
        public string ClassName { get; set; } = string.Empty;
        public int StudentCount { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
