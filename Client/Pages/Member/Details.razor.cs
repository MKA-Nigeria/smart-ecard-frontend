using Client.Shared;
using Infrastructure.ApiClient;
using Infrastructure.Common;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Client.Pages.Cards.CardRequests;

namespace Client.Pages.Member
{
    public partial class Details
    {
        public bool _loaded;
        public bool _hasCard;
        public bool _hasCardRequest;
        [Parameter]
        public string ExternalId { get; set; }
        private IBrowserFile _selectedFile;
        public MemberData MemberData { get; set; }
        public CardDto Card { get; set; }
        public CardRequestDto CardRequest { get; set; }
        CreateCardRequest cardRequest = new();
        bool BusySubmitting;
        [Inject]
        protected ICardRequestsClient CardRequestsClient { get; set; } = default!;

        [Inject]
        protected ICardsClient CardsClient { get; set; } = default!;


        bool _showGenotype = true;
        bool _showBloodGroup = true;
        string BloodGroup;
        string Genotype;
        private bool _uploaded;
        private bool dataLoaded;
        private string? _imageUrl;
        protected override async Task OnInitializedAsync()
        {

            var cardResponse = await ApiHelper.ExecuteCallGuardedAsync(
                    () => CardsClient.GetAsync(ExternalId), Snackbar);
            if (cardResponse != null)
            {
                _hasCard = true;
                Card = cardResponse;
                _loaded = true;
                return;
            }

            var cardRequestresponse = await ApiHelper.ExecuteCallGuardedAsync(
                    () => CardRequestsClient.Get2Async(ExternalId), Snackbar);

            if (cardRequestresponse != null)
            {
                _hasCardRequest = true;
                CardRequest = cardRequestresponse;
                _loaded = true;

                return;
            }

            var response = await ApiHelper.ExecuteCallGuardedAsync(
                    () => CardRequestsClient.GetMemberDataAsync(ExternalId), Snackbar);
            if (response.Status)
            {
                MemberData = response.Data;
                cardRequest.ExternalId = ExternalId;
                cardRequest.MemberData = MemberData;
                _loaded = true;
                return;
            }
        }
        public async Task SubmitCardRequestAsync()
        {
            BusySubmitting = true;

            // Prepare member data
            MemberData.PhotoUrl = _imageUrl;
            if (_showBloodGroup)
            {
                MemberData.CustomData.Add("BloodGroup", BloodGroup);
            }
            if (_showGenotype)
            {
                MemberData.CustomData.Add("Genotype", Genotype);
            }

            // Create card request

            cardRequest.ExternalId = ExternalId;
            cardRequest.MemberData = MemberData;
            cardRequest.ImageRequest = await UploadFiles();

            // Submit card request
            if (await ApiHelper.ExecuteCallGuardedAsync(
                () => CardRequestsClient.CreateAsync(cardRequest),
                Snackbar) is Guid id)
            {
                Navigation.NavigateTo(Navigation.Uri, true);
            }

            BusySubmitting = false;
        }

        private async Task<FileUploadRequest> UploadFiles()
        {
            var file = _selectedFile;
            if (file != null)
            {
                string extension = Path.GetExtension(file.Name);
                if (!ApplicationConstants.SupportedImageFormats.Contains(extension.ToLower()))
                {
                    Snackbar.Add("Image Format Not Supported.", Severity.Error);
                    return null;
                }

                string fileName = $"{ExternalId}-{Guid.NewGuid():N}";
                fileName = fileName[..Math.Min(fileName.Length, 90)];
                var imageFile = await file.RequestImageFileAsync(ApplicationConstants.StandardImageFormat, ApplicationConstants.MaxImageWidth, ApplicationConstants.MaxImageHeight);
                byte[] buffer = new byte[imageFile.Size];
                await imageFile.OpenReadStream(ApplicationConstants.MaxAllowedSize).ReadAsync(buffer);
                string base64String = $"data:{ApplicationConstants.StandardImageFormat};base64,{Convert.ToBase64String(buffer)}";
                var fileUploadRequest = new FileUploadRequest() { Name = fileName, Data = base64String, Extension = extension };
                return fileUploadRequest;
            }
            return null;
        }

        private async Task OnFileSelection(InputFileChangeEventArgs e)
        {
            var files = e.GetMultipleFiles();
            if (files != null && files.Count > 0)
            {
                _selectedFile = files[0];
                await LoadAndDisplayImage();
            }
        }

        private async Task LoadAndDisplayImage()
        {
            if (_selectedFile != null)
            {
                var file = _selectedFile;

                var extension = Path.GetExtension(file.Name);
                if (!ApplicationConstants.SupportedImageFormats.Contains(extension.ToLower()))
                {
                    Snackbar.Add("Image Format Not Supported.", Severity.Error);
                    return;
                }

                var imageFile = await file.RequestImageFileAsync(ApplicationConstants.StandardImageFormat, ApplicationConstants.MaxImageWidth, ApplicationConstants.MaxImageHeight);
                byte[] buffer = new byte[imageFile.Size];
                await imageFile.OpenReadStream(ApplicationConstants.MaxAllowedSize).ReadAsync(buffer);
                string base64String = $"data:{ApplicationConstants.StandardImageFormat};base64,{Convert.ToBase64String(buffer)}";
                _imageUrl = base64String;

                StateHasChanged();
            }
        }
        private async Task Cancel()
        {

            BusySubmitting = true;
            if (await ApiHelper.ExecuteCallGuardedAsync(
                () => CardRequestsClient.CancelAsync(CardRequest.Id),
            Snackbar) is Guid id)
            {
                Navigation.NavigateTo($"/cardrequests");
            }

            BusySubmitting = false;
        }

    }
}
