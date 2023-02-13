using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using static Microsoft.AspNetCore.WebHost;
using Microsoft.Extensions.DependencyInjection;

namespace PortfolioRebalanceSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
