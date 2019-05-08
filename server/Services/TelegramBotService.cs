using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace server.Services
{
  public class TelegramBotService
  {
    public TelegramBotClient Client { get; }

    public TelegramBotService()
    {
      string appDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName;
      if (!Directory.Exists(appDir + "/Data")) // built in bin/{debug/release}/{version}/
      {
        appDir = appDir + "/../../..";
      }

      string telegram_bot_token = System.IO.File.ReadAllText(appDir + "/Data/telegram_bot_token.json");
      string site_base_url = System.IO.File.ReadAllText(appDir + "/Data/site_base_url.json");


      Client = new TelegramBotClient(telegram_bot_token);
      Client.SetWebhookAsync("https://" + site_base_url + "/api/telegram").Wait();
    }
  }
}
