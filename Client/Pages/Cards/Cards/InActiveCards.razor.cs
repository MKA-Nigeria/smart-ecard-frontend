using Client.Pages.Cards.CardRequests;
using Client.Shared;
using Infrastructure.ApiClient;
using Microsoft.AspNetCore.Components;

namespace Client.Pages.Cards.Cards
{
    public partial class InActiveCards
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


        private async Task Fetch(SearchInActiveCardsRequest SearchCardRequest)
        {
            if (await ApiHelper.ExecuteCallGuardedAsync(() => CardsClient.SearchInActiveAsync(SearchCardRequest), Snackbar) is PaginationResponseOfCardDto cards)
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
            var searchRequest = new SearchInActiveCardsRequest
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
                    data.Data = data.Data.OrderByDirection(state.SortDirection, o => o.CardNumber).ToList();
                    break;
                case "name":
                    data.Data = [.. data.Data.OrderByDirection(state.SortDirection, o => o.Name)];
                    break;
                case "approvedDate":
                    data.Data = [.. data.Data.OrderByDirection(state.SortDirection, o => o.ApprovedDate)];
                    break;
                case "printStatus":
                    data.Data = [.. data.Data.OrderByDirection(state.SortDirection, o => o.PrintStatus)];
                    break;
                case "isCollected":
                    data.Data = [.. data.Data.OrderByDirection(state.SortDirection, o => o.IsCollected)];
                    break;
            }
            return new TableData<CardDto>() { TotalItems = totalItems, Items = CardsResponse.Data };
        }

        private void OnSearch(string text)
        {
            searchString = text;
            table.ReloadServerData();
        }

        private void OpenModal(Guid id, string name)
        {
            var parameters = new DialogParameters<CardsActionModal<Guid>>
            {
                { x => x.ContentText, $"Are you sure you want to activate {name}" },
                { x => x.ButtonText, "Activate" },
                { x => x.Color, Color.Success },
                { x => x.SubmitAction, () =>  Activate(id) }
            };

            DialogService.Show<CardsActionModal<Guid>>("Confirm", parameters);
        }

        private async Task<Guid> Activate(Guid cardId)
        {

            BusySubmitting = true;
            if (await ApiHelper.ExecuteCallGuardedAsync(
                () => CardsClient.ActivateAsync(cardId),
            Snackbar) is Guid id)
            {
                return id;
            }

            BusySubmitting = false;
            return cardId;
        }
    }
}
