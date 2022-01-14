namespace DerivcoKitchenClient.Models.OnlineStore
{
    public class MenuItemGridSessionModel
    {
        public Guid MenuItemId { get; set; }
        public string PictureFileName { get; set; }
        public int Quantity { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public virtual decimal SubTotal
        {
            get
            {
                return Price * Quantity;
            }
        }
    }
}
