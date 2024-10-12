using Carter;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Persistence;
using FSH.Starter.WebApi.Setting.Domain;
using FSH.Starter.WebApi.Setting.Features.v1.Dimensions;
using FSH.Starter.WebApi.Setting.Features.v1.EntityCodes;
using FSH.Starter.WebApi.Setting.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.Starter.WebApi.Setting;
public static class SettingModule
{
    public class Endpoints : CarterModule
    {
        public Endpoints() : base("setting") { }
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            var dimensionGroup = app.MapGroup("Dimensions").WithTags("Dimensions");
            dimensionGroup.MapCreateDimensionEndpoint();
            dimensionGroup.MapGetDimensionEndpoint();
            dimensionGroup.MapGetDimensionsEndpoint();
            dimensionGroup.MapSearchDimensionsEndpoint();
            dimensionGroup.MapUpdateDimensionEndpoint();
            dimensionGroup.MapDeleteDimensionEndpoint();
            dimensionGroup.MapExportDimensionsEndpoint();
            dimensionGroup.MapImportDimensionsEndpoint();
            
            var entityCodeGroup = app.MapGroup("EntityCodes").WithTags("EntityCodes");
            entityCodeGroup.MapCreateEntityCodeEndpoint();
            entityCodeGroup.MapGetEntityCodeEndpoint();
            entityCodeGroup.MapGetEntityCodesEndpoint();
            entityCodeGroup.MapSearchEntityCodeEndpoint();
            entityCodeGroup.MapUpdateEntityCodeEndpoint();
            entityCodeGroup.MapDeleteEntityCodeEndpoint();
            entityCodeGroup.MapExportEntityCodesEndpoint();
            entityCodeGroup.MapImportEntityCodesEndpoint();
        }
    }
    
    public static WebApplicationBuilder RegisterSettingServices(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Services.BindDbContext<SettingDbContext>();
        builder.Services.AddScoped<IDbInitializer, SettingDbInitializer>();
        
        builder.Services.AddKeyedScoped<IRepository<Dimension>, SettingRepository<Dimension>>("setting:dimension");
        builder.Services.AddKeyedScoped<IReadRepository<Dimension>, SettingRepository<Dimension>>("setting:dimension");
        
        builder.Services.AddKeyedScoped<IRepository<EntityCode>, SettingRepository<EntityCode>>("setting:EntityCode");
        builder.Services.AddKeyedScoped<IReadRepository<EntityCode>, SettingRepository<EntityCode>>("setting:EntityCode");
        
        return builder;
    }
    
    public static WebApplication UseSettingModule(this WebApplication app)
    {
        return app;
    }
}
