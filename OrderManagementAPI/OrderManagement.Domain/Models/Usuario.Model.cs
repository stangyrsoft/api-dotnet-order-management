
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManagement.Domain.Models
{
    public class Usuario : Audit
    {
        [Key]
        [Required]
        public long Id { get; set; }
        [Required]
        [Column(TypeName = "varchar(50)")]
        public string? Email { get; set; }
        [Required]
        [Column(TypeName = "varchar(250)")]
        public string? Password { get; set; }
    }
}
