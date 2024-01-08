namespace SellingBuddy.BasketService.Api.Core.Domain.Models;

public class BasketCheckout
{
    // Address
    public string City { get; set; }

    public string Street { get; set; }

    public string State { get; set; }

    public string Country { get; set; }

    public string ZipCode { get; set; }

    // Card info
    public string CardNumber { get; set; }

    public string CardHolderName { get; set; }

    public string CardSecurityNumber { get; set; }

    public int CardTypeId { get; set; }

    public DateTime CardExpiration { get; set; }

    // User info
    public string Buyer { get; set; }
}
