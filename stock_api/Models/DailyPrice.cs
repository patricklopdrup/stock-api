using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace stock_api.Models
{
    public class DailyPrice : CustomModelExtension
    {
        public int Id { get; set; }
        public double Price { get; set; }

        public double? OpenPrice { get; set; }
        public double? HighPrice { get; set; }
        public double? LowPrice { get; set; }
        public double? ClosePrice { get; set; }

        
        public string StockTicker { get; set; }
        [ForeignKey("StockTicker")]
        public Stock? Stock { get; set; }


        #region Methods

        #endregion
    }
}
