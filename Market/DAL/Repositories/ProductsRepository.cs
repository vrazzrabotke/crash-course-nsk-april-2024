using Market.Enums;
using Market.Models;
using Microsoft.EntityFrameworkCore;

namespace Market.DAL.Repositories;

internal sealed class ProductsRepository
{
    private readonly RepositoryContext _context;

    public ProductsRepository()
    {
        _context = new RepositoryContext();
    }

    public async Task<DbResult<IReadOnlyCollection<Product>>> GetProductsAsync(
        Guid? sellerId = null, 
        int skip = 0,
        int take = 50)
    {
        IQueryable<Product> query = _context.Products;

        // оставил такую реализацию для будущих фильтров
        if (sellerId.HasValue)
            query = query.Where(p => p.SellerId == sellerId.Value);

        var products = await query.Skip(skip).Take(take).ToListAsync();

        return new DbResult<IReadOnlyCollection<Product>>(products, DbResultStatus.Ok);
    }

    public async Task<DbResult<Product>> GetProductAsync(Guid productId)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);

        return product != null
            ? new DbResult<Product>(product, DbResultStatus.Ok)
            : new DbResult<Product>(null!, DbResultStatus.NotFound);
    }

    public async Task<DbResult> CreateProductAsync(Product product)
    {
        try
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return new DbResult(DbResultStatus.Ok);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new DbResult(DbResultStatus.UnknownError);
        }
    }

    public async Task<DbResult> UpdateProductAsync(Guid productId, ProductUpdateInfo updateInfo)
    {
        var productToUpdate = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);

        if (productToUpdate is null)
            return new DbResult(DbResultStatus.NotFound);

        if(updateInfo.Name != null)
            productToUpdate.Name = updateInfo.Name;
        if(updateInfo.Description != null)
            productToUpdate.Description = updateInfo.Description;
        if(updateInfo.Category.HasValue)
            productToUpdate.Category = updateInfo.Category.Value;
        if(updateInfo.PriceInRubles.HasValue)
            productToUpdate.PriceInRubles = updateInfo.PriceInRubles.Value;

        try
        {
            await _context.SaveChangesAsync();
            return new DbResult(DbResultStatus.Ok);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new DbResult(DbResultStatus.UnknownError);
        }
    }

    public async Task<DbResult> DeleteProductAsync(Guid productId)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);

        if (product is null)
            return new DbResult(DbResultStatus.NotFound);

        try
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return new DbResult(DbResultStatus.Ok);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new DbResult(DbResultStatus.UnknownError);
        }
    }
}