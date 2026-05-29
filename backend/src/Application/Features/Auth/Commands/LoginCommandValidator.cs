using FluentValidation;

using Application.Resources;
using Microsoft.Extensions.Localization;

namespace Application.Features.Auth.Commands;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(localizer["EmailRequired"].Value)
            .EmailAddress().WithMessage(localizer["EmailNotValid"].Value);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(localizer["PasswordRequired"].Value);
    }
}
