using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContextLayer.Configuration
{
    public class LearnerConfiguration : IEntityTypeConfiguration<Learner>
    {
        public void Configure(EntityTypeBuilder<Learner> builder)
        {
            builder.ToTable("Learner");

            builder.HasKey(l => l.Id);

            builder.Property(l => l.FullName)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(l => l.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(l => l.NationalId)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(l => l.Department)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasMany(l => l.Enrollments)
                .WithOne(e => e.Learner)
                .HasForeignKey(e => e.LearnerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(l => l.NationalId)
                .IsUnique();

            builder.HasQueryFilter(l => !l.IsDeleted);

            builder.HasData(
                new Learner
                {
                    Id = 1,
                    FullName = "Alice Johnson",
                    Email = "3laa.m0o0barak@gmail.com",
                    NationalId = "30209252755255",
                    Department = "Computer Science",
                },
                new Learner
                {
                    Id = 2,
                    FullName = "Bob Smith",
                    Email = "bob@gmail.com",
                    NationalId = "30209252755256",
                    Department = "Information Technology",
                },
                new Learner
                {
                    Id = 3,
                    FullName = "Charlie Brown",
                    Email = "charlieB@gmail.com",
                    NationalId = "30209252755257",
                    Department = "Software Engineering",
                }
            );
        }
    }
}
