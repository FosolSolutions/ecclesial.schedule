using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecclesial.Calendar.Helpers.Mail
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMailClient(this IServiceCollection services, Action<MailOptions> options = null)
        {
            if (options != null)
            {
                services.Configure(options);
            }
            services.AddSingleton<MailOptions>();
            services.AddScoped<MailClient>();
            return services;
        }

        public static IServiceCollection AddMailClient(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection("Mail");
            services.Configure<MailOptions>(section);

            //services.Configure<MailOptions>(configuration);
            services.AddScoped<MailClient>();
            return services;
        }
    }
}
