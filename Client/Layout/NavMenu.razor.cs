using Infrastructure.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Shared.Authorization;
using Infrastructure.Auth;

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
        
        private bool _canViewCards;
        private bool _canViewCardRequests;
        private bool CanViewAdministrationGroup => _canViewUsers || _canViewRoles;

        protected override async Task OnParametersSetAsync()
        {
            var user = (await AuthState).User;
            
            _canViewRoles = await AuthService.HasPermissionAsync(user, AppAction.View, Resource.Roles);
            _canViewUsers = await AuthService.HasPermissionAsync(user, AppAction.View, Resource.Users);
            _canViewCards = await AuthService.HasPermissionAsync(user, AppAction.View, Resource.Card);
            _canViewCardRequests = await AuthService.HasPermissionAsync(user, AppAction.View, Resource.CardRequest);
        }
    }
}
