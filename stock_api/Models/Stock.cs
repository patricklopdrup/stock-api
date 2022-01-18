using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace stock_api.Models
{
    public class Stock : CustomModelExtension
    {
        public Stock()
        {
            this.DailyPrices = new List<DailyPrice>();
        }

        [Column(Order = 0)]
        public int Id { get; set; }

        [Key]
        public string Ticker { get; set; }
        public string? Name { get; set; }
        public double Amount { get; set; }
        public string? Currency { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public DateTime? SoldDate { get; set; }
        public StockType Type { get; set; }

        // One-to-many between Stock and User. One user can have many stocks
        public int UserId { get; set; }
        public User? User { get; set; }


        public ICollection<DailyPrice> DailyPrices { get; set; }



        #region Methods
        /// <summary>
        /// Get the total value of a stock
        /// </summary>
        public double GetTotalValue(CustomDbContext db)
        {
            var latestPrice = db.DailyPrices.Where(stock => stock.StockTicker == this.Ticker).OrderByDescending(dailyPrice => dailyPrice.CreatedDate).Select(dailyPrice => dailyPrice.Price).FirstOrDefault();
            return latestPrice * this.Amount;
        }


        /// <summary>
        /// Check if a stock is new for the user by its ticker.
        /// </summary>
        /// <param name="ticker">The ticker of the stock to check.</param>
        /// <param name="userId">The ID of the user to check for.</param>
        /// <returns>True if the stock is new, otherwise false.</returns>
        public bool IsNewInPortfolio(CustomDbContext db)
        {
            var userStock = db.Stocks.Where(stock => stock.UserId == this.UserId);
            return !userStock.Any(stock => stock.Ticker == this.Ticker);
        }
        #endregion

    }


    public enum StockType : int
    {
        Fund,
        Share,
        Crypto,
        Bond,
        Cash
    }
}
