namespace MyClientAPI.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int quantity { get; set; }
        public float price { get; set; }
        public DateTime ExpireDate { get; set; }
        public ProductCategory Category { get; set; }
    }

    public enum ProductCategory
    {
        CATEGORY_UNSPECIFIED = 0,
        LAPTOP = 1,
        MOBILE = 2,
        FOOD = 3
    }
}
