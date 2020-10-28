using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Applications.ExtentionMethods;
using Applications.Handlers.Commands;
using Applications.Interfaces;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistence;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;

namespace Presentation
{
    public class Startup
    {

        public Startup(IHostEnvironment hostingEnvironment)
        {



            var builder = new ConfigurationBuilder()
                .SetBasePath(hostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{hostingEnvironment.EnvironmentName}.json", reloadOnChange: true, optional: true)
                .AddEnvironmentVariables();


            Configuration = builder.Build();

            var elasticUri = Configuration["ElasticConfiguration:Uri"];

            var logFile = $"{Assembly.GetEntryAssembly().FullName}".Replace(".", "_");
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext().Enrich.WithExceptionDetails().Enrich.WithMachineName().WriteTo.File($"../logs/{logFile}", rollingInterval: RollingInterval.Day).WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticUri))
                {

                    AutoRegisterTemplate = true,
                    IndexFormat = "onboardingservice-{0:yyyy.MM.dd}"
                })
            .CreateLogger();
        }


        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCqrs();
            services.AddSwagger();
            services.AddJwtAuthentication(Configuration);


            var conString = Configuration.GetValue<string>("DefaultConnection");
            services.AddDbContext<IAlertDbContext, AppDbContext>(options => options.UseSqlServer(conString));

            //services.AddPersistence(Configuration);

            services.AddControllers()
                 .AddFluentValidation(fv =>
                 {
                     fv.RegisterValidatorsFromAssemblyContaining<CreateUserCommand>();
                 });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("../swagger/v1/swagger.json", "Alert Profiling V1"); });

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
