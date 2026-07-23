// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using Apps.Shared;
using cCoder.AppSecurity;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.OData;

namespace AppSecurity.Web;

public static class WebApplicationExtensions
{
    public static void UseAppSecurityWebApplication(
        this WebApplication app)
    {
        ILogger logger =
            app.Services.GetRequiredService<ILogger<Program>>();

        app.UseHttpsRedirection();
        app.UseSession();
        app.UseStaticFiles();

        app.UseSwagger()
            .UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint(
                    url: "/swagger/AppSecurity/swagger.json",
                    name: "AppSecurity API");

                options.SwaggerEndpoint(
                    url: "/swagger/Core/swagger.json",
                    name: "Core API");

                options.SwaggerEndpoint(
                    url: "/swagger/v1/swagger.json",
                    name: "Core API");
            })
            .UseODataBatching()
            .UseODataRouteDebug();

        app.UseDomainApiShell();
        app.MapGet(
            pattern: "/Health",
            handler: () => Results.Text(content: "OK"));

        app.MapGet(
            pattern: "/",
            handler: () => Results.Redirect(url: "/tools/index.html"));

        app.UseDomainDefaultCors();
        app.UseDomainExceptionHandling(
            errorHandler: context => HandleUnhandledException(
                context: context,
                logger: logger));

        app.StartAppSecurityWeb(log: logger);
    }

    private static async Task HandleUnhandledException(
        HttpContext context,
        ILogger logger)
    {
        Exception exception =
            context.Features.Get<IExceptionHandlerPathFeature>()?.Error;

        context.Response.StatusCode =
            exception?.GetType() == typeof(SecurityException)
                ? StatusCodes.Status401Unauthorized
                : StatusCodes.Status500InternalServerError;

        context.Response.ContentType = "application/json";

        if (exception is null)
        {
            return;
        }

        logger.LogError(
            message: "{Message}\n{StackTrace}",
            args:
            [
                exception.Message,
                exception.StackTrace,
            ]);

        await context.Response.WriteAsync(
            text: "{ \"error\": \""
                + exception.Message.Replace(
                    oldValue: "\"",
                    newValue: "\'")
                + "\" }");
    }
}
