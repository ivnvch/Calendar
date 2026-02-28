using Calendar.Domain.Calendars.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Calendar.Infrastructure.Postgres.Configurations;

public class WorkoutDayConfiguration : IEntityTypeConfiguration<WorkoutDay>
{
    public void Configure(EntityTypeBuilder<WorkoutDay> builder)
    {
        builder.ToTable("workout_days");
        
        builder.HasKey(x => x.Id)
            .HasName("pk_workout_days_id");
        
        builder.HasIndex(x => x.Date)
            .IsUnique()
            .HasName("idx_workout_days_date");

        builder.Property(x => x.Id)
            .HasColumnName("id");
        
        builder.Property(x => x.Date)
            .HasColumnName("date")
            .IsRequired();
        
        builder.HasMany(w  => w.Exercises)
            .WithOne()
            .HasForeignKey("workout_day_id")
            .IsRequired() 
            .OnDelete(DeleteBehavior.Cascade);
    }
}