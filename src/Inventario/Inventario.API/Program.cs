using Inventario.Application.Interfaces;
using Inventario.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<InventarioDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IInventarioService, InventarioService>();
builder.Services.AddScoped<ICompraService, CompraService>();


builder.Services.AddHttpClient<IProductoApiClient, ProductoApiClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7109"); 
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();
app.Run();
