using Client.Components.Common;
using Client.Shared;
using Infrastructure.ApiClient;
using Mapster;
using Microsoft.AspNetCore.Components;

namespace Client.Pages.Cards.CardRequests
{
    public partial class MemberDetails
    {
        public bool _loaded;
        [Parameter]
        public string ExternalId { get; set; }

        public MemberData MemberData { get; set; }
        bool BusySubmitting;
        [Inject]
        protected ICardRequestsClient CardRequestsClient { get; set; } = default!;
        protected override async Task OnInitializedAsync()
        {
            //BusySubmitting = true;
            if (await ApiHelper.ExecuteCallGuardedAsync(
                    () => CardRequestsClient.GetMemberDataAsync(ExternalId), Snackbar)
                is MemberData member)
            {
                MemberData = member;
                _loaded = true;
            }
            //BusySubmitting = false;
        }
        public async Task SubmitCardRequestAsync()
        {
            BusySubmitting = true;

            if (await ApiHelper.ExecuteCallGuardedAsync(
                () => CardRequestsClient.CreateAsync(MemberData.Adapt<CreateCardRequest>()),
            Snackbar) is Guid)
            {
                Snackbar.Add($"Card request submitted successfully", Severity.Info);
            }

            BusySubmitting = false;
        }
    }
}
