using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models
{
  public class Procedure
  {
    [Key]
    public long Id { get; set; }

    public string name { get; set; }
  }

  public class DateTimePair
  {
    [Key]
    public long Id { get; set; }

    public string date { get; set; }
    public string time { get; set; }
  }

  public class Doctor
  {
    [Key]
    public long Id { get; set; }

    public string name { get; set; }

    public List<Procedure> procedures { get; set; }
    public List<DateTimePair> dateTimes { get; set; }
  }
}
