using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;

using Newtonsoft.Json;

namespace MoePic.Models
{
    public class Settings
    {

        public static Settings Current
        {
            get;
            private set;
        }

        public bool IsWifiOnly { get; set; }

        public int PeerNum { get; set; }

        public String Website { get; set; }

        [JsonIgnore]
        public MoePic.Controls.Logos WebSiteLogo
        {
            get
            {
                switch (Website)
                {
                    case "Y":
                        return Controls.Logos.Yandere;
                    case "K":
                        return Controls.Logos.Konachan;
                    default:
                        break;
                }
                return Controls.Logos.Yandere;
            }
        }

        public int NewLastPostK { get; set; }

        public int NewLastPostY { get; set; }

        public int LastPostK { get; set; }

        public bool HideTags { get; set; }

        public bool AutoHideTags
        {
            get;
            set;
        }

        public int LastPostY { get; set; }

        public String VisionString { get; set; }

        public int ThemeColorUnlock { get; set; }

        public String Rating
        {
            get;
            set;
        }

        public bool EnableCDN { get; set; }

        public String NameFormat { get; set; }

        public int Limit { get; set; }

        public bool SaveRAM { get; set; }

        public ThemeColorsEnum ThemeColor {get;set;}

        public static void ReadSettings()
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();

            if (file.FileExists("Settings.json"))
            {
                IsolatedStorageFileStream stream = file.OpenFile("Settings.json", System.IO.FileMode.Open);
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, (int)stream.Length);
                String jsonString = Encoding.UTF8.GetString(buffer, 0, (int)stream.Length);
                if (jsonString == "")
                {
                    Init();
                }
                else
                {

                    Current = JsonConvert.DeserializeObject<Settings>(jsonString);

                    if(Current.VisionString != "0.4.4.0")
                    {
                        ToastService.Show("设定档已过期,将初始化设定档.");
                        Init();
                    }
                    else
                    {
                        if(!(App.Current as App).IsTrial)
                        {
                            int i = 0;
                            while (true)
                            {
                                if (!IsolatedStorageSettings.ApplicationSettings.Contains((++i).ToString()))
                                {
                                    break;
                                }
                            }
                            Current.ThemeColorUnlock = Math.Max(Current.ThemeColorUnlock, i);
                            if (Current.ThemeColorUnlock < 8)
                            {
                                if (IsolatedStorageSettings.ApplicationSettings.Contains(Current.ThemeColorUnlock.ToString()))
                                {
                                    if (DateTime.Now.Date - (DateTime)IsolatedStorageSettings.ApplicationSettings[Current.ThemeColorUnlock.ToString()] >= new TimeSpan(1, 0, 0, 0))
                                    {
                                        Current.ThemeColorUnlock++;
                                        IsolatedStorageSettings.ApplicationSettings.Add(Current.ThemeColorUnlock.ToString(), DateTime.Now.Date);
                                        IsolatedStorageSettings.ApplicationSettings.Save();
                                        ToastService.Show("已解锁新主题色.");
                                    }
                                }
                                else
                                {
                                    IsolatedStorageSettings.ApplicationSettings.Add(Current.ThemeColorUnlock.ToString(), DateTime.Now.Date);
                                    IsolatedStorageSettings.ApplicationSettings.Save();
                                }
                            }
                            (App.Current.Resources["ThemeColor"] as ThemeColor).Init(Current.ThemeColor);
                        }
                        else
                        {
                            Current.ThemeColor = ThemeColorsEnum.Cyan;
                            Current.ThemeColorUnlock = 0;
                            Current.AutoHideTags = false;
                            Current.EnableCDN = false;
                            Models.Settings.Current.Password = null;
                        }
                    }

                    if (!Current.IsExAble && (DateTime.Now - Current.FirstLaunchTime > new TimeSpan(5, 0, 0, 0) || Current.Rating == "e"))
                    {
                        Current.IsExAble = true;
                        ToastService.Show("已解锁新分级.");
                    }

                    if (Current.IsExAble && !Current.IsExOnlyAble && (DateTime.Now - Current.FirstLaunchTime > new TimeSpan(14, 0, 0, 0) || Current.Rating == "o"))
                    {
                        Current.IsExOnlyAble = true;
                        ToastService.Show("已解锁ExOnly分级.");
                    }

                    (App.Current.Resources["RuntimeResources"] as RuntimeResources).NameFormat = Current.NameFormat;
                }
                stream.Close();
            }
            else
            {
                Init();
            }
            if(Current.Website == "Y")
            {
                MoebooruAPI.SetWebsiteY();
            }
            else
            {
                MoebooruAPI.SetWebsiteK();
            }

            if(Current.UserK != null)
            {

                MoebooruAPI.userK = Current.UserK.name;
                MoebooruAPI.passwordHashK = Current.UserKHash;
            }
            if (Current.UserY != null)
            {

                MoebooruAPI.userY = Current.UserY.name;
                MoebooruAPI.passwordHashY = Current.UserYHash;
            }
        }

        public String UserYHash { get; set; }
        public String UserKHash { get; set; }

        public MoePost[] BookMarkK { get; set; }
        public MoePost[] BookMarkY { get; set; }

        public MoeUser UserY { get; set; }
        public MoeUser UserK { get; set; }

        public ImageType DefaultType { get; set; }

        public String Password { get; set; }

        public bool NeedPasswordActive { get; set; }

        public int YLast { get; set; }

        public int KLast { get; set; }

        public bool AddFavSnyc { get; set; }

        public DateTime FirstLaunchTime { get; set; }

        public bool IsExAble { get; set; }
        public bool IsExOnlyAble { get; set; }

        public static void SaveSetting()
        {
            if(Current.NewLastPostK >0)
            {
                Current.LastPostK = Current.NewLastPostK;
            }
            if (Current.NewLastPostY > 0)
            {
                Current.LastPostY = Current.NewLastPostY;
            }
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream stream = file.OpenFile("Settings.json", System.IO.FileMode.Create);
            byte[] buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Current));
            stream.Write(buffer, 0, buffer.Length);
            stream.Close();
        }

        public static void Init()
        {
            Current = new Settings()
            {
                Website = "K",
                Rating = "s",
                Limit = 16,
                SaveRAM = false,
                YLast = -1,
                KLast = -1,
                LastPostK = -1,
                LastPostY = -1,
                VisionString = "0.4.4.0",
                UserY = null,
                UserK = null,
                UserYHash = null,
                UserKHash = null,
                NameFormat = "{FN} {LN}",
                PeerNum = 2,
                IsWifiOnly = true,
                DefaultType = ImageType.JPG,
                Password = null,
                NeedPasswordActive = true,
                BookMarkK = new MoePost[6],
                BookMarkY = new MoePost[6],
                AddFavSnyc = false,
                FirstLaunchTime = DateTime.Now,
                IsExAble = false,
                IsExOnlyAble = false,
                ThemeColor = ThemeColorsEnum.Cyan,
                ThemeColorUnlock = 0,
                HideTags = false,
                AutoHideTags = false,
                EnableCDN = false
            };
            (App.Current.Resources["RuntimeResources"] as RuntimeResources).NameFormat = Current.NameFormat;
        }
        
        public Settings()
        {

        }

        
    }
}
