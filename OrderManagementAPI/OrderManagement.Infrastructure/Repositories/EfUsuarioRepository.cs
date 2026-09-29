using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Models;
using OrderManagement.Domain.Repositories;
using OrderManagement.Infrastructure.Persistence;

namespace OrderManagement.Infrastructure.Repositories;

public sealed class EfUsuarioRepository(OrderDbContext context)
    : EfRepository<Usuario>(context), IUsuarioRepository
{
    public Task<Usuario?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return Context.Usuarios.AsNoTracking()
            .FirstOrDefaultAsync(usuario => usuario.Email == email, cancellationToken);
    }
}