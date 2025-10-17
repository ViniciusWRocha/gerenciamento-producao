using GerenciamentoProducao.Data;
using GerenciamentoProducao.Interfaces;
using GerenciamentoProducao.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddControllers();

// DB
builder.Services.AddDbContext<GerenciamentoProdDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Repositórios
builder.Services.AddScoped<IObraRepository, ObraRepository>();
builder.Services.AddScoped<ICaixilhoRepository, CaixilhoRepository>();


//configuração de CORS
//Não esquecer de colocar enavledCors nas controllers 
builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddSwaggerGen();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//ativar o Cors
app.UseCors("MyPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
