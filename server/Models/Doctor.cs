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
  }

  //public class AppointmentInformation
  //{
  //  public Procedure Procedure { get; set; }
  //  public string PatientName { get; set; }
  //}

  public class DateTimePair
  {
    [Key]
    public long Id { get; set; }

    public string Date { get; set; }
    public string Time { get; set; }

    public string Status { get; set; }

   // public AppointmentInformation Info { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; }
  }

  public class Doctor
  {
    [Key]
    public long Id { get; set; }

    public string Name { get; set; }
    public List<Procedure> Procedures { get; set; }
    public List<DateTimePair> DateTimes { get; set; }
  }
}
