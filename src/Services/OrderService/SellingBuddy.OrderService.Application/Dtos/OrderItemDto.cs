namespace SellingBuddy.OrderService.Application.Dtos;

public class OrderItemDto
{
    public int ProductId { get; init; }

    public string ProductName { get; init; }

    public string PictureUrl { get; init; }

    public decimal UnitPrice { get; init; }

    public int Units { get; init; }
}
