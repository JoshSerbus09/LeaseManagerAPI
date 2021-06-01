using LeaseManagerAPI.Data;
using LeaseManagerAPI.Helpers;
using LeaseManagerAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

namespace LeaseManagerAPI
{
    public class Startup
    {
        private readonly IConfiguration _configuration; 

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            // register EF Sqlite --> FIRST <--
            services.AddEntityFrameworkSqlite();

            // register infrastructure
            services.AddCors(options =>
            {
                options.AddPolicy("AllowedOrigins", builder =>
                {
                    builder.WithOrigins("http://localhost:3000");
                });
            });

            // register db storage
            services.Configure<LeaseModelOptions>(_configuration.GetSection("LeaseModelOptions").Bind);
            services.AddSingleton(new LeaseSqliteDbContext());

            // register lease data classes
            services.AddSingleton<LeaseModelValidator>();
            services.AddSingleton<ILeaseDao, LeaseSqliteDao>();

            // register swagger gen & UI
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

            app.UseCors("AllowedOrigins");

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
