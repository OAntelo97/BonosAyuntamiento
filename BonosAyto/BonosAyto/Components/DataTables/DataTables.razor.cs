using Microsoft.AspNetCore.Components;

namespace BonosAyto.Components.DataTables
{
    public partial class DataTables
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }
    }
}
