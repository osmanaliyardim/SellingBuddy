namespace SellingBuddy.OrderService.Application.Features.Queries.ViewModels;

public class OrderDetailViewModel
{
    public string OrderNumber { get; init; }

    public DateTime OrderDate { get; init; }

    public string Status { get; init; }

    public string Description { get; init; }

    public string Street { get; init; }

    public string City { get; init; }

    public string ZipCode { get; init; }

    public string Country { get; init; }

    public List<OrderItem> OrderItems { get; set; }

    public decimal Total { get; set; }
}
