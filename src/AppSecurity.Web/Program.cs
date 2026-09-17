// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity;
using cCoder.Eventing;

namespace AppSecurity.Web;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder =
            WebApplication.CreateBuilder(args);

        builder.Services.AddWeb(
            configuration: builder.Configuration);

        WebApplication app = builder.Build();
        app.Services
            .GetRequiredService<IEventHub>()
            .ListenToAppSecurityEvents();
        app.UseAppSecurityWebApplication();
        app.Run();
    }
}