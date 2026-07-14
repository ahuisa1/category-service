using CategoryService.Data;
using CategoryService.Repositories;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var cosmosSettings = builder.Configuration.GetSection("CosmosDb");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseCosmos(
        cosmosSettings["EndpointUri"]!,
        cosmosSettings["PrimaryKey"]!,
        cosmosSettings["DatabaseName"]!,
        cosmosOptions =>
        {
            // 1. Forzar a usar SOLO localhost y no la IP interna de Docker
            cosmosOptions.LimitToEndpoint(true);

            // 2. Usar el modo Gateway, ideal para evitar bloqueos de puertos en local
            cosmosOptions.ConnectionMode(Microsoft.Azure.Cosmos.ConnectionMode.Gateway);
            // Esta configuración le dice a .NET que ignore el error de SSL del emulador local
            cosmosOptions.HttpClientFactory(() =>
            {
                var httpMessageHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
                return new HttpClient(httpMessageHandler);
            });
        }
    ));


var app = builder.Build();

// Crear la base de datos y contenedores automáticamente si no existen
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // EnsureCreated le dice a Cosmos DB: "Si no existe CategoryDb o Categories, créalos ahora"
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
