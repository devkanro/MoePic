using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using MoePic.Models;
using MoePic.Controls;

namespace MoePic
{
    public partial class UserPage : Models.MoePicPage
    {
        public UserPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New || e.NavigationMode == NavigationMode.Back)
            {
                UpdataLoginStatus();
            }
            if(settingsSnycing)
            {
                SettingsSync.Begin();
            }
            if(favSnycing)
            {
                FavSync.Begin();
            }
            if(YSnycing)
            {
                RefreshY.Begin();
            }
            if (KSnycing)
            {
                RefreshK.Begin();
            }
            StatusBarView view = new Controls.StatusBarView(Controls.Logos.MoePic, MoePic.Resources.AppResources.Me, false, true);
            StatusBarService.Init(view);
            StatusBarService.Change(view);
            base.OnNavigatedTo(e);
        }

        bool msLogin = false;
        bool yLogin = false;
        bool kLogin = false;

        bool firstLoad = true;

        void UpdataLoginStatus()
        {
            if (msLogin != (App.Current.Resources["RuntimeResources"] as RuntimeResources).IsLogin || firstLoad)
            {
                if ((App.Current.Resources["RuntimeResources"] as RuntimeResources).IsLogin)
                {
                    if ((App.Current.Resources["RuntimeResources"] as RuntimeResources).NameFormat != "{FN} {LN}")
                    {
                        userName.Text = String.Format("({0} {1})", (App.Current.Resources["RuntimeResources"] as RuntimeResources).FirstName, (App.Current.Resources["RuntimeResources"] as RuntimeResources).LastName);
                    }
                    else
                    {
                        userName.Text = "";
                    }
                    Login.Visibility = System.Windows.Visibility.Collapsed;
                    Logout.Visibility = System.Windows.Visibility.Visible;
                    noImage.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    showName.Text = MoePic.Resources.AppResources.NotLogin;
                    userName.Text = "";
                    ProfilePhoto.Source = null;
                    Login.Visibility = System.Windows.Visibility.Visible;
                    Logout.Visibility = System.Windows.Visibility.Collapsed;
                    noImage.Visibility = System.Windows.Visibility.Visible;
                }

                msLogin = (App.Current.Resources["RuntimeResources"] as RuntimeResources).IsLogin;
            }

            if (kLogin != (Settings.Current.UserK != null && Settings.Current.UserKHash != null) || firstLoad)
            {
                if (Settings.Current.UserK != null && Settings.Current.UserKHash != null)
                {
                    konachanData.Visibility = System.Windows.Visibility.Visible;
                    meK.Text = Settings.Current.UserK.name;
                    meK.ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri(MoebooruAPI.GetUserAvatar(Settings.Current.UserK.id, MoebooruAPI.Konachan)));
                    meK.Visibility = System.Windows.Visibility.Visible;

                    KLogin.Visibility = System.Windows.Visibility.Collapsed;
                    KLogout.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {

                    konachanData.Visibility = System.Windows.Visibility.Collapsed;
                    KLogin.Visibility = System.Windows.Visibility.Visible;
                    KLogout.Visibility = System.Windows.Visibility.Collapsed;
                }

                kLogin = (Settings.Current.UserK != null && Settings.Current.UserKHash != null);
            }

            if (yLogin != (Settings.Current.UserY != null && Settings.Current.UserYHash != null) || firstLoad)
            {
                if (Settings.Current.UserY != null && Settings.Current.UserYHash != null)
                {

                    yandereData.Visibility = System.Windows.Visibility.Visible;
                    meY.Text = Settings.Current.UserY.name;
                    meY.ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri(MoebooruAPI.GetUserAvatar(Settings.Current.UserY.id, MoebooruAPI.Yandere)));
                    meY.Visibility = System.Windows.Visibility.Visible;

                    YLogin.Visibility = System.Windows.Visibility.Collapsed;
                    YLogout.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    yandereData.Visibility = System.Windows.Visibility.Collapsed;
                    YLogin.Visibility = System.Windows.Visibility.Visible;
                    YLogout.Visibility = System.Windows.Visibility.Collapsed;
                }

            }
            firstLoad = false;
            yLogin = (Settings.Current.UserY != null && Settings.Current.UserYHash != null);
        }

        private void ProfilePhoto_ImageOpened(object sender, RoutedEventArgs e)
        {
            PhotoOk.Begin();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(Logout.Visibility == System.Windows.Visibility.Visible)
            {
                    LiveAPI.Logout();
                    UpdataLoginStatus();
            }
            else
            {
                if((App.Current as App).IsTrial)
                {
                    Models.MessageBoxService.Show("无法关联微软账户", "购买正式版以解锁关联微软账户功能?", true, new Controls.Command("购买", (s, a) =>
                    {
                        new Microsoft.Phone.Tasks.MarketplaceDetailTask()
                        {
                            ContentIdentifier = "58755b21-9c40-4c17-9d39-b1b73e595ffd"
                        }.Show();
                    }), new Controls.Command("取消", null));
                }
                else
                {
                    Models.NavigationService.Navigate("LoginPage.xaml", "notNull");
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (YLogout.Visibility == System.Windows.Visibility.Visible)
            {
                MoebooruAPI.passwordHashY = null;
                MoebooruAPI.userY = null;
                Settings.Current.UserY = null;
                Settings.Current.UserYHash = null;
                UpdataLoginStatus();
            }
            else
            {
                Models.NavigationService.Navigate("YandereLoginPage.xaml");
            }
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (KLogout.Visibility == System.Windows.Visibility.Visible)
            {
                MoebooruAPI.passwordHashK = null;
                MoebooruAPI.userK = null;
                Settings.Current.UserK = null;
                Settings.Current.UserKHash = null;
                UpdataLoginStatus();
            }
            else
            {
                Models.NavigationService.Navigate("KonachanLoginPage.xaml");
            }
        }

        static bool settingsSnycing = false;
        static bool favSnycing = false;

        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (!settingsSnycing)
            {
                Settings.SaveSetting();
                settingsSnycing = true;
                SettingsSync.Begin();

                String id = await LiveAPI.GetFolderIdFormPath("MoePic");
                if (id == null)
                {

                    bool createok = await LiveAPI.CreateFolder("MoePic", MoePic.Resources.AppResources.MoeFolder);
                    if (!createok)
                    {
                        ToastService.Show(MoePic.Resources.AppResources.CreatFolderFail);
                        return;
                    }
                    else
                    {
                        IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
                        IsolatedStorageFileStream stream = file.OpenFile("Settings.json", System.IO.FileMode.Open);
                        bool updataOk = await LiveAPI.UploadFile("MoePic", "Settings", stream);
                        if(updataOk)
                        {
                            ToastService.Show(MoePic.Resources.AppResources.UploadSettingOK);
                        }
                        else
                        {
                            ToastService.Show(MoePic.Resources.AppResources.UploadSettingFail);
                        }
                        stream.Close();
                    }

                    settingsSnycing = false;
                }
                else
                {
                    List<LiveFile> fileList = await LiveAPI.GetFilesListFormId(id);
                    if(fileList == null || fileList.Count == 0 || fileList.Find((f) => {return f.name == "Settings" ? true : false;}) == null)
                    {
                        IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
                        IsolatedStorageFileStream stream = file.OpenFile("Settings.json", System.IO.FileMode.Open);
                        bool updataOk = await LiveAPI.UploadFileUseId(id, "Settings", stream);
                        if (updataOk)
                        {
                            ToastService.Show(MoePic.Resources.AppResources.UploadSettingOK);
                        }
                        else
                        {
                            ToastService.Show(MoePic.Resources.AppResources.UploadSettingFail);
                        }
                        stream.Close();

                        settingsSnycing = false;
                    }
                    else
                    {
                        MessageBoxService.Show(MoePic.Resources.AppResources.SettingConflictTitle, MoePic.Resources.AppResources.SettingConflictContent, true, new Command(MoePic.Resources.AppResources.Upload, async (s, a) =>
                            {
                                IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
                                IsolatedStorageFileStream stream = file.OpenFile("Settings.json", FileMode.Open);
                                bool updataOk = await LiveAPI.UploadFileUseId(id, "Settings", stream);
                                if (updataOk)
                                {
                                    ToastService.Show(MoePic.Resources.AppResources.UploadSettingOK);
                                }
                                else
                                {
                                    ToastService.Show(MoePic.Resources.AppResources.UploadSettingFail);
                                }
                                stream.Close();

                                settingsSnycing = false;
                            }), new Command(MoePic.Resources.AppResources.Download, async(s,a) => 
                            {
                                Stream stream = await LiveAPI.DownloadFileUseId(id,"Settings");
                                if(stream != null)
                                {
                                    stream.Seek(0, SeekOrigin.Begin);
                                    byte[] buff = new byte[stream.Length];
                                    stream.Read(buff, 0, (int)stream.Length);


                                    String jsonString = System.Text.Encoding.UTF8.GetString(buff, 0, (int)stream.Length);

                                    stream.Close();

                                    IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
                                    IsolatedStorageFileStream fileStream = file.OpenFile("Settings.json", System.IO.FileMode.Create);
                                    fileStream.Write(buff, 0, buff.Length);
                                    fileStream.Close();
                                    Settings.ReadSettings();
                                    ToastService.Show(MoePic.Resources.AppResources.DownloadSettingOK);
                                }
                                else
                                {
                                    ToastService.Show(MoePic.Resources.AppResources.DownloadSettingFail);
                                }
                                settingsSnycing = false;
                            }));
                    }
                }
            }
        }

        private void SettingsSync_Completed(object sender, EventArgs e)
        {
            if(settingsSnycing)
            {
                SettingsSync.Begin();
            }
        }

        private async void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if(!favSnycing)
            {
                favSnycing = true;
                FavoriteHelp.SaveFavorite();
                FavSync.Begin();

                String id = await LiveAPI.GetFolderIdFormPath("MoePic");
                if (id == null)
                {

                    bool createok = await LiveAPI.CreateFolder("MoePic", MoePic.Resources.AppResources.MoeFolder);
                    if (!createok)
                    {
                        ToastService.Show(MoePic.Resources.AppResources.CreatFolderFail);
                        return;
                    }
                    else
                    {
                        IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
                        IsolatedStorageFileStream stream = file.OpenFile("FavoriteList.json", System.IO.FileMode.Open);
                        bool updataOk = await LiveAPI.UploadFile("MoePic", "FavoriteList", stream);
                        if (updataOk)
                        {
                            ToastService.Show(MoePic.Resources.AppResources.UploadFavOK);
                        }
                        else
                        {
                            ToastService.Show(MoePic.Resources.AppResources.UploadFavFail);
                        }
                        stream.Close();
                    }

                    favSnycing = false;
                }
                else
                {
                    List<LiveFile> fileList = await LiveAPI.GetFilesListFormId(id);
                    if (fileList == null || fileList.Count == 0 || fileList.Find((f) => { return f.name == "FavoriteList" ? true : false; }) == null)
                    {
                        IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
                        IsolatedStorageFileStream stream = file.OpenFile("FavoriteList.json", System.IO.FileMode.Open);
                        bool updataOk = await LiveAPI.UploadFileUseId(id, "FavoriteList", stream);
                        if (updataOk)
                        {
                            ToastService.Show(MoePic.Resources.AppResources.UploadFavOK);
                        }
                        else
                        {
                            ToastService.Show(MoePic.Resources.AppResources.UploadFavFail);
                        }
                        stream.Close();

                        favSnycing = false;
                    }
                    else
                    {
                        MessageBoxService.Show(MoePic.Resources.AppResources.FavConflictTitle, MoePic.Resources.AppResources.FavConflictContent, true, new Command(MoePic.Resources.AppResources.Upload, async (s, a) =>
                        {
                            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
                            IsolatedStorageFileStream stream = file.OpenFile("FavoriteList.json", FileMode.Open);
                            bool updataOk = await LiveAPI.UploadFileUseId(id, "FavoriteList", stream);
                            if (updataOk)
                            {
                                ToastService.Show(MoePic.Resources.AppResources.UploadFavOK);
                            }
                            else
                            {
                                ToastService.Show(MoePic.Resources.AppResources.UploadFavFail);
                            }
                            stream.Close();

                            favSnycing = false;
                        }), new Command(MoePic.Resources.AppResources.Download, async (s, a) =>
                        {
                            Stream stream = await LiveAPI.DownloadFileUseId(id, "FavoriteList");
                            if (stream != null)
                            {
                                stream.Seek(0, SeekOrigin.Begin);
                                byte[] buff = new byte[stream.Length];
                                stream.Read(buff, 0, (int)stream.Length);


                                String jsonString = System.Text.Encoding.UTF8.GetString(buff, 0, (int)stream.Length);

                                stream.Close();

                                IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
                                IsolatedStorageFileStream fileStream = file.OpenFile("FavoriteList.json", System.IO.FileMode.Create);
                                fileStream.Write(buff, 0, buff.Length);
                                fileStream.Close();
                                FavoriteHelp.ReadFavorite();
                                ToastService.Show(MoePic.Resources.AppResources.DownloadFavOK);
                            }
                            else
                            {
                                ToastService.Show(MoePic.Resources.AppResources.DownloadFavFail);
                            }
                            favSnycing = false;
                        }), new Command(MoePic.Resources.AppResources.Merge, async (s, a) =>
                            {
                                Stream stream = await LiveAPI.DownloadFileUseId(id, "FavoriteList");
                                if (stream != null)
                                {
                                    stream.Seek(0, SeekOrigin.Begin);
                                    byte[] buff = new byte[stream.Length];
                                    stream.Read(buff, 0, (int)stream.Length);


                                    String jsonString = System.Text.Encoding.UTF8.GetString(buff, 0, (int)stream.Length);

                                    List<MoePost> postList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MoePost>>(jsonString);

                                    foreach (var item in postList)
                                    {
                                        if (!FavoriteHelp.ContainsPost(item))
                                        {
                                            FavoriteHelp.AddFavorite(item);
                                        }
                                    }
                                    FavoriteHelp.SaveFavorite();
                                    stream.Close();
                                    ToastService.Show(MoePic.Resources.AppResources.MergeFavOK);

                                }
                                else
                                {
                                    ToastService.Show(MoePic.Resources.AppResources.MergeFavFail);
                                }
                                favSnycing = false;
                            }));
                    }
                }
            }
        }

        private void FavSync_Completed(object sender, EventArgs e)
        {
            if(favSnycing)
            {
                FavSync.Begin();
            }
        }

        static bool YSnycing = false;
        static bool KSnycing = false;

        private async void Button_Click_5(object sender, RoutedEventArgs e)
        {
            if(!YSnycing)
            {
                YSnycing = true;
                RefreshY.Begin();
                if(Settings.Current.UserY != null && await MoebooruAPI.LoginY())
                {
                    ToastService.Show(new Uri(MoebooruAPI.GetUserAvatar(Settings.Current.UserY.id, MoebooruAPI.Yandere)), MoePic.Resources.AppResources.VerifyAccountOk);
                }
                else
                {
                    MoebooruAPI.passwordHashY = null;
                    MoebooruAPI.userY = null;
                    Settings.Current.UserY = null;
                    Settings.Current.UserYHash = null;
                    UpdataLoginStatus();
                    ToastService.Show(MoePic.Resources.AppResources.VerifyAccountFail);
                }
                YSnycing = true;
            }
        }

        private void RefreshY_Completed(object sender, EventArgs e)
        {
            if (YSnycing)
            {
                RefreshY.Begin();
            }
        }

        private async void Button_Click_6(object sender, RoutedEventArgs e)
        {
            if (!KSnycing)
            {
                KSnycing = true;
                RefreshK.Begin();
                if (Settings.Current.UserK != null && await MoebooruAPI.LoginK())
                {
                    ToastService.Show(new Uri(MoebooruAPI.GetUserAvatar(Settings.Current.UserK.id,MoebooruAPI.Konachan)), MoePic.Resources.AppResources.VerifyAccountOk);
                }
                else
                {
                    MoebooruAPI.passwordHashK = null;
                    MoebooruAPI.userK = null;
                    Settings.Current.UserK = null;
                    Settings.Current.UserKHash = null;
                    UpdataLoginStatus();
                    ToastService.Show(MoePic.Resources.AppResources.VerifyAccountFail);
                }
                KSnycing = false;
            }
        }

        private void RefreshK_Completed(object sender, EventArgs e)
        {
            if (KSnycing)
            {
                RefreshK.Begin();
            }
        }
    }
}