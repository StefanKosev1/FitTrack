using FitTrack.Core.Interfaces.Repositories;
using FitTrack.Web.Tests.Fakes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FitTrack.Web.Tests;

internal sealed class FitTrackWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IUserRepository>();
            services.RemoveAll<IMembershipRepository>();
            services.RemoveAll<IQRCodeRepository>();

            services.AddSingleton<IUserRepository, InMemoryUserRepository>();
            services.AddSingleton<IMembershipRepository, InMemoryMembershipRepository>();
            services.AddSingleton<IQRCodeRepository, InMemoryQRCodeRepository>();
        });
    }
}
