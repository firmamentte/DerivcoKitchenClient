namespace DerivcoKitchenClient.Models.OnlineStore
{
    public class PurchaseOrderSessionModel
    {
        public Guid PurchaseOrderId { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string PaymentStatus { get; set; }
        public string ShippingStatus { get; set; }
        public DateTime PurchaseOrderDate { get; set; }
        public List<MenuItemGridSessionModel> LineItems { get; set; }
        public decimal SubTotal
        {
            get
            {
                return LineItems.Sum(lineItem => lineItem.SubTotal);
            }
        }
    }
}
