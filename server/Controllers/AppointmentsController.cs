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

    private string StringDate(DateTime dt)
    {
      return dt.Year + "-" + dt.Month.ToString("00") + "-" + dt.Day.ToString("00");
    }

    private string StringTime(DateTime dt)
    {
      return dt.Hour.ToString("00") + ":" + dt.Minute.ToString("00");
    }

    [Authorize]
    [HttpGet]
    public IActionResult GetAppointments()
    {
      var appointments = _dbContext.Appointments.AsNoTracking().Include(a => a.Doctor).Include(a => a.Info).Select(a => new {
        Id = a.Id,
        RowVersion = a.RowVersion,
        Status = a.Status,
        Doctor = a.Doctor.Name,
        Start = StringDate(a.Start) + " " + StringTime(a.Start),
        Date = StringDate(a.Start),
        Time = StringTime(a.Start),
        Duration = (a.End - a.Start).TotalMinutes,
        Created = StringDate(a.Created) + " " + StringTime(a.Created)
      });

      var doctors = _dbContext.Doctors.Select(p => new { p.Name }).ToList();

      return Ok(new { Appointments = appointments, Doctors = doctors });
    }

    public struct AInfo
    {
      public long id;
      public byte[] rowVersion;
      public string date;
      public string time;
      public int duration;
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmAppointment([FromBody]AInfo info)
    {
      try
      {
        var appointment = await _dbContext.Appointments.AsNoTracking().Include(a => a.Doctor).Include(a => a.Info).SingleOrDefaultAsync(a => a.Id == info.id);

        if (appointment == null)
          return Ok("Fail");

        bool update = false;
        if (appointment.Status == "Confirmed")
          //return Ok("Invalid status");
          update = true;


        appointment.RowVersion = info.rowVersion;
        appointment.Status = "Confirmed";

        var start = DateTime.ParseExact(info.date + " " + info.time, "yyyy-MM-dd HH:mm", null);
        appointment.Start = start;
        appointment.End = start.AddMinutes(info.duration);

        _dbContext.Appointments.Update(appointment);
        _dbContext.SaveChanges();




        Event newEvent = new Event()
        {
          Summary = appointment.Info.PatientName + " - " + appointment.Doctor.Name,
          Description = "Procedure: " + appointment.Info.AdditionalInfo +
            "\nDoctor Name: " + appointment.Doctor.Name + "\nPatient Name: " + appointment.Info.PatientName,
          //Attendees = new EventAttendee[]
          //{
          //  new EventAttendee{ DisplayName = info.patient, Email = "" },
          //  new EventAttendee{ DisplayName = info.doctor, Email = "" },
          //},
          Start = new EventDateTime()
          {
            DateTime = appointment.Start,
          },
          End = new EventDateTime()
          {
            DateTime = appointment.End,
          }
        };

        if (update)
        {
          var calendarId = appointment.CalendarId;
          var request = _calendar.Service.Events.Update(newEvent, _calendar.CalendarId, calendarId);
          Event createdEvent = request.Execute();
        }
        else
        {
          var request = _calendar.Service.Events.Insert(newEvent, _calendar.CalendarId);
          Event createdEvent = await request.ExecuteAsync();

          appointment.CalendarId = createdEvent.Id;
          _dbContext.Appointments.Update(appointment);
          _dbContext.SaveChanges();
        }
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
    public async Task<IActionResult> RemoveAppointment([FromBody]AInfo info)
    {
      try
      {
        var appointment = await _dbContext.Appointments.AsNoTracking().Include(a => a.Doctor).Include(a => a.Info).SingleOrDefaultAsync(a => a.Id == info.id);

        if (appointment == null)
          return Ok("Fail");

        appointment.RowVersion = info.rowVersion;

        _dbContext.Appointments.Remove(appointment);
        _dbContext.SaveChanges();

        if (appointment.Status == "Confirmed")
        {
          var calendarId = appointment.CalendarId;
          var request = _calendar.Service.Events.Delete(_calendar.CalendarId, calendarId);
          await request.ExecuteAsync();
        }
      }
      catch (DbUpdateConcurrencyException)
      {
        return Ok("Fail");
      }

      return Ok("Removed");
    }

    private (bool, DateTime) IsAvailable(DateTime dt, List<Appointment> appointments, long doctorId)
    {
      var start = dt;
      var end = dt.AddMinutes(30);

      foreach (var a in appointments)
      {
        if (doctorId == a.Doctor.Id && a.End > start.AddSeconds(1) && a.Start < end.AddSeconds(-1))
          return (false, a.End);
      }

      return (true, new DateTime());
    }

    [HttpGet]
    public IActionResult GetAvailableDateTimes()
    {
      var allDateTimes = new Dictionary<string, List<dynamic>>();

      var freeTimes = _dbContext.FreeTimes.AsNoTracking().Include(ft => ft.Doctor);
      var appointments = _dbContext.Appointments.AsNoTracking().Include(a => a.Doctor).Where(a => a.Status == "Confirmed").ToList();

      foreach (var ft in freeTimes)
      {
        List<dynamic> dateTimes;
        if (allDateTimes.ContainsKey(ft.Doctor.Name))
          dateTimes = allDateTimes[ft.Doctor.Name];
        else
          dateTimes = new List<dynamic>();

        int count = 0;
        var dt = ft.Start;
        while (dt.AddMinutes(30) < ft.End.AddSeconds(1))
        {
          var check = IsAvailable(dt, appointments, ft.Doctor.Id);
          if (!check.Item1)
          {
            //dt = dt.AddMinutes(30);

            dt = (check.Item2.Minute % 30 == 0) ? check.Item2 : check.Item2.AddMinutes(30 - check.Item2.Minute % 30);
            continue;
          }

          dateTimes.Add(new { Date = StringDate(dt), Time = StringTime(dt), EndTime = StringTime(dt.AddMinutes(30)) });
          dt = dt.AddMinutes(30);

          if (++count > 24 * 60 / 30) break;
        }

        allDateTimes[ft.Doctor.Name] = dateTimes;
      }

      return Ok(allDateTimes);
    }

    public struct AppointmentInfo
    {
      public string patient;
      public string doctor;
      public string date;
      public string time;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SetAppointment([FromBody]AppointmentInfo info)
    {
      if (info.patient == "" || info.doctor == "" || info.date == "" || info.time == "")
        return Ok("Invalid info");

      try
      {
        var start = DateTime.ParseExact(info.date + " " + info.time, "yyyy-MM-dd HH:mm", null);

        var appointment = new Appointment {
          Doctor = _dbContext.Doctors.First(d => d.Name == info.doctor),
          Start = start,
          End = start.AddMinutes(30),
          Info = new Information { PatientName = info.patient },
          Status = "Unconfirmed",
          Created = DateTime.UtcNow.AddHours(3)
        };

        _dbContext.Appointments.Attach(appointment);
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
