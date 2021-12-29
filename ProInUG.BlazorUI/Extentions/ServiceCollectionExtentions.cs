using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProInUG.BlazorUI.Services;
using System;

namespace ProInUG.BlazorUI.Extentions
{
    public static class ServiceCollectionExtentions
    {
        public static IServiceCollection AddAuthService(this IServiceCollection services, IConfiguration configuration)
        {
            var authApiBaseAddress = configuration["AuthApiBaseAddress"];
            services.AddHttpClient<IAuthService, AuthService>(client =>
            {
                client.BaseAddress = new Uri(authApiBaseAddress);
            });
            return services;
        }

        public static IServiceCollection AddKktCloudService(this IServiceCollection services, IConfiguration configuration)
        {
            var kktApiBaseAddress = configuration["WebApiBaseAddress"];
            services.AddHttpClient<IKktCloudService, KktCloudService>(client =>
            {
                client.BaseAddress = new Uri(kktApiBaseAddress);
            });
            return services;
        }

        public static IServiceCollection AddUsersService(this IServiceCollection services, IConfiguration configuration)
        {
            var usersApiBaseAddress = configuration["AuthApiBaseAddress"];
            services.AddHttpClient<IUsersService, UsersService>(client =>
            {
                client.BaseAddress = new Uri((usersApiBaseAddress));
            });
            return services;
        }
    }
}
