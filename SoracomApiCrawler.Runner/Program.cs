using System.Threading.Tasks;
using System.Configuration;

namespace SoracomApiCrawler.Runner
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
