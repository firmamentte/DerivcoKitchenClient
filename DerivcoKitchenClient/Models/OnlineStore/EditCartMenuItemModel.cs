namespace DerivcoKitchenClient.Models.OnlineStore
{
    public class EditCartMenuItemModel
    {
        public Guid MenuItemId { get; set; }
        public string PictureFileName { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
