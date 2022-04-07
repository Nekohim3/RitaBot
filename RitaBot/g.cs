using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.ViewModel;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RitaBot
{
    public static class g
    {
        public static string              UserDataPath;
        public static MainWindowViewModel MainVM;
        public static List<int>           UsedIds = new List<int>();
        public static List<string>        FavIds  = new List<string>();
        public static TelegramBotClient   TLC;
        public static ChatId              Chat = new ChatId(1082852576);
        static g()
        {
            TLC          = new TelegramBotClient("5240710415:AAHEpsaDb3jCRyOQX2Ju7Xkg4x8tycew7b0");
            UserDataPath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            if (!Directory.Exists($"{UserDataPath}\\UserData"))
                Directory.CreateDirectory($"{UserDataPath}\\UserData");
            UserDataPath = $"{UserDataPath}\\UserData";
        }
    }
}
