using System.Configuration;
using System.Threading.Tasks;

namespace SoracomApiCrawler.RunnerSample
{
    class Program
    {
        static void Main()
        {
            var crawler = new SoracomCrawler();
            var connectionString = ConfigurationManager.ConnectionStrings["SoracomApiStorage"].ConnectionString;
            Task.Run(() => crawler.PerformCrawl(connectionString)).Wait();
        }
    }
}
