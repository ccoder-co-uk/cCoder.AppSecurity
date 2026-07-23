// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace AppSecurity.Web;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder =
            WebApplication.CreateBuilder(args);

        builder.Services.AddAppSecurityWebApplication(
            configuration: builder.Configuration);

        WebApplication app = builder.Build();
        app.UseAppSecurityWebApplication();
        app.Run();
    }
}