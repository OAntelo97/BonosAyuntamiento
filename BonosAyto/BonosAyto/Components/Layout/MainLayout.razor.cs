using BonosAytoService.Services;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BonosAyto.Components.Layout
{
    public partial class MainLayout
    {
        [Inject] private IJSRuntime JS { get; set; }

        async Task DeleteCookie()
        {
            await JS.InvokeVoidAsync("cookieHelper.deleteCookie", "UsId");
            GlobalVariables.usuario = null;
            Navigate.NavigateTo("/");
        }
    }
}
