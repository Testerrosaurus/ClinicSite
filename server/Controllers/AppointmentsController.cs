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
using server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Data.SqlClient;

namespace server.Controllers
{
  [Route("api/[controller]/[action]")]
  [ApiController]
  public class AppointmentsController : ControllerBase
  {
    static object _locker = new object();

    private readonly Services.Calendar _calendar;
    private readonly List<Doctor> _filtered_db;

    private readonly DoctorsContext _dbContext;

    public AppointmentsController(Services.Calendar calendar, DoctorsContext context, Services.BookedFiltrator filtrator)
    {
      _calendar = calendar;

      _dbContext = context;

      _filtered_db = filtrator.Filtered;
    }

    [Authorize]
    [HttpGet]
    public ActionResult<List<Doctor>> GetDb()
    {
      Response.ContentType = "application/json";
      return _dbContext.Doctors.Include(d => d.dateTimes).Include(d => d.procedures).ToList();
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult<string> RemoveDt(long id, byte[] rowVersion)
    {
      Response.ContentType = "application/json";

      try
      {
        var dateTime = new DateTimePair { Id = id, RowVersion = rowVersion };
        _dbContext.DateTimePairs.Remove(dateTime);
        _dbContext.SaveChanges();
      }
      catch (DbUpdateConcurrencyException)
      {
        return Ok("Fail");
      }

      return Ok("Removed");
    }


    [HttpGet]
    public ActionResult<List<Doctor>> GetFilteredDb()
    {
      Response.ContentType = "application/json";
      return _filtered_db;
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
    [ValidateAntiForgeryToken]
    public ActionResult<string> SetAppointment([FromBody]AppointmentInfo info)
    {
      Response.ContentType = "application/json";


      if (info.patient == "" || info.procedure == "" || info.doctor == "" ||
          info.date == "" || info.time == "")
        return Ok("Invalid info");

      int deletedCount = -1;
      lock (_locker)
      {
        deletedCount = _filtered_db.Find(d => d.name == info.doctor).dateTimes
        .RemoveAll(dt => dt.date == info.date && dt.time == info.time);
      }

      if (deletedCount == 0) // didn't delete <=> this datetime is already booked
        return Ok("Already booked");


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


      return Ok("Created");
    }
  }
}
