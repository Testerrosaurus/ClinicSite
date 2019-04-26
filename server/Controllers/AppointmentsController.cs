using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Globalization;
using Newtonsoft.Json;

namespace server.Controllers
{
  [Route("api/[controller]/[action]")]
  [ApiController]
  public class AppointmentsController : ControllerBase
  {
    private readonly Services.Calendar _calendar;
    private List<Doctor> _db;

    public AppointmentsController(Services.Calendar calendar)
    {
      _calendar = calendar;


      var json = System.IO.File.ReadAllText("DoctorsDatabase.json");
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

      _db = db;
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

    [HttpGet]
    public ActionResult<List<Doctor>> GetDb()
    {
      Response.ContentType = "application/json";
      return _db;
    }

    public struct AppointmentInfo
    {
      public string patient;
      public string procedure;
      public string doctor;
      public string date;
      public string time;
    }

    [HttpPost]
    public ActionResult<string> SetAppointment([FromBody]AppointmentInfo info)
    {
      var start = DateTime.ParseExact(info.date + " " + info.time, "yyyy-MM-dd H:mm", CultureInfo.InvariantCulture);

      Event newEvent = new Event()
      {
        Summary = info.procedure + " - " + info.doctor,
        Description = "Procedure: " + info.procedure + "\nDoctor Name: " + info.doctor + "\nPatient Name: " + info.patient,
        //Attendees = new EventAttendee[]
        //{
        //  new EventAttendee{ DisplayName = info.patient, Email = "" },
        //  new EventAttendee{ DisplayName = info.doctor, Email = "" },
        //},
        Start = new EventDateTime()
        {
          DateTime = start,
        },
        End = new EventDateTime()
        {
          DateTime = start.AddMinutes(30),
        }
      };

      EventsResource.InsertRequest request = _calendar.Service.Events.Insert(newEvent, _calendar.CalendarId);
      Event createdEvent = request.Execute();

      Response.ContentType = "application/json";
      return createdEvent.HtmlLink;
    }
  }
}
