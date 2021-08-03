using System;
using System.IO;
using System.Threading;
using System.Net;
using System.Net.Http;
using Telegram.Bot;
using Telegram.Bot.Args;
using GiphyDotNet.Manager;
using GiphyDotNet.Model.Parameters;
using GiphyDotNet.Model.Results;

namespace SummonBot {
  class Program {
    static ITelegramBotClient botClient;
    private static readonly AutoResetEvent _closing = new AutoResetEvent(false);

  public static class APIKeys 
  {
  public static string discordURLKey = Environment.GetEnvironmentVariable("DiscordURL");
  public static string TelegramBotClientKey = Environment.GetEnvironmentVariable("TelegramBotKey");
  public static string imageLinkKey = Environment.GetEnvironmentVariable("ImageLink");
  public static string GiphyKey = Environment.GetEnvironmentVariable("GiphyKey");
  }
    static void Main() {
      botClient = new TelegramBotClient (APIKeys.TelegramBotClientKey);

      var me = botClient.GetMeAsync().Result;
      Console.WriteLine(
        $"Hello, World! I am user {me.Id} and my name is {me.FirstName}."
      );

      botClient.OnMessage += Bot_OnMessage;
      botClient.StartReceiving();

      Console.WriteLine("Press Ctrl-C to exit");
      Console.CancelKeyPress += new ConsoleCancelEventHandler(OnExit);
      _closing.WaitOne();

      botClient.StopReceiving();
    }

    static async void giphySearch(string searchText, bool isSticker, MessageEventArgs e) {
      var giphy = new Giphy (APIKeys.GiphyKey);
      var searchParameter = new SearchParameter() { Query = searchText };

      GiphySearchResult gifResult;

      if (isSticker)
      {
        gifResult = await giphy.StickerSearch(searchParameter);

      }
      else
      {
        gifResult = await giphy.GifSearch(searchParameter);
      }

      await botClient.SendAnimationAsync(
        chatId: e.Message.Chat,
        animation: gifResult.Data[0].Images.Original.Url);
      
    }

  static async void randGiphy(string searchText, bool isSticker, MessageEventArgs e) {
      var giphy = new Giphy (APIKeys.GiphyKey);
      RandomParameter searchParameter = new RandomParameter() { Tag = searchText };

      GiphyRandomResult gifResult;

      if (isSticker)
      {
        gifResult = await giphy.RandomSticker(searchParameter);

      }
      else
      {
        gifResult = await giphy.RandomGif(searchParameter);
      }

      await botClient.SendAnimationAsync(
        chatId: e.Message.Chat,
        animation: gifResult.Data.ImageUrl);

    }

    static void postImgToDiscord(string message) {
      string imageLink = APIKeys.imageLinkKey;
      string discordUrl = APIKeys.discordURLKey;
      
      using (HttpClient httpClient = new HttpClient())
      {
        using (WebClient webClient = new WebClient())
        {
          MultipartFormDataContent form = new MultipartFormDataContent();
          var file_bytes = webClient.DownloadData(imageLink);
          form.Add(new ByteArrayContent(file_bytes, 0, file_bytes.Length), "embeds", "elmo.gif");
          form.Add(new StringContent(message), "content");
          httpClient.PostAsync(discordUrl, form).Wait();
          httpClient.Dispose();
        }
      }
    }

    static async void Bot_OnMessage(object sender, MessageEventArgs e) {
        if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
        {
            int delimiter = -1;
            string query;

            if(e.Message.Text.StartsWith("/")) delimiter = e.Message.Text.IndexOf(" ");

            if (e.Message.Text.StartsWith("/elmo"))
            {
                string summonMsg = e.Message.From.FirstName + " has summoned the group!";
                
                await botClient.SendAnimationAsync(
                  chatId: e.Message.Chat,
                  animation: APIKeys.imageLinkKey,
                  caption: summonMsg);

                postImgToDiscord(summonMsg);
            }
            else if (e.Message.Text.StartsWith("/gif"))
            {
              if(delimiter != -1) query = e.Message.Text.Substring(delimiter);
              else query = "Rick Roll";

              giphySearch(query, false, e);
            }
            else if (e.Message.Text.StartsWith("/sticker"))
            {
              if(delimiter != -1) query = e.Message.Text.Substring(delimiter);
              else query = "Rick Roll";

              giphySearch(query, true, e);
            }
            else if (e.Message.Text.StartsWith("/randsticker"))
            {
              if(delimiter != -1) query = e.Message.Text.Substring(delimiter);
              else  query = "";

              randGiphy(query, true, e);
            }
            else if (e.Message.Text.StartsWith("/rand"))
            {
              if(delimiter != -1)  query = e.Message.Text.Substring(delimiter);
              else query = "";

              randGiphy(query, false, e);
            }
        }
    }

    protected static void OnExit(object sender, ConsoleCancelEventArgs args)
    {
      Console.WriteLine("Exit");
      _closing.Set();
    }
  }
}