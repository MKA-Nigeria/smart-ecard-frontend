using Client.Dialogs;
using Client.Pages.Cards.CardRequests;
using Client.Shared;
using Infrastructure.ApiClient;
using Microsoft.AspNetCore.Components;
using static MudBlazor.CategoryTypes;
using static System.Net.Mime.MediaTypeNames;

namespace Client.Pages.Cards.Cards
{
    public partial class CardTable
    {
        [Inject]
        private ICardsClient CardsClient { get; set; }

        [Inject]
        private IAppConfigurationsClient AppConfigurationsClient { get; set; }
        public PaginationResponseOfCardDto CardsResponse = new();

        private MudTable<CardDto> table;
        private int totalItems;
        private string searchString = null;

        private PrintStatus? printStatus = null;
        private bool? _isCollected = null;
        private CardStatus? cardStatus = null;
        private string appClient = null;
        bool BusySubmitting;
        protected override async Task OnInitializedAsync()
        {
            var client = await AppConfigurationsClient.GetAppConfigurationByKeyAsync("AppDomain");
            appClient = client.Value;
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
                PrintStatus = printStatus,
                IsCollected = _isCollected,
                CardStatus = cardStatus,
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

        private async Task IsPrinted(PrintStatus? status)
        {
            printStatus = status;
            await table.ReloadServerData();
        }

        private async Task IsCollected(bool? value)
        {
            _isCollected = value;
            await table.ReloadServerData();
        }

        public async Task ActivateCard(string CardNumber)
        {

            BusySubmitting = true;

            if (await ApiHelper.ExecuteCallGuardedAsync(
                () => CardsClient.ActivateAsync(CardNumber),
            Snackbar) is Guid id)
            {
                Snackbar.Add("Card activated successfully", Severity.Success);
                table.ReloadServerData();
            }

            BusySubmitting = false;
        }

        public async Task DeactivateCard(string CardNumber)
        {

            BusySubmitting = true;

            if (await ApiHelper.ExecuteCallGuardedAsync(
                () => CardsClient.DeactivateAsync(CardNumber),
            Snackbar) is Guid id)
            {
                Snackbar.Add("Card deactivated successfully", Severity.Success);
                table.ReloadServerData();
            }

            BusySubmitting = false;
        }

        public async Task PrintCard(string CardNumber)
        {

            BusySubmitting = true;

            if (await ApiHelper.ExecuteCallGuardedAsync(
                () => CardsClient.PrintCardAsync(CardNumber),
            Snackbar) is Guid id)
            {
                Snackbar.Add("Card set to printed successfully", Severity.Success);
                table.ReloadServerData();
            }

            BusySubmitting = false;
        }

        public async Task CollectCard(string CardNumber)
        {

            BusySubmitting = true;

            if (await ApiHelper.ExecuteCallGuardedAsync(
                () => CardsClient.CollectCardAsync(CardNumber),
            Snackbar) is Guid id)
            {
                Snackbar.Add("Card set to collected successfully", Severity.Success);
                table.ReloadServerData();
            }

            BusySubmitting = false;
        }

        private async Task Print(string CardNumber)
        {

            Navigation.NavigateTo($"/cardrequests");
        }

        private async Task Send(string CardNumber)
        {

            Navigation.NavigateTo($"/cardrequests");
        }

        private async Task ShowConfirmationDialog(string cardNumber, string action)
        {
            var message = action == "print" ? "Are you sure you want to set this card to print?"
                                            : "Are you sure you want to set this card to collected?";

            var parameters = new DialogParameters { ["Message"] = message, ["OnConfirmed"] = EventCallback.Factory.Create(this, () => ConfirmAction(cardNumber, action)) };

            var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Small, FullWidth = true };

            DialogService.Show<CardConfirmation>("Confirmation", parameters, options);
        }

        private async Task ConfirmAction(string cardNumber, string action)
        {
            BusySubmitting = true;

            if (action == "print")
            {
                var response = await ApiHelper.ExecuteCallGuardedAsync(
                    () => CardsClient.PrintCardAsync(cardNumber),
                    Snackbar);

                if (response is Guid id)
                {
                    Snackbar.Add("Card set to printed successfully", Severity.Success);
                    await table.ReloadServerData();
                }
                else
                {
                    Snackbar.Add("Failed to set card to printed", Severity.Error);
                }
            }
            else if (action == "collect")
            {
                var response = await ApiHelper.ExecuteCallGuardedAsync(
                    () => CardsClient.CollectCardAsync(cardNumber),
                    Snackbar);

                if (response is Guid id)
                {
                    Snackbar.Add("Card set to collected successfully", Severity.Success);
                    await table.ReloadServerData();
                }
                else
                {
                    Snackbar.Add("Failed to set card to collected", Severity.Error);
                }
            }

            BusySubmitting = false;
        }
    }
}
