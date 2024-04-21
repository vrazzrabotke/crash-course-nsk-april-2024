using Market.Enums;
using Market.Misc;
using Market.Models;

namespace Market.DAL;

internal static class ProductsInitializer
{
    private static readonly Random Random = Random.Shared;
    private static readonly ProductCategory[] Categories = Enum.GetValues<ProductCategory>();

    public static Product[] Initialize(int count = 10)
    {
        return Enumerable.Range(1, count).Select(number =>
            new Product
            {
                Id = Guid.NewGuid(),
                Name = $"Product-{number}",
                Description = $"Some description for product-{number}",
                PriceInRubles = (decimal)Random.NextDouble(100, 10000),
                Category = Categories[Random.Next(Categories.Length)],
                SellerId = Guid.NewGuid()
            })
            .ToArray();
    }
}