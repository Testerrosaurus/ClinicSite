using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace server.Models
{
  public class AppointmentsContext : DbContext
  {
    public DbSet<Procedure> Procedures { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<DoctorProcedure> DoctorProcedures { get; set; }

    public DbSet<Appointment> Appointments { get; set; }

    public AppointmentsContext(DbContextOptions<AppointmentsContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<DoctorProcedure>()
        .HasKey(dp => new { dp.DoctorId, dp.ProcedureId });

      modelBuilder.Entity<DoctorProcedure>()
        .HasOne(dp => dp.Doctor)
        .WithMany(d => d.DoctorProcedures);

      modelBuilder.Entity<DoctorProcedure>()
        .HasOne(dp => dp.Procedure)
        .WithMany(p => p.DoctorProcedures);
    }
  }
}
