namespace stock_api.Models
{
    public class User : CustomModelExtension
    {
        public User()
        {
        }


        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public List<Stock> Stocks { get; set; }



        #region Methods
        public string? GetFullName()
        {
            return FirstName + " " + LastName;
        }


        #endregion

    }
}
