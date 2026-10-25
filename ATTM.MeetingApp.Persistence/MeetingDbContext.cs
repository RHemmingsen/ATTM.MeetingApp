using ATTM.MeetingApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ATTM.MeetingApp.Persistence;

public class MeetingDbContext(DbContextOptions<MeetingDbContext> options) : DbContext(options)
{
    public DbSet<Meeting> Meetings => Set<Meeting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MeetingDbContext).Assembly);
    }
}
