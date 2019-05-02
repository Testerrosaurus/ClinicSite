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

    private readonly DoctorsContext _dbContext;

    public AppointmentsController(Services.Calendar calendar, DoctorsContext context, Services.BookedFiltrator filtrator)
    {
      _calendar = calendar;

      _dbContext = context;
    }

    [Authorize]
    [HttpGet]
    public ActionResult<List<Doctor>> GetDb()
    {
      Response.ContentType = "application/json";
      return _dbContext.Doctors.Include(d => d.DateTimes).Include(d => d.Procedures).ToList();
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult<string> ConfirmDt(long id, byte[] rowVersion)
    {
      Response.ContentType = "application/json";

      try
      {
        var dateTime = new DateTimePair { Id = id, RowVersion = rowVersion };
        _dbContext.DateTimePairs.Attach(dateTime);
        dateTime.Status = "Confirmed";
        _dbContext.SaveChanges();
      }
      catch (DbUpdateConcurrencyException)
      {
        return Ok("Fail");
      }

      var dt = _dbContext.DateTimePairs.Find(id);


      //var start = DateTime.ParseExact(info.date + " " + info.time, "yyyy-MM-dd H:mm", CultureInfo.InvariantCulture);

      //Event newEvent = new Event()
      //{
      //  Summary = info.procedure + " - " + info.doctor,
      //  Description = "Procedure: " + info.procedure + "\nDoctor Name: " + info.doctor + "\nPatient Name: " + info.patient,
      //  //Attendees = new EventAttendee[]
      //  //{
      //  //  new EventAttendee{ DisplayName = info.patient, Email = "" },
      //  //  new EventAttendee{ DisplayName = info.doctor, Email = "" },
      //  //},
      //  Start = new EventDateTime()
      //  {
      //    DateTime = start,
      //  },
      //  End = new EventDateTime()
      //  {
      //    DateTime = start.AddMinutes(30),
      //  }
      //};

      //EventsResource.InsertRequest request = _calendar.Service.Events.Insert(newEvent, _calendar.CalendarId);
      //Event createdEvent = request.Execute();

      return Ok("Confirmed");
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

      var doctors = _dbContext.Doctors.Include(d => d.DateTimes).Include(d => d.Procedures).ToList();
      foreach (var doctor in doctors)
      {
        doctor.DateTimes.RemoveAll(dt => dt.Status != "Free");
      }

      return doctors;
    }

    public struct AppointmentInfo
    {
      public string patient;
      public string procedure;
      public long id;
      public byte[] rowVersion;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult<string> SetAppointment([FromBody]AppointmentInfo info)
    {
      Response.ContentType = "application/json";

      if (info.patient == "" || info.procedure == "" || info.id == 0)
        return Ok("Invalid info");

      try
      {
        var dateTime = new DateTimePair { Id = info.id, RowVersion = info.rowVersion };
        _dbContext.DateTimePairs.Attach(dateTime);
        dateTime.Status = "Unconfirmed";
        _dbContext.SaveChanges();
      }
      catch (DbUpdateConcurrencyException)
      {
        return Ok("Info changed");
      }

     
      return Ok("Created");
    }
  }
}
