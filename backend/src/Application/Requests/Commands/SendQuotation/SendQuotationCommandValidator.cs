using FluentValidation;

using Application.Resources;
using Microsoft.Extensions.Localization;

namespace Application.Requests.Commands.SendQuotation;

public class SendQuotationCommandValidator : AbstractValidator<SendQuotationCommand>
{
    public SendQuotationCommandValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(v => v.RequestId)
            .NotEmpty().WithMessage(localizer["RequestIdRequired"].Value);

        RuleFor(v => v.Items)
            .NotEmpty().WithMessage(localizer["AtLeastOneItem"].Value);

        RuleForEach(v => v.Items).SetValidator(new SendQuotationItemDtoValidator(localizer));
    }
}

public class SendQuotationItemDtoValidator : AbstractValidator<SendQuotationItemDto>
{
    public SendQuotationItemDtoValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(v => v.ProductId)
            .NotEmpty().WithMessage(localizer["ProductIdRequired"].Value);

        RuleFor(v => v.UnitPrice)
            .GreaterThan(0).WithMessage(localizer["UnitPricePositive"].Value);
    }
}
