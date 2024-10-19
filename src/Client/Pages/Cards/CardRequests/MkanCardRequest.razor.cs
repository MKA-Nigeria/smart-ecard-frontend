using Client.Shared;
using Infrastructure.ApiClient;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Client.Pages.Cards.CardRequests
{
    public partial class MkanCardRequest
    {
        public bool _loaded;
        [Parameter]
        public string MemberNumber { get; set; }

        bool BusySubmitting;
        [Inject]
        protected ICardsClient CardsClient { get; set; } = default!;
        public CardRequestDto CardRequest { get; set; }
        private string appClient = null;
        [Inject]
        protected ICardRequestsClient CardRequestsClient { get; set; } = default!;
        private List<string> keysToDisplay;
        [Inject]
        protected IJSRuntime JsRuntime { get; set; } = default!;
        private string qrCodeImage;

        bool _showGenotype = true;
        bool _showBloodGroup = true;
        string BloodGroup;
        string Genotype;
        string entityId;
        private string qrCodeUrl = "";
        private bool arrows = true;
        private bool bullets = true;
        private bool enableSwipeGesture = true;
        private bool autocycle = true;
        private Transition transition = Transition.Slide;
        protected override async Task OnInitializedAsync()
        {
            keysToDisplay = ["Muqam", "DilaName", "Ilaqa", "RegionName"];
            var cardRequest = await ApiHelper.ExecuteCallGuardedAsync(
                   () => CardRequestsClient.Get2Async(MemberNumber), Snackbar);
            
            if (cardRequest is not null)
            {
                CardRequest = cardRequest.Data;
                
                entityId = cardRequest.Data.ExternalId;
                _loaded = true;
            }
        }

        private async Task Cancel()
        {

            BusySubmitting = true;
            if (await ApiHelper.ExecuteCallGuardedAsync(
                () => CardRequestsClient.CancelAsync(CardRequest.Id),
            Snackbar) is Guid id)
            {
                Navigation.NavigateTo($"/cardrequests");
            }

            BusySubmitting = false;
        }
    }
}
