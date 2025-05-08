using Microsoft.AspNetCore.Components;
using static BonosAytoService.Models.Enums.Enums;

namespace BonosAyto.Components.Templates
{
    public partial class DetallesTemplate
    {
        public string Titulo { get; set; }
        public string TituloFormulario { get; set; }
        public string TituloLista { get; set; }
        public string TituloPestana { get; set; }
        [Parameter]
        public RenderFragment PestanaFragment { get; set; }
        [Parameter]
        public RenderFragment FormFragment { get; set; }
        [Parameter]
        public RenderFragment ListFragment { get; set; }
        [Parameter]
        public RenderFragment PestanaContenidoFragment { get; set; }
        [Parameter]
        public PageType Tipo { get; set; }

        [Parameter]
        public int Id { get; set; }
        [Parameter] public string? Modo { get; set; } 
        private bool EsModoLectura;


        
        protected override void OnInitialized()
        {
            base.OnInitialized();

            var uri = new Uri(Navigate.Uri);
            var segments = uri.AbsolutePath.Trim('/').Split('/');

            Modo = segments[segments.Length-2];
            Id = int.Parse(segments[segments.Length - 1]);
        }

        private string GetBasePath()
        {
            var uri = new Uri(Navigate.Uri);
            var segments = uri.AbsolutePath.Trim('/').Split('/');

            if (segments.Length >= 3)
            {
                return $"/{segments[segments.Length-3]}";
            }

            return "/";
        }

        private void VerDetalle()
        {
           Modo = "ver";
            var basePath = GetBasePath(); 
            Navigate.NavigateTo($"{basePath}/ver/{Id}");
        }

        private void Editar()
        {
            Modo = "editar";
            var basePath = GetBasePath(); 
            Navigate.NavigateTo($"{basePath}/editar/{Id}");
        }



        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            EsModoLectura = Modo?.ToLower() != "editar";
            


            switch (Tipo)
            {
                case PageType.Usuarios:
                    Titulo = "Información del usuario";
                    TituloFormulario = "Datos";
                    TituloPestana = "Datos";
                    break;
                case PageType.Beneficiarios:
                    Titulo = "Información del beneficiario";
                    TituloFormulario = "Datos";
                   // TituloLista = ":::::::::::::::";
                    TituloPestana = "Datos";
                    break;
                case PageType.Establecimientos:
                    Titulo = "Información del establecimiento";
                    TituloFormulario = "Datos";
                    //TituloLista = "Nuevo Establecimiento";
                    TituloPestana = "Datos";
                    break;
            }
        }


    }
}
