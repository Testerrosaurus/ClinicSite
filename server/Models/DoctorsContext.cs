using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace server.Models
{
  public class DoctorsContext : DbContext
  {
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Procedure> Procedures { get; set; }
    public DbSet<DateTimePair> DateTimePairs { get; set; }

    public DoctorsContext(DbContextOptions<DoctorsContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Doctor>()
          .HasMany(d => d.DateTimes)
          .WithOne()
          .HasForeignKey("DoctorId")
          .OnDelete(DeleteBehavior.Cascade);
    }
  }
}
