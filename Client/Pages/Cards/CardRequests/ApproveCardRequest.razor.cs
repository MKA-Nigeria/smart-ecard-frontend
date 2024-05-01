using Client.Shared;
using Infrastructure.ApiClient;
using Microsoft.AspNetCore.Components;

namespace Client.Pages.Cards.CardRequests
{
    public partial class ApproveCardRequest
    {
        [Parameter]
        public Guid CardRequestId { get; set; }
        [Inject]
        private ICardRequestsClient CardRequestsClient { get; set; } = default!;
        CardRequestDto CardRequest { get; set; }

        bool _loaded;
        bool _showGenotype = true;
        bool _showBloodGroup = true;
        string BloodGroup;
        string Genotype;
        protected override async Task OnInitializedAsync()
        {
            if(await ApiHelper.ExecuteCallGuardedAsync(() => CardRequestsClient.GetAsync(CardRequestId), Snackbar) is CardRequestDto cardRequestDto)
            {
                CardRequest = cardRequestDto;
            }
            _loaded = true;
        }
        private async Task Submit()
        {

        }
    }
}
