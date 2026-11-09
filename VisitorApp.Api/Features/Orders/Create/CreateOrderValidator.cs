using FluentValidation;
using VisitorApp.Contract.Features.Orders.Create;

namespace VisitorApp.API.Features.Orders.Create;

public class CreateOrderValidator : Validator<CreateOrderRequest>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("شناسه مشتری الزامی است");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("حداقل یک آیتم برای سفارش الزامی است")
            .Must(items => items != null && items.Count > 0).WithMessage("سفارش باید حداقل یک آیتم داشته باشد");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("شناسه محصول الزامی است");

            item.RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("تعداد باید بزرگتر از صفر باشد");
        });
    }
}
