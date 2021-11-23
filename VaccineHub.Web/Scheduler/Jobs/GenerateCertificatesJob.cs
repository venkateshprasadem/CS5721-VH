using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using VaccineHub.Persistence;
using VaccineHub.Persistence.Types;
using VaccineHub.Service.Abstractions;

namespace VaccineHub.Web.Scheduler
{
    public class GenerateCertificatesJob : IJob
    {
        private IServiceProvider _serviceProvider;
        private IPdfService _pdfService;
        public GenerateCertificatesJob(IPdfService pdfService, IServiceProvider _serviceProvider)
        {
            this._serviceProvider = _serviceProvider;
            this._pdfService = pdfService;
        }

        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                    #if DEBUG
                        _pdfService.GenerateCertificate("AstaZeneca", "First",
                            "Mumbai", "uit13328@rmd.ac.in", DateTime.Now);
                    #else
                        var scope = _serviceProvider.CreateScope();
                        var dbContext =scope.ServiceProvider.GetRequiredService<IVaccineHubDbContext>();

                        var bookings = dbContext.Bookings.
                            Where(i => !i.IsCertGenerated 
                                       && i.BookingType.Value == BookingType.Book 
                                       && i.AppointmentDate.Date == DateTime.Today.AddDays(-1).Date).
                            Include(i => i.ApiUser ).
                            Include(i=> i.Product).ToList();
                        Parallel.ForEach(bookings, bookingObj =>
                        {
                            if (bookingObj.AppointmentDate >= DateTime.Now)
                            {
                                _pdfService.GenerateCertificate(bookingObj.Product.Name, bookingObj.DosageType.ToString(),
                                    bookingObj.Center.Name,bookingObj.ApiUser.EmailId, bookingObj.AppointmentDate);
                            }
                        });
                    #endif
                    return Task.CompletedTask;

            }
            catch(Exception )
            {
                throw;
            }
        }
    }
}