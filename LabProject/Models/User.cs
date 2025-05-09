using System;
using System.ComponentModel.DataAnnotations;

namespace LabProject.Models
{
    public class User
    {
        [Key]  // ✅ Primary Key belirtiyoruz
        public int Id { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
