using LeaseManagerAPI.Data;
using LeaseManagerAPI.Helpers;
using LeaseManagerAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaseManagerAPI
{
    public class Startup
    {
        private readonly IConfiguration _configuration; 

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {


            services.Configure<LeaseModelOptions>(_configuration.GetSection("LeaseModelOptions").Bind);

            var dbConnectionString = _configuration.GetConnectionString("LeaseSqliteDB");

            services.AddSingleton<ILeaseDao, LeaseSqliteDao>();

            services.AddTransient<LeaseSqliteDbContext>();

            services.AddSingleton<LeaseModelValidator>();

            services.AddControllers().AddNewtonsoftJson();

            services.AddSwaggerGen(configuration =>
            {
                configuration.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Lease Manager API",
                    Version = "v1",
                    Description = "Lease Management Web API for Create, Read, Update, and Delete operations."
                });
            });

            services.AddSwaggerGenNewtonsoftSupport();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            
            app.UseRouting();

            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("./v1/swagger.json", $"Lease Manager API");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
