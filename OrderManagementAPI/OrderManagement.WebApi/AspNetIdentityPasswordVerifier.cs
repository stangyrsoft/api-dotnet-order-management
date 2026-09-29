using Microsoft.AspNetCore.Identity;
using OrderManagement.Application.Security;
using OrderManagement.Domain.Models;

namespace OrderManagement.WebApi;

public sealed class AspNetIdentityPasswordVerifier(IPasswordHasher<Usuario> passwordHasher)
    : IPasswordVerifier
{
    public bool Verify(Usuario usuario, string providedPassword)
    {
        if (string.IsNullOrWhiteSpace(usuario.Password))
        {
            return false;
        }

        try
        {
            return passwordHasher.VerifyHashedPassword(usuario, usuario.Password, providedPassword)
                != PasswordVerificationResult.Failed;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}