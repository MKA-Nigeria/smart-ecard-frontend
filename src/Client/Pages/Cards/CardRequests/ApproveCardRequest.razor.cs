using Client.Shared;
using Infrastructure.ApiClient;
using Infrastructure.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Client.Pages.Cards.CardRequests
{
    public partial class ApproveCardRequest
    {
        [Parameter]
        public Guid CardRequestId { get; set; }
        [Inject]
        private ICardRequestsClient CardRequestsClient { get; set; } = default!;
        CardRequestDto CardRequest { get; set; }
        bool BusySubmitting;
        bool _loaded;
        //private bool _loaded;
        //private bool _uploaded;
        private bool _uploaded;
        private bool dataLoaded;
        private string? _imageUrl;
        protected override async Task OnInitializedAsync()
        {
            if(await ApiHelper.ExecuteCallGuardedAsync(() => CardRequestsClient.GetAsync(CardRequestId), Snackbar) is CardRequestDto cardRequestDto)
            {
                CardRequest = cardRequestDto;
            }
            _loaded = true;
        }
        private async Task Approve()
        {

            BusySubmitting = true;
            if (await ApiHelper.ExecuteCallGuardedAsync(
                () => CardRequestsClient.ApproveAsync(CardRequestId),
            Snackbar) is Guid id)
            {
                Navigation.NavigateTo($"/cardrequests");
            }

            BusySubmitting = false;
        }
        
        private async Task Reject()
        {

            BusySubmitting = true;
            if (await ApiHelper.ExecuteCallGuardedAsync(
                () => CardRequestsClient.RejectAsync(CardRequestId),
            Snackbar) is Guid id)
            {
                Navigation.NavigateTo($"/cardrequests");
            }

            BusySubmitting = false;
        }
        
        private async Task Cancel()
        {

            BusySubmitting = true;
            if (await ApiHelper.ExecuteCallGuardedAsync(
                () => CardRequestsClient.CancelAsync(CardRequestId),
            Snackbar) is Guid id)
            {
                Navigation.NavigateTo($"/cardrequests");
            }

            BusySubmitting = false;
        }

       
    }
}
