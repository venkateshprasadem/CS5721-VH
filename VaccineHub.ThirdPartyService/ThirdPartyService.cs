using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VaccineHub.ThirdPartyService.Models;

namespace VaccineHub.ThirdPartyService
{
    internal class ThirdPartyService : IThirdPartyService, IDisposable
    {
        private readonly HttpClient _client;

        private static readonly TimeSpan MaximumTimeout = TimeSpan.FromSeconds(30);

        public ThirdPartyService()
        {
            _client = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            });
        }

        public async Task CallAsync<T>(T t, CancellationToken cancellationToken)
        {
            if (t == null)
            {
                throw new ArgumentNullException(nameof(t));
            }

            // Should be configurable from configuration
            // Bad Coding
            var url = t switch
            {
                PaymentServiceRequest => "http://localhost:5010/Transact",
                NotifyDetailsRequest => "http://localhost:5010/NotifyDetails",
                _ => throw new ArgumentException("Input parameter type is not supported")
            };

            using var response = await PostAsync(new Uri(url), t, cancellationToken);

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
        
        public void Dispose()
        {
            _client.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}