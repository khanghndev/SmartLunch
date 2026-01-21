using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace SmartLunch.Backend.Service.Infrastructure.Data
{
    /// <summary>
    /// SmartLunchDBContext is the main database context for the SmartLunch application.
    /// </summary>
    public class SmartLunchDBContext : DbContext
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;
        private readonly ILogger<SmartLunchDBContext>? _logger;

        public SmartLunchDBContext(DbContextOptions<SmartLunchDBContext> options) : base(options)
        {
        }

        public SmartLunchDBContext(
            DbContextOptions<SmartLunchDBContext> options,
            IHttpContextAccessor httpContextAccessor,
            ILogger<SmartLunchDBContext> logger) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        #region DbSets

        #endregion

        #region Utilities
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var currentUserId = GetCurrentUserId();
            var timestamp = DateTime.UtcNow;

            try
            {
                var result = await base.SaveChangesAsync(cancellationToken);
                _logger?.LogInformation("SaveChanges completed successfully. {EntityCount} entities affected.", result);
                return result;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error occurred during SaveChanges");
                throw;
            }
        }

        public override int SaveChanges()
        {
            var currentUserId = GetCurrentUserId();
            var timestamp = DateTime.UtcNow;

            try
            {
                var result = base.SaveChanges();
                _logger?.LogInformation("SaveChanges completed successfully. {EntityCount} entities affected.", result);
                return result;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error occurred during SaveChanges");
                throw;
            }
        }

        private long? GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return long.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        private static void SetProperty(object entity, string propertyName, object? value)
        {
            var property = entity.GetType().GetProperty(propertyName);
            if (property != null && property.CanWrite)
            {
                property.SetValue(entity, value);
            }
        }

        private static bool HasProperty(object entity, string propertyName)
        {
            return entity.GetType().GetProperty(propertyName) != null;
        }

        private static object? GetEntityId(object entity)
        {
            var idProperty = entity.GetType().GetProperty("Id") ??
                            entity.GetType().GetProperties().FirstOrDefault(p => p.Name.EndsWith("Id"));
            return idProperty?.GetValue(entity);
        }

        public IQueryable<T> IncludeSoftDeleted<T>() where T : class
        {
            return Set<T>().IgnoreQueryFilters();
        }

        public IQueryable<T> OnlySoftDeleted<T>() where T : class
        {
            var entityType = typeof(T);
            var instance = Activator.CreateInstance<T>();
            if (instance != null && HasProperty(instance, "IsActive"))
            {
                return Set<T>().IgnoreQueryFilters().Where(e => EF.Property<bool?>(e, "IsActive") == false);
            }
            return Set<T>().Where(e => false); // Return empty if no IsActive property
        }

        public async Task<int> BulkDeleteAsync<T>(IEnumerable<T> entities) where T : class
        {
            Set<T>().RemoveRange(entities);
            return await SaveChangesAsync();
        }

        public async Task<int> BulkInsertAsync<T>(IEnumerable<T> entities) where T : class
        {
            await Set<T>().AddRangeAsync(entities);
            return await SaveChangesAsync();
        }

        public async Task<int> BulkUpdateAsync<T>(IEnumerable<T> entities) where T : class
        {
            Set<T>().UpdateRange(entities);
            return await SaveChangesAsync();
        }
        #endregion

        #region Model Creating
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        #endregion
    }
}