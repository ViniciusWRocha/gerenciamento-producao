using GerenciamentoProducao.Data;
using GerenciamentoProducao.Interfaces;
using GerenciamentoProducao.Repositories;
using Microsoft.EntityFrameworkCore;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<GerenciamentoProdDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<IObraRepository, ObraRepository>();
builder.Services.AddScoped<ICaixilhoRepository, CaixilhoRepository>();

builder.Services.AddCors(o => o.AddPolicy("MyPolicy", policy =>
{
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader();
}));

var credentialsPath = Path.Combine(builder.Environment.ContentRootPath, "Credentials", "credentials-service.json");
var appName = "Gerenciador de Producao";

var calendarService = new GoogleCalendarService(credentialsPath, appName);
builder.Services.AddSingleton(calendarService);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("MyPolicy");
app.UseAuthorization();
app.MapControllers();
app.Run();
