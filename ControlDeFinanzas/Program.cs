
using ControlDeFinanzas.Models;
using ControlDeFinanzas.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//CREO UNA POLITICA DE SEGURIDAD

var politicaUsuariosAutenticados = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

builder.Services.AddControllersWithViews(opciones => 
{
    opciones.Filters.Add(new AuthorizeFilter(politicaUsuariosAutenticados));

});
builder.Services.AddTransient<IRepositorioTiposCuentas, RepositorioTiposCuentas>();
builder.Services.AddTransient<IServicioUsuarios, ServicioUsuarios>();
builder.Services.AddTransient<IRepositorioCuentas, RepositorioCuentas>();
builder.Services.AddTransient<IRepositorioCategorias, RepositorioCategorias>();
builder.Services.AddTransient<IRepositorioTransacciones, repostorioTransacciones>();
builder.Services.AddTransient<IServicioReportes, ServicioReportes>();
builder.Services.AddTransient<IRepositorioUsuarios, RepositorioUsuarios>();
builder.Services.AddTransient < SignInManager<Usuario>>();
builder.Services.AddHttpContextAccessor();
//configuro identity
builder.Services.AddTransient<IUserStore<Usuario>, UsuarioStore>();
//Defino las reglas del password
builder.Services.AddIdentityCore<Usuario>(opciones => 
{
    //requiere números
    opciones.Password.RequireDigit = false;
    //requiere minúsculas
    opciones.Password.RequireLowercase = false;
    //requiere mayúsculas
    opciones.Password.RequireUppercase = false;
    //requiere un alfanumerico
    opciones.Password.RequireNonAlphanumeric = false;


}).AddErrorDescriber<MensajesDeErrorIdentity>();
//configuro el uso e cookies para autentificación
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
}).AddCookie(IdentityConstants.ApplicationScheme, opciones => 
        {
            opciones.LoginPath = "/usuarios/login";
        }
        );



//coloco el automapper para poder utilizarlo
builder.Services.AddAutoMapper(typeof(Program));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
//coloco la autentificación antes de la autorización

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Transacciones}/{action=Index}/{id?}");

app.Run();
