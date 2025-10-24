using Microsoft.Extensions.DependencyInjection;
using SportsTicker.Repositories;
using SportsTicker.Services;

namespace SportsTicker;

public static class ServicesConfiguration
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddSingleton<ITickerDAL, TickerDAL>();
        services.AddSingleton<ITickerFileRepository, TickerFileRepository>();
        services.AddSingleton<ITicker, Ticker>();
        services.AddSingleton<IGameManager, GameManager>();

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed(origin =>
                    {
                        return origin.Contains("localhost");
                    }));
        });
    }
}
