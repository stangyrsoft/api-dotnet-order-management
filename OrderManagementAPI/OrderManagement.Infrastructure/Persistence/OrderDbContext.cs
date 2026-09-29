using System.Collections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OrderManagement.Domain.Models;

namespace OrderManagement.Infrastructure.Persistence;

public sealed class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options)
{
    private static readonly ValueConverter<bool, BitArray> ActiveConverter = new(
        value => new BitArray(new[] { value }),
        value => value[0]);

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<PedidoDetalle> DetallesPedido => Set<PedidoDetalle>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("orders");

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuario");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Id).HasColumnName("id").UseIdentityAlwaysColumn();
            entity.Property(item => item.Email).HasColumnName("email").HasMaxLength(50).IsRequired();
            entity.Property(item => item.Password).HasColumnName("password").HasMaxLength(250).IsRequired();
            ConfigureAudit(entity);
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("cliente");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Id).HasColumnName("id").UseIdentityAlwaysColumn();
            entity.Property(item => item.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
            entity.Property(item => item.Apellido).HasColumnName("apellido").HasMaxLength(100);
            entity.Property(item => item.Direccion).HasColumnName("direccion").HasColumnType("text");
            entity.Property(item => item.DNI).HasColumnName("dni").HasColumnType("char(8)").IsRequired();
            entity.HasIndex(item => item.DNI).IsUnique().HasDatabaseName("uq_cliente_dni");
            ConfigureAudit(entity);
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.ToTable("producto");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Id).HasColumnName("id").UseIdentityAlwaysColumn();
            entity.Property(item => item.Nombre).HasColumnName("nombre").HasMaxLength(200).IsRequired();
            entity.Property(item => item.Precio).HasColumnName("precio").HasColumnType("numeric(14,2)").IsRequired();
            ConfigureAudit(entity);
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.ToTable("pedido");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Id).HasColumnName("id").UseIdentityAlwaysColumn();
            entity.Property(item => item.NumeroPedido).HasColumnName("numero_pedido").HasMaxLength(50).IsRequired();
            entity.Property(item => item.Estado).HasColumnName("estado").HasMaxLength(50).IsRequired();
            entity.Property(item => item.Observacion).HasColumnName("observacion").HasColumnType("text");
            entity.Property(item => item.ClienteId).HasColumnName("cliente_id").IsRequired();
            entity.Property(item => item.FechaPedido).HasColumnName("fecha_pedido").HasColumnType("timestamp without time zone").IsRequired();
            entity.Property(item => item.TotalImporte).HasColumnName("total_importe").HasColumnType("numeric(10,2)").IsRequired();
            entity.HasIndex(item => item.NumeroPedido).IsUnique().HasDatabaseName("uq_numero_pedido");
            entity.HasOne(item => item.Cliente)
                .WithMany()
                .HasForeignKey(item => item.ClienteId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_pedidos_cliente");
            ConfigureAudit(entity);
        });

        modelBuilder.Entity<PedidoDetalle>(entity =>
        {
            entity.ToTable("detalle_pedido");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Id).HasColumnName("id").UseIdentityAlwaysColumn();
            entity.Property(item => item.ProductoId).HasColumnName("producto_id").IsRequired();
            entity.Property(item => item.PedidoId).HasColumnName("pedido_id").IsRequired();
            entity.Property(item => item.Cantidad).HasColumnName("cantidad").HasDefaultValue(1).IsRequired();
            entity.Property(item => item.PrecioUnitario).HasColumnName("precio_unitario").HasColumnType("numeric(14,2)").IsRequired();
            entity.HasOne(item => item.Producto)
                .WithMany()
                .HasForeignKey(item => item.ProductoId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_detalle_producto");
            entity.HasOne(item => item.Pedido)
                .WithMany()
                .HasForeignKey(item => item.PedidoId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_detalle_pedido");
            ConfigureAudit(entity);
        });

        base.OnModelCreating(modelBuilder);
    }

    private static void ConfigureAudit<TEntity>(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<TEntity> entity)
        where TEntity : Audit
    {
        entity.Property(item => item.Activo)
            .HasColumnName("activo")
            .HasColumnType("bit")
            .HasConversion(ActiveConverter)
            .IsRequired();
        entity.Property(item => item.UsuarioCreacion).HasColumnName("usuario_crea").HasMaxLength(30).IsRequired();
        entity.Property(item => item.FechaCreacion).HasColumnName("fecha_crea").HasColumnType("timestamp with time zone").IsRequired();
        entity.Property(item => item.UsuarioModificacion).HasColumnName("usuario_modifica").HasMaxLength(30);
        entity.Property(item => item.FechaModificacion).HasColumnName("fecha_modifica").HasColumnType("timestamp with time zone");
    }
}