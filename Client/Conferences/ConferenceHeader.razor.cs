using Microsoft.AspNetCore.Components;

namespace ConfTool.Client.Conferences
{
    public partial class ConferenceHeader
    {
        [Parameter]
        public string Mode { get; set; }
    }
}