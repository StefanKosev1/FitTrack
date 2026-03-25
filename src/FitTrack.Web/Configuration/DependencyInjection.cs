namespace FitTrack.Web.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddFitTrackDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services;
    }
}
