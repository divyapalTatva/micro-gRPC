using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AllProtos.Protos;
using System.Linq.Expressions;

namespace Order.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ProductService.ProductServiceClient _productClient;

        public OrdersController(ProductService.ProductServiceClient productClient)
        {
            _productClient = productClient;
        }

        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct()
        {
            try
            {

                var product = await _productClient.CreateProductAsync(new CreateProductRequest { 
                    Name = "TV",
                    Description = "Some Desc",
                    Price = 10,
                    Stock = 100
                });

                return Ok(product);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestModel request)
        {
            try
            {

                var product = await _productClient.GetProductAsync(new GetProductRequest { Id = request.ProductId });

                if (product.Stock < request.Quantity)
                {
                    throw new Exception("No stock available");
                }

                var order = new
                {
                    OrderId = Guid.NewGuid().ToString(),
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    TotalPrice = product.Price * request.Quantity,
                    Status = "Confirmed"
                };

                await _productClient.UpdateProductAsync(new UpdateProductRequest
                {
                    Id = request.ProductId,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Stock = product.Stock - request.Quantity,
                });

                return Ok(order);
            }
            catch (Exception ex) {
                throw;
            }
        }
    }

    public record CreateOrderRequestModel(int ProductId, int Quantity);

}
