using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.OrganizationChatbot;
using SmartLunch.Backend.Service.Application.OrganizationMealOrders;
using SmartLunch.Backend.Service.Application.Services;

namespace SmartLunch.Backend.Service.Application.DependencyInjection
{
    /// <summary>
    /// Dependency injection configuration for Application layer
    /// </summary>
    public class DependencyInjection
    {
        /// <summary>
        /// Configures and registers all Application services using Scrutor
        /// </summary>
        /// <param name="services">The service collection to register services with</param>
        public static void ConfigureServices(IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;

            // Scan and register all classes that implement interfaces
            // This will register: Services (JwtService, PasswordHasher), etc.
            // Note: Command/Query Handlers are registered by MediatR, not here
            services.Scan(scan => scan
                .FromAssemblies(assembly)
                .AddClasses(classes => classes
                    .Where(type =>
                        type.IsClass &&
                        !type.IsAbstract &&
                        !type.IsGenericTypeDefinition &&
                        !type.IsSealed && // Exclude static classes
                        !type.Name.EndsWith("Options") && // Exclude options classes
                        !type.Name.EndsWith("Extensions") && // Exclude extension classes
                        !type.Name.EndsWith("Profile") && // Exclude AutoMapper profiles
                        !type.Name.EndsWith("Command") && // Exclude command classes (handled by MediatR)
                        !type.Name.EndsWith("Query") && // Exclude query classes (handled by MediatR)
                        !type.Name.EndsWith("Handler") && // Exclude handlers (registered by MediatR)
                        !type.Name.EndsWith("Request") && // Exclude request DTOs
                        !type.Name.EndsWith("Response") && // Exclude response DTOs
                        !type.Name.EndsWith("Dto"))) // Exclude DTO classes
                .UsingRegistrationStrategy(RegistrationStrategy.Append)
                .AsMatchingInterface() // Only register classes that have a matching interface (e.g., IJwtService -> JwtService)
                .WithScopedLifetime());

            // sealed classes are excluded by the scan above
            services.AddScoped<IPromotionEngine, PromotionEngine>();
            services.AddScoped<OrganizationMealContractDraftPersistence>();
            services.AddScoped<OrganizationMealContractOrders.OrganizationMealPeriodContractPersistence>();
            services.AddScoped<OrganizationMealContractOrders.OrganizationMealContractWeeklyJobService>();
            services.AddScoped<OrganizationComplaints.ComplaintRefundRecorder>();
            services.AddScoped<OrganizationComplaints.ComplaintNotificationService>();
            services.AddScoped<Deliveries.DeliveryNotificationService>();
            services.AddScoped<Integration.Email.ComplaintEmailService>();
            services.AddScoped<Integration.Email.DeliveryEmailService>();
            services.AddScoped<IOrganizationChatbotContextBuilder, OrganizationChatbotContextBuilder>();
            services.AddScoped<OrganizationChatbotRuleFallback>();
            services.AddScoped<IOrganizationChatbotService, OrganizationChatbotService>();
        }
    }
}