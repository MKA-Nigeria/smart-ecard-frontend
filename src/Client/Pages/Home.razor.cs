using Infrastructure.ApiClient;
using Microsoft.AspNetCore.Components;

namespace Client.Pages
{
    public partial class Home
    {
        [Inject]
        private IAppConfigurationsClient AppConfigurationsClient { get; set; }
        [Inject]
        private IDashboardClient DashboardClient { get; set; }
        private DashboardData data { get; set; }
        private string appClient = null;
        bool BusySubmitting;
        protected override async Task OnInitializedAsync()
        {
            var client = await AppConfigurationsClient.GetAppConfigurationByKeyAsync("AppDomain");
            appClient = client.Value;

            var dashboardDataResponse = await DashboardClient.GetAsync();
             data = dashboardDataResponse;
            _loaded = true;
        }
    }
}
