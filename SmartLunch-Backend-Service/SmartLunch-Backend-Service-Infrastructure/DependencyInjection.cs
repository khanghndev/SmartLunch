using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace SmartLunch.Backend.Service.Infrastructure.DependencyInjection
{
    /// <summary>
    /// Dependency injection configuration for Infrastructure layer
    /// </summary>
    public class DependencyInjection
    {
        /// <summary>
        /// Configures and registers all Infrastructure services using Scrutor
        /// </summary>
        /// <param name="services">The service collection to register services with</param>
        public static void ConfigureServices(IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;

            // Scan and register all classes that implement interfaces
            // This will register: Repositories, Services, Adapters, etc.
            services.Scan(scan => scan
                .FromAssemblies(assembly)
                .AddClasses(classes => classes
                    .Where(type =>
                        type.IsClass &&
                        !type.IsAbstract &&
                        !type.IsGenericTypeDefinition &&
                        !type.IsSealed && // Exclude static classes (sealed classes without instance constructors)
                        !type.Name.EndsWith("Options") && // Exclude options classes
                        !type.Name.EndsWith("Extensions") && // Exclude extension classes
                        !type.Name.EndsWith("Context"))) // Exclude DbContext (registered separately)
                .UsingRegistrationStrategy(RegistrationStrategy.Append)
                .AsMatchingInterface() // Only register classes that have a matching interface (e.g., IUserRepository -> UserRepository)
                .WithScopedLifetime());
        }
    }
}
