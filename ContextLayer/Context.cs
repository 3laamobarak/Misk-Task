using System.Security.Claims;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ContextLayer
{
    public class Context : IdentityDbContext<ApplicationUser>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public Context() { }
        public Context(DbContextOptions<Context> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(Context).Assembly);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;

            // 1. Maintain tracking dates for BaseEntity
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = null;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = now;
                }
            }

            // 2. Extract active user identities from JWT Bearer Claims
            var user = _httpContextAccessor.HttpContext?.User;
            var performedBy = user?.Identity?.Name 
                              ?? user?.FindFirst(ClaimTypes.Email)?.Value 
                              ?? user?.FindFirst("uid")?.Value 
                              ?? "System/Anonymous";

            // 3. Pre-capture tracking objects (Critical for Modified/Deleted states)
            var auditEntries = PreCaptureAuditLogs(performedBy);

            // 4. Save primary changes to the database (Generates temporary/new IDs)
            var result = await base.SaveChangesAsync(cancellationToken);

            // 5. Apply post-save operations (Binds IDs for newly Created entries)
            PostCommitNewIds(auditEntries);

            // 6. Save audit trail records securely
            if (auditEntries.Any())
            {
                var finalLogs = auditEntries.Select(e => e.ToAuditLog()).ToList();
                AuditLogs.AddRange(finalLogs);
                await base.SaveChangesAsync(cancellationToken);
            }

            return result;
        }

        private List<AuditEntryTracker> PreCaptureAuditLogs(string performedBy)
        {
            var trackers = new List<AuditEntryTracker>();

            // Intercept tracked operations for Course, Learner, and Enrollment
            var targetEntries = ChangeTracker.Entries<BaseEntity>()
                .Where(e => e.State == EntityState.Added || 
                            e.State == EntityState.Modified || 
                            e.State == EntityState.Deleted)
                .ToList();

            foreach (var entry in targetEntries)
            {
                // Skip tracking audit logs recursively to prevent loops
                if (entry.Entity is AuditLog) continue;

                var tracker = new AuditEntryTracker(entry)
                {
                    EntityName = entry.Entity.GetType().Name,
                    PerformedBy = performedBy,
                    PerformedAt = DateTime.UtcNow
                };

                trackers.Add(tracker);

                if (entry.State == EntityState.Added)
                {
                    tracker.Action = "Create";
                    // For creations, the new state is represented by the full object serialization
                    tracker.NewValue = System.Text.Json.JsonSerializer.Serialize(entry.Entity);
                }
                else if (entry.State == EntityState.Deleted)
                {
                    tracker.Action = "Delete";
                    // For removals, the old state is preserved via serialization
                    tracker.OldValue = System.Text.Json.JsonSerializer.Serialize(entry.Entity);
                }
                else if (entry.State == EntityState.Modified)
                {
                    tracker.Action = "Update";
                    var changedFieldsOld = new Dictionary<string, object>();
                    var changedFieldsNew = new Dictionary<string, object>();

                    foreach (var property in entry.Properties)
                    {
                        // Log changes while omitting metadata noise
                        if (property.IsModified && property.Metadata.Name != "UpdatedAt")
                        {
                            changedFieldsOld[property.Metadata.Name] = property.OriginalValue;
                            changedFieldsNew[property.Metadata.Name] = property.CurrentValue;
                        }
                    }

                    if (changedFieldsOld.Any())
                    {
                        tracker.OldValue = System.Text.Json.JsonSerializer.Serialize(changedFieldsOld);
                        tracker.NewValue = System.Text.Json.JsonSerializer.Serialize(changedFieldsNew);
                    }
                    else
                    {
                        // Remove tracker if no relevant changes occurred
                        trackers.Remove(tracker);
                    }
                }
            }

            return trackers;
        }

        private void PostCommitNewIds(List<AuditEntryTracker> trackers)
        {
            foreach (var tracker in trackers)
            {
                // For new creations, fetch the actual auto-incremented database ID
                if (tracker.Entry.State == EntityState.Detached || tracker.Action == "Create")
                {
                    var baseEntity = tracker.Entry.Entity as BaseEntity;
                    tracker.EntityId = baseEntity?.Id.ToString() ?? "0";
                    
                    // Re-serialize to include the freshly generated entity ID
                    tracker.NewValue = System.Text.Json.JsonSerializer.Serialize(tracker.Entry.Entity);
                }
                else
                {
                    var baseEntity = tracker.Entry.Entity as BaseEntity;
                    tracker.EntityId = baseEntity?.Id.ToString() ?? "0";
                }
            }
        }

        #region Dbsets
        public DbSet<Course> Courses { get; set; }
        public DbSet<Learner> Learners { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        #endregion
        internal class AuditEntryTracker
        {
            public Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry Entry { get; }
            public string EntityName { get; set; }
            public string EntityId { get; set; }
            public string Action { get; set; }
            public string OldValue { get; set; } = "None";
            public string NewValue { get; set; } = "None";
            public string PerformedBy { get; set; }
            public DateTime PerformedAt { get; set; }

            public AuditEntryTracker(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
            {
                Entry = entry;
            }

            public AuditLog ToAuditLog()
            {
                return new AuditLog
                {
                    EntityName = this.EntityName,
                    EntityId = this.EntityId,
                    Action = this.Action,
                    OldValue = this.OldValue,
                    NewValue = this.NewValue,
                    PerformedBy = this.PerformedBy,
                    PerformedAt = this.PerformedAt
                };
            }
        }

    }
}
