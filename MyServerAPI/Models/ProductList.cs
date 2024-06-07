namespace ServerAPI.Models
{
    public static class ProductList
    {
        public static List<Product> Products = new List<Product>()
        {
            new Product() {
                Id = 1,
                Name = "OPPO",
                price = 400,
                quantity = 10,
                ExpireDate = DateTime.Now.AddMonths(12),
                Category = ProductCategory.LAPTOP,


            },
            new Product() {
                Id = 2,
                Name = "Dell",
                price = 4000,
                quantity = 10,
                ExpireDate = DateTime.Now.AddMonths(12),
                Category = ProductCategory.LAPTOP
            },

             new Product() {
                Id = 3,
                Name = "HP",
                price = 4000,
                quantity = 10,
                ExpireDate = DateTime.Now.AddMonths(12),
                Category = ProductCategory.LAPTOP
            },

             new Product() {
                Id = 2,
                Name = "Fish",
                price = 40,
                quantity = 100,
                ExpireDate = DateTime.Now.AddMonths(12),
                Category = ProductCategory.FOOD
            },

        };
    }
}
