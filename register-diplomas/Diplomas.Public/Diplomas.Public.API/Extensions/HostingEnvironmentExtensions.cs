using Microsoft.AspNetCore.Hosting;
using System;

namespace Diplomas.Public.API.Extensions
{
    public static class HostingEnvironmentExtensions
    {
        public static bool IsTestiis(this IWebHostEnvironment hostingEnvironment)
        {
            ArgumentNullException.ThrowIfNull(hostingEnvironment);

            return hostingEnvironment.EnvironmentName.Equals("testiis", StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsProd(this IWebHostEnvironment hostingEnvironment)
        {
            ArgumentNullException.ThrowIfNull(hostingEnvironment);

            return hostingEnvironment.EnvironmentName.Equals("prod", StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsTest(this IWebHostEnvironment hostingEnvironment)
        {
            ArgumentNullException.ThrowIfNull(hostingEnvironment);

            return hostingEnvironment.EnvironmentName.Equals("test", StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsDev(this IWebHostEnvironment hostingEnvironment)
        {
            ArgumentNullException.ThrowIfNull(hostingEnvironment);

            return hostingEnvironment.EnvironmentName.Equals("dev", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
