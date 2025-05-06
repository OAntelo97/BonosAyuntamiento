using Microsoft.Extensions.Configuration;

namespace BonosAytoService
{
    public static class ConexionBD
    {
        public static string CadenaDeConexion()
        {
            string rootExeProject = Directory.GetCurrentDirectory();

            if (rootExeProject.Contains("BonosAytoTest")) rootExeProject = Directory.GetParent(rootExeProject)?.Parent?.Parent?.FullName!;

            string rootBonosAytoService = Path.GetFullPath(Path.Combine(rootExeProject, @"..", "BonosAytoService"));

            var builder = new ConfigurationBuilder()
                            .SetBasePath(rootBonosAytoService)
                            .AddJsonFile("configuration.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            return configuration.GetConnectionString("Connection")!;
        }
    }
}
