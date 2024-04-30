using Client.Shared;
using Infrastructure.ApiClient;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Components;

namespace Client.Pages.Authentication
{
    public partial class Login
    {
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
                return;
            }
            Snackbar.Add($"Invalid credential", Severity.Error);
            return;
        }
    }
}
