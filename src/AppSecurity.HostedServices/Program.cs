// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity;
using cCoder.Eventing;

namespace AppSecurity.HostedServices;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder =
            WebApplication.CreateBuilder(args);

        builder.Logging.ClearProviders();
        builder.Logging.AddSimpleConsole();

        builder.Services.AddHostedServices(
            configuration: builder.Configuration);

        WebApplication app = builder.Build();
        app.Services
            .GetRequiredService<IEventHub>()
            .ListenToAppSecurityEvents();
        app.UseHostedServicesApplication();
        app.Run();
    }
}