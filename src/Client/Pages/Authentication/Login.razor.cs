using Client.Components.Common;
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

        private CustomValidation? _customValidation;

        public bool BusySubmitting { get; set; }
        private bool _passwordVisibility;
        private InputType _passwordInput = InputType.Password;
        private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
        public async Task SubmitAsync()
        {
            BusySubmitting = true;
            if (await ApiHelper.ExecuteCallGuardedAsync(
             () => AuthService.LoginAsync(_tokenRequest),
             Snackbar))
            {
                Snackbar.Add($"Logged in as {_tokenRequest.UserName}", Severity.Info);
                BusySubmitting = false;
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
            BusySubmitting = false;
            return;
        }

        private void TogglePasswordVisibility()
        {
            if (_passwordVisibility)
            {
                _passwordVisibility = false;
                _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
                _passwordInput = InputType.Password;
            }
            else
            {
                _passwordVisibility = true;
                _passwordInputIcon = Icons.Material.Filled.Visibility;
                _passwordInput = InputType.Text;
            }
        }
    }
}
