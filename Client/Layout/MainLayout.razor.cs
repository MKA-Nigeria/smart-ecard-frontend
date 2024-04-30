using Microsoft.AspNetCore.Components;

namespace Client.Layout
{
    public partial class MainLayout
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; } = default!;
    }
}
