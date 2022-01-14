namespace DerivcoKitchenClient.Models.OnlineStore
{
    public class MainMenuItemGridModel
    {
        public List<MenuCategoryModel> MenuCategories { get; set; }=new List<MenuCategoryModel>();
        public List<MenuItemGridModel> MenuItems { get; set; }=new List<MenuItemGridModel>();
    }
}
