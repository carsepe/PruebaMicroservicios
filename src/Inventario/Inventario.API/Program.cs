using Inventario.API.Common.Http;
using Inventario.API.Common.Middleware;
using Inventario.Application.Interfaces;
using Inventario.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<InventarioDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IInventarioService, InventarioService>();
builder.Services.AddScoped<ICompraService, CompraService>();

var productoApiUrl = builder.Configuration["Apis:Producto"];
var apiKey = builder.Configuration["ApiKey"];

builder.Services.AddHttpClient<IProductoApiClient, ProductoApiClient>(client =>
{
    client.BaseAddress = new Uri(productoApiUrl!);
    client.DefaultRequestHeaders.Add("X-API-KEY", apiKey!);
})
.AddPolicyHandler(PollyPolicies.GetRetryPolicy())
.AddPolicyHandler(PollyPolicies.GetTimeoutPolicy());

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);

    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "Clave de autenticación vía encabezado `X-API-KEY`",
        Type = SecuritySchemeType.ApiKey,
        Name = "X-API-KEY",
        In = ParameterLocation.Header,
        Scheme = "ApiKeyScheme"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<InventarioDbContext>();
    context.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<ApiKeyMiddleware>();

app.UseAuthorization();
app.MapControllers();
app.Run();
