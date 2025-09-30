using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainMonitor.Models;

namespace TrainMonitor.Database.Configurations;

public class IncidentConfiguration : IEntityTypeConfiguration<Incident>
{
    public void Configure(EntityTypeBuilder<Incident> builder)
    {
        builder.ToTable("incidents");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.TrainNumber)
            .IsRequired();

        builder.Property(i => i.Username)
            .IsRequired();

        builder.Property(i => i.Reason)
            .IsRequired();

        builder.Property(i => i.AdditionalComment)
            .IsRequired(false);

        //relationship with Train (one to many)
        builder.HasOne(i => i.Train)
            .WithMany(t => t.Incidents)
            .HasForeignKey(i => i.TrainId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
