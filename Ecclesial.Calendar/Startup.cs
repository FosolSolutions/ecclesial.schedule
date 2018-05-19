using System;
using System.IO;
using System.Reflection;
using Ecclesial.Calendar.DAL.Extensions;
using Ecclesial.Calendar.Filters;
using Ecclesial.Calendar.Helpers.Mail;
using Fosol.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Ecclesial.Calendar
{
    public class Startup
    {
        #region Properties
        public IConfiguration Configuration { get; }
        #endregion

        #region Constructors
        public Startup(IHostingEnvironment env) //, IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", false, true)
                .AddJsonFile($"appSettings.{env.EnvironmentName}.json", true, true)
                .AddJsonFile("connectionStrings.json", false, true)
                .AddJsonFile($"connectionStrings.{env.EnvironmentName}.json", true, true)
                .AddJsonFile("mailSettings.json", true, true)
                .AddJsonFile($"mailSettings.{env.EnvironmentName}.json", true, true);

            if (env.IsDevelopment())
            {
                var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                if (appAssembly != null)
                {
                    builder.AddUserSecrets(appAssembly, optional: true);
                }
            }

            builder.AddEnvironmentVariables();

            Configuration = builder.Build();
        }
        #endregion

        #region Methods
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddMvc( options =>
            {
                options.Filters.Add(typeof(ViewBagFilterAttribute));
                options.Filters.Add(typeof(GlobalExceptionFilter));
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie( options =>
                {
                    options.LoginPath = "/auth/signin";
                });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/auth/signin";
            });

            var cs = this.Configuration.GetConnectionString("Ecclesial.Calendar");
            services.AddDataSource(options =>
            {
               options.Clear = false;
               options.Create = false;
               options.Initialize = false;
               options.Seed = false;
               options.ConnectionString = cs;
            });

            services.Configure<MailOptions>(options => this.Configuration.GetSection("Mail").Bind(options));
            services.AddScoped<MailClient>(provider => new MailClient(provider.GetService<IOptions<MailOptions>>().Value));
            //services.AddMailClient(options => new MailOptions());
            //services.AddMailClient(this.Configuration);

            // new DAL.DataSource(new DataSourceOptions() { Clear = false, Create = false, Initialize = true, Seed = true, ConnectionString = cs }).Initialize();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStatusCodePagesWithReExecute("/Error/{0}");
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areaRoute",
                    template: "{area:exists}/{controller}/{action}/{id?}",
                    defaults: new { controller = "Schedule", action = "Index" }
                );
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });
            });
            app.UseStaticFiles();
        }
        #endregion
    }
}
