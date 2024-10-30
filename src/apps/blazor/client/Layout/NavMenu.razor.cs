using FSH.Starter.Blazor.Infrastructure.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Shared.Authorization;

namespace FSH.Starter.Blazor.Client.Layout;

public partial class NavMenu
{
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;
    
    private bool _canViewHangfire;
    private bool _canViewSwagger;
    private bool _canViewAuditTrails;
    
    private bool _canViewRoles;
    private bool _canViewUsers;
    private bool _canViewTenants;
    private bool CanViewUserGroup => _canViewUsers || _canViewRoles || _canViewTenants;

    private bool _canViewDimensions;
    private bool _canViewEntityCodes;
    private bool CanViewSettingGroup => _canViewDimensions || _canViewEntityCodes;
    
    private bool _canViewQuizs;
    private bool _canViewQuizResults;
    private bool CanViewElearningGroup => _canViewQuizs || _canViewQuizResults;
    
    private bool _canViewProducts;
    private bool _canViewTodos;
    private bool CanViewDemoGroup => _canViewTodos || _canViewProducts;
    
   
    protected override async Task OnParametersSetAsync()
    {
        var user = (await AuthState).User;
        _canViewHangfire = await AuthService.HasPermissionAsync(user, FshActions.View, FshResources.Hangfire);
        _canViewAuditTrails = await AuthService.HasPermissionAsync(user, FshActions.View, FshResources.AuditTrails);
        
        _canViewTenants = await AuthService.HasPermissionAsync(user, FshActions.View, FshResources.Tenants);
        _canViewRoles = await AuthService.HasPermissionAsync(user, FshActions.View, FshResources.Roles);
        _canViewUsers = await AuthService.HasPermissionAsync(user, FshActions.View, FshResources.Users);
        
        _canViewDimensions = await AuthService.HasPermissionAsync(user, FshActions.View, FshResources.Dimensions);
        _canViewEntityCodes = await AuthService.HasPermissionAsync(user, FshActions.View, FshResources.EntityCodes);
        
        _canViewQuizs = await AuthService.HasPermissionAsync(user, FshActions.View, FshResources.Quizs);
        _canViewQuizResults = await AuthService.HasPermissionAsync(user, FshActions.View, FshResources.QuizResults);
        
        
        
        _canViewProducts = await AuthService.HasPermissionAsync(user, FshActions.View, FshResources.Products);
        _canViewTodos = await AuthService.HasPermissionAsync(user, FshActions.View, FshResources.Todos);
    }
}
