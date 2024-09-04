﻿using Carter;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Persistence;
using FSH.Starter.WebApi.Setting.Dimension.Features.v1;
using FSH.Starter.WebApi.Setting.Dimension.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.Starter.WebApi.Setting.Dimension;
public static class DimensionModule
{

    public class Endpoints : CarterModule
    {
        public Endpoints() : base("setting") { }
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            var settingGroup = app.MapGroup("dimensions").WithTags("dimensions");
            settingGroup.MapDimensionCreationEndpoint();
            settingGroup.MapGetDimensionEndpoint();
            settingGroup.MapGetDimensionListEndpoint();
            settingGroup.MapDimensionUpdationEndpoint();
            settingGroup.MapDimensionDeletionEndpoint();
        }
    }
    public static WebApplicationBuilder RegisterDimensionServices(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Services.BindDbContext<DimensionDbContext>();
        builder.Services.AddScoped<IDbInitializer, DimensionDbInitializer>();
        builder.Services.AddKeyedScoped<IRepository<Dimension.Domain.Dimension>, DimensionRepository<Dimension.Domain.Dimension>>("setting:dimension");
        builder.Services.AddKeyedScoped<IReadRepository<Dimension.Domain.Dimension>, DimensionRepository<Dimension.Domain.Dimension>>("setting:dimension");
        return builder;
    }
    public static WebApplication UseDimensionModule(this WebApplication app)
    {
        return app;
    }
}
