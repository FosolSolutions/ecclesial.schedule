using System;
using Ecclesial.Calendar.DAL.Extensions;
using Ecclesial.Calendar.Filters;
using Fosol.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Ecclesial.Calendar
{
    public class Startup
    {
        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
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

            //new DAL.DataSource(new DataSourceOptions() { Clear = false, Create = false, Initialize = true, Seed = true, ConnectionString = cs }).Initialize();
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
    }
}
