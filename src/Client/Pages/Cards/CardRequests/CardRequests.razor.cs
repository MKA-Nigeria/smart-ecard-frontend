using Client.Shared;
using Infrastructure.ApiClient;
using Microsoft.AspNetCore.Components;

namespace Client.Pages.Cards.CardRequests
{
    public partial class CardRequests
    {
        public bool _loaded;
        [Inject]
        private ICardRequestsClient CardRequestsClient { get; set; }
        [Inject]
        private IAppConfigurationsClient AppConfigurationsClient { get; set; }
        public PaginationResponseOfCardRequestDto CardRequestsResponse = new();

        private MudTable<CardRequestDto> table;
        private int totalItems;
        private string searchString = null;
        private string appClient = null;
        private string keysString;
        bool BusySubmitting;
        private List<string> keysToDisplay;

        protected override async Task OnInitializedAsync()
        {
            var client = await AppConfigurationsClient.GetAppConfigurationByKeyAsync("AppDomain");
            var displayKeys = await AppConfigurationsClient.GetAppConfigurationByKeyAsync("DisplayKeys");
            appClient = client.Value;
            keysString = displayKeys.Value;
            keysToDisplay = keysString?.Split(',').ToList();
            _loaded = true;
        }


        private async Task Fetch(SearchCardRequest SearchCardRequest)
        {
            if (await ApiHelper.ExecuteCallGuardedAsync(() => CardRequestsClient.SearchAsync(SearchCardRequest), Snackbar) is PaginationResponseOfCardRequestDto cardRequests)
            {
                CardRequestsResponse = cardRequests;
                
            }
            else
            {
                CardRequestsResponse.Data = [];
            }

        }
        private async Task<TableData<CardRequestDto>> ServerReload(TableState state)
        {
            var searchRequest = new SearchCardRequest
            {
                Keyword = searchString,
                PageNumber = state.Page,
                PageSize = state.PageSize
            };
            await Fetch(searchRequest);
            var data = CardRequestsResponse;
            totalItems = data.TotalCount;
            switch (state.SortLabel)
            {
                case "externalId":
                    data.Data = data.Data.OrderByDirection(state.SortDirection, o => o.ExternalId).ToList();
                    break;
                case "firstName":
                    data.Data = [.. data.Data.OrderByDirection(state.SortDirection, o => o.MemberData.FirstName)];
                    break;
                case "name_field":
                    data.Data = [.. data.Data.OrderByDirection(state.SortDirection, o => o.MemberData.LastName)];
                    break;
                case "position_field":
                    data.Data = [.. data.Data.OrderByDirection(state.SortDirection, o => o.MemberData.LastName)];
                    break;
                case "mass_field":
                    data.Data = data.Data.OrderByDirection(state.SortDirection, o => o.Status).ToList();
                    break;
            }
            return new TableData<CardRequestDto>() { TotalItems = totalItems, Items = CardRequestsResponse.Data };
        }

        private void OnSearch(string text)
        {
            searchString = text;
            table.ReloadServerData();
        }

        private async Task NewRequest()
        {
            Navigation.NavigateTo($"/cardrequest/new");
        }

        private async Task Approve(Guid CardRequestId)
        {

            BusySubmitting = true;
            if (await ApiHelper.ExecuteCallGuardedAsync(
                () => CardRequestsClient.ApproveAsync(CardRequestId),
            Snackbar) is Guid id)
            {
                table.ReloadServerData();
            }

            BusySubmitting = false;
        }

        private async Task Reject(Guid CardRequestId)
        {

            BusySubmitting = true;
            if (await ApiHelper.ExecuteCallGuardedAsync(
                () => CardRequestsClient.RejectAsync(CardRequestId),
            Snackbar) is Guid id)
            {
                table.ReloadServerData();
            }

            BusySubmitting = false;
        }

    }
}
