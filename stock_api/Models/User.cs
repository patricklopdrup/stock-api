namespace stock_api.Models
{
    public class User : CustomModelExtension
    {
        public User()
        {
            this.Stocks = new List<Stock>();
        }


        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public ICollection<Stock> Stocks { get; set; }



        #region Methods
        public string? GetFullName()
        {
            return FirstName + " " + LastName;
        }


        #endregion

    }
}
