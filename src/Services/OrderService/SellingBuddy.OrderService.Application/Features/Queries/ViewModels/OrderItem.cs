namespace SellingBuddy.OrderService.Application.Features.Queries.ViewModels;

public class OrderItem
{
    public string ProductName { get; init; }

    public int Units { get; init; }

    public double UnitPrice { get; init; }

    public string PictureUrl { get; init; }
}
