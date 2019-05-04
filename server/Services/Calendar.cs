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
          context.Doctors.RemoveRange(context.Doctors.ToList());
          context.FreeTimes.RemoveRange(context.FreeTimes.ToList());
          context.Appointments.RemoveRange(context.Appointments.ToList());


          var doctors = new List<Doctor>
          {
            new Doctor { Name = "doctor1"},
            new Doctor { Name = "doctor2"}
          };

          context.Doctors.AddRange(doctors);


          var freeTimes = new List<FreeTime>
          {
            new FreeTime {
              Start = DateTime.ParseExact("2019-04-26 15:00", "yyyy-MM-dd HH:mm", null),
              End = DateTime.ParseExact("2019-04-26 17:00", "yyyy-MM-dd HH:mm", null),
              Doctor = doctors.Find(d => d.Name == "doctor1")
            },
            new FreeTime {
              Start = DateTime.ParseExact("2019-04-26 18:00", "yyyy-MM-dd HH:mm", null),
              End = DateTime.ParseExact("2019-04-26 21:00", "yyyy-MM-dd HH:mm", null),
              Doctor = doctors.Find(d => d.Name == "doctor1")
            },
            new FreeTime {
              Start = DateTime.ParseExact("2019-04-28 09:30", "yyyy-MM-dd HH:mm", null),
              End = DateTime.ParseExact("2019-04-28 13:30", "yyyy-MM-dd HH:mm", null),
              Doctor = doctors.Find(d => d.Name == "doctor1")
            },
            new FreeTime {
              Start = DateTime.ParseExact("2019-04-26 16:20", "yyyy-MM-dd HH:mm", null),
              End = DateTime.ParseExact("2019-04-26 20:40", "yyyy-MM-dd HH:mm", null),
              Doctor = doctors.Find(d => d.Name == "doctor2")
            },
            new FreeTime {
              Start = DateTime.ParseExact("2019-04-27 16:00", "yyyy-MM-dd HH:mm", null),
              End = DateTime.ParseExact("2019-04-27 20:00", "yyyy-MM-dd HH:mm", null),
              Doctor = doctors.Find(d => d.Name == "doctor2")
            }
          };

          context.FreeTimes.AddRange(freeTimes);

          context.SaveChanges();
        }
      }
    }
  }
}
