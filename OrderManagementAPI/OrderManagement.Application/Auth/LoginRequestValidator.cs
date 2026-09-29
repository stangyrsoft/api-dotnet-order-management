using FluentValidation;

namespace OrderManagement.Application.Auth;

public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(50);
        RuleFor(request => request.Password)
            .NotEmpty()
            .MaximumLength(250);
    }
}

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(command => command.Request).SetValidator(new LoginRequestValidator());
    }
}