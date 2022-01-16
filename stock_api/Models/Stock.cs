namespace stock_api.Models
{
    public class Stock : CustomModelExtension
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public double Amount { get; set; }
        public double Price { get; set; }
        public string? Currency { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public StockType Type { get; set; }


        public int UserId { get; set; }
        public User? User { get; set; }
    }


    public enum StockType : int
    {
        Fund,
        Share,
        Crypto,
        Bond
    }
}
