using M241.Server;
using M241.Server.Components;
using M241.Server.Data;
using M241.Server.Data.Models;
using M241.Server.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Radzen;
using System.Net.WebSockets;

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

builder.Services.AddIdentityApiEndpoints<AppUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AeroSenseDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddTransient<AeroSenseDbContext>();
builder.Services.AddHostedService<MqttService>();
builder.Services.AddHealthChecks();
string frontendUrl = builder.Configuration["FrontendUrl"] ??
    throw new ArgumentException("Missing frontend url in appsettings.");


builder.Services.AddCors(options => options.AddPolicy("SPA", policy => policy
    .WithOrigins(frontendUrl)
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (true /*app.Environment.IsDevelopment()*/)
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "M241"));
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AeroSenseDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        await SeedData.SeedDb(db, userManager, roleManager);
        await db.Database.MigrateAsync();
    }
}

app.UseAuthorization();
app.UseAuthentication();
app.UseCors("SPA");

app.MapControllers();
app.MapGroup("/api")
    .MapIdentityApi<AppUser>();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};
webSocketOptions.AllowedOrigins.Add(frontendUrl);

app.UseWebSockets(webSocketOptions);

app.MapHealthChecks("/healthz");

app.Run();
