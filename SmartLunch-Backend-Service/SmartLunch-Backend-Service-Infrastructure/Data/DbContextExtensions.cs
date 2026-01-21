using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace SmartLunch.Backend.Service.Infrastructure.Data
{
    public static class DbContextExtensions
    {
        /// <summary>
        /// Adds or updates an entity based on a key selector
        /// </summary>
        public static async Task<T> AddOrUpdateAsync<T>(this DbContext context, T entity, Expression<Func<T, object>> keySelector) where T : class
        {
            var dbSet = context.Set<T>();
            var key = keySelector.Compile()(entity);
            var existing = await dbSet.FirstOrDefaultAsync(CreateEqualityExpression(keySelector, key));

            if (existing != null)
            {
                context.Entry(existing).CurrentValues.SetValues(entity);
                return existing;
            }
            else
            {
                await dbSet.AddAsync(entity);
                return entity;
            }
        }

        /// <summary>
        /// Executes a raw SQL query and returns the results
        /// </summary>
        public static async Task<List<T>> ExecuteQueryAsync<T>(this DbContext context, string sql, params object[] parameters) where T : class
        {
            return await context.Set<T>().FromSqlRaw(sql, parameters).ToListAsync();
        }

        /// <summary>
        /// Executes a raw SQL command
        /// </summary>
        public static async Task<int> ExecuteCommandAsync(this DbContext context, string sql, params object[] parameters)
        {
            return await context.Database.ExecuteSqlRawAsync(sql, parameters);
        }

        /// <summary>
        /// Gets the connection string being used by the context
        /// </summary>
        public static string GetConnectionString(this DbContext context)
        {
            return context.Database.GetConnectionString() ?? string.Empty;
        }

        /// <summary>
        /// Checks if the database exists
        /// </summary>
        public static async Task<bool> DatabaseExistsAsync(this DbContext context)
        {
            return await context.Database.CanConnectAsync();
        }

        /// <summary>
        /// Gets pending migrations
        /// </summary>
        public static async Task<IEnumerable<string>> GetPendingMigrationsAsync(this DbContext context)
        {
            return await context.Database.GetPendingMigrationsAsync();
        }

        /// <summary>
        /// Applies pending migrations
        /// </summary>
        public static async Task MigrateDatabaseAsync(this DbContext context)
        {
            await context.Database.MigrateAsync();
        }

        /// <summary>
        /// Reloads an entity from the database
        /// </summary>
        public static async Task ReloadAsync<T>(this DbContext context, T entity) where T : class
        {
            await context.Entry(entity).ReloadAsync();
        }

        /// <summary>
        /// Detaches all entities from the context
        /// </summary>
        public static void DetachAllEntities(this DbContext context)
        {
            var entries = context.ChangeTracker.Entries().ToList();
            foreach (var entry in entries)
            {
                entry.State = EntityState.Detached;
            }
        }

        /// <summary>
        /// Gets all modified entities
        /// </summary>
        public static IEnumerable<object> GetModifiedEntities(this DbContext context)
        {
            return context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Modified)
                .Select(e => e.Entity);
        }

        /// <summary>
        /// Gets all added entities
        /// </summary>
        public static IEnumerable<object> GetAddedEntities(this DbContext context)
        {
            return context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added)
                .Select(e => e.Entity);
        }

        /// <summary>
        /// Gets all deleted entities
        /// </summary>
        public static IEnumerable<object> GetDeletedEntities(this DbContext context)
        {
            return context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Deleted)
                .Select(e => e.Entity);
        }

        /// <summary>
        /// Performs a soft delete on an entity
        /// </summary>
        public static void SoftDelete<T>(this DbContext context, T entity) where T : class
        {
            var entry = context.Entry(entity);
            var deletedAtProperty = entry.Property("DeletedAt");

            if (deletedAtProperty != null)
            {
                deletedAtProperty.CurrentValue = DateTime.UtcNow;
                entry.State = EntityState.Modified;
            }
            else
            {
                context.Set<T>().Remove(entity);
            }
        }

        /// <summary>
        /// Restores a soft deleted entity
        /// </summary>
        public static void RestoreSoftDeleted<T>(this DbContext context, T entity) where T : class
        {
            var entry = context.Entry(entity);
            var deletedAtProperty = entry.Property("DeletedAt");

            if (deletedAtProperty != null)
            {
                deletedAtProperty.CurrentValue = null;
                entry.State = EntityState.Modified;
            }
        }

        private static Expression<Func<T, bool>> CreateEqualityExpression<T>(Expression<Func<T, object>> keySelector, object key)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            MemberExpression propertyAccess;

            // Handle both UnaryExpression (boxing) and direct MemberExpression
            if (keySelector.Body is UnaryExpression unary && unary.Operand is MemberExpression member)
            {
                propertyAccess = Expression.MakeMemberAccess(parameter, member.Member);
            }
            else if (keySelector.Body is MemberExpression directMember)
            {
                propertyAccess = Expression.MakeMemberAccess(parameter, directMember.Member);
            }
            else
            {
                throw new ArgumentException("Key selector must access a property or field", nameof(keySelector));
            }

            var constant = Expression.Constant(key, propertyAccess.Type);
            var equality = Expression.Equal(propertyAccess, constant);
            return Expression.Lambda<Func<T, bool>>(equality, parameter);
        }
    }

    /// <summary>
    /// Query helper for building complex queries
    /// </summary>
    public static class QueryHelper
    {
        public static IQueryable<T> ApplyFiltering<T>(this IQueryable<T> query, string? searchTerm, params Expression<Func<T, string>>[] searchProperties)
        {
            if (string.IsNullOrEmpty(searchTerm) || !searchProperties.Any())
                return query;

            var parameter = Expression.Parameter(typeof(T), "x");
            Expression? filterExpression = null;
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var searchConstant = Expression.Constant(searchTerm, typeof(string));

            foreach (var propertyExpr in searchProperties)
            {
                // Rebuild the property access expression using the new parameter
                // Replace the parameter in the original expression with our new parameter
                var visitor = new ParameterReplacer(propertyExpr.Parameters[0], parameter);
                var propertyAccess = visitor.Visit(propertyExpr.Body);

                // Handle nullable strings by using null-coalescing operator
                // This ensures Contains() doesn't throw on null values
                if (propertyAccess.Type != typeof(string))
                {
                    // If it's a nullable value type, unwrap it
                    var underlyingType = Nullable.GetUnderlyingType(propertyAccess.Type);
                    if (underlyingType == typeof(string))
                    {
                        propertyAccess = Expression.Coalesce(propertyAccess, Expression.Constant(string.Empty));
                    }
                }
                else
                {
                    // For nullable reference types (string?), add null-coalescing for safety
                    // Note: At runtime, string? is still typeof(string), so we add protection
                    propertyAccess = Expression.Coalesce(propertyAccess, Expression.Constant(string.Empty));
                }

                var containsCall = Expression.Call(propertyAccess, containsMethod!, searchConstant);
                filterExpression = filterExpression == null ? containsCall : Expression.OrElse(filterExpression, containsCall);
            }

            if (filterExpression != null)
            {
                var lambda = Expression.Lambda<Func<T, bool>>(filterExpression, parameter);
                query = query.Where(lambda);
            }

            return query;
        }

        /// <summary>
        /// Helper class to replace parameters in expression trees
        /// </summary>
        private class ParameterReplacer : ExpressionVisitor
        {
            private readonly ParameterExpression _oldParameter;
            private readonly ParameterExpression _newParameter;

            public ParameterReplacer(ParameterExpression oldParameter, ParameterExpression newParameter)
            {
                _oldParameter = oldParameter;
                _newParameter = newParameter;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == _oldParameter ? _newParameter : base.VisitParameter(node);
            }
        }

        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, string? sortBy, bool sortDescending, Dictionary<string, Expression<Func<T, object>>>? sortExpressions = null)
        {
            if (string.IsNullOrEmpty(sortBy) || sortExpressions == null || !sortExpressions.ContainsKey(sortBy.ToLower()))
                return query;

            var sortExpression = sortExpressions[sortBy.ToLower()];
            return sortDescending ? query.OrderByDescending(sortExpression) : query.OrderBy(sortExpression);
        }

        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, int page, int pageSize)
        {
            return query.Skip((page - 1) * pageSize).Take(pageSize);
        }
    }

    /// <summary>
    /// Database health check and monitoring
    /// </summary>
    public class DatabaseHealthService
    {
        private readonly RhetorAIServiceDBContext _context;
        private readonly ILogger<DatabaseHealthService> _logger;

        public DatabaseHealthService(RhetorAIServiceDBContext context, ILogger<DatabaseHealthService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DatabaseHealthInfo> CheckHealthAsync()
        {
            var healthInfo = new DatabaseHealthInfo();
            var startTime = DateTime.UtcNow;

            try
            {
                // Test database connectivity
                healthInfo.IsConnected = await _context.Database.CanConnectAsync();

                if (healthInfo.IsConnected)
                {
                    // Check if database is up to date
                    var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
                    healthInfo.HasPendingMigrations = pendingMigrations.Any();
                    healthInfo.PendingMigrations = pendingMigrations.ToList();

                    // Performance test - simple query
                    var testStart = DateTime.UtcNow;
                    await _context.Roles.CountAsync();
                    healthInfo.QueryResponseTime = DateTime.UtcNow - testStart;

                    // Get table counts
                    healthInfo.TableCounts = new Dictionary<string, int>
                    {
                        ["Users"] = await _context.Users.CountAsync(),
                        ["Roles"] = await _context.Roles.CountAsync(),
                        ["Topics"] = await _context.Topics.CountAsync(),
                        ["Debates"] = await _context.Debates.CountAsync(),
                        ["Messages"] = await _context.Messages.CountAsync(),
                        ["Articles"] = await _context.Articles.CountAsync(),
                        ["Evaluations"] = await _context.Evaluations.CountAsync()
                    };
                }

                healthInfo.Status = healthInfo.IsConnected ? "Healthy" : "Unhealthy";
                healthInfo.CheckedAt = DateTime.UtcNow;
                healthInfo.TotalCheckTime = DateTime.UtcNow - startTime;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database health check failed");
                healthInfo.Status = "Unhealthy";
                healthInfo.Error = ex.Message;
                healthInfo.IsConnected = false;
            }

            return healthInfo;
        }
    }

    public class DatabaseHealthInfo
    {
        public string Status { get; set; } = "Unknown";
        public bool IsConnected { get; set; }
        public bool HasPendingMigrations { get; set; }
        public List<string> PendingMigrations { get; set; } = new();
        public TimeSpan QueryResponseTime { get; set; }
        public Dictionary<string, int> TableCounts { get; set; } = new();
        public DateTime CheckedAt { get; set; }
        public TimeSpan TotalCheckTime { get; set; }
        public string? Error { get; set; }
    }
}