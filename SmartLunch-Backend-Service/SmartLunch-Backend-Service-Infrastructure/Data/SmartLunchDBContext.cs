using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Infrastructure.Data
{
    /// <summary>
    /// SmartLunchDBContext is the main database context for the SmartLunch application.
    /// </summary>
    public class SmartLunchDBContext : DbContext
    {
        private readonly ILogger<SmartLunchDBContext> _logger;

        // Logger can be injected via service locator pattern if needed, but is optional
        public SmartLunchDBContext(
            DbContextOptions<SmartLunchDBContext> options,
            ILogger<SmartLunchDBContext> logger) : base(options)
        {
            _logger = logger;
        }

        #region DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }
        public DbSet<MediaFile> MediaFiles { get; set; }
        #endregion

        #region Utilities
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
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

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.IsActive);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(500);
                entity.Property(e => e.FirstName).HasMaxLength(100);
                entity.Property(e => e.LastName).HasMaxLength(100);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.Provider).IsRequired().HasMaxLength(50).HasDefaultValue("system");
            });

            // Configure Role entity
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("roles");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasIndex(e => e.IsActive);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
            });

            // Configure Permission entity
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("permissions");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasIndex(e => e.Resource);
                entity.HasIndex(e => e.Action);
                entity.HasIndex(e => e.IsActive);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Resource).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Action).IsRequired().HasMaxLength(50);
            });

            // Configure UserRole entity
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("user_roles");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.UserId, e.RoleId }).IsUnique();
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.RoleId);
                entity.HasIndex(e => e.IsActive);
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure UserPermission entity
            modelBuilder.Entity<UserPermission>(entity =>
            {
                entity.ToTable("user_permissions");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.UserId, e.PermissionId }).IsUnique();
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.PermissionId);
                entity.HasIndex(e => e.IsActive);
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(up => up.User)
                    .WithMany(u => u.UserPermissions)
                    .HasForeignKey(up => up.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(up => up.Permission)
                    .WithMany(p => p.UserPermissions)
                    .HasForeignKey(up => up.PermissionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure RolePermission entity
            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.ToTable("role_permissions");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.RoleId, e.PermissionId }).IsUnique();
                entity.HasIndex(e => e.RoleId);
                entity.HasIndex(e => e.PermissionId);
                entity.HasIndex(e => e.IsActive);
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(rp => rp.Role)
                    .WithMany(r => r.RolePermissions)
                    .HasForeignKey(rp => rp.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(rp => rp.Permission)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(rp => rp.PermissionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure UserToken entity
            modelBuilder.Entity<UserToken>(entity =>
            {
                entity.ToTable("user_tokens");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.AccessToken);
                entity.HasIndex(e => e.RefreshToken);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.ExpiresAt);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.AccessToken).IsRequired().HasMaxLength(2000);
                entity.Property(e => e.RefreshToken).IsRequired().HasMaxLength(2000);
                entity.Property(e => e.ReplacedByToken).HasMaxLength(2000);

                entity.HasOne(ut => ut.User)
                    .WithMany(u => u.UserTokens)
                    .HasForeignKey(ut => ut.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure MediaFile entity
            modelBuilder.Entity<MediaFile>(entity =>
            {
                entity.ToTable("media_files");
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.OwnerUserId);
                entity.HasIndex(e => new { e.OwnerUserId, e.CreatedAt });
                entity.HasIndex(e => e.MediaType);
                entity.HasIndex(e => new { e.Bucket, e.ObjectName }).IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.OwnerUserId).IsRequired();
                entity.Property(e => e.Bucket).IsRequired().HasMaxLength(255);
                entity.Property(e => e.ObjectName).IsRequired().HasMaxLength(1024);
                entity.Property(e => e.OriginalFileName).HasMaxLength(255);
                entity.Property(e => e.ContentType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.MediaType).IsRequired().HasMaxLength(20).HasDefaultValue("unknown");
                entity.Property(e => e.Md5HashBase64).HasMaxLength(128);

                entity.HasOne(e => e.OwnerUser)
                    .WithMany()
                    .HasForeignKey(e => e.OwnerUserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
        #endregion
    }
}