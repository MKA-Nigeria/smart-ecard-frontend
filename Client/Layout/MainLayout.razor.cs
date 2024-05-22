﻿using Infrastructure.Preferences;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Client.Layout
{
    public partial class MainLayout
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; } = default!;
        [Parameter]
        public EventCallback OnDarkModeToggle { get; set; }
        [Parameter]
        public EventCallback<bool> OnRightToLeftToggle { get; set; }

        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; } = default!;

        private bool _drawerOpen;
        private bool _rightToLeft;

        protected override async Task OnInitializedAsync()
        {
            var user = (await AuthState).User;
            if(user.IsInRole("Nominal Member"))
            {
                Navigation.NavigateTo($"/member/{user.GetUserName()}");
            }
            if (await ClientPreferences.GetPreference() is ClientPreference preference)
            {
                _rightToLeft = preference.IsRTL;
                _drawerOpen = preference.IsDrawerOpen;
            }
        }

        private async Task RightToLeftToggle()
        {
            bool isRtl = await ClientPreferences.ToggleLayoutDirectionAsync();
            _rightToLeft = isRtl;

            await OnRightToLeftToggle.InvokeAsync(isRtl);
        }

        public async Task ToggleDarkMode()
        {
            await OnDarkModeToggle.InvokeAsync();
        }

        private async Task DrawerToggle()
        {
            _drawerOpen = await ClientPreferences.ToggleDrawerAsync();
        }

        private void Logout()
        {
            var parameters = new DialogParameters
            {
                { nameof(Dialogs.Logout.ContentText), "Logout Confirmation"},
                { nameof(Dialogs.Logout.ButtonText), "Logout"},
                { nameof(Dialogs.Logout.Color), Color.Error}
            };

            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
            DialogService.Show<Dialogs.Logout>("Logout", parameters, options);
        }

        private void Profile()
        {
            Navigation.NavigateTo("/account");
        }
    }
}
