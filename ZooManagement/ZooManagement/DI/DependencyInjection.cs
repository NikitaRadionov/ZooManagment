using Application.Services;
using Domain.Interfaces;
using Infrastructure.Events;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

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

        services.AddScoped<IDomainEventDispatcher, ConsoleEventDispatcher>();

        return services;
    }

}