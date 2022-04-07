using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RitaBot
{
    public class MainWindowViewModel : NotificationObject
    {
        [DllImport("winmm.dll")]
        public static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);
        [DllImport("winmm.dll", SetLastError = true)]
        public static extern bool PlaySound(string pszSound, IntPtr hmod, uint fdwSound);

        
        private Driver      _driver;

        public Driver Driver
        {
            get => _driver;
            set
            {
                _driver = value;
                RaisePropertyChanged(() => Driver);
            }
        }

        private string _status;
        private bool   _work;

        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                RaisePropertyChanged(() => Status);
            }
        }

        public Thread WorkThread { get; set; }

        public bool Work
        {
            get => _work;
            set
            {
                _work = value; 
                RaisePropertyChanged(() => Work);
                IsEnabled = !_work;
            }
        }

        private bool _isEnabled;

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                RaisePropertyChanged(() => IsEnabled);
            }
        }

        private int _delay;

        public int Delay
        {
            get => _delay;
            set
            {
                _delay = value;
                RaisePropertyChanged(() => Delay);
            }
        }


        public DelegateCommand TestCmd  { get; }
        public DelegateCommand StartCmd { get; }
        public DelegateCommand StopCmd  { get; }

        public MainWindowViewModel()
        {
            waveOutSetVolume(IntPtr.Zero, 0x08880888);
            g.MainVM = this;
            Logger.Init();
            ThreadPool.QueueUserWorkItem(state =>
                                         {
                                             Driver = new Driver();
                                             Driver.Init();
                                             Driver.Chrome.Navigate().GoToUrl("https://auction.tdera.ru/#/");
                                         });

            TestCmd   = new DelegateCommand(OnTest);
            StartCmd  = new DelegateCommand(OnStart, () => !Work);
            StopCmd   = new DelegateCommand(OnStop,  () => Work);
            Delay     = 5;
            IsEnabled = true;
            Status    = "Простаивает";
            RaiseCanExecChange();
        }

        public void RaiseCanExecChange()
        {
            StartCmd.RaiseCanExecuteChanged();
            StopCmd.RaiseCanExecuteChanged();
        }

        private void ThFunc()
        {
            g.FavIds.Clear();
            var favs = Driver.GetFavs();
            foreach (var x in favs)
                g.FavIds.Add(x.Code.ToString());

            var errCounter = 0;
            while (Work)
            {
                var works      = new List<WorkClass>();
                var data       = Driver.Get();
                if (data != null)
                {
                    errCounter = 0;
                    foreach (var x in data)
                        works.Add(new WorkClass(x));
                    works = works.Where(x => x.CanRegister).Where(x => g.FavIds.Contains(x.ShopCode)).Where(x => !g.UsedIds.Contains(x.Id)).ToList();
                    //works = works.Where(x => !g.UsedIds.Contains(x.Id)).ToList();
                    if (works.Count != 0)
                    {
                        g.TLC.SendTextMessageAsync(g.Chat, "====================").GetAwaiter().GetResult();
                        foreach (var x in works)
                        {
                            g.TLC.SendTextMessageAsync(g.Chat, x.ToString()).GetAwaiter().GetResult();
                            g.UsedIds.Add(x.Id);
                        }
                    }
                }
                else
                {
                    errCounter++;
                    Logger.Error($"Error {errCounter}");
                    if (errCounter > 5)
                    {
                        Logger.Error("Too many errors!");
                        Status = "Error! Restart and relogin";
                        Work   = false;
                        PlaySound(@"customhitsound.wav", IntPtr.Zero, 0x2001);
                        Thread.Sleep(100);
                        PlaySound(@"customhitsound.wav", IntPtr.Zero, 0x2001);
                        Thread.Sleep(100);
                        PlaySound(@"customhitsound.wav", IntPtr.Zero, 0x2001);
                        Thread.Sleep(100);
                        PlaySound(@"customhitsound.wav", IntPtr.Zero, 0x2001);
                        Thread.Sleep(100);
                        PlaySound(@"customhitsound.wav", IntPtr.Zero, 0x2001);
                        Thread.Sleep(100);
                        PlaySound(@"customhitsound.wav", IntPtr.Zero, 0x2001);
                        Thread.Sleep(100);
                        PlaySound(@"customhitsound.wav", IntPtr.Zero, 0x2001);
                        Thread.Sleep(100);
                        PlaySound(@"customhitsound.wav", IntPtr.Zero, 0x2001);
                        Thread.Sleep(100);
                        PlaySound(@"customhitsound.wav", IntPtr.Zero, 0x2001);
                        Thread.Sleep(100);
                        PlaySound(@"customhitsound.wav", IntPtr.Zero, 0x2001);
                    }
                }

                Thread.Sleep(Delay * 1000);
            }
        }

        private void OnStart()
        {
            if(Work) return;
            WorkThread = new Thread(ThFunc);
            Work       = true;
            WorkThread.Start();
            Status = "Работает";
            RaiseCanExecChange();
        }

        private void OnStop()
        {
            if(!Work) return;
            Work   = false;
            Status = "Простаивает";
            RaiseCanExecChange();
        }

        private void OnTest()
        {
            PlaySound(@"customhitsound.wav", IntPtr.Zero, 0x2001);
            Thread.Sleep(100);
            PlaySound(@"customhitsound.wav", IntPtr.Zero, 0x2001);
            Thread.Sleep(100);
            PlaySound(@"customhitsound.wav", IntPtr.Zero, 0x2001);
            Thread.Sleep(100);
            PlaySound(@"customhitsound.wav", IntPtr.Zero, 0x2001);
            Thread.Sleep(100);
            PlaySound(@"customhitsound.wav", IntPtr.Zero, 0x2001);
            Thread.Sleep(100);
            PlaySound(@"customhitsound.wav", IntPtr.Zero, 0x2001);
            Thread.Sleep(100);
            PlaySound(@"customhitsound.wav", IntPtr.Zero, 0x2001);
            Thread.Sleep(100);
            PlaySound(@"customhitsound.wav", IntPtr.Zero, 0x2001);
            Thread.Sleep(100);
            PlaySound(@"customhitsound.wav", IntPtr.Zero, 0x2001);
            Thread.Sleep(100);
            PlaySound(@"customhitsound.wav", IntPtr.Zero, 0x2001);
        }
    }
}
