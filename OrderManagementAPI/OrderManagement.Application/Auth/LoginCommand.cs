using MediatR;
using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Application.Security;
using OrderManagement.Domain.Repositories;

namespace OrderManagement.Application.Auth;

public sealed record LoginCommand(LoginRequest Request) : IRequest<LoginResponse>;

public sealed class LoginCommandHandler(
    IUsuarioRepository usuarios,
    IPasswordVerifier passwordVerifier,
    IJwtTokenGenerator tokenGenerator) : IRequestHandler<LoginCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var email = request.Request.Email.Trim();
        var usuario = await usuarios.GetByEmailAsync(email, cancellationToken);
        if (usuario is null || !usuario.Activo ||
            !passwordVerifier.Verify(usuario, request.Request.Password))
        {
            throw new InvalidCredentialsException();
        }

        var token = tokenGenerator.Generate(usuario);
        return new LoginResponse(token.Token, "Bearer", token.ExpiresIn);
    }
}