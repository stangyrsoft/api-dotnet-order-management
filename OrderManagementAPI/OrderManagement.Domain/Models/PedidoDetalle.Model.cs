
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManagement.Domain.Models
{
    public class PedidoDetalle : Audit
    {
        [Key]
        [Required]
        public long Id { get; set; }
        [Required]
        public long ProductoId { get; set; }
        [Required]
        public long PedidoId { get; set; }
        [Required]
        public int Cantidad { get; set; }
        [Required]
        [Column(TypeName ="decimal(14,2)")]
        public decimal PrecioUnitario { get; set; }
        [ForeignKey("ProductoId")]
        public Producto? Producto { get; set; }
        [ForeignKey("PedidoId")]
        public Pedido? Pedido { get; set; }
    }
}
