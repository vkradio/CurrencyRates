using ApplicationCore.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using UnitTests.ApplicationCore.Specifications;
using UnitTests.Builders;
using WebAPI;

namespace FunctionalTests.Web.Controllers
{
    public class StartupTesting : Startup
    {
        public StartupTesting(IConfiguration configuration) : base(configuration) { }

        public void ConfigureTestingServices(IServiceCollection services)
        {
            services.AddSingleton<ITimeService, ManagedTimeService>();
            services.AddDbContext<CurrencyRateContext>(c => c.UseInMemoryDatabase("CurrencyRate"));
            ConfigureServices(services);
        }
    }

    public abstract class BaseWebTest
    {
        protected readonly HttpClient httpClient;
        protected ITimeService timeService;
        protected string contentRoot;

        public BaseWebTest()
        {
            timeService = new ManagedTimeService();
            httpClient = GetClient();
        }

        protected HttpClient GetClient()
        {
            var startupAssembly = typeof(Startup).GetTypeInfo().Assembly;
            contentRoot = GetProjectPath("src", startupAssembly);
            var builder = new WebHostBuilder()
                .UseContentRoot(contentRoot)
                .UseEnvironment("Testing")
                .UseStartup<StartupTesting>();

            var server = new TestServer(builder);

            // Seed.
            using (var scope = server.Host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var currencyRateContext = services.GetRequiredService<CurrencyRateContext>();
                timeService = services.GetRequiredService<ITimeService>();
                if (!currencyRateContext.CurrencyRates.Any())
                {
                    currencyRateContext.AddRange(RatesOfCurrency.GetTestRates(setIds: false, now: timeService.Now));
                    currencyRateContext.SaveChanges();
                }
                //var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                //var catalogContext = services.GetRequiredService<CatalogContext>();
                //CatalogContextSeed.SeedAsync(catalogContext, loggerFactory)
                //    .Wait();

                //var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                //AppIdentityDbContextSeed.SeedAsync(userManager).Wait();
            }

            return server.CreateClient();
        }

        protected static string GetProjectPath(string solutionRelativePath, Assembly startupAssembly)
        {
            var projectName = startupAssembly.GetName().Name;

            var applicationBasePath = PlatformServices.Default.Application.ApplicationBasePath;

            var directoryInfo = new DirectoryInfo(applicationBasePath);
            do
            {
                var solutionFileInfo = new FileInfo(Path.Combine(directoryInfo.FullName, "CurrencyRates.sln"));
                if (solutionFileInfo.Exists)
                    return Path.GetFullPath(Path.Combine(directoryInfo.FullName, solutionRelativePath, projectName));

                directoryInfo = directoryInfo.Parent;
            }
            while (directoryInfo.Parent != null);

            throw new Exception($"Solution root could not be located using application root {applicationBasePath}.");
        }
    }
}
