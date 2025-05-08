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
        public string Titulo { get; set; }
        public string Pestaña { get; set; }
        public string Alta { get; set; }
        public string Listado { get; set; }
        [Parameter]
        public RenderFragment RenderAltaFragment { get; set; }
        [Parameter]
        public RenderFragment RenderListadoFragment { get; set; }
        [Parameter]
        public PageType Tipo { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            switch (Tipo)
            {
                case PageType.Usuarios:
                    Titulo = "Gestión de Usuarios";
                    Pestaña = "Alta de Usuarios";
                    Alta = "Nuevo Usuario";
                    Listado = "Listado de Usuarios";
                    break;
                case PageType.Beneficiarios:
                    Titulo = "Gestión de Beneficiarios";
                    Pestaña = "Alta de Beneficiarios";
                    Alta = "Nuevo Beneficiario";
                    Listado = "Listado de Beneficiarios";
                    break;
                case PageType.Establecimientos:
                    Titulo = "Gestión de Establecimientos";
                    Pestaña = "Alta de Establecimientos";
                    Alta = "Nuevo Establecimiento";
                    Listado = "Listado de Establecimientos";
                    break;
            }
        }

    }
}