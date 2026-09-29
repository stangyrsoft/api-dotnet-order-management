
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManagement.Domain.Models
{
    public class Cliente : Audit
    {
        [Key]
        [Required]
        public long Id { get; set; }
        [Required]
        [Column(TypeName = "varchar(100)")]
        public string? Nombre { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string? Apellido { get; set; } = null;
        [Column(TypeName = "text")]
        public string? Direccion { get; set; }
        [Required]
        [Column(TypeName = "char(8)")]
        public string? DNI { get; set; }
    }
}
