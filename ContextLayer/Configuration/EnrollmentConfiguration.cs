using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContextLayer.Configuration
{
    public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            builder.ToTable("Enrollment");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Status)
                .IsRequired();

            builder.HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Learner)
                .WithMany(l => l.Enrollments)
                .HasForeignKey(e => e.LearnerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(e => !e.IsDeleted);

            builder.HasData(
                new Enrollment
                {
                    Id = 1,
                    CourseId = 1,
                    LearnerId = 1,
                    Status = Domain.Enums.EnrollmentStatus.Approved,
                },
                new Enrollment
                {
                    Id = 2,
                    CourseId = 2,
                    LearnerId = 1,
                    Status = Domain.Enums.EnrollmentStatus.PendingApproval,
                },
                new Enrollment
                {
                    Id = 3,
                    CourseId = 3,
                    LearnerId = 2,
                    Status = Domain.Enums.EnrollmentStatus.Rejected,
                    Reason = "Does not meet requirements"
                }
            );
        }
    }
}
