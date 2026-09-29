# Order Management API

Backend en .NET 8 con PostgreSQL, autenticación JWT y CRUD de pedidos mediante CQRS.

## Requisitos

- .NET 8 SDK
- PostgreSQL disponible en `localhost:5432` con la base `orderdatabase` y el esquema `orders` creado
- Un usuario activo en `orders.usuario` con contraseña almacenada mediante ASP.NET Core Identity
- Al menos un producto activo para probar la creación de pedidos

## Configuración local

La cadena de conexión y la llave de firma JWT se guardan en User Secrets, no en `appsettings.json`:

```powershell
dotnet user-secrets set "ConnectionStrings:OrderDatabase" "Host=localhost;Port=5432;Database=orderdatabase;Username=<usuario>;Password=<contraseña>" --project "OrderManagementAPI\OrderManagement.WebApi\OrderManagement.WebApi.csproj"
dotnet user-secrets set "Jwt:Key" "<llave aleatoria Base64 de al menos 32 bytes>" --project "OrderManagementAPI\OrderManagement.WebApi\OrderManagement.WebApi.csproj"
```

No guardes credenciales ni llaves en archivos versionados. El proyecto rechaza llaves JWT ausentes, inválidas o menores a 256 bits.

## Ejecutar

Desde la raíz del repositorio:

```powershell
dotnet build "OrderManagementAPI\OrderManagementAPI.sln"
dotnet run --project "OrderManagementAPI\OrderManagement.WebApi\OrderManagement.WebApi.csproj"
```

El perfil `http` de Development escucha en `http://localhost:5275` y carga User Secrets. Swagger está en `http://localhost:5275/swagger`; la conexión a PostgreSQL se comprueba en `http://localhost:5275/health/database`.

## Probar autenticación y pedidos

1. En Swagger, ejecuta `POST /auth/login` con `email` y `password`. El endpoint devuelve un token Bearer con expiración en segundos.
2. Pulsa **Authorize** y pega el token sin anteponer `Bearer`; Swagger configura el esquema HTTP Bearer.
3. Prueba `GET /api/pedidos` y `GET /api/pedidos/{id}`.
4. Para `POST /api/pedidos`, envía un pedido con un cliente identificado por DNI de 8 dígitos y uno o más detalles con `productoId` y `cantidad`. El cliente activo se reutiliza por DNI; si no existe, se crea. El total se calcula con los precios guardados en PostgreSQL.
5. Prueba `PUT /api/pedidos/{id}` con el mismo formato y `DELETE /api/pedidos/{id}` para eliminación lógica.

Respuestas esperadas: `201` al crear, `200` al consultar, `204` al actualizar o eliminar, `400` para validación, `401` para credenciales/token inválidos o vencidos, `404` para pedidos inexistentes/inactivos y `409` para conflictos de negocio.

## Verificación realizada

La solución compila con `dotnet build`. Durante la verificación de integración contra PostgreSQL se probaron login válido e inválido, token malformado y vencido, los cinco endpoints de pedidos y health check. El pedido/cliente usado en el recorrido se eliminó al finalizar.