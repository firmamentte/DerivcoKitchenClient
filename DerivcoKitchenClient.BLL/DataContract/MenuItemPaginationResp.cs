namespace DerivcoKitchenClient.BLL.DataContract
{
    public class MenuItemPaginationResp
    {
        public PaginationMeta Meta { get; set; }
        public List<MenuItemResp> MenuItems { get; set; } = new List<MenuItemResp>();
    }
}
