using GerenciamentoProducao.Data;
using GerenciamentoProducao.Interfaces;
using GerenciamentoProducao.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// DB
builder.Services.AddDbContext<GerenciamentoProdDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services.AddScoped<IFilmeRepository, FilmeRepository>();

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ICaixilhoRepository, CaixilhoRepository>();
builder.Services.AddScoped<ITipoCaixilhoRepository, TipoCaixilhoRepository>();
builder.Services.AddScoped<ITipoUsuarioRepository, TipoUsuarioRepository>();
builder.Services.AddScoped<IFamiliaCaixilhoRepository, FamiliaCaixilhoRepository>();
builder.Services.AddScoped<IObraRepository, ObraRepository>();
builder.Services.AddScoped<IProducaoRepository, ProducaoRepository>();


// ======================================================
// Autentica��o com COOKIES
// ======================================================
builder.Services.AddAuthentication("GerenciadorProd")
    .AddCookie("GerenciadorProd", options =>
    {
        options.LoginPath = "/Usuario/Login";             
        options.AccessDeniedPath = "/Usuario/AcessoNegado"; 
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); 
        options.SlidingExpiration = true;                  
    });




// Add services to the container.
builder.Services.AddControllersWithViews();

// Configuração de cultura para datas
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "pt-BR", "en-US" };
    options.SetDefaultCulture("pt-BR")
           .AddSupportedCultures(supportedCultures)
           .AddSupportedUICultures(supportedCultures);
});

var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}


app.UseRouting();
app.UseHttpMethodOverride();
app.UseRequestLocalization();
app.UseAuthentication();
app.UseAuthorization();


app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
