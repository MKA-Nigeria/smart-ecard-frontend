using Client.Shared;
using Infrastructure.ApiClient;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Shared.Authorization;
using System.Security.Claims;

namespace Client.Pages.Authentication
{
    public partial class Login
    {
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; } = default!;

        private readonly TokenRequest _tokenRequest = new();
        [Inject]
        protected ITokensClient TokenClient { get; set; } = default!;
        [Inject]
        public IAuthenticationService AuthService { get; set; } = default!;
        public async Task Submit()
        {
            if (await ApiHelper.ExecuteCallGuardedAsync(
             () => AuthService.LoginAsync(_tokenRequest),
             Snackbar))
            {
                Snackbar.Add($"Logged in as {_tokenRequest.UserName}", Severity.Info);
                var user = (await AuthState).User;
                if (user.GetRoles().Contains(AppRoles.Basic))
                {
                    Navigation.NavigateTo($"/{user.GetUserName()}");
                    return;
                }
                Navigation.NavigateTo($"/");
                return;
            }
            Snackbar.Add($"Invalid credential", Severity.Error);
            return;
        }
    }
}
