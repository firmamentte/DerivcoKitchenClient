using DerivcoKitchenClient.BLL.BLLClasses;
using DerivcoKitchenClient.BLL.DataContract;
using DerivcoKitchenClient.Controllers.ControllerHelpers;
using DerivcoKitchenClient.Models.OnlineStore;
using DerivcoKitchenClient.Models.Reporting;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;

namespace DerivcoKitchenClient.Controllers
{
    [ResponseCache(NoStore = true, Duration = 0, Location = ResponseCacheLocation.None, VaryByHeader = "*")]
    public class OnlineStoreController : Controller
    {
        private readonly IWebHostEnvironment WebHostEnvironment;
        private readonly IConfiguration Configuration;
        private readonly MenuCategoryBLL MenuCategoryBLL;
        private readonly MenuItemBLL MenuItemBLL;
        private readonly PurchaseOrderBLL PurchaseOrderBLL;
        private readonly MenuItemControllerHelper MenuItemControllerHelper;
        private readonly ReportingControllerHelper ReportingControllerHelper;
        private readonly SharedHelper SharedHelper;

        public OnlineStoreController(IHttpClientFactory httpClientFactory, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            WebHostEnvironment = webHostEnvironment;
            Configuration = configuration;
            MenuCategoryBLL = new(httpClientFactory);
            MenuItemBLL = new(httpClientFactory);
            PurchaseOrderBLL = new(httpClientFactory);
            MenuItemControllerHelper = new();
            ReportingControllerHelper = new();
            SharedHelper = new();
        }

