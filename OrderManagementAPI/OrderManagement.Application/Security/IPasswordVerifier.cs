using OrderManagement.Domain.Models;

namespace OrderManagement.Application.Security;

public interface IPasswordVerifier
{
    bool Verify(Usuario usuario, string providedPassword);
}