using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OrderManagement.WebApi;

public sealed class AuthorizeCheckOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var method = context.MethodInfo;
        var controller = method.DeclaringType;
        var allowsAnonymous = method.GetCustomAttribute<AllowAnonymousAttribute>() is not null ||
            controller?.GetCustomAttribute<AllowAnonymousAttribute>() is not null;
        var requiresAuthorization = method.GetCustomAttribute<AuthorizeAttribute>() is not null ||
            controller?.GetCustomAttribute<AuthorizeAttribute>() is not null;

        if (allowsAnonymous || !requiresAuthorization)
        {
            return;
        }

        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            }] = Array.Empty<string>()
        });
        operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
        operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });
    }
}
