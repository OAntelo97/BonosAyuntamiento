using Microsoft.AspNetCore.Components;

namespace BonosAyto.Components.Modales
{
    public partial class AlertaModal
    {   
        [Parameter]
        public RenderFragment? Titulo { get; set; }
        [Parameter]
        public RenderFragment? Contenido { get; set; }
        [Parameter]
        public RenderFragment? botonAceptar { get; set; }

        public string modalClass { get; set; }
        private bool showBackdrop = false;

        public void Open()
        {
            modalClass = "show d-block";
            showBackdrop = true;
        }

        public void Close()
        {
            modalClass = "d-none";
            showBackdrop = false;
        }
    }
}
