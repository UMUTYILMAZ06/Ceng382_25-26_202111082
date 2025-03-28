using System.ComponentModel.DataAnnotations;

namespace LabProject.Models
{
    public class ClassInformationModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string ClassName { get; set; } = string.Empty; // Boş değeri önlemek için varsayılan değer

        [Range(0, 1000)]
        public int StudentCount { get; set; }

        public string Description { get; set; } = string.Empty; // Boş değeri önlemek için varsayılan değer
    }
}
