
namespace ShopApp.Models
{
    public class Sale
    {
        public Guid SaleId { get; set; }
        public Guid UserId { get; set; }
        public string Name {  get; set; }
        public DateTime Date {  get; set; }
        public int Amount { get; set; }
        public int Price { get; set; }

    }
}
