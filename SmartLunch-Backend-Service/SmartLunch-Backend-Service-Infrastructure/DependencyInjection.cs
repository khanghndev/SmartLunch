using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Infrastructure.ExternalServices;
using SmartLunch.Backend.Service.Infrastructure.Pdf;
using SmartLunch.Backend.Service.Infrastructure.Services;

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
            // ICacheService -> RedisCacheService (excluded from Scrutor scan by "Service" suffix)
            services.AddScoped<ICacheService, RedisCacheService>();
            services.AddScoped<ICartCacheService, CartCacheService>();
            services.AddScoped<IOrganizationMealOrderDraftCache, OrganizationMealOrderDraftCacheService>();
            services.AddScoped<IStorageService, AppwriteStorageService>();
            services.AddScoped<IContractPdfService, QuestPdfContractFileService>();
            services.AddScoped<IOrganizationMealDocumentPdfService, QuestPdfOrganizationMealDocumentService>();
            services.AddScoped<IOrderAnnexPdfService, QuestPdfOrderAnnexPdfService>();

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
                        type.Name != "FirebaseStorageService" && // Use AppwriteStorageService for IFirebaseStorageService
                        type.Name != "CartCacheService" && // Registered explicitly as ICartCacheService
                        type.Name != "OrganizationMealOrderDraftCacheService" && // Registered explicitly as IOrganizationMealOrderDraftCache
                        !type.Name.EndsWith("Options") && // Exclude options classes
                        !type.Name.EndsWith("Extensions") && // Exclude extension classes
                        !type.Name.EndsWith("Context") && // Exclude DbContext (registered separately)
                        !type.Name.EndsWith("Adapter")))// Exclude message broker adapter (IEventPublisher from Shared is used)
                .UsingRegistrationStrategy(RegistrationStrategy.Append)
                .AsMatchingInterface() // Only register classes that have a matching interface (e.g., IUserRepository -> UserRepository)
                .WithScopedLifetime());
        }
    }
}
