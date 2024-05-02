using Client.Components.Common;
using Client.Shared;
using Infrastructure.ApiClient;
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


        bool _showGenotype = true;
        bool _showBloodGroup = true;
        string BloodGroup;
        string Genotype;
        protected override async Task OnInitializedAsync()
        {
            var response = await ApiHelper.ExecuteCallGuardedAsync(
                    () => CardRequestsClient.GetMemberDataAsync(ExternalId), Snackbar);
            //BusySubmitting = true;
            if (response.Status)
            {
                MemberData = response.Data;
                _loaded = true;
            }
            //BusySubmitting = false;
        }
        public async Task SubmitCardRequestAsync()
        {

            BusySubmitting = true;
            if (_showBloodGroup)
            {
                MemberData.CustomData.Add("BloodGroup", BloodGroup);
            }
            if (_showGenotype)
            {
                MemberData.CustomData.Add("Genotype", Genotype);
            }
            CreateCardRequest cardRequest = new();
            cardRequest.ExternalId = ExternalId;
            cardRequest.MemberData = MemberData;
            
            if(await ApiHelper.ExecuteCallGuardedAsync(
                () => CardRequestsClient.CreateAsync(cardRequest),
            Snackbar) is Guid id)
            {
                Navigation.NavigateTo($"/cardrequests");
            }

            BusySubmitting = false;
        }
    }
}
