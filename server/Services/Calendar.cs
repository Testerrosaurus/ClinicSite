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
      using (var stream = new FileStream("Data/server_server_cred.json", FileMode.Open, FileAccess.Read))
      {
        credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
      }



      Service = new CalendarService(new BaseClientService.Initializer()
      {
        HttpClientInitializer = credential,
        ApplicationName = ApplicationName,
      });

      CalendarId = File.ReadAllText("Data/calendar_id.txt");
    }
  }
}
