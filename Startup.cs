using FakeCommerce.Catalog.Core.Repositories;
using FakeCommerce.Catalog.Infrastructure.Data;
using FakeCommerce.Catalog.Infrastructure.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FakeCommerce.Catalog
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddSingleton(typeof(ILogger<>), typeof(FakeCommerceLogger<>));

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                });

            services.AddTransient<ICatalogRepository, CatalogRepository>();
            services.AddApplicationInsightsTelemetry();
            
            services.AddSingleton(provider =>
            {
                var loggerFactory = provider.GetService<ILoggerFactory>();
                return new CatalogDataSource(Configuration.GetConnectionString("Redis"),
                    loggerFactory.CreateLogger<CatalogDataSource>());
            });

            services.AddOpenApiDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "FakeCommerce Catalog API";
                    document.Info.Description = "A Simple catalog api of Fake Ecommerce";
                    document.Info.TermsOfService = "None";
                    document.Info.Contact = new NSwag.OpenApiContact
                    {
                        Name = "Felipe Oliveira",
                        Email = "felipe.ed.oliveira@gmail.com",
                        Url = "https://twitter.com/felipe_thechuck"
                    };
                };
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseOpenApi();
            app.UseReDoc();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
