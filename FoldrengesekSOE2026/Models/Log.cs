using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoldrengesekSOE2026.Models
{
    [Table("logs")]
    public class Log
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        public string? UserId { get; set; }

        public string? UserEmail { get; set; }

        [Required]
        [MaxLength(10)]
        public string HttpMethod { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Path { get; set; } = string.Empty;

        [Required]
        public int StatusCode { get; set; }

        [MaxLength(1000)]
        public string? Message { get; set; }

        [Required]
        [MaxLength(20)]
        public string LogLevel { get; set; } = string.Empty;

        [Required]
        public bool IsAuthFailure { get; set; }

        [MaxLength(50)]
        public string? IpAddress { get; set; }

        [MaxLength(500)]
        public string? UserAgent { get; set; }

        [MaxLength(50)]
        public string? EntityType { get; set; }

        [MaxLength(50)]
        public string? EntityId { get; set; }

        [MaxLength(50)]
        public string? Action { get; set; }

        [Column(TypeName = "text")]
        public string? OldValue { get; set; }

        [Column(TypeName = "text")]
        public string? NewValue { get; set; }
    }
}
