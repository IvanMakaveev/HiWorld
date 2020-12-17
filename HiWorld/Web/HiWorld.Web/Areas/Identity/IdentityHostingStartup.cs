using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(HiWorld.Web.Areas.Identity.IdentityHostingStartup))]

namespace HiWorld.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}