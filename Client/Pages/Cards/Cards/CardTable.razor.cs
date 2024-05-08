using Client.Shared;
using Infrastructure.ApiClient;
using Microsoft.AspNetCore.Components;

namespace Client.Pages.Cards.Cards
{
    public partial class CardTable
    {
        [Inject]
        private ICardsClient CardsClient { get; set; }
        public PaginationResponseOfCardDto CardsResponse = new();

        private MudTable<CardDto> table;
        private int totalItems;
        private string searchString = null;
        bool BusySubmitting;
        protected override async Task OnInitializedAsync()
        {

        }


        private async Task Fetch(SearchCardsRequest SearchCardRequest)
        {
            if (await ApiHelper.ExecuteCallGuardedAsync(() => CardsClient.SearchActiveAsync(SearchCardRequest), Snackbar) is PaginationResponseOfCardDto cards)
            {
                CardsResponse = cards;
            }
            else
            {
                CardsResponse.Data = [];
            }

        }
        private async Task<TableData<CardDto>> ServerReload(TableState state)
        {
            var searchRequest = new SearchCardsRequest
            {
                Keyword = searchString,
                PageNumber = state.Page,
                PageSize = state.PageSize
            };
            await Fetch(searchRequest);
            var data = CardsResponse;
            totalItems = data.TotalCount;
            switch (state.SortLabel)
            {
                case "cardNumber":
                    data.Data = [.. data.Data.OrderByDirection(state.SortDirection, o => o.CardNumber)];
                    break;
                case "memberNumber":
                    data.Data = [.. data.Data.OrderByDirection(state.SortDirection, o => o.ExternalId)];
                    break;
                case "fullName":
                    data.Data = [.. data.Data.OrderByDirection(state.SortDirection, o => o.FullName)];
                    break;
                case "cardStatus":
                    data.Data = [.. data.Data.OrderByDirection(state.SortDirection, o => o.CardStatus)];
                    break;               
                case "isCollected":
                    data.Data = [.. data.Data.OrderByDirection(state.SortDirection, o => o.IsCollected)];
                    break;
                case "printStatus":
                    data.Data = [.. data.Data.OrderByDirection(state.SortDirection, o => o.PrintStatus)];
                    break;
            }
            return new TableData<CardDto>() { TotalItems = totalItems, Items = CardsResponse.Data };
        }

        private void OnSearch(string text)
        {
            searchString = text;
            table.ReloadServerData();
        }
    }
}
