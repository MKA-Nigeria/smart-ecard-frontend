using Client.Pages.Identity.Users;
using Infrastructure.ApiClient;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Shared.Authorization;
using System.Security.Claims;

namespace Client.Pages
{
    public partial class Home
    {
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; } = default!;
        [Inject]
        private IAppConfigurationsClient AppConfigurationsClient { get; set; }
        [Inject]
        private IDashboardClient DashboardClient { get; set; }
        private DashboardData data { get; set; }
        private string appClient = null;
        private bool _canViewMyCard;
        bool BusySubmitting;
        protected override async Task OnInitializedAsync()
        {
            var user = (await AuthState).User;
            var client = await AppConfigurationsClient.GetAppConfigurationByKeyAsync("AppDomain");
            appClient = client.Value;
            _canViewMyCard = user.GetRoles().Contains(AppRoles.Basic);
            if(_canViewMyCard)
            {
                string userName = user.GetUserName();
                Navigation.NavigateTo($"/{userName}");
                return;
            }
            var dashboardDataResponse = await DashboardClient.GetAsync();
            data = dashboardDataResponse;

            _loaded = true;
        }
    }
}
