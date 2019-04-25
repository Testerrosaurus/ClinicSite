using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace server.Services
{
  public class DoctorsDatabase
  {
    public string Data { get; }

    public DoctorsDatabase()
    {
      Data = File.ReadAllText("Data/DoctorsDatabase.json");
    }
  }
}
