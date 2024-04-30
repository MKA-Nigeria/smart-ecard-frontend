using Client.Components.Common;
using Client.Shared;
using Infrastructure.ApiClient;
using Microsoft.AspNetCore.Components;

namespace Client.Pages.Cards.CardRequests
{
    public partial class FetchUser
    {
        [Inject]
        protected ICardRequestsClient CardRequestClient { get; set; } = default!;

        private readonly UserRequest UserRequest = new();
        public bool BusySubmitting { get; set; }

        private CustomValidation? _customValidation;

        private async Task SubmitAsync()
        {
            BusySubmitting = true;

            if (await ApiHelper.ExecuteCallGuardedAsync(
                () => CardRequestClient.GetMemberDataAsync(UserRequest.ExternalId),
                Snackbar,
                _customValidation) is not null)
            {
                Snackbar.Add($"Member data retrieved", Severity.Info);
                Navigation.NavigateTo($"/cardRequest/{UserRequest.ExternalId}");
            }

            BusySubmitting = false;
        }
    }

    public class UserRequest
    {
        public string ExternalId { get; set; } = default!;
    }
}
