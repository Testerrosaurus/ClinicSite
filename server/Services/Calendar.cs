using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.Extensions.DependencyInjection;
using server.Models;

namespace server.Services
{
  public class Calendar
  {
    public string CalendarId { get; }
    public CalendarService Service { get; }


    public Calendar(IServiceProvider service)
    {
      string[] Scopes = { CalendarService.Scope.Calendar };
      string ApplicationName = "SDSG";

      GoogleCredential credential;

      string appDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName;
      if (!Directory.Exists(appDir + "/Data")) // built in bin/{debug/release}/{version}/
      {
        appDir = appDir + "/../../..";
      }

      using (var stream = new FileStream(appDir + "/Data/server_server_cred.json", FileMode.Open, FileAccess.Read))
      {
        credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
      }



      Service = new CalendarService(new BaseClientService.Initializer()
      {
        HttpClientInitializer = credential,
        ApplicationName = ApplicationName,
      });

      CalendarId = File.ReadAllText(appDir + "/Data/calendar_id.json");



      using (var scope = service.CreateScope())
      {
        var context = scope.ServiceProvider.GetService<AppointmentsContext>();

        //if (!context.Appointments.Any())
        {
          context.Appointments.RemoveRange(context.Appointments.ToList());
          context.Doctors.RemoveRange(context.Doctors.ToList());
          context.Procedures.RemoveRange(context.Procedures.ToList());
          context.DoctorProcedures.RemoveRange(context.DoctorProcedures.ToList());


          var procedures = new List<Procedure>
          {
            new Procedure { Name = "procedure1" },
            new Procedure { Name = "procedure2" },
            new Procedure { Name = "procedure3" }
          };

          var doctors = new List<Doctor>
          {
            new Doctor { Name = "doctor1",
              DoctorProcedures = new List<DoctorProcedure> {
                new DoctorProcedure { Procedure = procedures.Find(p => p.Name == "procedure1") },
                new DoctorProcedure { Procedure = procedures.Find(p => p.Name == "procedure2") }
              }
            },
            new Doctor { Name = "doctor2",
              DoctorProcedures = new List<DoctorProcedure> {
                new DoctorProcedure { Procedure = procedures.Find(p => p.Name == "procedure2") },
                new DoctorProcedure { Procedure = procedures.Find(p => p.Name == "procedure3") }
              }
            }
          };

          context.Doctors.AddRange(doctors);


          var appointments = new List<Appointment>
          {
            new Appointment { Date = "2019-04-26", Time = "13:00", Status = "Free", Doctor = doctors.Find(d => d.Name == "doctor1") },
            new Appointment { Date = "2019-04-26", Time = "14:00", Status = "Free", Doctor = doctors.Find(d => d.Name == "doctor1") },
            new Appointment { Date = "2019-04-27", Time = "10:00", Status = "Free", Doctor = doctors.Find(d => d.Name == "doctor1") },
            new Appointment { Date = "2019-04-27", Time = "11:00", Status = "Free", Doctor = doctors.Find(d => d.Name == "doctor1") },

            new Appointment { Date = "2019-04-26", Time = "8:30", Status = "Free", Doctor = doctors.Find(d => d.Name == "doctor2") },
            new Appointment { Date = "2019-04-26", Time = "9:30", Status = "Free", Doctor = doctors.Find(d => d.Name == "doctor2") },
            new Appointment { Date = "2019-04-27", Time = "11:30", Status = "Free", Doctor = doctors.Find(d => d.Name == "doctor2") },
            new Appointment { Date = "2019-04-27", Time = "12:30", Status = "Free", Doctor = doctors.Find(d => d.Name == "doctor2") },
            new Appointment { Date = "2019-04-28", Time = "7:30", Status = "Free", Doctor = doctors.Find(d => d.Name == "doctor2") },
            new Appointment { Date = "2019-04-28", Time = "10:05", Status = "Free", Doctor = doctors.Find(d => d.Name == "doctor2") }
          };

          context.Appointments.AddRange(appointments);

          context.SaveChanges();
        }
      }
    }
  }
}
