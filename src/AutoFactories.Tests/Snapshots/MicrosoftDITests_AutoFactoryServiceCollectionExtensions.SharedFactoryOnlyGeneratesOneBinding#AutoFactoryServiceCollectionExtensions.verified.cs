// -----------------------------| Notes |-----------------------------
// 1. Two classes are sharing the same factory, we need to valdiate they only bind once
// -------------------------------------------------------------------
#pragma warning disable CS8019 // Unnecessary using directive.
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides functionality for automatically binding factory interfaces to their corresponding implementations.
/// This class is part of the AutoFactories library, which simplifies the registration of factories for dependency injection
/// by using the [AutoFactory] attribute to identify and register factory classes dynamically.
/// </summary>
public static partial class AutoFactoryServiceCollectionExtensions
{
    /// <summary>
    /// Registers factory interfaces to their corresponding implementations.
    /// This registration process occurs for each class decorated with the [AutoFactory] attribute,
    /// ensuring that the appropriate factory is bound to its interface for dependency injection or service resolution.
    /// </summary>
    public static Microsoft.Extensions.DependencyInjection.IServiceCollection AddAutoFactories(this Microsoft.Extensions.DependencyInjection.IServiceCollection services)
    {
        services.AddTransient<IAnimalFactory, AnimalFactory>();
        return services;
    }
}