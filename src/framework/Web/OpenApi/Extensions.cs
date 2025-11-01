using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

namespace FSH.Framework.Web.OpenApi;
public static class Extensions
{
    private const string SectionName = "OpenApi";
    public static IServiceCollection EnableApiDocs(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        // Bind options from appsettings
        services.Configure<OpenApiOptions>(configuration.GetSection(SectionName));

        // Minimal OpenAPI generator (ASP.NET Core 8)
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer(async (document, context, ct) =>
            {
                var provider = context.ApplicationServices;
                var openApi = provider.GetRequiredService<IOptions<OpenApiOptions>>().Value;

                // Title/metadata
                document.Info = new OpenApiInfo
                {
                    Title = openApi.Title,
                    Version = openApi.Version,
                    Description = openApi.Description,
                    Contact = openApi.Contact is null ? null : new OpenApiContact
                    {
                        Name = openApi.Contact.Name,
                        Url = openApi.Contact.Url,
                        Email = openApi.Contact.Email
                    },
                    License = openApi.License is null ? null : new OpenApiLicense
                    {
                        Name = openApi.License.Name,
                        Url = openApi.License.Url
                    }
                };

                // JWT Bearer security (for auth’d endpoints in Scalar)
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Input: Bearer {token}",
                    In = ParameterLocation.Header,
                    Name = "Authorization"
                };

                document.SecurityRequirements.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    }] = Array.Empty<string>()
                });

                await Task.CompletedTask;
            });
        });

        return services;
    }

    public static void ExposeApiDocs(
        this WebApplication app,
        string openApiPath = "/openapi/{documentName}.json")
    {
        ArgumentNullException.ThrowIfNull(app);

        app.MapOpenApi(openApiPath);

        app.MapScalarApiReference(options =>
        {
            var configuration = app.Configuration;
            options
                .WithTitle(configuration["OpenApi:Title"] ?? "FSH API")
                .WithTheme(Scalar.AspNetCore.ScalarTheme.Default)
                .EnableDarkMode()
                .WithOpenApiRoutePattern(openApiPath)
                .AddPreferredSecuritySchemes("Bearer");
        });
    }
}