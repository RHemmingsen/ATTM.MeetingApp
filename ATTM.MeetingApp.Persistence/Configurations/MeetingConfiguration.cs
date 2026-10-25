using ATTM.MeetingApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ATTM.MeetingApp.Persistence.Configurations;

public class MeetingConfiguration : IEntityTypeConfiguration<Meeting>
{
    public void Configure(EntityTypeBuilder<Meeting> builder)
    {
        builder.ToTable("Meetings");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(Meeting.NameMaxLength);
        builder.Property(x => x.Location).IsRequired();
        builder.Property(x => x.Seats).IsRequired();

        builder.HasData(new Meeting
        {
            Id = new Guid("dc9930a3-079d-4f31-addd-f02141d7422f"),
            Name = "Meeting 1",
            Location = LocationsEnum.Office,
            Seats = 10
        });
    }
}
