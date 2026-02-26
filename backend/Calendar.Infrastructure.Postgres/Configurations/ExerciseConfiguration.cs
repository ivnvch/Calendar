using Calendar.Domain.Calendars;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Calendar.Infrastructure.Postgres.Configurations;

public class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
{
    public void Configure(EntityTypeBuilder<Exercise> builder)
    {
        builder.ToTable("exercises", t =>
        {
            t.HasCheckConstraint(
                "CK_TargetValue_Positive",
                "target_value > 0");
        });
        
        builder.HasKey(e => e.Id)
            .HasName("pk_exercises_id");

        builder.Property(e => e.Id)
            .IsRequired()
            .HasColumnName("id");

        builder.Property(e => e.ActivityType)
            .IsRequired()
            .HasColumnName("activity_type")
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(e => e.Status)
            .IsRequired()
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(e => e.TargetValue)
            .IsRequired()
            .HasColumnName("target_value");
        
        builder.Property(e => e.ActualValue)
            .IsRequired()
            .HasColumnName("actual_value");

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");



    }
}