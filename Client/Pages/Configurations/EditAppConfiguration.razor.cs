using Client.Pages.Cards.CardRequests;
using Client.Shared;
using Infrastructure.ApiClient;
using Microsoft.AspNetCore.Components;

namespace Client.Pages.Configurations
{
    public partial class EditAppConfiguration
    {
        [Inject]
        private IAppConfigurationsClient AppConfigurationsClient { get; set; } = default!;
        [Parameter]
        public string Key { get; set; }

        AppConfigurationDto AppConfiguration { get; set; } = new();
        bool BusySubmitting;
        bool _loading = true;

        protected override async Task OnInitializedAsync()
        {
            if (await ApiHelper.ExecuteCallGuardedAsync(() => AppConfigurationsClient.GetAppConfigurationByKeyAsync(Key), Snackbar) is AppConfigurationDto appConfiguration)
            {
                AppConfiguration = appConfiguration;
            }
            _loading = false; ;
        }

        private async Task Update()
        {

            BusySubmitting = true;
            if (await ApiHelper.ExecuteCallGuardedAsync(
                () => AppConfigurationsClient.UpdateAppConfigurationsAsync(AppConfiguration),
            Snackbar) is Guid id)
            {
                Navigation.NavigateTo($"/app-configurations");
            }

            BusySubmitting = false;
        }
    }
}
