using Client.Shared;
using Infrastructure.ApiClient;
using Microsoft.AspNetCore.Components;

namespace Client.Pages.Cards.Cards
{
    public partial class CardDetails
    {
        public bool _loaded;
        [Parameter]
        public string CardNumber { get; set; }

        public CardDto Card { get; set; }
        bool BusySubmitting;
        [Inject]
        protected ICardsClient CardsClient { get; set; } = default!;
        bool _showGenotype = true;
        bool _showBloodGroup = true;
        string BloodGroup;
        string Genotype;

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

