using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Domain.Repositories;
using OrderManagement.Infrastructure.Persistence;
using OrderManagement.Infrastructure.Repositories;

namespace OrderManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("OrderDatabase");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'OrderDatabase' is missing. Configure it using user secrets or ConnectionStrings__OrderDatabase.");
        }

        services.AddDbContext<OrderDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<OrderDbContext>());

        services.AddScoped<IUsuarioRepository, EfUsuarioRepository>();
        services.AddScoped<IClienteRepository, EfClienteRepository>();
        services.AddScoped<IProductoRepository, EfProductoRepository>();
        services.AddScoped<IPedidoRepository, EfPedidoRepository>();
        services.AddScoped<IPedidoDetalleRepository, EfPedidoDetalleRepository>();

        return services;
    }
}