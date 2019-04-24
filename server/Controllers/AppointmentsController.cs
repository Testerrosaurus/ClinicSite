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

namespace server.Controllers
{
  [Route("api/[controller]/[action]")]
  [ApiController]
  public class AppointmentsController : ControllerBase
  {
    private Services.Calendar _calendar;

    public AppointmentsController(Services.Calendar calendar)
    {
      _calendar = calendar;
    }

    [HttpGet]
    public ActionResult<string> GetAllEvents()
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
      string result = "";
      if (events.Items != null && events.Items.Count > 0)
      {
        foreach (var eventItem in events.Items)
        {
          string start = eventItem.Start.DateTime.ToString();
          string end = eventItem.End.DateTime.ToString();
          result += $"{eventItem.Summary} ({start}) - ({end})";
        }
      }
      else
      {
        result = "No upcoming events found.";
      }

      Response.ContentType = "application/json";
      return result;
    }

    public struct AppointmentInfo
    {
      public string summary;
      public string date;
      public string time;
    }

    [HttpPost]
    public ActionResult<string> SetAppointment([FromBody]AppointmentInfo info)
    {
      var start = DateTime.ParseExact(info.date + " " + info.time, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);

      Event newEvent = new Event()
      {
        Summary = info.summary,
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
