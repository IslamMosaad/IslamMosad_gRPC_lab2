using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GRPS_Test.Protos;
using Microsoft.AspNetCore.Authorization;
using static GRPS_Test.Protos.InventoryService;
using MyServerAPI.Models;
namespace MyServerAPI.Services
{
    public class MessageInventoryService : InventoryServiceBase
    {
        public MessageInventoryService()
        {
        }
        public override async Task<ProdExistResponse> GetProdById(prodIdMsg request, ServerCallContext context)
        {
            var res = ProductList.Products.FirstOrDefault(x => x.Id == request.ProdId);
            if (res == null)
                return await Task.FromResult(new ProdExistResponse() { Exists = false });
            return await Task.FromResult(new ProdExistResponse() { Exists = true });

        }

        public override async Task<productMsg> AddProduct(productMsg request, ServerCallContext context)
        {
            Product newProd = new Product()
            {
                Id = request.ProdId,
                Name = request.Name,
                price = request.Price,
                quantity = request.Quantity,
                ExpireDate = request.Expiredates.ToDateTime(), // Convert Google protobuf Timestamp to DateTime
                Category = (MyServerAPI.Models.ProductCategory)request.Category,
            };

            ProductList.Products.Add(newProd);
            return await Task.FromResult(request);
        }

        public override async Task<productMsg> UpdateProduct(productMsg request, ServerCallContext context)
        {
            var product = ProductList.Products.FirstOrDefault(x => x.Id == request.ProdId);
            if (product != null)
            {
                product.Name = request.Name;
                product.price = request.Price;
                product.quantity = request.Quantity;
                product.ExpireDate = request.Expiredates.ToDateTime(); // Convert Google protobuf Timestamp to DateTime
                product.Category = (MyServerAPI.Models.ProductCategory)request.Category;          

            }
            return await Task.FromResult(request);
        }


        //[Authorize(AuthenticationSchemes = "ApiKey")]
        public override async Task<ProductListResponse> GetAllProd(Empty request, ServerCallContext context)
        {
            var response = new ProductListResponse();

            response.Products.AddRange(ProductList.Products.Select(p => new productMsg
            {
                ProdId = p.Id,
                Name = p.Name,
                Price = p.price,
                Quantity = p.quantity,
                Expiredates = Timestamp.FromDateTime(p.ExpireDate.ToUniversalTime()), // Convert DateTime to UTC and then to Timestamp
                Category = (GRPS_Test.Protos.ProductCategory) p.Category,
            }));

            return await Task.FromResult(response);
        }


        public override async Task<NumOfInsertedRowsMsg> AddBulkProd(IAsyncStreamReader<productMsg> requestStream, ServerCallContext context)
        {
            int num = 0;
            await foreach(var request in requestStream.ReadAllAsync())
            {
                this.AddProduct(request,context);
                num++;
            }
            return await Task.FromResult(new NumOfInsertedRowsMsg() { NumOfInsertedRows = num });
        }

        public override async Task GetProductsByCriteria(ProductCriteriaMsg request, IServerStreamWriter<productMsg> responseStream, ServerCallContext context)
        {
            var filteredProducts = ProductList.Products.AsQueryable();

            if (request.Category != GRPS_Test.Protos.ProductCategory.CategoryUnspecified)
                filteredProducts = filteredProducts.Where(p => p.Category == (MyServerAPI.Models.ProductCategory)request.Category);
            
            // Optional: Order by price if requested
            if (request.OrderByPrice)      
                filteredProducts = filteredProducts.OrderBy(p => p.price);


            foreach (var product in filteredProducts)
            {
                var productMsg = new productMsg
                {
                    ProdId = product.Id,
                    Name = product.Name,
                    Quantity = product.quantity,
                    Price = product.price,
                    Expiredates = Timestamp.FromDateTime(product.ExpireDate.ToUniversalTime()),
                    Category = (GRPS_Test.Protos.ProductCategory)product.Category
                };

                // Write product to the response stream
                await responseStream.WriteAsync(productMsg);
            }
        }
    }
}
