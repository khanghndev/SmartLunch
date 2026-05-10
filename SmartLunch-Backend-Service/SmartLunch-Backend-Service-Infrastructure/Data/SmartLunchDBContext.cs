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
        public DbSet<SystemLog> SystemLogs { get; set; }
        public DbSet<SystemBackup> SystemBackups { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }
        public DbSet<MediaFile> MediaFiles { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<UserOrganization> UserOrganizations { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<PartnerDocument> PartnerDocuments { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<PartnerPayment> PartnerPayments { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<IngredientSource> IngredientSources { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<InternalStockIssue> InternalStockIssues { get; set; }
        public DbSet<InternalStockIssueLine> InternalStockIssueLines { get; set; }
        public DbSet<IngredientIntakeProposal> IngredientIntakeProposals { get; set; }
        public DbSet<IngredientIntakeProposalLine> IngredientIntakeProposalLines { get; set; }
        public DbSet<IngredientActualIntake> IngredientActualIntakes { get; set; }
        public DbSet<IngredientActualIntakeLine> IngredientActualIntakeLines { get; set; }
        public DbSet<CookingMethod> CookingMethods { get; set; }
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<DishCategory> DishCategories { get; set; }
        public DbSet<DishDishCategory> DishDishCategories { get; set; }
        public DbSet<DishImage> DishImages { get; set; }
        public DbSet<DishIngredient> DishIngredients { get; set; }
        public DbSet<WeeklyMenu> WeeklyMenus { get; set; }
        public DbSet<WeeklyMenuImage> WeeklyMenuImages { get; set; }
        public DbSet<CustomerType> CustomerTypes { get; set; }
        public DbSet<MenuSchedule> MenuSchedules { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Sentiment> Sentiments { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<ChatbotLog> ChatbotLogs { get; set; }
        public DbSet<MenuSuggestion> MenuSuggestions { get; set; }
        public DbSet<MenuSuggestionPlan> MenuSuggestionPlans { get; set; }
        public DbSet<MenuSuggestionPlanDay> MenuSuggestionPlanDays { get; set; }
        public DbSet<MenuSuggestionPlanItem> MenuSuggestionPlanItems { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Recruitment> Recruitment { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<IngredientCategory> IngredientCategories { get; set; }
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

            modelBuilder.Entity<SystemLog>(entity =>
            {
                entity.ToTable("system_logs");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .IsRequired()
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Timestamp);

                entity.Property(e => e.Level)
                    .HasMaxLength(50);

                entity.Property(e => e.Message)
                    .HasColumnType("text");

                entity.Property(e => e.Template)
                    .HasColumnType("text");

                entity.Property(e => e.Exception)
                    .HasColumnType("text");

                entity.Property(e => e.Properties)
                    .HasColumnType("text");
            });

            modelBuilder.Entity<SystemBackup>(entity =>
            {
                entity.ToTable("system_backups");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .IsRequired()
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.StorageBucket)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.StorageObjectName)
                    .IsRequired()
                    .HasMaxLength(1024);

                entity.Property(e => e.SizeBytes);
                entity.Property(e => e.CreatedAtUtc);
                entity.Property(e => e.RestoredAtUtc);
                entity.Property(e => e.DeletedAtUtc);
                entity.Property(e => e.IsDeleted);

                entity.HasIndex(e => e.CreatedAtUtc);
                entity.HasIndex(e => e.IsDeleted);
            });

            modelBuilder.Entity<WeeklyMenuImage>(entity =>
            {
                entity.ToTable("weekly_menu_images");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Code)
                    .HasMaxLength(20);

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasDefaultValue("gallery");

                entity.Property(e => e.SortOrder)
                    .HasDefaultValue(0);

                entity.Property(e => e.CreatedAt);
                entity.Property(e => e.UpdatedAt);

                entity.HasOne(e => e.WeeklyMenu)
                    .WithMany(m => m.WeeklyMenuImages)
                    .HasForeignKey(e => e.WeeklyMenuId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.MediaFile)
                    .WithMany()
                    .HasForeignKey(e => e.MediaFileId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.WeeklyMenuId, e.SortOrder });
                entity.HasIndex(e => e.MediaFileId);
            });

            modelBuilder.Entity<CustomerType>(entity =>
            {
                entity.ToTable("customer_types");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Code).HasMaxLength(20);
                entity.Property(e => e.ProfileKey).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.CreatedAt);
                entity.HasIndex(e => e.Code).IsUnique();
                entity.HasIndex(e => e.ProfileKey).IsUnique();
            });
       
            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.IsActive);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
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
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
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
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
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
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

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
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

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
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

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
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.AccessToken).IsRequired().HasMaxLength(2000);
                entity.Property(e => e.RefreshToken).IsRequired().HasMaxLength(2000);
                entity.Property(e => e.ReplacedByToken).HasMaxLength(2000);
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

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.OwnerUserId).IsRequired();
                entity.Property(e => e.Bucket).IsRequired().HasMaxLength(255);
                entity.Property(e => e.ObjectName).IsRequired().HasMaxLength(1024);
                entity.Property(e => e.OriginalFileName).HasMaxLength(255);
                entity.Property(e => e.ContentType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.MediaType).IsRequired().HasMaxLength(20).HasDefaultValue("unknown");
                entity.Property(e => e.Md5HashBase64).HasMaxLength(128);

                entity.HasOne(e => e.OwnerUser)
                    .WithMany(u => u.MediaFiles)
                    .HasForeignKey(e => e.OwnerUserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Organization
            modelBuilder.Entity<Organization>(entity =>
            {
                entity.ToTable("organizations");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasIndex(e => new { e.IsActive, e.Name });
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Address).HasMaxLength(255);
                entity.Property(e => e.Phone).HasMaxLength(50);
                entity.Property(e => e.ContactPerson).HasMaxLength(255);
                entity.Property(e => e.ContactEmail).HasMaxLength(255);
            });

            // UserOrganization
            modelBuilder.Entity<UserOrganization>(entity =>
            {
                entity.ToTable("user_organizations");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.UserId, e.OrganizationId }).IsUnique();
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.HasOne(e => e.User).WithMany(u => u.UserOrganizations).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Organization).WithMany(u => u.UserOrganizations).HasForeignKey(e => e.OrganizationId).OnDelete(DeleteBehavior.Cascade);
            });

            // Partner
            modelBuilder.Entity<Partner>(entity =>
            {
                entity.ToTable("partners");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.TaxId).IsUnique();
                entity.HasIndex(e => new { e.IsActive, e.LegalName });
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.LegalName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.BusinessRegistrationNumber).HasMaxLength(100);
                entity.Property(e => e.TaxId).HasMaxLength(50);
                entity.Property(e => e.LegalRepresentative).HasMaxLength(255);
                entity.Property(e => e.Address).HasMaxLength(255);
                entity.Property(e => e.ContactPerson).HasMaxLength(255);
                entity.Property(e => e.Phone).HasMaxLength(50);
                entity.Property(e => e.Email).HasMaxLength(255);
                entity.Property(e => e.PerformanceRating).HasPrecision(3, 2);
                entity.Property(e => e.ComplianceInfo).HasMaxLength(2000);
                entity.Property(e => e.FinancialTerms).HasMaxLength(1000);
            });

            // Contract
            modelBuilder.Entity<Contract>(entity =>
            {
                entity.ToTable("contracts");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.PartnerId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => new { e.StartDate, e.EndDate });
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.ContractNumber).HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.SupplySchedule).HasMaxLength(500);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
                entity.Property(e => e.TotalValue).HasPrecision(12, 2);
                entity.Property(e => e.DepositAmount).HasPrecision(12, 2);
                entity.HasOne(e => e.Partner).WithMany(p => p.Contracts).HasForeignKey(e => e.PartnerId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Organization).WithMany(o => o.Contracts).HasForeignKey(e => e.OrganizationId).OnDelete(DeleteBehavior.SetNull);
            });

            // PartnerPayment
            modelBuilder.Entity<PartnerPayment>(entity =>
            {
                entity.ToTable("partner_payments");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.ContractId);
                entity.HasIndex(e => e.PartnerId);
                entity.HasIndex(e => e.PaymentDate);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Amount).HasPrecision(12, 2);
                entity.Property(e => e.Method).IsRequired().HasMaxLength(30);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
                entity.HasOne(e => e.Contract).WithMany(c => c.PartnerPayments).HasForeignKey(e => e.ContractId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Partner).WithMany(p => p.PartnerPayments).HasForeignKey(e => e.PartnerId).OnDelete(DeleteBehavior.Restrict);
            });

            // Ingredient
            modelBuilder.Entity<Ingredient>(entity =>
            {
                entity.ToTable("ingredients");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasIndex(e => e.DefaultSupplierId);
                entity.HasIndex(e => e.IsActive);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Unit).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Description).HasMaxLength(255);
                entity.Property(e => e.CostPerUnit).HasPrecision(10, 2);
                entity.HasOne(e => e.DefaultSupplier).WithMany(p => p.IngredientsAsDefaultSupplier).HasForeignKey(e => e.DefaultSupplierId).OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(e => e.Category).WithMany(c => c.Ingredients).HasForeignKey(e => e.CategoryId).OnDelete(DeleteBehavior.SetNull);
            });

            // IngredientCategory
            modelBuilder.Entity<IngredientCategory>(entity =>
            {
                entity.ToTable("ingredient_categories");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Code).IsUnique();
                entity.HasIndex(e => e.NameEnglish).IsUnique();
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.NameEnglish).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(255);
            });

            // IngredientSource
            modelBuilder.Entity<IngredientSource>(entity =>
            {
                entity.ToTable("ingredient_sources");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.IngredientId);
                entity.HasIndex(e => e.PartnerId);
                entity.HasIndex(e => e.ExpirationDate);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.BatchNumber).HasMaxLength(50);
                entity.Property(e => e.OriginDetails).HasMaxLength(255);
                entity.Property(e => e.Certification).HasMaxLength(255);
                entity.HasOne(e => e.Ingredient).WithMany(i => i.IngredientSources).HasForeignKey(e => e.IngredientId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Partner).WithMany(p => p.IngredientSources).HasForeignKey(e => e.PartnerId).OnDelete(DeleteBehavior.SetNull);
            });

            // Inventory
            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.ToTable("inventory");
                entity.HasKey(e => e.IngredientId);
                entity.Property(e => e.QuantityAvailable).HasPrecision(12, 2);
                entity.Property(e => e.ReorderLevel).HasPrecision(12, 2);
                entity.HasOne(e => e.Ingredient).WithOne(i => i.Inventory).HasForeignKey<Inventory>(e => e.IngredientId).OnDelete(DeleteBehavior.Cascade);
            });

            // InternalStockIssue
            modelBuilder.Entity<InternalStockIssue>(entity =>
            {
                entity.ToTable("internal_stock_issues");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.IssueCode).IsUnique();
                entity.HasIndex(e => e.IssuedAt);
                entity.HasIndex(e => e.CreatedByUserId);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.IssueCode).IsRequired().HasMaxLength(40);
                entity.Property(e => e.Reason).HasMaxLength(500);
                entity.HasOne(e => e.CreatedByUser)
                    .WithMany(u => u.InternalStockIssuesCreated)
                    .HasForeignKey(e => e.CreatedByUserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<InternalStockIssueLine>(entity =>
            {
                entity.ToTable("internal_stock_issue_lines");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.IssueId);
                entity.HasIndex(e => e.IngredientId);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Quantity).HasPrecision(12, 2);
                entity.HasOne(e => e.Issue)
                    .WithMany(i => i.Lines)
                    .HasForeignKey(e => e.IssueId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Ingredient)
                    .WithMany()
                    .HasForeignKey(e => e.IngredientId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // IngredientIntakeProposal
            modelBuilder.Entity<IngredientIntakeProposal>(entity =>
            {
                entity.ToTable("ingredient_intake_proposals");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.ProposalCode).IsUnique();
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.CreatedByUserId);
                entity.HasIndex(e => e.CreatedAt);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.ProposalCode).IsRequired().HasMaxLength(40);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
                entity.Property(e => e.HeaderNote).HasMaxLength(500);
                entity.Property(e => e.ReviewNote).HasMaxLength(500);
                entity.HasOne(e => e.CreatedByUser)
                    .WithMany(u => u.IngredientIntakeProposalsCreated)
                    .HasForeignKey(e => e.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.ReviewedByUser)
                    .WithMany(u => u.IngredientIntakeProposalsReviewed)
                    .HasForeignKey(e => e.ReviewedByUserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<IngredientIntakeProposalLine>(entity =>
            {
                entity.ToTable("ingredient_intake_proposal_lines");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.ProposalId);
                entity.HasIndex(e => e.IngredientId);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Quantity).HasPrecision(12, 2);
                entity.Property(e => e.LineNote).HasMaxLength(255);
                entity.HasOne(e => e.Proposal)
                    .WithMany(p => p.Lines)
                    .HasForeignKey(e => e.ProposalId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Ingredient)
                    .WithMany()
                    .HasForeignKey(e => e.IngredientId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // IngredientActualIntake
            modelBuilder.Entity<IngredientActualIntake>(entity =>
            {
                entity.ToTable("ingredient_actual_intakes");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.ReceiptCode).IsUnique();
                entity.HasIndex(e => e.ProposalId).IsUnique();
                entity.HasIndex(e => e.CreatedByUserId);
                entity.HasIndex(e => e.ReceivedAt);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.ReceiptCode).IsRequired().HasMaxLength(40);
                entity.Property(e => e.Note).HasMaxLength(500);
                entity.HasOne(e => e.Proposal)
                    .WithOne(p => p.ActualIntake)
                    .HasForeignKey<IngredientActualIntake>(e => e.ProposalId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.CreatedByUser)
                    .WithMany(u => u.IngredientActualIntakesCreated)
                    .HasForeignKey(e => e.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<IngredientActualIntakeLine>(entity =>
            {
                entity.ToTable("ingredient_actual_intake_lines");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.IntakeId);
                entity.HasIndex(e => e.IngredientId);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Quantity).HasPrecision(12, 2);
                entity.HasOne(e => e.Intake)
                    .WithMany(i => i.Lines)
                    .HasForeignKey(e => e.IntakeId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Ingredient)
                    .WithMany()
                    .HasForeignKey(e => e.IngredientId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CookingMethod>(entity =>
            {
                entity.ToTable("cooking_methods");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Code).IsUnique();
                entity.HasIndex(e => e.MethodKey).IsUnique();
                entity.Property(e => e.Code).HasMaxLength(20);
                entity.Property(e => e.MethodKey).IsRequired().HasMaxLength(32);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            });

            // Dish
            modelBuilder.Entity<Dish>(entity =>
            {
                entity.ToTable("dishes");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasIndex(e => e.IsActive);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.NameEnglish).HasMaxLength(255);
                entity.Property(e => e.Description).HasMaxLength(255);
                entity.Property(e => e.Price).HasPrecision(10, 2);
                entity.Property(e => e.DietaryLabel).HasMaxLength(50);
                entity.HasOne(e => e.CookingMethod)
                    .WithMany(m => m.Dishes)
                    .HasForeignKey(e => e.CookingMethodId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DishImage>(entity =>
            {
                entity.ToTable("dish_images");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Code).IsUnique();
                entity.HasIndex(e => new { e.DishId, e.SortOrder });
                entity.HasIndex(e => e.MediaFileId);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Code).HasMaxLength(20);
                entity.Property(e => e.Role).IsRequired().HasMaxLength(20).HasDefaultValue("gallery");
                entity.HasOne(e => e.Dish)
                    .WithMany(d => d.DishImages)
                    .HasForeignKey(e => e.DishId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.MediaFile)
                    .WithMany()
                    .HasForeignKey(e => e.MediaFileId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DishCategory>(entity =>
            {
                entity.ToTable("dish_categories");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Code).IsUnique();
                entity.HasIndex(e => e.SlotKey).IsUnique();
                entity.Property(e => e.Code).HasMaxLength(20);
                entity.Property(e => e.SlotKey).IsRequired().HasMaxLength(32);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<DishDishCategory>(entity =>
            {
                entity.ToTable("dish_dish_categories");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Code).IsUnique();
                entity.Property(e => e.Code).HasMaxLength(20);
                entity.HasIndex(e => new { e.DishId, e.DishCategoryId }).IsUnique();
                entity.HasOne(e => e.DishCategory)
                    .WithMany(c => c.DishDishCategories)
                    .HasForeignKey(e => e.DishCategoryId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Dish)
                    .WithMany(d => d.DishDishCategories)
                    .HasForeignKey(e => e.DishId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // DishIngredient
            modelBuilder.Entity<DishIngredient>(entity =>
            {
                entity.ToTable("dish_ingredients");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.DishId, e.IngredientId }).IsUnique();
                entity.HasIndex(e => e.DishId);
                entity.HasIndex(e => e.IngredientId);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Quantity).HasPrecision(10, 2);
                entity.Property(e => e.Unit).HasMaxLength(20);
                entity.HasOne(e => e.Dish).WithMany(d => d.DishIngredients).HasForeignKey(e => e.DishId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Ingredient).WithMany(i => i.DishIngredients).HasForeignKey(e => e.IngredientId).OnDelete(DeleteBehavior.Restrict);
            });

            // WeeklyMenu
            modelBuilder.Entity<WeeklyMenu>(entity =>
            {
                entity.ToTable("weekly_menus");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.StartDate, e.EndDate }).IsUnique();
                entity.HasIndex(e => e.CreatedBy);
                entity.HasIndex(e => e.CustomerTypeId);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Description).HasMaxLength(255);
                entity.HasOne(e => e.CreatedByUser).WithMany(u => u.WeeklyMenusCreated).HasForeignKey(e => e.CreatedBy).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.CustomerType).WithMany(ct => ct.WeeklyMenus).HasForeignKey(e => e.CustomerTypeId).OnDelete(DeleteBehavior.SetNull);
            });

            // MenuSchedule
            modelBuilder.Entity<MenuSchedule>(entity =>
            {
                entity.ToTable("menu_schedule");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.MenuId, e.Date, e.MealSlot, e.DishId }).IsUnique();
                entity.HasIndex(e => e.DishId);
                entity.HasIndex(e => e.Date);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.MealSlot).IsRequired().HasMaxLength(20);
                entity.HasOne(e => e.Menu).WithMany(m => m.MenuSchedules).HasForeignKey(e => e.MenuId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Dish).WithMany(d => d.MenuSchedules).HasForeignKey(e => e.DishId).OnDelete(DeleteBehavior.Restrict);
            });

            // Order
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("orders");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.CreatedBySalesUserId);
                entity.HasIndex(e => e.InvoiceCode).IsUnique();
                entity.HasIndex(e => new { e.ScheduledDate, e.Status });
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
                entity.Property(e => e.TotalAmount).HasPrecision(12, 2);
                entity.Property(e => e.PaymentStatus).IsRequired().HasMaxLength(20);
                entity.Property(e => e.InvoiceCode).HasMaxLength(40);
                entity.HasOne(e => e.User).WithMany(u => u.Orders).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(e => e.Contract).WithMany(c => c.Orders).HasForeignKey(e => e.ContractId).OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(e => e.CreatedBySalesUser).WithMany(u => u.SalesOrdersCreated).HasForeignKey(e => e.CreatedBySalesUserId).OnDelete(DeleteBehavior.SetNull);
            });

            // OrderItem
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("order_items");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.OrderId);
                entity.HasIndex(e => e.DishId);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.UnitPrice).HasPrecision(10, 2);
                entity.Property(e => e.TotalPrice).HasPrecision(12, 2);
                entity.HasOne(e => e.Order).WithMany(o => o.OrderItems).HasForeignKey(e => e.OrderId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Dish).WithMany(d => d.OrderItems).HasForeignKey(e => e.DishId).OnDelete(DeleteBehavior.Restrict);
            });

            // Delivery
            modelBuilder.Entity<Delivery>(entity =>
            {
                entity.ToTable("deliveries");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.OrderId);
                entity.HasIndex(e => e.AssignedStaffId);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.DeliveryAddress).IsRequired().HasMaxLength(255);
                entity.Property(e => e.DeliveryStatus).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Notes).HasMaxLength(255);
                entity.HasOne(e => e.Order).WithMany(o => o.Deliveries).HasForeignKey(e => e.OrderId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.AssignedStaff).WithMany(u => u.DeliveriesAssigned).HasForeignKey(e => e.AssignedStaffId).OnDelete(DeleteBehavior.SetNull);
            });

            // Payment
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("payments");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.OrderId);
                entity.HasIndex(e => e.PayerId);
                entity.HasIndex(e => e.PaymentDate);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Amount).HasPrecision(12, 2);
                entity.Property(e => e.Method).IsRequired().HasMaxLength(30);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
                entity.HasOne(e => e.Order).WithMany(o => o.Payments).HasForeignKey(e => e.OrderId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Payer).WithMany(u => u.PaymentsMade).HasForeignKey(e => e.PayerId).OnDelete(DeleteBehavior.SetNull);
            });

            // Transaction
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.ToTable("transactions");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Date);
                entity.HasIndex(e => e.Category);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Description).HasMaxLength(255);
                entity.Property(e => e.Amount).HasPrecision(12, 2);
                entity.Property(e => e.Category).HasMaxLength(100);
                entity.Property(e => e.Method).HasMaxLength(50);
            });

            // Review
            modelBuilder.Entity<Review>(entity =>
            {
                entity.ToTable("reviews");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.DishId);
                entity.HasIndex(e => e.OrderId);
                entity.HasIndex(e => e.CreatedAt);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.HasOne(e => e.User).WithMany(u => u.Reviews).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Dish).WithMany(d => d.Reviews).HasForeignKey(e => e.DishId).OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(e => e.Order).WithMany(o => o.Reviews).HasForeignKey(e => e.OrderId).OnDelete(DeleteBehavior.SetNull);
            });

            // Sentiment
            modelBuilder.Entity<Sentiment>(entity =>
            {
                entity.ToTable("sentiments");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.ReviewId).IsUnique();
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.SentimentLabel).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Confidence).HasPrecision(4, 2);
                entity.HasOne(e => e.Review).WithOne(r => r.Sentiment).HasForeignKey<Sentiment>(e => e.ReviewId).OnDelete(DeleteBehavior.Cascade);
            });

            // Complaint
            modelBuilder.Entity<Complaint>(entity =>
            {
                entity.ToTable("complaints");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.OrderId);
                entity.HasIndex(e => e.AssignedTo);
                entity.HasIndex(e => e.Status);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
                entity.HasOne(e => e.User).WithMany(u => u.ComplaintsRaised).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Order).WithMany(o => o.Complaints).HasForeignKey(e => e.OrderId).OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(e => e.AssignedToUser).WithMany(u => u.ComplaintsAssigned).HasForeignKey(e => e.AssignedTo).OnDelete(DeleteBehavior.SetNull);
            });

            // ChatbotLog
            modelBuilder.Entity<ChatbotLog>(entity =>
            {
                entity.ToTable("chatbot_logs");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.CreatedAt);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Message).IsRequired();
                entity.HasOne(e => e.User).WithMany(u => u.ChatbotLogs).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.SetNull);
            });

            // MenuSuggestion
            modelBuilder.Entity<MenuSuggestion>(entity =>
            {
                entity.ToTable("menu_suggestions");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.WeekStart);
                entity.HasIndex(e => e.CreatedBy);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.SuggestionText).IsRequired();
                entity.Property(e => e.AlgorithmVersion).HasMaxLength(50);
                entity.Property(e => e.RulesKey).HasMaxLength(50);
                entity.Property(e => e.Version).HasDefaultValue(1);
                entity.HasOne(e => e.CreatedByUser).WithMany(u => u.MenuSuggestionsCreated).HasForeignKey(e => e.CreatedBy).OnDelete(DeleteBehavior.SetNull);
                entity.HasMany(e => e.Plans)
                    .WithOne(p => p.MenuSuggestion)
                    .HasForeignKey(p => p.MenuSuggestionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // MenuSuggestionPlan
            modelBuilder.Entity<MenuSuggestionPlan>(entity =>
            {
                entity.ToTable("menu_suggestion_plans");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.MenuSuggestionId);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Rank).IsRequired();
                entity.Property(e => e.PlanScore).HasPrecision(6, 3);
                entity.Property(e => e.ObjectiveValue).HasPrecision(18, 3);
                entity.HasMany(e => e.Days)
                    .WithOne(d => d.MenuSuggestionPlan)
                    .HasForeignKey(d => d.MenuSuggestionPlanId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // MenuSuggestionPlanDay
            modelBuilder.Entity<MenuSuggestionPlanDay>(entity =>
            {
                entity.ToTable("menu_suggestion_plan_days");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.MenuSuggestionPlanId);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.DayName).IsRequired().HasMaxLength(20);
                entity.Property(e => e.DayIndex).IsRequired();
                entity.HasMany(e => e.Items)
                    .WithOne(i => i.MenuSuggestionPlanDay)
                    .HasForeignKey(i => i.MenuSuggestionPlanDayId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // MenuSuggestionPlanItem
            modelBuilder.Entity<MenuSuggestionPlanItem>(entity =>
            {
                entity.ToTable("menu_suggestion_plan_items");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.MenuSuggestionPlanDayId);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.SlotCategory).IsRequired().HasMaxLength(20);
                entity.Property(e => e.DishName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.DishSourceCategory).HasMaxLength(20);
                entity.Property(e => e.Score).HasPrecision(6, 3);
                entity.Property(e => e.CostPerServing).HasPrecision(10, 2);
                entity.Property(e => e.ReasonsJson);
            });

            modelBuilder.Entity<News>(entity =>
            {
                entity.ToTable("news");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.CreatedBy);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Content).IsRequired();
            });

            modelBuilder.Entity<Recruitment>(entity =>
            {
                entity.ToTable("recruitment");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.CreatedBy);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            });

            modelBuilder.Entity<Banner>(entity =>
            {
                entity.ToTable("banners");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.CreatedBy);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("notifications");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.IsRead);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.SendAt);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Message).IsRequired();

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Notifications)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
        #endregion
    }
}