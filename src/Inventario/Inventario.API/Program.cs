using Inventario.Application.Interfaces;
using Inventario.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<InventarioDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IInventarioService, InventarioService>();
builder.Services.AddScoped<ICompraService, CompraService>();

var productoApiUrl = builder.Configuration["Apis:Producto"];

builder.Services.AddHttpClient<IProductoApiClient, ProductoApiClient>(client =>
{
    client.BaseAddress = new Uri(productoApiUrl!);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<InventarioDbContext>();
    context.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();
app.Run();
