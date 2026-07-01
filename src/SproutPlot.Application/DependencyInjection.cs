using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SproutPlot.Application.Common.Interfaces;
using SproutPlot.Application.Features.Gardens;
using SproutPlot.Application.Features.Plants;

namespace SproutPlot.Application;

/// <summary>Registers Application-layer services into the DI container.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<IGardenService, GardenService>();
        services.AddScoped<IPlantService, PlantService>();
        services.AddScoped<IPlantTypeService, PlantTypeService>();

        return services;
    }
}
