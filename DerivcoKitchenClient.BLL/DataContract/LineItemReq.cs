namespace DerivcoKitchenClient.BLL.DataContract
{
    public class LineItemReq
    {
        public Guid MenuItemId { get; set; }
        public string PictureFileName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
