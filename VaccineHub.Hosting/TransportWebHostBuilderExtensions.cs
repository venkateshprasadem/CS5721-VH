using System;
using System.ComponentModel;
using Microsoft.AspNetCore.Hosting;

namespace VaccineHub.Hosting
{
    public static class TransportWebHostBuilderExtensions
    {
        public static IWebHostBuilder UseTransport(this IWebHostBuilder webHostBuilder, KestrelTransport transport)
        {
            if (webHostBuilder == null)
            {
                throw new ArgumentNullException(nameof(webHostBuilder));
            }

            if (!Enum.IsDefined(typeof(KestrelTransport), transport))
            {
                throw new InvalidEnumArgumentException(nameof(transport), (int) transport, typeof(KestrelTransport));
            }

            return transport == KestrelTransport.Libuv 
                ? webHostBuilder.UseLibuv() 
                : webHostBuilder.UseSockets();
        }
    }
}
