using Application.Contracts;
using Application.Contracts.IService;
using Application.DTOS;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;

namespace Application.CQRS;
public record RegisterCommand(RegisterDto Data) : IRequest<string?>;
public record LoginCommand(LoginDto Data) : IRequest<string?>;
public  class RegistrationDtoValidator : AbstractValidator<RegisterCommand>
{
    public RegistrationDtoValidator()
    {
        RuleFor(x => x.Data.Username).NotEmpty().WithMessage("Username is required.");
        RuleFor(x => x.Data.Password) .NotEmpty().WithMessage("Password is required.").MinimumLength(6).WithMessage("Password must be at least 6 characters.");
        RuleFor(x => x.Data.Email).NotEmpty().EmailAddress().WithMessage("Email is required.").WithMessage("Invalid Email Format.")
.Must(email =>
{
    var parts = email.Split('@');
    return parts.Length == 2 && parts[1].Length >= 2;
}).WithMessage("Invalid Email Domain.");}

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
public class AuthHandler(ITalkRealServices service) :
    IRequestHandler<RegisterCommand, string>,
    IRequestHandler<LoginCommand, string>
{
    public async Task<string?> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        return await service.UseService.RegistertUser(request.Data);
    }
    public async Task<string?> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return await service.UseService.Login(request.Data);
    }

}