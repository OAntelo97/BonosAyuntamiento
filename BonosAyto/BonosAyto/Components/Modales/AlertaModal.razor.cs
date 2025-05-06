using Microsoft.AspNetCore.Components;

namespace BonosAyto.Components.Modales
{
    public partial class AlertaModal
    {
        [Parameter]
        public string modalId { get; set; }
        [Parameter]
        public string color { get; set; }
        [Parameter]
        public string icono { get; set; }
        [Parameter]
        public RenderFragment? Titulo { get; set; }
        [Parameter]
        public RenderFragment? Texto { get; set; }
        [Parameter]
        public RenderFragment? Descripcion { get; set; }
        [Parameter]
        public RenderFragment? botonAceptar { get; set; }
    }
}
