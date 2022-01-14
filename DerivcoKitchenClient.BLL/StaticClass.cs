using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;

namespace DerivcoKitchenClient.BLL
{
    public static class StaticClass
    {
        public static MediaTypeWithQualityHeaderValue MediaTypeWithQualityHeaderValue
        {
            get
            {
                return new MediaTypeWithQualityHeaderValue("application/json");
            }
        }

        public static string AccessToken { get; set; }

        public static string AddressSendFrom { get; set; }

        public static string AddressSendFromPassword { get; set; }

        public static string EmailClientHost { get; set; }

        public static int? SendEmailPort { get; set; }

        public static string DerivcoKitchenWebAPIBaseAddress { get; set; }

        public static void InitializeAppSettings(IConfiguration configuration)
        {
            DerivcoKitchenWebAPIBaseAddress ??= configuration["AppSettings:Urls:DerivcoKitchenWebAPIBaseAddress"];
            AddressSendFrom ??= configuration["AppSettings:AddressSendFrom"];
            AddressSendFromPassword ??= configuration["AppSettings:Passwords:AddressSendFromPassword"];
            EmailClientHost ??= configuration["AppSettings:Urls:EmailClientHost"];
            SendEmailPort ??= Convert.ToInt32(configuration["AppSettings:Ports:SendEmailPort"]);
        }
    }
}