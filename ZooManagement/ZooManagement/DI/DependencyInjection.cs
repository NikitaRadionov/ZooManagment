using Application.Services;
using Domain.Interfaces;
using Infrastructure.Events;
using Infrastructure.Repositories;
using Infrastructure.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<AnimalTransferService>();
        services.AddScoped<FeedingOrganizationService>();
        services.AddScoped<ZooStatisticsService>();

        return services;
    }
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IAnimalRepository, InMemoryAnimalRepository>();
        services.AddSingleton<IEnclosureRepository, InMemoryEnclosureRepository>();
        services.AddSingleton<IFeedingScheduleRepository, InMemoryFeedingScheduleRepository>();

        services.AddSingleton<IDomainEventDispatcher, ConsoleEventDispatcher>();

        services.AddSingleton<IHostedService>(provider =>
            new FeedingSchedulerService(
                provider.GetRequiredService<IFeedingScheduleRepository>(),
                provider.GetRequiredService<IDomainEventDispatcher>()));

        return services;
    }


}