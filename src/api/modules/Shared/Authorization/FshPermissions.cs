﻿using System.Collections.ObjectModel;

namespace FSH.Starter.WebApi.Shared.Authorization;

public static class FshAction
{
    public const string View = nameof(View);
    public const string Search = nameof(Search);
    public const string Create = nameof(Create);
    public const string Update = nameof(Update);
    public const string Delete = nameof(Delete);
    public const string Export = nameof(Export);
    public const string Import = nameof(Import);
    public const string Unlock = nameof(Unlock);
    public const string Generate = nameof(Generate);
    public const string Clean = nameof(Clean);
    public const string UpgradeSubscription = nameof(UpgradeSubscription);
}

public static class FshResource
{
    public const string Tenants = nameof(Tenants);
    public const string Dashboard = nameof(Dashboard);
    public const string Hangfire = nameof(Hangfire);
    public const string Users = nameof(Users);
    public const string UserRoles = nameof(UserRoles);
    public const string Roles = nameof(Roles);
    public const string RoleClaims = nameof(RoleClaims);
    public const string Products = nameof(Products);
    public const string Todos = nameof(Todos);
    public const string AuditTrails = nameof(AuditTrails);
    
    public const string Menus = nameof(Menus);
    public const string Dimensions = nameof(Dimensions);
    public const string EntityCodes = nameof(EntityCodes);
    
    public const string Quizs = nameof(Quizs);
    public const string QuizResults = nameof(QuizResults);
}

