using ATTM.MeetingApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATTM.MeetingApp.Persistence
{
    public class MeetingDbContext : DbContext
    {
        public MeetingDbContext(DbContextOptions<MeetingDbContext> options) : base(options)
        {
        }
        public DbSet<Meeting> Meetings { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MeetingDbContext).Assembly);
            modelBuilder.Entity<Meeting>().ToTable("Meetings");
            modelBuilder.Entity<Meeting>().HasKey(x => x.Id);
            modelBuilder.Entity<Meeting>().Property(x => x.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Meeting>().Property(x => x.Name).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Meeting>().Property(x => x.Location).IsRequired();
            modelBuilder.Entity<Meeting>().Property(x => x.Seats).IsRequired();

            var meeting = new Meeting
            {
                Id = Guid.NewGuid(),
                Name = "Meeting 1",
                Location = LocationsEnum.Office,
                Seats = 10
            };
            modelBuilder.Entity<Meeting>().HasData(meeting);    
        }

        //public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        //{
        //    foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        //    {
        //        switch (entry.State)
        //        {
        //            case EntityState.Added:
        //                entry.Entity.CreatedDate = DateTime.Now;
        //                entry.Entity.CreatedBy = _loggedInUserService.UserId;
        //                break;
        //            case EntityState.Modified:
        //                entry.Entity.LastModifiedDate = DateTime.Now;
        //                entry.Entity.LastModifiedBy = _loggedInUserService.UserId;
        //                break;
        //        }
        //    }
        //    return base.SaveChangesAsync(cancellationToken);
        //}
    }
}
