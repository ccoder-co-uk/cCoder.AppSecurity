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

        builder.Services.AddHostedServicesApplication(
            configuration: builder.Configuration,
            loggingBuilder: builder.Logging);

        WebApplication app = builder.Build();
        app.UseHostedServicesApplication();
        app.Run();
    }
}