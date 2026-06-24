using FitTrack.Base.Data;
using FitTrack.Base.Repositories;
using FitTrack.Core.Interfaces.Data;
using FitTrack.Core.Interfaces.Repositories;
using FitTrack.Core.Interfaces.Services;
using FitTrack.Core.Services;

namespace FitTrack.Web.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddFitTrackDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");

        services.AddSingleton<IDbConnectionFactory>(_ => new SqlConnectionFactory(connectionString));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IMembershipRepository, MembershipRepository>();
        services.AddScoped<IQRCodeRepository, QRCodeRepository>();
        services.AddScoped<ILoginService, LoginService>();
        services.AddScoped<IRegistrationService, RegistrationService>();
        services.AddScoped<IMembershipService, MembershipService>();
        services.AddScoped<IQRCodeService, QRCodeService>();

        return services;
    }
}
