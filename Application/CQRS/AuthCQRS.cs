using Application.Contracts;
using Application.DTOS;

using FluentValidation;

using MediatR;

namespace Application.CQRS;
public record RegisterCommand(RegisterDto Data) : IRequest;
public record LoginCommand(LoginDto Data) : IRequest<TokenDto>;

public record GenerateRefreshTokenCommand (string RefreshToken) : IRequest<TokenDto>;
public class RegistrationDtoValidator : AbstractValidator<RegisterCommand>
{
    public RegistrationDtoValidator()
    {
        RuleFor(x => x.Data.Username).NotEmpty().WithMessage("Username is required.");
        RuleFor(x => x.Data.Password).NotEmpty().WithMessage("Password is required.").MinimumLength(6).WithMessage("Password must be at least 6 characters.");
        RuleFor(x => x.Data.Email).NotEmpty().EmailAddress().WithMessage("Email is required.").WithMessage("Invalid Email Format.")
        .Must(email =>
        {
            var parts = email.Split('@');
            return parts.Length == 2 && parts[1].Length >= 2;
        }).WithMessage("Invalid Email Domain.");
    }
}
public class LoginValidator : AbstractValidator<LoginDto>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
    .NotEmpty()
    .WithMessage("Email is required.")
    .EmailAddress()
    .WithMessage("Invalid Email Format.")
    .Must(email =>
    {
        var parts = email.Split('@');
        return parts.Length == 2 && parts[1].Length >= 2;
    })
    .WithMessage("Invalid Email Domain.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters.");
    }
}

public class GenerateRefreshTokenCommandValidator : AbstractValidator<GenerateRefreshTokenCommand>
{
    public GenerateRefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token is required.");

        RuleFor(x => x.RefreshToken)
            .MinimumLength(44)
            .WithMessage("Invalid refresh token format.");

        RuleFor(x => x.RefreshToken)
            .Must(t => !t.Contains(' '))
            .WithMessage("Refresh token cannot contain spaces.");
    }
}
public class AuthHandler(ITalkRealServices service) :
    IRequestHandler<RegisterCommand>,
    IRequestHandler<LoginCommand, TokenDto>,
    IRequestHandler<GenerateRefreshTokenCommand, TokenDto>
{
    public async Task Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        await service.UseService.RegistertUser(request.Data);
    }
    public async Task<TokenDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return await service.TokenService.Login(request.Data);
    }
    public async Task<TokenDto> Handle(GenerateRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        return await service.TokenService.GetRefreshTokenAsync(request.RefreshToken);
    }

}