// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        app.UseHostedServicesApplication();
        app.Run();
    }
}