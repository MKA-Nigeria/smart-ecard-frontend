﻿using Client.Shared;
using Infrastructure.ApiClient;
using Infrastructure.Auth;
using Infrastructure.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Shared.Authorization;

namespace Client.Pages.Cards.Cards
{
    public partial class MkanCard
    {
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; } = default!;
        [Inject]
        protected IAuthorizationService AuthService { get; set; } = default!;
        public bool _loaded;
        [Parameter]
        public string CardNumber { get; set; }

        private bool _canSearchCardRequests;

        public CardDto Card { get; set; }
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

        protected override async Task OnParametersSetAsync()
        {
            var user = (await AuthState).User;
            _canSearchCardRequests = await AuthService.HasPermissionAsync(user, AppAction.Search, Resource.CardRequest);
        }
        protected override async Task OnInitializedAsync()
        {
            keysToDisplay = ["Muqam", "DilaName", "Ilaqa", "RegionName"];
            var card = await ApiHelper.ExecuteCallGuardedAsync(
                   () => CardsClient.GetAsync(CardNumber), Snackbar);
            
            if (card.Status)
            {
                Card = card.Data;
                
                entityId = card.Data.ExternalId;
                var cardRequestresponse = await CardRequestsClient.Get2Async(entityId);
                CardRequest = cardRequestresponse.Data;
                _loaded = true;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await Task.Delay(10000);
            await GenerateQRCode();
        }
        /* private void GenerateQRCode()
         {
             var qrCodeBytes = QrCodeService.GenerateQRCode(CardNumber);
             qrCodeImage = $"data:image/png;base64,{Convert.ToBase64String(qrCodeBytes)}";
         }*/
        private async Task GenerateQRCode()
        {
            await JsRuntime.InvokeVoidAsync("qrcodeInterop.clearQRCode", "qrcode");
            await JsRuntime.InvokeVoidAsync("qrcodeInterop.generateQRCode", "qrcode", $"https://ecard.khuddam.ng/profile/public?id={entityId}");
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

        public async Task PrintCard()
        {

            BusySubmitting = true;

            if (await ApiHelper.ExecuteCallGuardedAsync(
                () => CardsClient.PrintCardAsync(CardNumber),
            Snackbar) is Guid id)
            {
                Snackbar.Add("Card set to printed successfully", Severity.Success);
            }

            BusySubmitting = false;
        }

        public async Task CollectCard()
        {

            BusySubmitting = true;

            if (await ApiHelper.ExecuteCallGuardedAsync(
                () => CardsClient.CollectCardAsync(CardNumber),
            Snackbar) is Guid id)
            {
                Snackbar.Add("Card set to collected successfully", Severity.Success);
            }

            BusySubmitting = false;
        }
        public async Task ViewCard()
        {
            Navigation.NavigateTo($"http://localhost:3000/idcard?print={CardNumber}");
        }
    }
}
