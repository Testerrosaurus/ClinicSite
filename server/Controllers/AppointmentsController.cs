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
    private readonly Services.Calendar _calendar;

    private readonly AppointmentsContext _dbContext;

    public AppointmentsController(Services.Calendar calendar, AppointmentsContext context)
    {
      _calendar = calendar;

      _dbContext = context;
    }

    [Authorize]
    [HttpGet]
    public ActionResult<List<Appointment>> GetDb()
    {
      var appointments = _dbContext.Appointments.Include(a => a.Doctor).Include(a => a.Info).ToList();
      var doctors = _dbContext.Doctors.Select(p => new { p.Name, Doctors = p.DoctorProcedures.Select(dp => dp.Procedure.Name) }).ToList();

      return Ok(new { Appointments = appointments, Doctors = doctors });
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult<string> ConfirmAppointment(long id, byte[] rowVersion)
    {
      Response.ContentType = "application/json";

      try
      {
        var appointment = _dbContext.Appointments.Include(a => a.Doctor).Include(a => a.Info).ThenInclude(i => i.Procedure).SingleOrDefault(a => a.Id == id);

        if (appointment == null)
          return Ok("Fail");

        if (appointment.Status != "Unconfirmed")
          return Ok("Invalid status");

        _dbContext.Entry(appointment).State = EntityState.Detached;


        var ap = new Appointment { Id = id, RowVersion = rowVersion };  // new entity with rowVersion recieved from client to get correct concurrency check

        _dbContext.Appointments.Attach(ap);
        ap.Status = "Confirmed";
        _dbContext.SaveChanges();



        var start = DateTime.ParseExact(appointment.Date + " " + appointment.Time, "yyyy-MM-dd H:mm", CultureInfo.InvariantCulture);

        Event newEvent = new Event()
        {
          Summary = appointment.Info.Procedure.Name + " - " + appointment.Doctor.Name,
          Description = "Procedure: " + appointment.Info.Procedure.Name +
            "\nDoctor Name: " + appointment.Doctor.Name + "\nPatient Name: " + appointment.Info.PatientName,
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
      }
      catch (DbUpdateConcurrencyException)
      {
        return Ok("Fail");
      }      

      return Ok("Confirmed");
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult<string> RemoveAppointment(long id, byte[] rowVersion)
    {
      Response.ContentType = "application/json";

      try
      {
        var appointment = new Appointment { Id = id, RowVersion = rowVersion };
        _dbContext.Appointments.Remove(appointment);
        _dbContext.SaveChanges();
      }
      catch (DbUpdateConcurrencyException)
      {
        return Ok("Fail");
      }

      return Ok("Removed");
    }

    [HttpGet]
    public ActionResult<dynamic> GetFilteredDb()
    {
      var appointments = _dbContext.Appointments.Include(d => d.Doctor).Where(a => a.Status == "Free").ToList();
      var procedures = _dbContext.Procedures.Select(p => new { p.Name, Doctors = p.DoctorProcedures.Select(dp => dp.Doctor.Name) }).ToList();

      return Ok(new { Appointments = appointments, Procedures = procedures });
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
        var appointment = new Appointment { Id = info.id, RowVersion = info.rowVersion,
          Info = new Information { PatientName = info.patient, Procedure = _dbContext.Procedures.Single(p => p.Name == info.procedure) } };

        _dbContext.Appointments.Attach(appointment);
        appointment.Status = "Unconfirmed";
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
