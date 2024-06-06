using Infrastructure.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Shared.Authorization;
using Infrastructure.Auth;
using System.Security.Claims;
using Infrastructure.ApiClient;

namespace Client.Layout
{
    public partial class NavMenu
    {
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; } = default!;
        [Inject]
        protected IAuthorizationService AuthService { get; set; } = default!;

        private bool _canViewRoles;
        private bool _canViewUsers;
        
        private bool _canSearchCards;
        private bool _canViewMyCard;
        private bool _canSearchCardRequests;
        string userName;
        private bool CanViewAdministrationGroup => _canViewUsers || _canViewRoles;

        protected override async Task OnParametersSetAsync()
        {
            var user = (await AuthState).User;
            
            _canViewRoles = await AuthService.HasPermissionAsync(user, AppAction.View, Resource.Roles);
            _canViewUsers = await AuthService.HasPermissionAsync(user, AppAction.View, Resource.Users);
            _canSearchCards = await AuthService.HasPermissionAsync(user, AppAction.Search, Resource.Card);
            _canSearchCardRequests = await AuthService.HasPermissionAsync(user, AppAction.Search, Resource.CardRequest);
            _canViewMyCard = user.GetRoles().Contains(AppRoles.Basic);
            userName = user.GetUserName();
        }
    }
}
