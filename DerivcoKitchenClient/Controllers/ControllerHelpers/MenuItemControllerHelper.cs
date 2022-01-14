using DerivcoKitchenClient.BLL.DataContract;
using DerivcoKitchenClient.Models.OnlineStore;

namespace DerivcoKitchenClient.Controllers.ControllerHelpers
{
    public class MenuItemControllerHelper
    {
        public async Task CreateItemPictureFromBase64String(IWebHostEnvironment webHostEnvironment, MenuItemResp menuItemResp)
        {
            await CreateItemPictureFromBase64String(webHostEnvironment, menuItemResp.PictureFileName, menuItemResp.PictureBase64String);
        }

        public async Task CreateItemPictureFromBase64String(IWebHostEnvironment webHostEnvironment, List<MenuItemResp> menuItemResps)
        {
            foreach (var menuItemResp in menuItemResps)
            {
                await CreateItemPictureFromBase64String(webHostEnvironment, menuItemResp);
            }
        }

        private async Task CreateItemPictureFromBase64String(IWebHostEnvironment webHostEnvironment, string pictureName, string pictureBase64String)
        {
            if (!string.IsNullOrWhiteSpace(pictureName) && !string.IsNullOrWhiteSpace(pictureBase64String))
            {
                string _path = Path.Combine(webHostEnvironment.WebRootPath, "menuItemPictures", pictureName);

                if (!File.Exists(_path))
                {
                    await File.WriteAllBytesAsync(_path, Convert.FromBase64String(pictureBase64String));
                }
            }
        }

        public MainMenuItemGridModel FillMainMenuItemGridModel(List<MenuCategoryResp> menuCategoryResps, List<MenuItemResp> menuItemResps)
        {
            menuCategoryResps ??= new();
            menuItemResps ??= new();

            MainMenuItemGridModel _mainMenuItemGridModel = new();
            _mainMenuItemGridModel.MenuCategories.AddRange(FillMenuCategoryModel(menuCategoryResps));
            _mainMenuItemGridModel.MenuItems.AddRange(FillMenuItemGridModel(menuItemResps));

            return _mainMenuItemGridModel;
        }

        public List<MenuCategoryModel> FillMenuCategoryModel(List<MenuCategoryResp> menuCategoryResps)
        {
            menuCategoryResps ??= new();

            List<MenuCategoryModel> _menuCategoryModels = new();

            foreach (var menuCategoryResp in menuCategoryResps.OrderBy(item => item.Order).
                                                               ThenBy(item => item.Name))
            {
                _menuCategoryModels.Add(new MenuCategoryModel()
                {
                    Order = menuCategoryResp.Order,
                    Name = menuCategoryResp.Name
                });
            }

            return _menuCategoryModels;
        }

        public List<MenuItemGridModel> FillMenuItemGridModel(List<MenuItemResp> menuItemResps)
        {
            menuItemResps ??= new();

            List<MenuItemGridModel> _menuItemGridModels = new();

            foreach (var menuItemResp in menuItemResps.OrderBy(item => item.Order).
                                                       ThenBy(item => item.Name))
            {
                _menuItemGridModels.Add(new MenuItemGridModel()
                {
                    MenuItemId = menuItemResp.MenuItemId,
                    Price = menuItemResp.Price,
                    Name = menuItemResp.Name,
                    PictureFileName = string.Format("/menuItemPictures/{0}", menuItemResp.PictureFileName)
                });
            }

            return _menuItemGridModels;
        }

        public ViewEditCartModel FillViewEditCartModel(List<MenuItemGridSessionModel> menuItemGridSessionModels)
        {
            menuItemGridSessionModels ??= new();

            ViewEditCartModel _model = new();
            _model.LineItems.AddRange(menuItemGridSessionModels.OrderBy(lineItem => lineItem.Name));

            return _model;
        }

        public AddToCartModel FillAddToCartModel(MenuItemResp menuItemResp)
        {
            if (menuItemResp is null)
                return null;

            return new AddToCartModel()
            {
                MenuItemId = menuItemResp.MenuItemId,
                PictureFileName = string.Format("/menuItemPictures/{0}", menuItemResp.PictureFileName),
                Name = menuItemResp.Name,
                Price = menuItemResp.Price,
            };
        }

        public MenuItemGridSessionModel FillMenuItemGridSessionModel(AddToCartModel model)
        {
            return new MenuItemGridSessionModel()
            {
                MenuItemId = model.MenuItemId,
                PictureFileName = model.PictureFileName,
                Name = model.Name,
                Quantity = model.Quantity,
                Price = model.Price
            };
        }

        public void UpdateMenuItemGridSessionModel(MenuItemGridSessionModel model, int quantity)
        {
            model.Quantity = quantity;
        }

        public void UpdateAddToCartMenuItemGridSessionModel(MenuItemGridSessionModel model, int quantity)
        {
            model.Quantity += quantity;
        }

        public EditCartMenuItemModel FillEditCartMenuItemModel(MenuItemGridSessionModel model)
        {
            return new EditCartMenuItemModel()
            {
                MenuItemId = model.MenuItemId,
                PictureFileName = model.PictureFileName,
                Name = model.Name,
                Quantity = model.Quantity,
                Price = model.Price,
            };
        }

        public bool IsMenuItemAddedToCart(MenuItemGridSessionModel menuItemGridSessionModel1, MenuItemGridSessionModel menuItemGridSessionModel2)
        {
            return menuItemGridSessionModel1.MenuItemId == menuItemGridSessionModel2.MenuItemId;
        }
    }
}
