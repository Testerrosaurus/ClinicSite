using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace server.Models
{
  public class Procedure
  {
    [Key]
    public long Id { get; set; }

    public string Name { get; set; }

    public List<DoctorProcedure> DoctorProcedures { get; set; }
  }

  public class Doctor
  {
    [Key]
    public long Id { get; set; }

    public string Name { get; set; }

    public List<DoctorProcedure> DoctorProcedures { get; set; }
  }

  public class DoctorProcedure
  {
    public long DoctorId { get; set; }
    public Doctor Doctor { get; set; }

    public long ProcedureId { get; set; }
    public Procedure Procedure { get; set; }
  }



  public class Information
  {
    [Key]
    public long Id { get; set; }

    public Procedure Procedure { get; set; }
    public string PatientName { get; set; }
  }


  public class Appointment
  {
    [Key]
    public long Id { get; set; }


    public Doctor Doctor { get; set; }

    public string Date { get; set; }
    public string Time { get; set; }

    public string Status { get; set; }

    public Information Info { get; set; }


    [Timestamp]
    public byte[] RowVersion { get; set; }
  }
}
