using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using VaccineHub.Service.Abstractions;

namespace VaccineHub.Service.GenerateCertificates
{
    public class PdfService : IPdfService
    {   
        private readonly IServiceProvider _serviceProvider;

        
        public PdfService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            
        }

        public void GenerateCertificate(string productName, string dosage, string center, string mailId, DateTime AppointmentData)
        {
            PdfDocument document = new PdfDocument();
            PdfPage page = document.Pages.Add();
            PdfGraphics graphics = page.Graphics;
            PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 20);
            var content = new StringBuilder();
            content.AppendLine("Hi,");
            graphics.DrawString("Hi" + System.Environment.NewLine + 
                                "Your Vaccination Details" + System.Environment.NewLine +
                                $" Vaccine Name ={productName}. Center ={center}. {System.Environment.NewLine} AppointmentData = {AppointmentData.Date} {System.Environment.NewLine}.Dosage = {dosage}", font, PdfBrushes.Black, new PointF(0, 0));
            MemoryStream stream = new MemoryStream();
            document.Save(stream);
            stream.Position = 0;
            
            using (var mm = new MailMessage("uit13328@rmd.ac.in", mailId))
            {
                mm.Subject = "Vaccine Hub - Vaccination Certificate";
                
                mm.Body = $"Hi"+ System.Environment.NewLine +$", Please find your {dosage} dosage certificate. Thanks for using our service.";
                mm.Attachments.Add(new Attachment(stream, "Certificate.pdf"));
                mm.IsBodyHtml = false;
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials =  new NetworkCredential("uit13328@rmd.ac.in", "143password");
                    smtp.Send(mm);
                }
            }
            
        }
    }
}