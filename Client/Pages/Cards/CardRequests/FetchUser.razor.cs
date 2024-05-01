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
            if (response is MemberData memberData)
            {
                Snackbar.Add($"Member data retrieved", Severity.Info);
                Navigation.NavigateTo($"/member/{UserRequest.ExternalId}");
            }
            else
            {
                Snackbar.Add($"Error while fetching member data, check the provided info", Severity.Info);
            }

            BusySubmitting = false;
        }
    }

    public class UserRequest
    {
        public string ExternalId { get; set; } = default!;
    }
}
