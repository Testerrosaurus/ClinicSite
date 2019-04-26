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

namespace server.Services
{
  public class DoctorsDatabase
  {
    public List<Doctor> Data { get; }

    private Calendar _calendar;


    public DoctorsDatabase(Calendar calendar)
    {
      _calendar = calendar;

      Data = GetFilteredDb();
    }


    private List<Doctor> GetFilteredDb()
    {
      string appDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName;
      if (!Directory.Exists(appDir + "/Data")) // built in bin/{debug/release}/{version}/
      {
        appDir = appDir + "/../../..";
      }

      var json = File.ReadAllText(appDir + "/DoctorsDatabase.json");
      var db = JsonConvert.DeserializeObject<List<Doctor>>(json);

      var bookedAppointments = GetAllBookedAppointments();

      foreach (var doctor in db)
      {
        doctor.dateTimes.RemoveAll(dt =>
          bookedAppointments.ContainsKey(doctor.name) &&
          bookedAppointments[doctor.name].Exists(bdt =>
            dt.date == bdt.date && dt.time == bdt.time)
          );
      }

      return db;
    }

    private Dictionary<string, List<DateTimeStruct>> GetAllBookedAppointments()
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
      var ba = new Dictionary<string, List<DateTimeStruct>>();
      if (events.Items != null && events.Items.Count > 0)
      {
        foreach (var eventItem in events.Items)
        {
          if (!eventItem.Summary.Contains(" - ")) continue;

          string doctor = eventItem.Summary.Split(" - ")[1].Trim();

          DateTime start = (DateTime)eventItem.Start.DateTime;
          string date = start.Year + "-" + start.Month.ToString("00") + "-" + start.Day.ToString("00");
          string time = start.Hour + ":" + start.Minute.ToString("00");

          var dt = new DateTimeStruct { date = date, time = time };
          if (ba.ContainsKey(doctor))
          {
            ba[doctor].Add(dt);
          }
          else
          {
            var list = new List<DateTimeStruct>();
            list.Add(dt);

            ba.Add(doctor, list);
          }
        }
      }

      return ba;
    }
  }
}
