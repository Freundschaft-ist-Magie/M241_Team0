using M241.Server;
using M241.Server.Components;
using M241.Server.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddRadzenComponents();

var connectionstring = builder.Configuration.GetConnectionString("Default") ?? throw new Exception("No connectionstring provied");
builder.Services.AddDbContextFactory<AeroSenseDbContext>(opt =>
    opt.UseNpgsql(connectionstring));

builder.Services.AddTransient<AeroSenseDbContext>();
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (true /*app.Environment.IsDevelopment()*/)
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "M241"));
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AeroSenseDbContext>();
        await SeedData.SeedDb(db);
        await db.Database.MigrateAsync();
    }
}

app.UseAuthorization();

app.MapControllers();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapHealthChecks("/healthz");

app.Run();
