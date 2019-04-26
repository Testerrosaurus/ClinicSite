using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace server.Models
{
  public struct DateTimeStruct
  {
    public string date;
    public string time;
  }

  public struct Doctor
  {
    public string name;
    public List<dynamic> procedures;
    public List<DateTimeStruct> dateTimes;
  }
}
