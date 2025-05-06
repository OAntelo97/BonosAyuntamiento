using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using BonosAyto.Components.Pages.Login;
using DocumentFormat.OpenXml.Spreadsheet;

namespace BonosAyto.Components.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        public async Task Authenticate([FromBody] LoginViewModel usuario)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.Usuario),
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Role, usuario.Rol.ToString())
                };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext!.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        [HttpPost("logout")]
        public async Task Authenticatelogout()
        {
            await HttpContext!.SignOutAsync();
        }
    }
}
