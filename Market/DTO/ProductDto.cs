using Market.Enums;
using Market.Models;

namespace Market.DTO;

public class ProductDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string? Description { get; set; }

    public decimal PriceInRubles { get; set; }

    public ProductCategory Category { get; set; }

    public Guid SellerId { get; set; }

    internal static ProductDto FromModel(Product product) =>
        new()
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            PriceInRubles = product.PriceInRubles,
            Category = product.Category,
            SellerId = product.SellerId
        };
}