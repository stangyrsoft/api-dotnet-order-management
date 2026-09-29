
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManagement.Domain.Models
{
    public class Pedido : Audit
    {
        [Key]
        [Required]
        public long Id { get; set; }
        [Required]
        [Column(TypeName ="varchar(50)")]
        public string? NumeroPedido { get; set; }
        [Required]
        [Column(TypeName = "varchar(50)")]
        public string? Estado { get; set; }
        [Column(TypeName = "text")]
        public string? Observacion { get; set; }
        [Required]
        public long ClienteId { get; set; }
        [Required]
        [Column(TypeName = "TIMESTAMP")]
        public DateTime FechaPedido { get; set; }
        [Required]
        [Column(TypeName = "numeric(10,2)")]
        public decimal TotalImporte { get; set; }
        [ForeignKey("ClienteId")]
        public Cliente? Cliente { get; set; }
        public ICollection<PedidoDetalle> Detalles { get; set; } = new List<PedidoDetalle>();
    }
}
