using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using VaccineHub.Service.Abstractions;

namespace VaccineHub.Service.GenerateCertificates
{
    public class PdfService : IPdfService
    {
        public Task GenerateCertificateAndSend(string productName, string dosage, string center, string mailId, DateTime appointmentDate)
        {
            var document = new PdfDocument();
            var page = document.Pages.Add();
            var graphics = page.Graphics;
            PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 20);
            graphics.DrawString("***** Vaccination Certificate Issued by Vaccine Hub *****" + Environment.NewLine + Environment.NewLine + 
                                $"Vaccine : {productName}"  + Environment.NewLine + Environment.NewLine + 
                                $"Center : {center}" + Environment.NewLine + Environment.NewLine + 
                                $"VaccinationDate : {appointmentDate.Date:dd/MM/yyyy}" + Environment.NewLine + Environment.NewLine +  
                                $"Dosage : {dosage}",
                font, PdfBrushes.Black, new PointF(0, 0));
            var stream = new MemoryStream();
            document.Save(stream);
            stream.Position = 0;

            using var mm = new MailMessage("uit13328@rmd.ac.in", mailId);
            mm.Subject = "Vaccine Hub - Vaccination Certificate";
                
            mm.Body = $"Hi"+ Environment.NewLine +$", Please find your {dosage} dosage certificate. Thanks for using our service.";
            mm.Attachments.Add(new Attachment(stream, "Certificate.pdf"));
            mm.IsBodyHtml = false;

            using var smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials =  new NetworkCredential("uit13328@rmd.ac.in", "143password");
            smtp.Send(mm);

            return Task.CompletedTask;
        }
    }
}