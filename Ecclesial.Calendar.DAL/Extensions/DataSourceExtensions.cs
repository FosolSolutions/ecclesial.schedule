using Fosol.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecclesial.Calendar.DAL.Extensions
{
    public static class DataSourceExtensions
    {
        public static IServiceCollection AddDataSource(this IServiceCollection services, Action<DataSourceOptions> options)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            services.Configure(options);
            return services.AddScoped<DataSource>();
        }
    }
}
