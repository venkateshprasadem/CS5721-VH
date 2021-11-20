using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VaccineHub.PaymentService.Models;

namespace VaccineHub.PaymentService
{
    internal class PaymentService : IPaymentService, IDisposable
    {
        private readonly HttpClient _client;

        private static readonly TimeSpan MaximumTimeout = TimeSpan.FromSeconds(30);

        public PaymentService()
        {
            _client = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            });
        }

        public void Dispose()
        {
            _client.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task DebitPaymentAsync(PaymentInformation paymentInformation, CancellationToken cancellationToken)
        {
            if (paymentInformation == null)
            {
                throw new ArgumentNullException(nameof(paymentInformation));
            }

            // URL should be configurable from settings
            using var response = await PostAsync(new Uri("http://localhost:5010/DebitPayment"), paymentInformation, cancellationToken);

            response.EnsureSuccessStatusCode();
        }

        public async Task CreditPaymentAsync(PaymentInformation paymentInformation, CancellationToken cancellationToken)
        {
            if (paymentInformation == null)
            {
                throw new ArgumentNullException(nameof(paymentInformation));
            }

            // URL should be configurable from settings
            using var response = await PostAsync(new Uri("http://localhost:5010/CreditPayment"), paymentInformation, cancellationToken);

            response.EnsureSuccessStatusCode();
        }

        private async Task<HttpResponseMessage> PostAsync(Uri uri, object data, CancellationToken cancellationToken)
        {
            var json = JsonConvert.SerializeObject(data);

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(MaximumTimeout);

            using var request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            return await _client.SendAsync(request, cts.Token);
        }
    }
}