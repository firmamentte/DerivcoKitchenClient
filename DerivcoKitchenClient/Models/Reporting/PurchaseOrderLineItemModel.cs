namespace DerivcoKitchenClient.Models.Reporting
{
    public class PurchaseOrderLineItemModel
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal SubTotal { get; set; }
        public int Quantity { get; set; }
    }
}
