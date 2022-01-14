using DerivcoKitchenClient.Models.Shared;
using Microsoft.AspNetCore.Mvc;

namespace DerivcoKitchenClient.Controllers
{
    public class SharedController : Controller
    {
        private string? GetUsernameFromSession
        {
            get
            {
                return HttpContext.Session.GetString("Username") ?? null;
            }
        }

        [HttpGet]
        public ActionResult CustomerAccount()
        {
            if (!string.IsNullOrWhiteSpace(GetUsernameFromSession))
            {
                return PartialView("_CustomerAccount");
            }
            else
            {
                return PartialView("_Welcome");
            }
        }

        [HttpGet]
        public ActionResult Ok(string okMessage, string messageSymbol)
        {
            return PartialView("_Ok", new OkModel() { OkMessage = okMessage, MessageSymbol = messageSymbol });
        }
    }
}
