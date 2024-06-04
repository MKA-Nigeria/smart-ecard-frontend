using Client.Shared;
using Infrastructure.ApiClient;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace Client.Pages.Cards.Cards
{
    public partial class MkanCard
    {
        public bool _loaded;
        [Parameter]
        public string CardNumber { get; set; }
        [Parameter]
        public string InputText { get; set; } = "Sample Text";

        public CardDto Card { get; set; }
        bool BusySubmitting;
        [Inject]
        protected ICardsClient CardsClient { get; set; } = default!;

        [Inject]
        protected IJSRuntime JsRuntime { get; set; } = default!;

        bool _showGenotype = true;
        bool _showBloodGroup = true;
        string BloodGroup;
        string Genotype;
        private string qrCodeUrl = "";
        private bool arrows = true;
        private bool bullets = true;
        private bool enableSwipeGesture = true;
        private bool autocycle = true;
        private Transition transition = Transition.Slide;
        protected override async Task OnInitializedAsync()
        {
            if (await ApiHelper.ExecuteCallGuardedAsync(
                    () => CardsClient.GetAsync(CardNumber), Snackbar) is CardDto card)
            {
                Card = card;
                _loaded = true;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await Task.Delay(10000);
                await GenerateQRCode();
            }
        }

        private async Task GenerateQRCode()
        {
            await JsRuntime.InvokeVoidAsync("qrcodeInterop.clearQRCode", "qrcode");
            await JsRuntime.InvokeVoidAsync("qrcodeInterop.generateQRCode", "qrcode", InputText);
        }
        public async Task ActivateCard()
        {

            BusySubmitting = true;

            if (await ApiHelper.ExecuteCallGuardedAsync(
                () => CardsClient.ActivateAsync(CardNumber),
            Snackbar) is Guid id)
            {
                Snackbar.Add("Card activated successfully", Severity.Success);
            }

            BusySubmitting = false;
        }

        public async Task DeactivateCard()
        {

            BusySubmitting = true;

            if (await ApiHelper.ExecuteCallGuardedAsync(
                () => CardsClient.DeactivateAsync(CardNumber),
            Snackbar) is Guid id)
            {
                Snackbar.Add("Card deactivated successfully", Severity.Success);
            }

            BusySubmitting = false;
        }

        public async Task ViewCard()
        {
            Navigation.NavigateTo($"http://localhost:3000/idcard?print={CardNumber}");
        }
    }
}
