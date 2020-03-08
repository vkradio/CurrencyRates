using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Services;
using Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            services.AddSingleton<ITimeService, SystemTimeService>();

            // in-memory database
            services.AddDbContext<CurrencyRateContext>(c => c.UseInMemoryDatabase("CurrencyRate"));

            ConfigureServices(services);

            // real database
            // ConfigureProductionServices(services);
        }
        //public void ConfigureTestingServices(IServiceCollection services)
        //{
        //    services.AddDbContext<CurrencyRateContext>(c => c.UseInMemoryDatabase("CurrencyRate"));

        //    //services.AddDbContext<AppIdentityDbContext>(options =>
        //    //    options.UseInMemoryDatabase("Identity"));

        //    ConfigureServices(services);
        //}

        public void ConfigureProductionServices(IServiceCollection services)
        {
            services.AddSingleton<ITimeService, SystemTimeService>();

            services.AddDbContext<CurrencyRateContext>(c =>
            {
                try
                {
                    c.UseSqlServer(Configuration.GetConnectionString("CurrencyRatesConnection"));
                }
                catch (Exception ex)
                {
                    var message = ex.Message;
                }
            });

            ConfigureServices(services);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped(typeof(IAsyncRepository<>), typeof(EfRepository<>));

            services.AddSingleton<ICurrencyRateRepository, CurrencyRateRepository>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStatusCodePages();
            app.UseDeveloperExceptionPage();
            app.UseMvcWithDefaultRoute();
        }
    }
}
