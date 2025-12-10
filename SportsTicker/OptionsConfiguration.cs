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
            builder.Services.AddOptions<GameSettings>()
                .Bind(builder.Configuration.GetSection(GameSettings.Section))
                .ValidateDataAnnotations();
        }
    }
}
