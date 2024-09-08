﻿using FSH.Starter.Blazor.Infrastructure.Auth;
using FSH.Starter.Blazor.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FSH.Starter.Blazor.Client.Layout;

public partial class NavMenu
{
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;
    
    private bool _canViewDashboard;
    private bool _canViewProducts;
    private bool _canViewTodos;

    private bool CanViewAdministrationGroup => _canViewUsers || _canViewRoles || _canViewTenants;
    
    private bool _canViewHangfire;
    private bool _canViewSwagger;
    private bool _canViewAuditTrails;

    private bool _canViewMenus;
    private bool _canViewDimensions;
    private bool _canViewBackgroundJobs;
    private bool _canViewEntityCodes;
    private bool CanViewSystemGroup => _canViewHangfire || _canViewSwagger || _canViewAuditTrails || _canViewMenus || _canViewDimensions || _canViewBackgroundJobs || _canViewEntityCodes;

    private bool _canViewChatMessages;

    private bool _canViewRoles;
    private bool _canViewUsers;
    private bool _canViewTenants;
    private bool CanViewUserGroup => _canViewUsers || _canViewRoles || _canViewTenants || _canViewMenus || _canViewChatMessages;
    
    private bool _canViewUserStats;
    
    protected override async Task OnParametersSetAsync()
    {
        var user = (await AuthState).User;
        _canViewHangfire = await AuthService.HasPermissionAsync(user, FshAction.View, FshResource.Hangfire);
        _canViewDashboard = await AuthService.HasPermissionAsync(user, FshAction.View, FshResource.Dashboard);

        _canViewProducts = await AuthService.HasPermissionAsync(user, FshAction.View, FshResource.Products);
        _canViewTodos = await AuthService.HasPermissionAsync(user, FshAction.View, FshResource.Todos);
        
        _canViewTenants = await AuthService.HasPermissionAsync(user, FshAction.View, FshResource.Tenants);
        _canViewRoles = await AuthService.HasPermissionAsync(user, FshAction.View, FshResource.Roles);
        _canViewUsers = await AuthService.HasPermissionAsync(user, FshAction.View, FshResource.Users);
        
        // _canViewChatMessages = await AuthService.HasPermissionAsync(user, FshAction.View, FshResource.ChatMessages);
        _canViewAuditTrails = await AuthService.HasPermissionAsync(user, FshAction.View, FshResource.AuditTrails);
        _canViewMenus = await AuthService.HasPermissionAsync(user, FshAction.View, FshResource.Menus);
        _canViewDimensions = await AuthService.HasPermissionAsync(user, FshAction.View, FshResource.Dimensions);
        _canViewEntityCodes = await AuthService.HasPermissionAsync(user, FshAction.View, FshResource.EntityCodes);
        
    }
}
