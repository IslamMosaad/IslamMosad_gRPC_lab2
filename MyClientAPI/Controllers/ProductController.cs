using MyClientAPI.Models;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GRPS_Test.Protos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static GRPS_Test.Protos.InventoryService;

namespace MyClientAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        InventoryServiceClient client;
        public ProductController(InventoryServiceClient _client)
        {
            this.client = _client;
        }

        [HttpPost]
        public async Task<ActionResult> Post(Product product)
        {
            var req = new prodIdMsg() { ProdId = product.Id };
            var res = await client.GetProdByIdAsync(req);

            productMsg prod = new productMsg()
            {
                ProdId = product.Id,
                Name = product.Name,
                Price = product.price,
                Quantity = product.quantity,
                Expiredates = Timestamp.FromDateTime(product.ExpireDate.ToUniversalTime()), // Convert DateTime to UTC and then to Timestamp
                Category = (GRPS_Test.Protos.ProductCategory)product.Category,

            };
            if (res.Exists == true)
            {
                await client.UpdateProductAsync(prod);
                return Created("product updated successfully", product);
            }
            await client.AddProductAsync(prod);
            return Ok(product);
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            //var response = await client.GetAllProdAsync(new Empty());
            var response = await client.GetAllProdAsync(new Empty());

            var products = response.Products.Select(p => new Product
            {
                Id = p.ProdId,
                Name = p.Name,
                price = p.Price,
                quantity = p.Quantity,
                ExpireDate = p.Expiredates.ToDateTime(),
                Category = (MyClientAPI.Models.ProductCategory)p.Category
            }).ToList();

            return Ok(products);
        }

        [HttpPost]
        [Route("BulkProduct")]
        public async Task<ActionResult> addBulkProd(List<Product> products)
        {
            var call = client.AddBulkProd();
            foreach (var product in products)
            {
                await call.RequestStream.WriteAsync(new productMsg()
                {
                    ProdId = product.Id,
                    Name = product.Name,
                    Price = product.price,
                    Quantity = product.quantity,
                    Expiredates = Timestamp.FromDateTime(product.ExpireDate.ToUniversalTime()), // Convert DateTime to UTC and then to Timestamp
                    Category = (GRPS_Test.Protos.ProductCategory)product.Category,

                });
                await Task.Delay(1000);
            }

            await call.RequestStream.CompleteAsync();
            var response = await call.ResponseAsync;

            return new JsonResult(new { NumberOfInsertedProducts = response.NumOfInsertedRows });

        }


        [HttpGet]
        [Route("GetProductsWithFilter")]
        public async Task<ActionResult> GetFilteredProducts(Models.ProductCategory category = 0, bool orderByPrice = false)
        {
            var request = new ProductCriteriaMsg
            {
                Category = (GRPS_Test.Protos.ProductCategory)category,
                OrderByPrice = orderByPrice
            };

            // List to store the received products
            var productList = new List<Product>();

            using (var call = client.GetProductsByCriteria(request))
            {
                // Iterate over the response stream and collect products
                while (await call.ResponseStream.MoveNext())
                {
                    var productMsg = call.ResponseStream.Current;

                    // Convert productMsg to Product model and add to the list
                    var product = new Product
                    {
                        Id = productMsg.ProdId,
                        Name = productMsg.Name,
                        price = productMsg.Price,
                        quantity = productMsg.Quantity,
                        ExpireDate = productMsg.Expiredates.ToDateTime().ToLocalTime(), // Convert Timestamp to DateTime
                        Category = (Models.ProductCategory)productMsg.Category
                    };

                    productList.Add(product);
                }
            }

            // Return the list of products as ActionResult
            return Ok(productList);
        }

    }
}
