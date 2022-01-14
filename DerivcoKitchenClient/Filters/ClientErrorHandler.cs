using Microsoft.AspNetCore.Mvc.Filters;
using System.Net.Mime;

namespace DerivcoKitchenClient.Filters
{
    public class ClientErrorHandler : IAsyncExceptionFilter
    {
        public async Task OnExceptionAsync(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;

            var response = filterContext.HttpContext.Response;
            response.StatusCode = 500;
            response.ContentType = MediaTypeNames.Text.Plain;
            await response.WriteAsync(filterContext.Exception.Message.Replace("\n", "<br />"));
        }
    }
}
