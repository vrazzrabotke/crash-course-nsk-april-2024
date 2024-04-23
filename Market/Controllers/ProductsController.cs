﻿using Market.DAL;
using Market.DAL.Repositories;
using Market.DTO;
using Market.Enums;
using Market.Models;
using Microsoft.AspNetCore.Mvc;

namespace Market.Controllers;

[ApiController]
[Route("/v1/products")]
public sealed class ProductsController : ControllerBase
{
    public ProductsController()
    {
        ProductsRepository = new ProductsRepository();
    }

    private ProductsRepository ProductsRepository { get; }

    [HttpGet("{productId:guid}")]
    public async Task<IActionResult> GetProductByIdAsync(Guid productId)
    {
        var productResult = await ProductsRepository.GetProductAsync(productId);
        return DbResultIsSuccessful(productResult, out var error)
            ? new JsonResult(productResult.Result)
            : error;
    }
    public class SearchProductDto
    {
        public string? ProductName { get; set; }
        public SortType? SortType { get; set; }
        public ProductCategory? Category { get; set; }
        public bool Ascending { get; set; } = true;
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 50;
    }
    [HttpPost("search")]
    public async Task<IActionResult> SearchProductsAsync(
        string? productName,
        SortType? sortType,
        ProductCategory? category,
        bool ascending = true,
        int skip = 0,
        int take = 50)
    {
        var searchResult = await ProductsRepository.GetProductAsync(productName,sortType,category,ascending,skip,take);
        return DbResultIsSuccessful(searchResult, out var error)
            ? new JsonResult(searchResult.Result)
            : error;
            
        //throw new NotImplementedException("Нужно реализовать позже");
    }

    [HttpGet()]
    public async Task<IActionResult> GetSellerProductsAsync(
        [FromRoute] Guid sellerId,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50)
    {
        var productsResult = await ProductsRepository.GetProductsAsync(sellerId: sellerId, skip: skip, take: take);
        if (!DbResultIsSuccessful(productsResult, out var error))
            return error;

        var productDtos = productsResult.Result.Select(ProductDto.FromModel);
        return new JsonResult(productDtos);
    }

    [HttpPost()]
    public async Task<IActionResult> CreateProductAsync([FromBody] Product product)
    {
        var createResult = await ProductsRepository.CreateProductAsync(product);

        return DbResultIsSuccessful(createResult, out var error)
            ? new StatusCodeResult(StatusCodes.Status205ResetContent)
            : error;
    }

    [HttpPut("{productId}")]
    public async Task<IActionResult> UpdateProductAsync([FromRoute] Guid productId, [FromBody] UpdateProductRequestDto requestInfo)
    {
        var updateResult = await ProductsRepository.UpdateProductAsync(productId, new ProductUpdateInfo
        {
            Name = requestInfo.Name,
            Description = requestInfo.Description,
            Category = requestInfo.Category,
            PriceInRubles = requestInfo.PriceInRubles
        });

        return DbResultIsSuccessful(updateResult, out var error)
            ? new StatusCodeResult(StatusCodes.Status404NotFound)
            : error;
    }

    [HttpDelete("{productId:guid}")]
    public async Task<IActionResult> DeleteProductAsync(Guid productId)
    {
        var deleteResult = await ProductsRepository.DeleteProductAsync(productId);

        return DbResultIsSuccessful(deleteResult, out var error)
            ? new StatusCodeResult(StatusCodes.Status405MethodNotAllowed)
            : error;
    }

    private static bool DbResultIsSuccessful(DbResult dbResult, out IActionResult error) =>
        DbResultStatusIsSuccessful(dbResult.Status, out error);

    private static bool DbResultIsSuccessful<T>(DbResult<T> dbResult, out IActionResult error) =>
        DbResultStatusIsSuccessful(dbResult.Status, out error);

    private static bool DbResultStatusIsSuccessful(DbResultStatus status, out IActionResult error)
    {
        error = null!;
        switch (status)
        {
            case DbResultStatus.Ok:
                return true;
            case DbResultStatus.NotFound:
                error = new StatusCodeResult(StatusCodes.Status404NotFound);
                return false;
            default:
                error = new StatusCodeResult(StatusCodes.Status500InternalServerError);
                return false;
        }
    }
}