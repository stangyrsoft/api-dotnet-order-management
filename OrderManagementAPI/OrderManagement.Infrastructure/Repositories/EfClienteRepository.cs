using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Models;
using OrderManagement.Domain.Repositories;
using OrderManagement.Infrastructure.Persistence;

namespace OrderManagement.Infrastructure.Repositories;

public sealed class EfClienteRepository(OrderDbContext context)
    : EfRepository<Cliente>(context), IClienteRepository
{
    public Task<Cliente?> GetByDniAsync(string dni, CancellationToken cancellationToken = default)
    {
        return Context.Clientes.AsNoTracking()
            .FirstOrDefaultAsync(cliente => cliente.DNI == dni, cancellationToken);
    }
}