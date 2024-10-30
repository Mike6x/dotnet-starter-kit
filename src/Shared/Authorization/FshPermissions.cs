using System.Collections.ObjectModel;

namespace Shared.Authorization;

public static class FshPermissions
{
    private static readonly FshPermission[] allPermissions =
   {     
        //tenants
        new("View Tenants", FshActions.View, FshResources.Tenants, IsRoot: true),
        new("Search Tenants", FshActions.Search, FshResources.Tenants, IsRoot: true),
        new("Create Tenants", FshActions.Create, FshResources.Tenants, IsRoot: true),
        new("Update Tenants", FshActions.Update, FshResources.Tenants, IsRoot: true),
        new("Delete Tenants", FshActions.Delete, FshResources.Tenants, IsRoot: true),
        new("Export Tenants", FshActions.Export, FshResources.Tenants, IsRoot: true),
        new("Import Tenants", FshActions.Import, FshResources.Tenants, IsRoot: true),
        
        new("Upgrade Tenant Subscription", FshActions.UpgradeSubscription, FshResources.Tenants, IsRoot: true),

        //identity
        new("View Users", FshActions.View, FshResources.Users),
        new("Search Users", FshActions.Search, FshResources.Users),
        new("Create Users", FshActions.Create, FshResources.Users),
        new("Update Users", FshActions.Update, FshResources.Users),
        new("Delete Users", FshActions.Delete, FshResources.Users),
        new("Export Users", FshActions.Export, FshResources.Users),
        new("Import Users", FshActions.Import, FshResources.Users),
        
        new("View UserRoles", FshActions.View, FshResources.UserRoles),
        new("Update UserRoles", FshActions.Update, FshResources.UserRoles),
        
        new("View Roles", FshActions.View, FshResources.Roles),
        new("Search Roles", FshActions.Search, FshResources.Roles),
        new("Create Roles", FshActions.Create, FshResources.Roles),
        new("Update Roles", FshActions.Update, FshResources.Roles),
        new("Delete Roles", FshActions.Delete, FshResources.Roles),
        new("Export Roles", FshActions.Export, FshResources.Roles),
        new("Import Roles", FshActions.Import, FshResources.Roles),
        
        new("View RoleClaims", FshActions.View, FshResources.RoleClaims),
        new("Update RoleClaims", FshActions.Update, FshResources.RoleClaims),
        
        //products
        new("View Products", FshActions.View, FshResources.Products, IsBasic: true),
        new("Search Products", FshActions.Search, FshResources.Products, IsBasic: true),
        new("Create Products", FshActions.Create, FshResources.Products),
        new("Update Products", FshActions.Update, FshResources.Products),
        new("Delete Products", FshActions.Delete, FshResources.Products),
        new("Export Products", FshActions.Export, FshResources.Products),
        new("Import Products", FshActions.Import, FshResources.Products),
        
        //todos
        new("View Todos", FshActions.View, FshResources.Todos, IsBasic: true),
        new("Search Todos", FshActions.Search, FshResources.Todos, IsBasic: true),
        new("Create Todos", FshActions.Create, FshResources.Todos),
        new("Update Todos", FshActions.Update, FshResources.Todos),
        new("Delete Todos", FshActions.Delete, FshResources.Todos),
        new("Export Todos", FshActions.Export, FshResources.Todos),
        new("Import Todos", FshActions.Import, FshResources.Todos),
        
        new("View Hangfire", FshActions.View, FshResources.Hangfire),
        new("View Dashboard", FshActions.View, FshResources.Dashboard),

        //audit
        new("View Audit Trails", FshActions.View, FshResources.AuditTrails),
        
        #region Settings
        new("View Menus", FshActions.View, FshResources.Menus, IsBasic: true),
        new("Search Menus", FshActions.Search, FshResources.Menus, IsBasic: true),
        new("Create Menus", FshActions.Create, FshResources.Menus),
        new("Update Menus", FshActions.Update, FshResources.Menus),
        new("Delete Menus", FshActions.Delete, FshResources.Menus),
        new("Export Menus", FshActions.Export, FshResources.Menus),
        new("Import Menus", FshActions.Import, FshResources.Menus),

        new("View Dimensions", FshActions.View, FshResources.Dimensions, IsBasic: true),
        new("Search Dimensions", FshActions.Search, FshResources.Dimensions, IsBasic: true),
        new("Create Dimensions", FshActions.Create, FshResources.Dimensions),
        new("Update Dimensions", FshActions.Update, FshResources.Dimensions),
        new("Delete Dimensions", FshActions.Delete, FshResources.Dimensions),
        new("Export Dimensions", FshActions.Export, FshResources.Dimensions),
        new("Import Dimensions", FshActions.Import, FshResources.Dimensions),

        new("View EntityCodes", FshActions.View, FshResources.EntityCodes, IsBasic: true),
        new("Search EntityCodes", FshActions.Search, FshResources.EntityCodes, IsBasic: true),
        new("Create EntityCodes", FshActions.Create, FshResources.EntityCodes),
        new("Update EntityCodes", FshActions.Update, FshResources.EntityCodes),
        new("Delete EntityCodes", FshActions.Delete, FshResources.EntityCodes),
        new("Export EntityCodes", FshActions.Export, FshResources.EntityCodes),
        new("Import EntityCodes", FshActions.Import, FshResources.EntityCodes),

        #endregion
        
        #region Elearning
        new("View Quizs", FshActions.View, FshResources.Quizs, IsBasic: true),
        new("Search Quizs", FshActions.Search, FshResources.Quizs, IsBasic: true),
        new("Create Quizs", FshActions.Create, FshResources.Quizs),
        new("Update Quizs", FshActions.Update, FshResources.Quizs),
        new("Delete Quizs", FshActions.Delete, FshResources.Quizs),
        new("Export Quizs", FshActions.Export, FshResources.Quizs),
        new("Import Quizs", FshActions.Import, FshResources.Quizs),

        new("View QuizResults", FshActions.View, FshResources.QuizResults, IsBasic: true),
        new("Search QuizResults", FshActions.Search, FshResources.QuizResults, IsBasic: true),
        new("Create QuizResults", FshActions.Create, FshResources.QuizResults),
        new("Update QuizResults", FshActions.Update, FshResources.QuizResults),
        new("Delete QuizResults", FshActions.Delete, FshResources.QuizResults),
        new("Export QuizResults", FshActions.Export, FshResources.QuizResults),
        new("Import QuizResults", FshActions.Import, FshResources.QuizResults),

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