public static class FshPermissions
{
    private static readonly FshPermission[] allPermissions =
   { 
        //tenants
        new("View Tenants", FshAction.View, FshResource.Tenants, IsRoot: true),
        new("Search Tenants", FshAction.Search, FshResource.Tenants, IsRoot: true),
        new("Create Tenants", FshAction.Create, FshResource.Tenants, IsRoot: true),
        new("Update Tenants", FshAction.Update, FshResource.Tenants, IsRoot: true),
        new("Delete Tenants", FshAction.Delete, FshResource.Tenants, IsRoot: true),
        new("Export Tenants", FshAction.Export, FshResource.Tenants, IsRoot: true),
        new("Import Tenants", FshAction.Import, FshResource.Tenants, IsRoot: true),
        
        new("Upgrade Tenant Subscription", FshAction.UpgradeSubscription, FshResource.Tenants, IsRoot: true),

        //identity
        new("View Users", FshAction.View, FshResource.Users),
        new("Search Users", FshAction.Search, FshResource.Users),
        new("Create Users", FshAction.Create, FshResource.Users),
        new("Update Users", FshAction.Update, FshResource.Users),
        new("Delete Users", FshAction.Delete, FshResource.Users),
        new("Export Users", FshAction.Export, FshResource.Users),
        new("Import Users", FshAction.Import, FshResource.Users),
        
        new("View UserRoles", FshAction.View, FshResource.UserRoles),
        new("Update UserRoles", FshAction.Update, FshResource.UserRoles),
        
        new("View Roles", FshAction.View, FshResource.Roles),
        new("Search Roles", FshAction.Search, FshResource.Roles),
        new("Create Roles", FshAction.Create, FshResource.Roles),
        new("Update Roles", FshAction.Update, FshResource.Roles),
        new("Delete Roles", FshAction.Delete, FshResource.Roles),
        new("Export Roles", FshAction.Export, FshResource.Roles),
        new("Import Roles", FshAction.Import, FshResource.Roles),
        
        new("View RoleClaims", FshAction.View, FshResource.RoleClaims),
        new("Update RoleClaims", FshAction.Update, FshResource.RoleClaims),
        
        //products
        new("View Products", FshAction.View, FshResource.Products, IsBasic: true),
        new("Search Products", FshAction.Search, FshResource.Products, IsBasic: true),
        new("Create Products", FshAction.Create, FshResource.Products),
        new("Update Products", FshAction.Update, FshResource.Products),
        new("Delete Products", FshAction.Delete, FshResource.Products),
        new("Export Products", FshAction.Export, FshResource.Products),
        new("Import Products", FshAction.Import, FshResource.Products),
        
        //todos
        new("View Todos", FshAction.View, FshResource.Todos, IsBasic: true),
        new("Search Todos", FshAction.Search, FshResource.Todos, IsBasic: true),
        new("Create Todos", FshAction.Create, FshResource.Todos),
        new("Update Todos", FshAction.Update, FshResource.Todos),
        new("Delete Todos", FshAction.Delete, FshResource.Todos),
        new("Export Todos", FshAction.Export, FshResource.Todos),
        new("Import Todos", FshAction.Import, FshResource.Todos),
        
        //audit
        new("View Audit Trails", FshAction.View, FshResource.AuditTrails),
        
        #region BackgroundJobs
        new("View Hangfire", FshAction.View, FshResource.Hangfire),

        #endregion
        
        #region Settings
        new("View Menus", FshAction.View, FshResource.Menus, IsBasic: true),
        new("Search Menus", FshAction.Search, FshResource.Menus, IsBasic: true),
        new("Create Menus", FshAction.Create, FshResource.Menus),
        new("Update Menus", FshAction.Update, FshResource.Menus),
        new("Delete Menus", FshAction.Delete, FshResource.Menus),
        new("Export Menus", FshAction.Export, FshResource.Menus),
        new("Import Menus", FshAction.Import, FshResource.Menus),

        new("View Dimensions", FshAction.View, FshResource.Dimensions, IsBasic: true),
        new("Search Dimensions", FshAction.Search, FshResource.Dimensions, IsBasic: true),
        new("Create Dimensions", FshAction.Create, FshResource.Dimensions),
        new("Update Dimensions", FshAction.Update, FshResource.Dimensions),
        new("Delete Dimensions", FshAction.Delete, FshResource.Dimensions),
        new("Export Dimensions", FshAction.Export, FshResource.Dimensions),
        new("Import Dimensions", FshAction.Import, FshResource.Dimensions),

        new("View EntityCodes", FshAction.View, FshResource.EntityCodes, IsBasic: true),
        new("Search EntityCodes", FshAction.Search, FshResource.EntityCodes, IsBasic: true),
        new("Create EntityCodes", FshAction.Create, FshResource.EntityCodes),
        new("Update EntityCodes", FshAction.Update, FshResource.EntityCodes),
        new("Delete EntityCodes", FshAction.Delete, FshResource.EntityCodes),
        new("Export EntityCodes", FshAction.Export, FshResource.EntityCodes),
        new("Import EntityCodes", FshAction.Import, FshResource.EntityCodes),

        #endregion
        
        #region Elearning
        new("View Quizs", FshAction.View, FshResource.Quizs, IsBasic: true),
        new("Search Quizs", FshAction.Search, FshResource.Quizs, IsBasic: true),
        new("Create Quizs", FshAction.Create, FshResource.Quizs),
        new("Update Quizs", FshAction.Update, FshResource.Quizs),
        new("Delete Quizs", FshAction.Delete, FshResource.Quizs),
        new("Export Quizs", FshAction.Export, FshResource.Quizs),
        new("Import Quizs", FshAction.Import, FshResource.Quizs),

        new("View QuizResults", FshAction.View, FshResource.QuizResults, IsBasic: true),
        new("Search QuizResults", FshAction.Search, FshResource.QuizResults, IsBasic: true),
        new("Create QuizResults", FshAction.Create, FshResource.QuizResults),
        new("Update QuizResults", FshAction.Update, FshResource.QuizResults),
        new("Delete QuizResults", FshAction.Delete, FshResource.QuizResults),
        new("Export QuizResults", FshAction.Export, FshResource.QuizResults),
        new("Import QuizResults", FshAction.Import, FshResource.QuizResults),

        #endregion
   };

    public static IReadOnlyList<FshPermission> All { get; } = new ReadOnlyCollection<FshPermission>(allPermissions);
    public static IReadOnlyList<FshPermission> Root { get; } = new ReadOnlyCollection<FshPermission>(allPermissions.Where(p => p.IsRoot).ToArray());
    public static IReadOnlyList<FshPermission> Admin { get; } = new ReadOnlyCollection<FshPermission>(allPermissions.Where(p => !p.IsRoot).ToArray());
    public static IReadOnlyList<FshPermission> Basic { get; } = new ReadOnlyCollection<FshPermission>(allPermissions.Where(p => p.IsBasic).ToArray());
}

public record FshPermission(string Description, string Action, string Resource, bool IsBasic = false, bool IsRoot = false)
{
    public string Name => NameFor(Action, Resource);
    public static string NameFor(string action, string resource)
    {
        return $"Permissions.{resource}.{action}";
    }
}
