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

    public DoctorsContext(DbContextOptions<DoctorsContext> options) : base(options)
    {
      Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Doctor>()
          .HasMany(d => d.dateTimes)
          .WithOne()
          .HasForeignKey("DoctorId")
          .OnDelete(DeleteBehavior.Cascade);
    }
  }
}
