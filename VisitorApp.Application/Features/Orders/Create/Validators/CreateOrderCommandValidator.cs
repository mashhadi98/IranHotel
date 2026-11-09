using VisitorApp.Application.Common.Messaging;
using VisitorApp.Contract.Features.Orders.Create;

namespace VisitorApp.Application.Features.Orders.Create.Validators;

public class CreateOrderCommandValidator : IValidatorService<CreateOrderCommandRequest, CreateOrderCommandResponse>
{
    public void Execute(CreateOrderCommandRequest request)
    {
        if (request.CustomerId == Guid.Empty)
        {
            throw new ArgumentException("شناسه مشتری معتبر نیست", nameof(request.CustomerId));
        }

        if (request.Items is null || request.Items.Count == 0)
        {
            throw new ArgumentException("سفارش باید حداقل شامل یک محصول باشد", nameof(request.Items));
        }

        if (request.Items.Any(item => item.ProductId == Guid.Empty || item.Quantity <= 0))
        {
            throw new ArgumentException("شناسه محصول و تعداد باید معتبر باشند", nameof(request.Items));
        }
    }
}
