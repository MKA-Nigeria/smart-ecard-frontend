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
            var response = await CardRequestClient.GetMemberDataAsync(UserRequest.ExternalId);
            if (response.Status)
            {
                Snackbar.Add($"Member data retrieved", Severity.Info);
                Navigation.NavigateTo($"/member/{UserRequest.ExternalId}");
            }
            else
            {
                Snackbar.Add(response.Message, Severity.Error);
            }

            BusySubmitting = false;
        }
    }

    public class UserRequest
    {
        public string ExternalId { get; set; } = default!;
    }
}
