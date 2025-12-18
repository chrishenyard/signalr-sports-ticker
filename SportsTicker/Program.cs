using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using SportsTicker;
using SportsTicker.Hubs;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel(builder =>
{
    builder.AddServerHeader = false;
});

builder.Host
    .UseDefaultServiceProvider((context, options) =>
    {
        options.ValidateOnBuild = true;
    })
    .UseSerilog((context, configuration) =>
    {
        configuration
            .ReadFrom.Configuration(context.Configuration);
    });

var configuration = builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.AddOptions();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.KeepAliveInterval = TimeSpan.FromSeconds(5);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(5);
})
.AddNewtonsoftJsonProtocol();
builder.Services.AddServices();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.UseExceptionHandler();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();
app.MapHub<SportsTickerHub>("/sports-ticker");

app.MapControllerRoute(
    name: "default",
    pattern: "Index");

await app.RunAsync();
