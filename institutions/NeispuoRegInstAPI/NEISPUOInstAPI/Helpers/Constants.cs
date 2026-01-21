using Microsoft.AspNetCore.Hosting.Server.Features;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NEISPUORegInstAPI.Helpers
{
    public static class Constants
    {
        public static string _JSONPATH = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),"JSONMeta");
        public static string _CERTPATH = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "CERT");

    }
}
