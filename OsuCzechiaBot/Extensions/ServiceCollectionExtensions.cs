using OsuCzechiaBot.Jobs.OneTime.Base;

namespace OsuCzechiaBot.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOneTimeJob<T>(this IServiceCollection services) where T : class, IOneTimeJob
    {
        services.AddScoped<IOneTimeJob, T>();
        return services;
    }
}