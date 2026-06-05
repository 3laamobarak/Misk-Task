using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContextLayer.Configuration
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.ToTable("Course");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Description)
                .HasMaxLength(1000);

            builder.HasQueryFilter(c => !c.IsDeleted);

            builder.HasMany(c => c.Enrollments)
                .WithOne(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(
                new Course
                {
                    Id = 1,
                    Title = "HTML",
                    DurationHours = 20,
                    Description = "Learn the basics of HTML, the standard markup language for creating web pages.",
                    RequiresApproval = true,
                },
                new Course
                {
                    Id = 2,
                    Title = "CSS",
                    DurationHours = 25,
                    Description = "Master CSS to style and layout web pages effectively.",
                    RequiresApproval = false,
                },
                new Course
                {
                    Id = 3,
                    Title = "JavaScript",
                    DurationHours = 30,
                    Description = "Dive into JavaScript to create interactive and dynamic web applications.",
                    RequiresApproval = true,
                }
            );
        }
    }
}
