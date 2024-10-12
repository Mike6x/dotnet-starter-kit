﻿using System.Reflection;
using Asp.Versioning.Conventions;
using Carter;
using FluentValidation;
using FSH.Starter.WebApi.Catalog.Application;
using FSH.Starter.WebApi.Catalog.Infrastructure;
using FSH.Starter.WebApi.Elearning;
using FSH.Starter.WebApi.Todo;
using FSH.Starter.WebApi.Setting;

namespace FSH.Starter.WebApi.Host;

public static class Extensions
{
    public static WebApplicationBuilder RegisterModules(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        //define module assemblies
        var assemblies = new Assembly[]
        {
            typeof(CatalogMetadata).Assembly,
            typeof(TodoModule).Assembly,
            
            typeof(SettingModule).Assembly,
            typeof(ElearningModule).Assembly,

        };

        //register validators
        builder.Services.AddValidatorsFromAssemblies(assemblies);

        //register mediatr
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies);
        });

        //register module services
        builder.RegisterCatalogServices();
        builder.RegisterTodoServices();

        builder.RegisterSettingServices();
        
        //add carter endpoint modules
        builder.Services.AddCarter(configurator: config =>
        {
            config.WithModule<CatalogModule.Endpoints>();
            config.WithModule<TodoModule.Endpoints>();
            
            config.WithModule<SettingModule.Endpoints>();
            config.WithModule<ElearningModule.Endpoints>();

        });

        return builder;
    }

    public static WebApplication UseModules(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        //register modules
        app.UseCatalogModule();
        app.UseTodoModule();

        app.UseSettingModule();
        app.UseElearningModule();

        //register api versions
        var versions = app.NewApiVersionSet()
                    .HasApiVersion(1)
                    .HasApiVersion(2)
                    .ReportApiVersions()
                    .Build();

        //map versioned endpoint
        var endpoints = app.MapGroup("api/v{version:apiVersion}").WithApiVersionSet(versions);

        //use carter
        endpoints.MapCarter();

        return app;
    }
}
