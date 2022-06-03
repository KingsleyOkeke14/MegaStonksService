using AutoMapper;
using CorePush.Apple;
using MegaStonksService.Helpers;
using MegaStonksService.Hubs;
using MegaStonksService.Middleware;
using MegaStonksService.Services;
using MegaStonksService.Services.CoinMarketCapApi;
using MegaStonksService.Services.FinancialModellingPrepApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace MegaStonksService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();
            this.Configuration = builder.Build();
            //Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>();
            services.AddCors();
            services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore);
            services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddSwaggerGen();
            services.AddSignalR();

            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.Configure<FMPApiSettings>(Configuration.GetSection("FMPApi"));
            services.Configure<CMCApiSettings>(Configuration.GetSection("CMCApi"));
            services.Configure<MyApnSettings>(Configuration.GetSection("ApnSettings"));
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IFMPApiService, FMPApiService>();
            services.AddScoped<IStocksService, StocksService>();
            services.AddScoped<IWatchListService, WatchListService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<IAdService, AdService>();
            services.AddScoped<ICMCApiService, CMCApiService>();
            services.AddScoped<ICryptosService, CryptosService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IApplePushNotificationService, ApplePushNotificationService>();
            services.AddHttpClient<ApnSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DataContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            context.Database.Migrate();
            //app.UseSwagger();
            //app.UseSwaggerUI(x => x.SwaggerEndpoint("/swagger/v1/swagger.json", "ASP.NET Core Sign-up and Verification API"));
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(x => x
               .SetIsOriginAllowed(origin => true)
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials());
            app.UseAuthorization();

            app.UseMiddleware<ErrorHandlerMiddleware>();
            app.UseMiddleware<JwtMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/chatHub");
            });
        }
    }
}
