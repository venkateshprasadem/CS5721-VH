using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using VaccineHub.Service.Abstractions;

namespace VaccineHub.Service.GenerateCertificates
{
    public class PdfMailVisitor : IVisitor
    {
        private static void GenerateCertificateAndSend(string certContent, string mailBody, string mailId, string certName)
        {
            var document = new PdfDocument();
            var page = document.Pages.Add();
            var graphics = page.Graphics;
            PdfFont font = new PdfStandardFont(PdfFontFamily.TimesRoman, 20);
            graphics.DrawString(certContent,
                font, PdfBrushes.Black, new PointF(0, 0));
            var stream = new MemoryStream();
            document.Save(stream);
            stream.Position = 0;

            using var mm = new MailMessage("uit13328@rmd.ac.in", mailId);
            mm.Subject = "Vaccine Hub - Vaccination Certificate";

            mm.Body = mailBody;
            mm.Attachments.Add(new Attachment(stream, certName));
            mm.IsBodyHtml = false;
            
            using var smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials =  new NetworkCredential("uit13328@rmd.ac.in", "143password");
            smtp.Send(mm);
        }

        public void VisitProvisionalVaccinationCertificate(Persistence.Entities.Booking booking)
        {
            var certContent = "Vaccine Hub Org" + Environment.NewLine + Environment.NewLine + 
                              "***** Provisional Vaccination Certificate *****" + Environment.NewLine + Environment.NewLine + 
                              $"Vaccine : {booking.Product.Name}" + Environment.NewLine + Environment.NewLine +
                              $"Center : {booking.Center.Name}" + Environment.NewLine + Environment.NewLine +
                              $"VaccinationDate : {booking.AppointmentDate.Date:dd/MM/yyyy}" + Environment.NewLine + Environment.NewLine + 
                              $"Dosage : {booking.DosageType?.ToString()}";

            var mailBody = $"Hi,"+ Environment.NewLine +$"Please find your provisional dosage certificate. Thanks for using our service.";

            GenerateCertificateAndSend(certContent, mailBody, booking.ApiUser.EmailId, "provisionalCertificate.pdf");
        }

        public void VisitFinalVaccinationCertificate(Persistence.Entities.Booking booking)
        {
            var certContent = "Vaccine Hub Org" + Environment.NewLine + Environment.NewLine + 
                              "***** Final Vaccination Certificate *****" + Environment.NewLine + Environment.NewLine + 
                              $"Vaccine : {booking.Product.Name}" + Environment.NewLine + Environment.NewLine + 
                              $"Center : {booking.Center.Name}" + Environment.NewLine + Environment.NewLine + 
                              $"VaccinationDate : {booking.AppointmentDate.Date:dd/MM/yyyy}" + Environment.NewLine + Environment.NewLine + 
                              $"Dosage : {booking.DosageType?.ToString()}";

            var mailBody = $"Hi,"+ Environment.NewLine +$"Please find your final dosage certificate. Thanks for using our service.";

            GenerateCertificateAndSend(certContent, mailBody, booking.ApiUser.EmailId, "finalCertificate.pdf");
        }
    }
}