using DerivcoKitchenClient.BLL.BLLClasses;
using DerivcoKitchenClient.BLL.DataContract;
using DerivcoKitchenClient.Controllers.ControllerHelpers;
using DerivcoKitchenClient.Models.ApplicationUser;
using Microsoft.AspNetCore.Mvc;

namespace DerivcoKitchenClient.Controllers
{
    [ResponseCache(NoStore = true, Duration = 0, Location = ResponseCacheLocation.None, VaryByHeader = "*")]
    public class ApplicationUserController : Controller
    {
        private readonly ApplicationUserBLL ApplicationUserBLL;
        private readonly SharedHelper SharedHelper;

        public ApplicationUserController(IHttpClientFactory httpClientFactory)
        {
            ApplicationUserBLL = new(httpClientFactory);
            SharedHelper = new();
        }

        [HttpGet]
        public ActionResult SignUp()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SignUp(SignUpModel model)
        {
            await ApplicationUserBLL.SignUp(new SignUpReq()
            {
                EmailAddress = model.EmailAddress,
                UserPassword = model.UserPassword
            });

            return PartialView("_Ok", SharedHelper.FillOkModel("Congrats...! You have Signed Up successful, Please use your Email Address and Password to Sign in", StaticClass.EnumHelper.MessageSymbol.Information));
        }

        [HttpGet]
        public ActionResult CheckOutSignUp()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task CheckOutSignUp(SignUpModel model)
        {
            await ApplicationUserBLL.SignUp(new SignUpReq()
            {
                EmailAddress = model.EmailAddress,
                UserPassword = model.UserPassword
            });

            HttpContext.Session.SetString("Username", model.EmailAddress);
        }

        [HttpGet]
        public ActionResult SignIn()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task SignIn(SignInModel model)
        {
            await ApplicationUserBLL.SignIn(new SignInReq()
            {
                EmailAddress = model.EmailAddress,
                UserPassword = model.UserPassword
            });

            HttpContext.Session.SetString("Username", model.EmailAddress);
        }

        [HttpGet]
        public ActionResult CheckOutSignIn()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task CheckOutSignIn(SignInModel model)
        {
            await ApplicationUserBLL.SignIn(new SignInReq()
            {
                EmailAddress = model.EmailAddress,
                UserPassword = model.UserPassword
            });

            HttpContext.Session.SetString("Username", model.EmailAddress);
        }

        [HttpGet]
        public ActionResult UserSignOut()
        {
            HttpContext.Session.Clear();

            return RedirectToActionPermanent("Index", "OnlineStore");
        }
    }
}
