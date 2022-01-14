namespace DerivcoKitchenClient.BLL.DataContract
{
    public class GetMenuItemsByCriteriaReq
    {
        public string MenuCategoryName { get; set; }
        public string? Name { get; set; }
        public int Skip { get; set; }
    }
}
