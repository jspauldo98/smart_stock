using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Text;
using smart_stock.Services;
using smart_stock.JwtManagement;
using smart_stock.AlpacaServices;
using Hangfire;
using Hangfire.MySql;


namespace smart_stock
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
            // Add Hangfire services.
            services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseStorage(new MySqlStorage (Configuration.GetConnectionString("HangfireConnection"), new MySqlStorageOptions
            {
                TablesPrefix = "SmartStock",
                QueuePollInterval = TimeSpan.FromSeconds(15),
                JobExpirationCheckInterval = TimeSpan.FromHours(1),
                CountersAggregateInterval = TimeSpan.FromMinutes(5),
                DashboardJobListLimit = 50000,
                TransactionTimeout = TimeSpan.FromMinutes(1),
            })));
            
            // Add the processing server as IHostedService
            services.AddHangfireServer();

            services.AddControllersWithViews();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
            var jwtTokenConfig = Configuration.GetSection("jwtTokenConfig").Get<JwtTokenConfig>();
            services.AddSingleton(jwtTokenConfig);
            services.AddAuthentication(options => 
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(bearer =>
            {
                bearer.RequireHttpsMetadata = true;
                bearer.SaveToken = true;
                bearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtTokenConfig.Issuer,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtTokenConfig.Secret)),
                    ValidAudience = jwtTokenConfig.Audience,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)

                };
            });
            services.AddSingleton<IJwtAuthManager, JwtAuthManager>();
            services.AddSingleton<ITrading, Trading>();
            services.AddHostedService<JwtRefreshTokenCache>();
            services.AddTransient<IUserProvider, UserProvider>();
            services.AddTransient<IPreferenceProvider, PreferenceProvider>();
            services.AddTransient<IPortfolioProvider, PortfolioProvider>();
            services.AddTransient<ILogProvider, LogProvider>();
            services.AddTransient<ITradeProvider, TradeProvider>();
            

            services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
            {
                builder.WithOrigins(Configuration.GetSection("BaseUris").GetSection("DevUri").Value).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
            }));
            services.AddSwaggerGen(c => { c.SwaggerDoc("v0.1", new OpenApiInfo {Title = "Smart Stock API", Version= "v0.1"}); });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IBackgroundJobClient backgroundJobs)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //Incredibly helpful for tracking exactly what JSON goes to which endpoint (controller functions)
                // TODO Swagger throwing errors so commenting it out 
                app.UseSwagger();
                app.UseSwaggerUI(config => {
                    config.SwaggerEndpoint("/swagger/v1/swagger.json", "Smart Stock V1");
                });
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseCookiePolicy();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }
            
            app.UseHangfireDashboard();
            backgroundJobs.Enqueue<Trading>(x => x.GetUserData());
            app.UseRouting();

            app.UseCors(options => options.WithOrigins(Configuration.GetSection("BaseUris").GetSection("DevUri").Value)
                .AllowAnyMethod().AllowAnyHeader());

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
