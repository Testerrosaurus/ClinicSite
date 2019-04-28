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
using Newtonsoft.Json;
using server.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace server.Services
{
  public class BookedFiltrator
  {
    private Calendar _calendar;
    private Dictionary<string, List<DateTimePair>> _booked;

    public List<Doctor> Filtered;

    public BookedFiltrator(Calendar calendar, IServiceProvider service)
    {
      _calendar = calendar;

      _booked = GetAllBookedAppointments();

      using (var scope = service.CreateScope())
      {
        var context = scope.ServiceProvider.GetService<DoctorsContext>();

        if (!context.Doctors.Any())
        {
          string appDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName;
          if (!Directory.Exists(appDir + "/Data")) // built in bin/{debug/release}/{version}/
          {
            appDir = appDir + "/../../..";
          }

          var json = File.ReadAllText(appDir + "/DoctorsDatabase.json");
          var doctors = JsonConvert.DeserializeObject<List<Doctor>>(json);


          context.Doctors.AddRange(doctors);
          context.SaveChanges();
        }

        var db = context.Doctors.Include(d => d.dateTimes).Include(d => d.procedures).ToList();
        Filtered = GetFilteredDb(db);
      }
    }


    public List<Doctor> GetFilteredDb(List<Doctor> db)
    {
      foreach (var doctor in db)
      {
        doctor.dateTimes.RemoveAll(dt =>
          _booked.ContainsKey(doctor.name) &&
          _booked[doctor.name].Exists(bdt =>
            dt.date == bdt.date && dt.time == bdt.time)
          );
      }

      return db;
    }

    private Dictionary<string, List<DateTimePair>> GetAllBookedAppointments()
    {
      // Define parameters of request.
      EventsResource.ListRequest request = _calendar.Service.Events.List(_calendar.CalendarId);
      request.TimeMin = DateTime.Now;
      request.ShowDeleted = false;
      request.SingleEvents = true;
      request.MaxResults = 10;
      request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;


      // List events.
      Events events = request.Execute();
      var ba = new Dictionary<string, List<DateTimePair>>();
      if (events.Items != null && events.Items.Count > 0)
      {
        foreach (var eventItem in events.Items)
        {
          if (!eventItem.Summary.Contains(" - ")) continue;

          string doctor = eventItem.Summary.Split(" - ")[1].Trim();

          DateTime start = (DateTime)eventItem.Start.DateTime;
          string date = start.Year + "-" + start.Month.ToString("00") + "-" + start.Day.ToString("00");
          string time = start.Hour + ":" + start.Minute.ToString("00");

          var dt = new DateTimePair { date = date, time = time };
          if (ba.ContainsKey(doctor))
          {
            ba[doctor].Add(dt);
          }
          else
          {
            var list = new List<DateTimePair>();
            list.Add(dt);

            ba.Add(doctor, list);
          }
        }
      }

      return ba;
    }
  }
}
