namespace stock_api.Models
{
    public class CustomModelExtension
    {
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;
    }
}
