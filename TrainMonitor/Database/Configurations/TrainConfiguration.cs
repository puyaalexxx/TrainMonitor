using Microsoft.EntityFrameworkCore;
using TrainMonitor.Models;

namespace TrainMonitor.Database.Configurations;

public class TrainConfiguration : IEntityTypeConfiguration<Train>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Train> builder)
    {
        // Table name
        builder.ToTable("trains");

        // Primary Key
        builder.HasKey(t => t.Id);

        // Properties
        builder.Property(t => t.TrainName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.TrainNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(t => t.DelayTime)
            .IsRequired();

        builder.Property(t => t.DepartureTime)
            .IsRequired();

        builder.Property(t => t.ArrivalTime)
            .IsRequired(false);
    }
}