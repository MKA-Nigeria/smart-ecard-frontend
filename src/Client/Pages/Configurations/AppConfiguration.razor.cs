using Client.Shared;
using Infrastructure.ApiClient;
using Microsoft.AspNetCore.Components;

namespace Client.Pages.Configurations
{
    public partial class AppConfiguration
    {
        [Inject]
        private IAppConfigurationsClient AppConfigurationsClient { get; set; } = default!;
        public PaginationResponseOfAppConfigurationDto AppConfigurations = new();

        private MudTable<AppConfigurationDto> table;
        private int totalItems;
        private string searchString = null;

        protected override async Task OnInitializedAsync() { }

        private async Task Fetch()
        {
            if (await ApiHelper.ExecuteCallGuardedAsync(() => AppConfigurationsClient.GetAppConfigurationsAsync(), Snackbar) is PaginationResponseOfAppConfigurationDto appConfiguration)
            {
                AppConfigurations = appConfiguration;
            }
            else
            {
                AppConfigurations.Data = [];
            }

        }

        private async Task<TableData<AppConfigurationDto>> ServerReload(TableState state)
        {
            await Fetch();

            var data = AppConfigurations;

            switch (state.SortLabel)
            {
                case "key":
                    data.Data = [.. data.Data.OrderByDirection(state.SortDirection, o => o.Key)];
                    break;
                case "value":
                    data.Data = [.. data.Data.OrderByDirection(state.SortDirection, o => o.Value)];
                    break;
            }
            return new TableData<AppConfigurationDto>() { TotalItems = totalItems, Items = AppConfigurations.Data };
        }

        private void OnSearch(string text)
        {
            searchString = text;
            table.ReloadServerData();
        }
    }
}
