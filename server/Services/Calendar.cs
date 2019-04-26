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

namespace server.Services
{
  public class Calendar
  {
    public string CalendarId { get; }
    public CalendarService Service { get; }


    public Calendar()
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
    }
  }
}
