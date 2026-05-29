using FluentValidation;

namespace Application.Requests.Commands.SendQuotation;

public class SendQuotationCommandValidator : AbstractValidator<SendQuotationCommand>
{
    public SendQuotationCommandValidator()
    {
        RuleFor(v => v.RequestId)
            .NotEmpty().WithMessage("RequestId is required.");

        RuleFor(v => v.Items)
            .NotEmpty().WithMessage("At least one item must be provided.");

        RuleForEach(v => v.Items).SetValidator(new SendQuotationItemDtoValidator());
    }
}

public class SendQuotationItemDtoValidator : AbstractValidator<SendQuotationItemDto>
{
    public SendQuotationItemDtoValidator()
    {
        RuleFor(v => v.ProductId)
            .NotEmpty().WithMessage("ProductId is required.");

        RuleFor(v => v.UnitPrice)
            .GreaterThan(0).WithMessage("Unit price must be greater than zero.");
    }
}