        private string? GetUsernameFromSession
        {
            get
            {
                try
                {
                    return HttpContext.Session.GetString("Username") ?? null;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        [NonAction]
        private void InitialisePurchaseOrderSessionModelIfNull()
        {
            if (HttpContext.Session.Get<PurchaseOrderSessionModel>("PurchaseOrderSessionModel") is null)
            {
                HttpContext.Session.Set("PurchaseOrderSessionModel", new PurchaseOrderSessionModel()
                {
                    LineItems = new List<MenuItemGridSessionModel>(),
                });
            }
        }

        [NonAction]
        private void InitialisePurchaseOrderSessionModel()
        {
            HttpContext.Session.Set("PurchaseOrderSessionModel", new PurchaseOrderSessionModel()
            {
                LineItems = new List<MenuItemGridSessionModel>(),
            });
        }

        [NonAction]
        private void ClearSearchItem()
        {
            HttpContext.Session.Remove("MenuCategoryName");
            HttpContext.Session.Remove("Name");
        }

        [NonAction]
        private async Task<List<MenuItemResp>> GetMenuItemsByCriteria(string menuCategoryName, string? name, int skip)
        {
            MenuItemPaginationResp _menuItemPaginationResp = await MenuItemBLL.GetMenuItemsByCriteria(new GetMenuItemsByCriteriaReq()
            {
                MenuCategoryName = menuCategoryName,
                Name = name,
                Skip = skip
            });

            await MenuItemControllerHelper.CreateItemPictureFromBase64String(WebHostEnvironment, _menuItemPaginationResp.MenuItems);

            return _menuItemPaginationResp.MenuItems;
        }

        [NonAction]
        private void AddLineItemToPurchaseOrder(AddToCartModel model)
        {
            model.MenuItemId = Guid.Parse(HttpContext.Session.GetString("ApplicationValue"));

            PurchaseOrderSessionModel _purchaseOrderSessionModel = HttpContext.Session.Get<PurchaseOrderSessionModel>("PurchaseOrderSessionModel");

            MenuItemGridSessionModel? _menuItemGridSessionModel =
            HttpContext.Session.Get<PurchaseOrderSessionModel>("PurchaseOrderSessionModel").LineItems.
            Where(li => li.MenuItemId == model.MenuItemId).
            FirstOrDefault();

            if (_menuItemGridSessionModel is null)
            {
                _purchaseOrderSessionModel.LineItems.Add(MenuItemControllerHelper.FillMenuItemGridSessionModel(model));

                HttpContext.Session.Set("PurchaseOrderSessionModel", _purchaseOrderSessionModel);
            }
            else
            {
                List<MenuItemGridSessionModel> _menuItemGridSessionModels = new();

                foreach (var item in _purchaseOrderSessionModel.LineItems)
                {
                    if (!MenuItemControllerHelper.IsMenuItemAddedToCart(_menuItemGridSessionModel, item))
                    {
                        _menuItemGridSessionModels.Add(item);
                    }
                }

                MenuItemControllerHelper.UpdateAddToCartMenuItemGridSessionModel(_menuItemGridSessionModel, model.Quantity);
                _menuItemGridSessionModels.Add(_menuItemGridSessionModel);

                _purchaseOrderSessionModel.LineItems.Clear();
                _purchaseOrderSessionModel.LineItems.AddRange(_menuItemGridSessionModels);

                HttpContext.Session.Set("PurchaseOrderSessionModel", _purchaseOrderSessionModel);
            }
        }

        [NonAction]
        private async Task<PurchaseOrderResp> CreatePurchaseOrder()
        {
            List<LineItemReq> _lineItems = new();

            foreach (var item in HttpContext.Session.Get<PurchaseOrderSessionModel>("PurchaseOrderSessionModel").LineItems)
            {
                _lineItems.Add(new LineItemReq()
                {
                    MenuItemId = item.MenuItemId,
                    PictureFileName = item.PictureFileName,
                    Quantity = item.Quantity,
                    Price = item.Price
                });
            }

            return await PurchaseOrderBLL.CreatePurchaseOrder(GetUsernameFromSession, _lineItems);
        }

        public async Task<ActionResult> Index()
        {
            InitialisePurchaseOrderSessionModelIfNull();

            List<MenuCategoryResp> _menuCategoryResps = await MenuCategoryBLL.GetMenuCategories();

            if (HttpContext.Session.GetString("MenuCategoryName") is null)
            {
                HttpContext.Session.SetString("MenuCategoryName", _menuCategoryResps.OrderBy(menuCategoryResp => menuCategoryResp.Order).FirstOrDefault().Name);
            }

            PartialView("MainMenuItemGrid", MenuItemControllerHelper.FillMainMenuItemGridModel
                (_menuCategoryResps,
                await GetMenuItemsByCriteria(HttpContext.Session.GetString("MenuCategoryName"),
                                             HttpContext.Session.GetString("Name"),
                                             0)));

            return View();
        }

        [HttpGet]
        public ActionResult CartSummary()
        {
            PurchaseOrderSessionModel _purchaseOrderSessionModel = HttpContext.Session.Get<PurchaseOrderSessionModel>("PurchaseOrderSessionModel");

            return Json(new CartSummaryModel()
            {
                Quantity = _purchaseOrderSessionModel.LineItems.Sum(lineItem => lineItem.Quantity).ToString("N0"),
                SubTotal = $"{ StaticClass.EnumHelper.GetEnumDescription(StaticClass.EnumHelper.CurrencyCode.Code)} {_purchaseOrderSessionModel.LineItems.Sum(lineItem => lineItem.SubTotal):N}"
            });
        }

        [HttpGet]
        public async Task<ActionResult> GetMoreMenuItemsByCriteria(int skip)
        {
            return PartialView("MenuItemGrid", MenuItemControllerHelper.FillMenuItemGridModel(await GetMenuItemsByCriteria
                (HttpContext.Session.GetString("MenuCategoryName"),
                 HttpContext.Session.GetString("Name"),
                 skip)));
        }

        [HttpGet]
        public ActionResult SearchMenuItem()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SearchMenuItem(SearchMenuItemModel model)
        {
            List<MenuItemResp> _menuItemResps = await GetMenuItemsByCriteria
                  (HttpContext.Session.GetString("MenuCategoryName"),
                   model.Name,
                   0);

            if (!_menuItemResps.Any())
                ViewBag.GridViewMessage = "Menu Items not found...";

            if (model.Name != null)
                HttpContext.Session.SetString("Name", model.Name);
            else
                HttpContext.Session.Remove("Name");

            return PartialView("SubMenuItemGrid", MenuItemControllerHelper.FillMenuItemGridModel(_menuItemResps));
        }

        [HttpGet]
        public async Task<ActionResult> SearchMenuItemsByMenuCategoryName(string menuCategoryName)
        {
            List<MenuItemResp> _menuItemResps = await GetMenuItemsByCriteria
                  (menuCategoryName,
                   null,
                   0);

            if (!_menuItemResps.Any())
                ViewBag.GridViewMessage = "Menu Items not found...";

            ClearSearchItem();

            HttpContext.Session.SetString("MenuCategoryName", menuCategoryName);

            return PartialView("SubMenuItemGrid", MenuItemControllerHelper.FillMenuItemGridModel(_menuItemResps));
        }

        [HttpGet]
        public async Task<ActionResult> AddToCart(Guid itemMenuId)
        {
            HttpContext.Session.SetString("ApplicationValue", itemMenuId.ToString());

            return PartialView(MenuItemControllerHelper.FillAddToCartModel(await MenuItemBLL.GetMenuItemByMenuItemId(itemMenuId)));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void AddToCart(AddToCartModel model)
        {
            AddLineItemToPurchaseOrder(model);
        }

        [HttpGet]
        public ActionResult ViewCart()
        {
            ViewEditCartModel _model = new();
            _model.LineItems.AddRange(HttpContext.Session.Get<PurchaseOrderSessionModel>("PurchaseOrderSessionModel").LineItems);

            return PartialView(_model);
        }

        [HttpPost]
        public ActionResult UpdateItemQuantity(Guid menuItemId, int quantity)
        {
            PurchaseOrderSessionModel _purchaseOrderSessionModel = HttpContext.Session.Get<PurchaseOrderSessionModel>("PurchaseOrderSessionModel");

            if (quantity <= 0)
            {
                _purchaseOrderSessionModel.LineItems.Remove(_purchaseOrderSessionModel.LineItems.
                Where(lineItem => lineItem.MenuItemId == menuItemId).
                FirstOrDefault());
            }
            else
            {
                List<MenuItemGridSessionModel> _menuItemGridSessionModels = new();

                MenuItemGridSessionModel _menuItemGridSessionModel = _purchaseOrderSessionModel.LineItems.
                Where(lineItem => lineItem.MenuItemId == menuItemId).
                FirstOrDefault();

                MenuItemControllerHelper.UpdateMenuItemGridSessionModel(_menuItemGridSessionModel, quantity);

                _menuItemGridSessionModels.Add(_menuItemGridSessionModel);

                foreach (var lineItem in _purchaseOrderSessionModel.LineItems)
                {
                    if (!MenuItemControllerHelper.IsMenuItemAddedToCart(_menuItemGridSessionModel, lineItem))
                    {
                        _menuItemGridSessionModels.Add(lineItem);
                    }
                }

                _purchaseOrderSessionModel.LineItems.Clear();

                _purchaseOrderSessionModel.LineItems.AddRange(_menuItemGridSessionModels);
            }

            if (!_purchaseOrderSessionModel.LineItems.Any())
                ViewBag.GridViewMessage = "Your Cart is empty";

            HttpContext.Session.Set("PurchaseOrderSessionModel", _purchaseOrderSessionModel);

            return PartialView("MenuItemGridSession", _purchaseOrderSessionModel.LineItems.OrderBy(lineItem => lineItem.Name).ToList());
        }

        [HttpGet]
        public ActionResult ShouldAuthenticateCustomer()
        {
            if (string.IsNullOrWhiteSpace(GetUsernameFromSession))
                return RedirectToAction("CustomerAuthentication");
            else
                return RedirectToAction("PurchaseOrderConfirmation");
        }

        public ActionResult CustomerAuthentication()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> PurchaseOrderConfirmation()
        {
            PurchaseOrderResp _purchaseOrderResp = await CreatePurchaseOrder();

            PurchaseOrderSessionModel _purchaseOrderSessionModel = HttpContext.Session.Get<PurchaseOrderSessionModel>("PurchaseOrderSessionModel");
            _purchaseOrderSessionModel.PurchaseOrderId = _purchaseOrderResp.PurchaseOrderId;
            _purchaseOrderSessionModel.PurchaseOrderNumber = _purchaseOrderResp.PurchaseOrderNumber;
            _purchaseOrderSessionModel.PaymentStatus = _purchaseOrderResp.PaymentStatus;
            _purchaseOrderSessionModel.ShippingStatus = _purchaseOrderResp.ShippingStatus;
            _purchaseOrderSessionModel.PurchaseOrderDate = _purchaseOrderResp.CreationDate;

            PurchaseOrderReportModel _purchaseOrderReportModel = ReportingControllerHelper.FillPurchaseOrderReportModel(GetUsernameFromSession, Configuration, _purchaseOrderSessionModel);
            string _fileName = SharedHelper.GeneratePdfFile(WebHostEnvironment, "purchaseOrders", await this.RenderViewAsync("_PurchaseOrderPdf", _purchaseOrderReportModel, true));

            ViewBag.Title = "THANK YOU FOR SHOPPING WITH US";
            ViewBag.ShipToEmailAddress = _purchaseOrderReportModel.ShipToEmailAddress;
            ViewBag.EmailBody = $"Your order has been placed successful.\n\nPLEASE USE PURCHASE ORDER NUMBER AS YOUR REFERENCE WHEN MAKING PAYMENT, EMAIL AN OFFICIAL PROOF OF PAYMENT WITHIN 24 HOURS TO {Configuration["CompanyInformation:CompanyEmailAddress"].ToUpper()}. EMAILING YOUR PROOF OF PAYMENT FROM INTERNET BANKING DIRECTLY WILL SPEED UP YOUR PURCHASE ORDER SHIPPING. THANK YOU.\n\nPlease find attached your Purchase Order.\n";

            string _htmlMailBody = await this.RenderViewAsync("_EmailPurchaseOrder", null);

            List<string> _emailAddressTo = new() { _purchaseOrderReportModel.ShipToEmailAddress };
            List<string> _attachemts = new() { Path.Combine(WebHostEnvironment.WebRootPath, "purchaseOrders", _fileName) };

            PurchaseOrderConfirmationModel _purchaseOrderConfirmationModel = new()
            {
                AccountName = Configuration["CompanyInformation:AccountName"],
                AccountNumber = Configuration["CompanyInformation:AccountNumber"],
                BankName = Configuration["CompanyInformation:BankName"],
                BranchCode = Configuration["CompanyInformation:BranchCode"],
                BranchName = Configuration["CompanyInformation:BranchName"],
                AmountDue = _purchaseOrderSessionModel.SubTotal,
                PurchaseOrderNumber = _purchaseOrderSessionModel.PurchaseOrderNumber
            };

            InitialisePurchaseOrderSessionModel();

            SharedHelper.SendEmail(_emailAddressTo, $"{Configuration["CompanyInformation:CompanyName"]} - Purchase Order Placed Successful",
            _htmlMailBody, _attachemts, new List<LinkedResource>());

            return View(_purchaseOrderConfirmationModel);
        }
    }
}
