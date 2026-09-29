
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManagement.Domain.Models
{
    public class Producto : Audit
    {
        [Key]
        [Required]
        public long Id { get; set; }
        [Required]
        [Column(TypeName = "varchar(200)")]
        public string? Nombre { get; set; }
        [Required]
        [Column(TypeName = "numeric(14,2)")]
        public decimal Precio { get; set; }
    }
}
