using DerivcoKitchenClient.Models.OnlineStore;
using DerivcoKitchenClient.Models.Reporting;

namespace DerivcoKitchenClient.Controllers.ControllerHelpers
{
    public class ReportingControllerHelper
    {
        public PurchaseOrderReportModel FillPurchaseOrderReportModel(string username, IConfiguration configuration, PurchaseOrderSessionModel purchaseOrderSessionModel)
        {
            PurchaseOrderReportModel _purchaseOrderReportModel = new()
            {
                AccountName = configuration["CompanyInformation:AccountName"],
                AccountNumber = configuration["CompanyInformation:AccountNumber"],
                BankName = configuration["CompanyInformation:BankName"],
                BranchCode = configuration["CompanyInformation:BranchCode"],
                BranchName = configuration["CompanyInformation:BranchName"],
                CompanyName = configuration["CompanyInformation:CompanyName"],
                RegistrationNumber = configuration["CompanyInformation:RegistrationNumber"],
                VATNumber = configuration["CompanyInformation:VATNumber"],
                CompanyAddress = configuration["CompanyInformation:CompanyAddress"],
                CompanyEmailAddress = configuration["CompanyInformation:CompanyEmailAddress"],
                CompanyFaxNumber = configuration["CompanyInformation:CompanyFaxNumber"],
                CompanyMobileNumber = configuration["CompanyInformation:CompanyMobileNumber"],
                CompanyTelephoneNumber = configuration["CompanyInformation:CompanyTelephoneNumber"],
                CompanyWebsiteAddress = configuration["CompanyInformation:CompanyWebsiteAddress"],
                ShipToEmailAddress = username,
                PurchaseOrderDate = purchaseOrderSessionModel.PurchaseOrderDate,
                PurchaseOrderNumber = purchaseOrderSessionModel.PurchaseOrderNumber,
                PaymentStatus = purchaseOrderSessionModel.PaymentStatus,
                ShippingStatus = purchaseOrderSessionModel.ShippingStatus,
                AmountDue = purchaseOrderSessionModel.SubTotal
            };

            foreach (var lineItem in purchaseOrderSessionModel.LineItems.OrderBy(lineItem => lineItem.Quantity).ThenBy(lineItem => lineItem.Name))
            {
                _purchaseOrderReportModel.LineItems.Add(new PurchaseOrderLineItemModel()
                {
                    Name = lineItem.Name,
                    Quantity = lineItem.Quantity,
                    Price = lineItem.Price,
                    SubTotal = lineItem.SubTotal
                });
            }

            return _purchaseOrderReportModel;
        }
    }
}
