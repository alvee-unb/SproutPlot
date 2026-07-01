using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SproutPlot.Application.Common.Interfaces;
using SproutPlot.Application.Features.Calendar;
using SproutPlot.Application.Features.Gardens;
using SproutPlot.Application.Features.Notifications;
using SproutPlot.Application.Features.Plants;
using SproutPlot.Application.Features.Tasks;
using SproutPlot.Application.Features.Watering;
using SproutPlot.Application.Features.Weather;

namespace SproutPlot.Application;

/// <summary>Registers Application-layer services into the DI container.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.TryAddSingleton(TimeProvider.System);

        services.AddScoped<IGardenService, GardenService>();
        services.AddScoped<IPlantService, PlantService>();
        services.AddScoped<IPlantTypeService, PlantTypeService>();
        services.AddScoped<IWeatherService, WeatherService>();
        services.AddScoped<IWateringService, WateringService>();
        services.AddScoped<IGardenTaskService, GardenTaskService>();
        services.AddScoped<ICalendarService, CalendarService>();
        services.AddScoped<INotificationService, NotificationService>();

        return services;
    }
}
