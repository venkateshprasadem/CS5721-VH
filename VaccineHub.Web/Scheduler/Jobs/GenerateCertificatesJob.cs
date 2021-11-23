using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using VaccineHub.Persistence;
using VaccineHub.Persistence.Types;
using VaccineHub.Service.Abstractions;

namespace VaccineHub.Web.Scheduler.Jobs
{
    public class GenerateCertificatesJob : IJob
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IPdfService _pdfService;

        public GenerateCertificatesJob(IPdfService pdfService, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _pdfService = pdfService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var scope = _serviceProvider.CreateScope();
            var dbContext =scope.ServiceProvider.GetRequiredService<IVaccineHubDbContext>();

            var bookings = dbContext.Bookings.Where(i => !i.IsCertGenerated &&
                                                         i.BookingType.Value == BookingType.Book &&
                                                         i.AppointmentDate.Date ==
                                                         DateTime.Today.AddDays(-1)
                                                             .Date)
                .Include(i => i.ApiUser)
                .Include(i => i.Product)
                .Include(i => i.Center)
                .ToList();

            var tasks = bookings.Select(async bookingObj =>
            {
                try
                {
                    await _pdfService.GenerateCertificateAndSend(bookingObj.Product.Name, bookingObj.DosageType.ToString(),
                        bookingObj.Center.Name,bookingObj.ApiUser.EmailId, bookingObj.AppointmentDate);
                    bookingObj.IsCertGenerated = true;
                    await dbContext.SaveChangesAsync();
                }
                catch
                {
                    // ignored
                }
            });

            await Task.WhenAll(tasks);
        }
    }
}