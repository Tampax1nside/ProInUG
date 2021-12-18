using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProInUG.BlazorUI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProInUG.BlazorUI.Extentions
{
    public static class ServiceCollectionExtentions
    {
        public static IServiceCollection AddAuthService(this IServiceCollection services, IConfiguration configuration)
        {
            var authApiBaseAddress = configuration.GetSection("AuthApiBaseAddress").Value;
            services.AddHttpClient<IAuthService, AuthService>(client =>
            {
                client.BaseAddress = new Uri(authApiBaseAddress);
            });
            return services;
        }

        public static IServiceCollection AddKktCloudService(this IServiceCollection services, IConfiguration configuration)
        {
            var kktApiBaseAddress = configuration.GetSection("WebApiBaseAddress").Value;
            services.AddHttpClient<IKktCloudService, KktCloudService>(client =>
            {
                client.BaseAddress = new Uri(kktApiBaseAddress);
            });
            return services;
        }
    }
}
