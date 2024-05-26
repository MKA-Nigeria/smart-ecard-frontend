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

        int CalculateLines(string text)
        {
            // Calculate the number of lines based on the length of the text
            int textLength = text.Length;
            int defaultLineCount = 1; 
            int averageCharsPerLine = 30; 
            int calculatedLines = textLength / averageCharsPerLine + 1; 
            return Math.Max(defaultLineCount, calculatedLines); 
        }

    }
}
