using MediatR;
using OrderManagement.Domain.Repositories;

namespace OrderManagement.Application.Productos;

public sealed record ListarProductosQuery : IRequest<IReadOnlyList<ProductoDto>>;

public sealed class ListarProductosQueryHandler(IProductoRepository productos)
    : IRequestHandler<ListarProductosQuery, IReadOnlyList<ProductoDto>>
{
    public async Task<IReadOnlyList<ProductoDto>> Handle(
        ListarProductosQuery request,
        CancellationToken cancellationToken)
    {
        var activos = await productos.GetActivosAsync(cancellationToken);
        return activos
            .OrderBy(producto => producto.Nombre)
            .Select(producto => new ProductoDto(producto.Id, producto.Nombre ?? string.Empty, producto.Precio))
            .ToArray();
    }
}
