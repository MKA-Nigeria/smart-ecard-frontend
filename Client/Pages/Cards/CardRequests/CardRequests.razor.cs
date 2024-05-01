using Client.Shared;
using Infrastructure.ApiClient;
using Microsoft.AspNetCore.Components;

namespace Client.Pages.Cards.CardRequests
{
    public partial class CardRequests
    {
        [Inject]
        private ICardRequestsClient CardRequestsClient { get; set; }
        public PaginationResponseOfCardRequestDto CardRequestsResponse = new();

        private MudTable<CardRequestDto> table;
        private int totalItems;
        private string searchString = null;

        protected override async Task OnInitializedAsync()
        {
         
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
    }
}
