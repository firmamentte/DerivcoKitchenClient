using DerivcoKitchenClient.Models.Shared;
using SelectPdf;
using System.Net.Mail;

namespace DerivcoKitchenClient.Controllers.ControllerHelpers
{
    public class SharedHelper
    {
        public string GeneratePdfFile
        (IWebHostEnvironment webHostEnvironment, string webRootFolderName, string htmlString, PdfPageOrientation pdfPageOrientation = PdfPageOrientation.Portrait)
        {
            string _fileName = $"{Guid.NewGuid().ToString().Replace("-", "")}.pdf";

            HtmlToPdf _converter = new();
            _converter.Options.MarginTop = 20;
            _converter.Options.MarginRight = 20;
            _converter.Options.MarginBottom = 20;
            _converter.Options.MarginLeft = 20;
            _converter.Options.EmbedFonts = true;
            _converter.Options.KeepImagesTogether = true;
            _converter.Options.PdfPageOrientation = pdfPageOrientation;

            PdfDocument _doc = _converter.ConvertHtmlString(htmlString);
            _doc.Save(Path.Combine(webHostEnvironment.WebRootPath, webRootFolderName, _fileName));
            _doc.Close();

            return _fileName;
        }

        public void SendEmail(List<string> emailAddressTo, string subject, string htmlMailBody, List<string> attachemts, List<LinkedResource> listLinkedResources)
        {
                FirmamentUtilities.Utilities.EmailHelper.SendEmail
                (emailAddressTo, 
                BLL.StaticClass.AddressSendFrom, 
                BLL.StaticClass.AddressSendFromPassword, 
                BLL.StaticClass.EmailClientHost, 
                (int)BLL.StaticClass.SendEmailPort, false, false, subject, htmlMailBody, attachemts, listLinkedResources);
        }

        public OkModel FillOkModel(string message, StaticClass.EnumHelper.MessageSymbol messageSymbol)
        {
            return new OkModel() { OkMessage = message, MessageSymbol = StaticClass.EnumHelper.GetEnumDescription(messageSymbol) };
        }
    }
}
