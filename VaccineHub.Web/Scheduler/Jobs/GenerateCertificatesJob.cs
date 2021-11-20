using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Quartz;
using VaccineHub.Service.Abstractions;

namespace VaccineHub.Web.Scheduler
{
    public class GenerateCertificatesJob : IJob
    {
        
        private IPDFService pdfService;
        public GenerateCertificatesJob(IPDFService pdfService)
        {
            this.pdfService = pdfService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                Stream stream = await pdfService.GenerateCertificate();
                using (MailMessage mm = new MailMessage("uit13328@rmd.ac.in", "21004528@studentmail.ul.ie"))
                {
                    mm.Subject = "Generated Certificate";
                    mm.Body = "Include Body";
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
                //return Task.CompletedTask;
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}