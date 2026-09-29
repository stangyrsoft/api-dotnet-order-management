using OrderManagement.Domain.Models;

namespace OrderManagement.Application.Security;

public sealed record GeneratedToken(string Token, int ExpiresIn);

public interface IJwtTokenGenerator
{
    GeneratedToken Generate(Usuario usuario);
}