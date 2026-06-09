using System;
using System.Net.Http;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Abp.Demo.Web.HealthChecks;

public static class HealthChecksBuilderExtensions
{
    public static void AddDemoHealthChecks(this IServiceCollection services)
    {
        // Add your health checks here
        var healthChecksBuilder = services.AddHealthChecks();
        healthChecksBuilder.AddCheck<DemoDatabaseCheck>("Demo DbContext Check", tags: new string[] { "database" });

        services.ConfigureHealthCheckEndpoint("/health-status");

        var configuration = services.GetConfiguration();
        var healthCheckPath = configuration["App:HealthCheckUrl"];

        if (string.IsNullOrEmpty(healthCheckPath))
        {
            healthCheckPath = "/health-status";
        }

        var healthUiCheckUrl = configuration["App:HealthUiCheckUrl"];
        if (string.IsNullOrEmpty(healthUiCheckUrl))
        {
            var selfUrl = configuration["App:SelfUrl"]?.TrimEnd('/');
            healthUiCheckUrl = string.IsNullOrEmpty(selfUrl)
                ? healthCheckPath
                : $"{selfUrl}{healthCheckPath.EnsureStartsWith('/')}";
        }

        var hostingEnvironment = services.GetHostingEnvironment();

        var healthChecksUiBuilder = services.AddHealthChecksUI(settings =>
        {
            settings.AddHealthCheckEndpoint("Demo Health Status", healthUiCheckUrl);

            if (hostingEnvironment.IsDevelopment())
            {
                settings.UseApiEndpointHttpMessageHandler(_ => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                });
            }
        });

        // Set your HealthCheck UI Storage here
        healthChecksUiBuilder.AddInMemoryStorage();

        services.MapHealthChecksUiEndpoints(options =>
        {
            options.UIPath = "/health-ui";
            options.ApiPath = "/health-api";
        });
    }

    private static IServiceCollection ConfigureHealthCheckEndpoint(this IServiceCollection services, string path)
    {
        services.Configure<AbpEndpointRouterOptions>(options =>
        {
            options.EndpointConfigureActions.Add(endpointContext =>
            {
                endpointContext.Endpoints.MapHealthChecks(
                    new PathString(path.EnsureStartsWith('/')),
                    new HealthCheckOptions
                    {
                        Predicate = _ => true,
                        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
                        AllowCachingResponses = false,
                    });
            });
        });

        return services;
    }

    private static IServiceCollection MapHealthChecksUiEndpoints(this IServiceCollection services, Action<global::HealthChecks.UI.Configuration.Options>? setupOption = null)
    {
        services.Configure<AbpEndpointRouterOptions>(routerOptions =>
        {
            routerOptions.EndpointConfigureActions.Add(endpointContext =>
            {
                endpointContext.Endpoints.MapHealthChecksUI(setupOption);
            });
        });

        return services;
    }
}
