using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SportsTicker.Settings;

namespace SportsTicker
{
    static public class OptionsConfiguration
    {
        static public void AddOptions(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IGameSettings>(serviceProvider =>
            {
                var options = builder.Configuration
                    .GetSection(GameSettings.Section)
                    .Get<GameSettings>();
                return options!;
            });
        }
    }
}
