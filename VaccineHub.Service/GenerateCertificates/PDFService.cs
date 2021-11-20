using System;
using System.IO;
using System.Threading.Tasks;
using DinkToPdf;
using DinkToPdf.Contracts;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using VaccineHub.Service.Abstractions;

namespace VaccineHub.Service.GenerateCertificates
{
    public class PDFService : IPDFService
    {   
        private readonly IConverter _converter;
        public PDFService(IConverter converter)
        {
            _converter = converter;
        }

        public async  Task<Stream> GenerateCertificate()
        {
            PdfDocument document = new PdfDocument();
  
            //Add a page to the document
            PdfPage page = document.Pages.Add();
  
            //Create PDF graphics for the page
            PdfGraphics graphics = page.Graphics;
  
            //Set the standard font
            PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 20);
  
            //Draw the text
            graphics.DrawString("Fuck!!! Am vaccinated", font, PdfBrushes.Black, new PointF(0, 0));
  
            //Saving the PDF to the MemoryStream
            MemoryStream stream = new MemoryStream();
  
            document.Save(stream);
  
            //Set the position as '0'.
            stream.Position = 0;

            return stream;
        }
        
        
        public byte[] GeneratePdfReport()
        {
            var html = $@"
                   <!DOCTYPE html>
                   <html lang=""en"">
                   <head>
                       Vaccination Certificate.
                   </head>
                  <body>
                  <h1>This is the heading for demonstration purposes only.</h1>
                  <p>This is a line of text for demonstration purposes only.</p>
                  </body>
                  </html>
                  ";
            GlobalSettings globalSettings = new GlobalSettings();
            globalSettings.ColorMode = ColorMode.Color;
            globalSettings.Orientation = Orientation.Portrait;
            globalSettings.PaperSize = PaperKind.A4;
            globalSettings.Margins = new MarginSettings { Top = 25, Bottom = 25 };
            ObjectSettings objectSettings = new ObjectSettings();
            objectSettings.PagesCount = true;
            objectSettings.HtmlContent = html;
            WebSettings webSettings = new WebSettings();
            webSettings.DefaultEncoding = "utf-8";
            HeaderSettings headerSettings = new HeaderSettings();
            headerSettings.FontSize = 15;
            headerSettings.FontName = "Ariel";
            headerSettings.Right = "Page [page] of [toPage]";
            headerSettings.Line = true;
            FooterSettings footerSettings = new FooterSettings();
            footerSettings.FontSize = 12;
            footerSettings.FontName = "Ariel";
            footerSettings.Center = "Vaccine Hub";
            footerSettings.Line = true;
            objectSettings.HeaderSettings = headerSettings;
            objectSettings.FooterSettings = footerSettings;
            objectSettings.WebSettings = webSettings;
            HtmlToPdfDocument htmlToPdfDocument = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings },
            };
            return _converter.Convert(htmlToPdfDocument);
        }
    }
}