using BonosAyto.Components.Enums;
using BonosAytoService.DTOs;
using BonosAytoService.Model;
using BonosAytoService.Services;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.WebUtilities;

namespace BonosAyto.Components.Templates
{
    public partial class ListadoAltaTemplate
    {
        [Parameter]
        public string? titulo { get; set; }
        [Parameter]
        public string? pestaña { get; set; }
        [Parameter]
        public string? alta { get; set; }
        [Parameter]
        public string? listado { get; set; }
        [Parameter]
        public RenderFragment? renderAlta { get; set; }
        [Parameter]
        public RenderFragment? renderListado { get; set; }
        [Parameter]
        public PageType tipo { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            switch (tipo)
            {
                case PageType.Usuarios:
                    titulo = "Gestión de Usuarios";
                    pestaña = "Alta de Usuarios";
                    alta = "Nuevo Usuario";
                    listado = "Listado de Usuarios";
                    break;
                case PageType.Beneficiarios:
                    titulo = "Gestión de Beneficiarios";
                    pestaña = "Alta de Beneficiarios";
                    alta = "Nuevo Beneficiario";
                    listado = "Listado de Beneficiarios";
                    break;
                case PageType.Establecimientos:
                    titulo = "Gestión de Establecimientos";
                    pestaña = "Alta de Establecimientos";
                    alta = "Nuevo Establecimiento";
                    listado = "Listado de Establecimientos";
                    break;
            }
        }

    }
}
