namespace OrderManagement.Domain.Models
{
    public class Audit
    {
        public Audit() { }
        public bool Activo { get; set; } = true;
        public string UsuarioCreacion { get; set; } = string.Empty;
        public string? UsuarioModificacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}
