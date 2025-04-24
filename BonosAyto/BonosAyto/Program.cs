using BonosAyto.Components;
using BonosAytoService;
using BonosAytoService.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "UsId";
        options.LoginPath = "/login";
        options.Cookie.MaxAge = TimeSpan.FromDays(30);
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<EstablecimientoService>();
builder.Services.AddHttpContextAccessor();


var app = builder.Build();
ConexionBD.Inicilizar("Server=DESKTOP-N3LV49P\\SQLEXPRESS;Database=AytoCoruna;Trusted_Connection=True; TrustServerCertificate=True;");


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();


//app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

