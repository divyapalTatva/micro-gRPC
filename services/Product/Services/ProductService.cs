using Grpc.Core;
using AllProtos.Protos;

namespace Product.Services;

public class ProductService : AllProtos.Protos.ProductService.ProductServiceBase
{
    private static readonly List<Product.Models.Product> _products = new();
    private static int _nextId = 1;

    public override Task<ProductResponse> GetProduct(
        GetProductRequest request,
        ServerCallContext context)
    {
        var product = _products.FirstOrDefault(p => p.Id == request.Id);

        if (product == null)
        {
            throw new RpcException(
                new Status(StatusCode.NotFound, $"Product {request.Id} not found"));
        }

        return Task.FromResult(new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock
        });
    }

    public override Task<ProductsResponse> GetProducts(
        GetProductsRequest request,
        ServerCallContext context)
    {
        var skip = (request.Page - 1) * request.PageSize;
        var products = _products.Skip(skip).Take(request.PageSize);

        var response = new ProductsResponse
        {
            TotalCount = _products.Count
        };

        foreach (var product in products)
        {
            response.Products.Add(new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock
            });
        }

        return Task.FromResult(response);
    }

    public override Task<ProductResponse> CreateProduct(
        CreateProductRequest request,
        ServerCallContext context)
    {
        var product = new Product.Models.Product
        {
            Id = _nextId++,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock
        };

        _products.Add(product);

        return Task.FromResult(new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock
        });
    }

    public override Task<ProductResponse> UpdateProduct(
        UpdateProductRequest request,
        ServerCallContext context)
    {
        var product = _products.FirstOrDefault(p => p.Id == request.Id);

        if (product == null)
        {
            throw new RpcException(
                new Status(StatusCode.NotFound, $"Product {request.Id} not found"));
        }

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.Stock = request.Stock;

        return Task.FromResult(new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock
        });
    }

    public override Task<DeleteProductResponse> DeleteProduct(
        DeleteProductRequest request,
        ServerCallContext context)
    {
        var product = _products.FirstOrDefault(p => p.Id == request.Id);

        if (product == null)
        {
            return Task.FromResult(new DeleteProductResponse
            {
                Success = false,
                Message = $"Product {request.Id} not found"
            });
        }

        _products.Remove(product);

        return Task.FromResult(new DeleteProductResponse
        {
            Success = true,
            Message = "Product deleted successfully"
        });
    }
}