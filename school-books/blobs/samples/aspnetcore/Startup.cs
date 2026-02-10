using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace aspnetcore
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/myblobs/{blobId:int}", context =>
                {
                    // Important!!!
                    // Validate that the user has the right to access that blobId

                    int blobId = Convert.ToInt32(context.Request.RouteValues["blobId"]);
                    string HMACKey = "BGrFkf9yQ9JJoA47oNBE";
                    string blobServiceUrl = "http://blobs.neispuo.mon.bg";

                    long unixTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    string message = $"{blobId}/{unixTimeSeconds}";

                    using HMACSHA256 hash = new HMACSHA256(USAsciiStrict.GetBytes(HMACKey));
                    byte[] hmac = hash.ComputeHash(USAsciiStrict.GetBytes(message));
                    string urlSafeBase64HMAC =
                        Convert.ToBase64String(hmac)
                        // Url-safe Base64 / RFC 4648
                        // https://tools.ietf.org/html/rfc4648
                        .Replace('+', '-')
                        .Replace('/', '_')
                        .TrimEnd('=');

                    var location = $"{blobServiceUrl}/{blobId}?t={unixTimeSeconds}&h={urlSafeBase64HMAC}";
                    context.Response.Redirect(location);

                    return Task.CompletedTask;
                });
            });
        }

        public static readonly Encoding USAsciiStrict = Encoding.GetEncoding(
            "us-ascii",
            new EncoderExceptionFallback(),
            new DecoderExceptionFallback());
    }
}
