
using Kontrax.RegiX.Core.TestStandard.Shared;
using Kontrax.RegiX.Core.TestStandard.Tests;
using System.Threading.Tasks;

namespace Kontrax.RegiX.Core.TestStandard
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            //await TestRegiXClient.TestReports();
            await TestXsdToObject.TestParseXsd(ReportType.JobSeekerStatus);
            //TestXsd.TestBuildXMLRequest();
        }


    }
}
