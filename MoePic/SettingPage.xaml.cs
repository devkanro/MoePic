using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MoePic.Resources;
using System.IO;
using Microsoft.Phone.Tasks;

namespace MoePic
{
    public partial class SettingPage : Models.MoePicPage
    {
        public SettingPage()
        {
            InitializeComponent();
        }

        public List<ThemeColorItem> ThemeColorItems;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            MoePic.Controls.StatusBarView view = new MoePic.Controls.StatusBarView(Controls.Logos.Setting, AppResources.Settings, false, true);
            MoePic.Models.StatusBarService.Init(view);
            MoePic.Models.StatusBarService.Change(view);
            MoePic.Models.StatusBarService.ShowStatusBar();

            if(Models.NavigationService.GetNavigateArgs(this.NavigationContext) as String == "Feedback")
            {
                content.SelectedIndex = 2;
            }

            if(Models.Settings.Current.IsExAble)
            {
                rating.Maximum = 3;
            }
            else
            {
                rating.Maximum = 2;
            }

            if(Models.Settings.Current.IsExOnlyAble)
            {
                rating.Maximum = 4;
            }

            switch (MoePic.Models.Settings.Current.Rating)
            {
                case "s":
                    rating.Value = 0.5;
                    ratingText.Text = "Safe";
                    ratingInfo.Text = AppResources.SafeInfo;
                    break;
                case "q":
                    ratingText.Text = "Questionable";
                    rating.Value = 1.5;
                    ratingInfo.Text = AppResources.QuestionableInfo;
                    break;
                case "e":
                    ratingText.Text = "Explicit";
                    rating.Value = 2.5;
                    ratingInfo.Text = AppResources.ExplicitInfo;
                    break;
                case "o":
                    ratingText.Text = "ExplicitOnly";
                    rating.Value = 3.5;
                    ratingInfo.Text = "ExplicitOnly是只显示Explicit分级的图片的级别.";
                    break;
                default:
                    break;
            }
            limit.Value = MoePic.Models.Settings.Current.Limit;
            limitText.Text = MoePic.Models.Settings.Current.Limit.ToString();
            saveRAM.IsChecked = MoePic.Models.Settings.Current.SaveRAM;
            onlyWifi.IsChecked = MoePic.Models.Settings.Current.IsWifiOnly;
            peerNum.Value = MoePic.Models.Settings.Current.PeerNum;
            switch (Models.Settings.Current.DefaultType)
            {
                case MoePic.Models.ImageType.Sample:
                    defaultImageType.SelectedIndex = 0;
                    defaultImageTypeInfo.Text = AppResources.SampleImageInfo;
                    break;
                case MoePic.Models.ImageType.PNG:
                    defaultImageType.SelectedIndex = 2;
                    defaultImageTypeInfo.Text = AppResources.PNGImageInfo;
                    break;
                case MoePic.Models.ImageType.JPG:
                    defaultImageType.SelectedIndex = 1;
                    defaultImageTypeInfo.Text = AppResources.JpegImageInfo;
                    break;
                default:
                    break;
            }
            if(Models.Settings.Current.Password == null)
            {
                password.IsChecked = false;
                passwordSettigs.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                password.IsChecked = true;
                passwordSettigs.Visibility = System.Windows.Visibility.Visible;
            }
            needPW.SelectedIndex = Models.Settings.Current.NeedPasswordActive ? 0 : 1;
            addFavSnyc.IsChecked = Models.Settings.Current.AddFavSnyc;

            if((App.Current as App).IsTrial)
            {
                Models.Settings.Current.ThemeColorUnlock = 0;
            }

            if(e.NavigationMode == NavigationMode.New || ThemeColorItems == null)
            {
                switch (Models.Settings.Current.ThemeColorUnlock)
                {
                    case 0:
                        ThemeColorItems = new List<ThemeColorItem> { 
                        new ThemeColorItem(){ Name = "青色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Cyan.Foreground)}};
                        break;
                    case 1:
                        ThemeColorItems = new List<ThemeColorItem> { 
                        new ThemeColorItem(){ Name = "青色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Cyan.Foreground)},
                        new ThemeColorItem(){ Name = "粉色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Pink.Foreground)}
                    };
                        break;
                    case 2:
                        ThemeColorItems = new List<ThemeColorItem> { 
                        new ThemeColorItem(){ Name = "青色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Cyan.Foreground)},
                        new ThemeColorItem(){ Name = "粉色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Pink.Foreground)},
                        new ThemeColorItem(){ Name = "红色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Red.Foreground)}
                    };
                        break;
                    case 3:
                        ThemeColorItems = new List<ThemeColorItem> { 
                        new ThemeColorItem(){ Name = "青色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Cyan.Foreground)},
                        new ThemeColorItem(){ Name = "粉色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Pink.Foreground)},
                        new ThemeColorItem(){ Name = "红色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Red.Foreground)},
                        new ThemeColorItem(){ Name = "绿色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Green.Foreground)}
                    };
                        break;
                    case 4:
                        ThemeColorItems = new List<ThemeColorItem> { 
                        new ThemeColorItem(){ Name = "青色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Cyan.Foreground)},
                        new ThemeColorItem(){ Name = "粉色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Pink.Foreground)},
                        new ThemeColorItem(){ Name = "红色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Red.Foreground)},
                        new ThemeColorItem(){ Name = "绿色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Green.Foreground)},
                        new ThemeColorItem(){ Name = "橙色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Orange.Foreground)}
                    };
                        break;
                    case 5:
                        ThemeColorItems = new List<ThemeColorItem> { 
                        new ThemeColorItem(){ Name = "青色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Cyan.Foreground)},
                        new ThemeColorItem(){ Name = "粉色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Pink.Foreground)},
                        new ThemeColorItem(){ Name = "红色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Red.Foreground)},
                        new ThemeColorItem(){ Name = "绿色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Green.Foreground)},
                        new ThemeColorItem(){ Name = "橙色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Orange.Foreground)},
                        new ThemeColorItem(){ Name = "紫色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Violet.Foreground)}
                    };
                        break;
                    case 6:
                        ThemeColorItems = new List<ThemeColorItem> { 
                        new ThemeColorItem(){ Name = "青色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Cyan.Foreground)},
                        new ThemeColorItem(){ Name = "粉色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Pink.Foreground)},
                        new ThemeColorItem(){ Name = "红色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Red.Foreground)},
                        new ThemeColorItem(){ Name = "绿色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Green.Foreground)},
                        new ThemeColorItem(){ Name = "橙色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Orange.Foreground)},
                        new ThemeColorItem(){ Name = "紫色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Violet.Foreground)},
                        new ThemeColorItem(){ Name = "蓝色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Blue.Foreground)}
                    };
                        break;
                    case 7:
                        ThemeColorItems = new List<ThemeColorItem> { 
                        new ThemeColorItem(){ Name = "青色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Cyan.Foreground)},
                        new ThemeColorItem(){ Name = "粉色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Pink.Foreground)},
                        new ThemeColorItem(){ Name = "红色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Red.Foreground)},
                        new ThemeColorItem(){ Name = "绿色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Green.Foreground)},
                        new ThemeColorItem(){ Name = "橙色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Orange.Foreground)},
                        new ThemeColorItem(){ Name = "紫色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Violet.Foreground)},
                        new ThemeColorItem(){ Name = "蓝色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Blue.Foreground)},
                        new ThemeColorItem(){ Name = "灰色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Grey.Foreground)}
                    };
                        break;
                    case 8:
                        ThemeColorItems = new List<ThemeColorItem> { 
                        new ThemeColorItem(){ Name = "青色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Cyan.Foreground)},
                        new ThemeColorItem(){ Name = "粉色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Pink.Foreground)},
                        new ThemeColorItem(){ Name = "红色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Red.Foreground)},
                        new ThemeColorItem(){ Name = "绿色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Green.Foreground)},
                        new ThemeColorItem(){ Name = "橙色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Orange.Foreground)},
                        new ThemeColorItem(){ Name = "紫色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Violet.Foreground)},
                        new ThemeColorItem(){ Name = "蓝色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Blue.Foreground)},
                        new ThemeColorItem(){ Name = "灰色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.Grey.Foreground)},
                        new ThemeColorItem(){ Name = "墨绿色" , SampleColor = new System.Windows.Media.SolidColorBrush(Models.ThemeColors.BlackishGreen.Foreground)}
                    };
                        break;
                }

                themeColor.ItemsSource = ThemeColorItems;
                if ((int)Models.Settings.Current.ThemeColor > ThemeColorItems.Count - 1)
                {
                    Models.Settings.Current.ThemeColor = (Models.ThemeColorsEnum)(ThemeColorItems.Count - 1);
                }
                themeColor.SelectedIndex = (int)Models.Settings.Current.ThemeColor;

            }
            hideTags.IsChecked = Models.Settings.Current.HideTags;
            autoHideTags.IsChecked = Models.Settings.Current.AutoHideTags;
            cdnEnable.IsChecked = Models.Settings.Current.EnableCDN;
            feedback.IsChecked = Models.Settings.Current.FeedbackOn;

            base.OnNavigatedTo(e);
        }

        private void saveRAM_Checked(object sender, RoutedEventArgs e)
        {
            if (LoadOver)
            {
                MoePic.Models.Settings.Current.SaveRAM = true;
            }
        }

        private void saveRAM_Unchecked(object sender, RoutedEventArgs e)
        {
            if (LoadOver)
            {
                MoePic.Models.Settings.Current.SaveRAM = false;
            }
        }

        private void rating_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (LoadOver)
            {
                if (e.NewValue <= 1)
                {
                    MoePic.Models.Settings.Current.Rating = "s";
                    ratingText.Text = "Safe";
                    ratingInfo.Text = AppResources.SafeInfo;
                }
                else if (e.NewValue <= 2)
                {
                    MoePic.Models.Settings.Current.Rating = "q";
                    ratingText.Text = "Questionable";
                    ratingInfo.Text = AppResources.QuestionableInfo;
                }
                else if(e.NewValue <= 3)
                {
                    MoePic.Models.Settings.Current.Rating = "e";
                    ratingText.Text = "Explicit";
                    ratingInfo.Text = AppResources.ExplicitInfo;
                }
                else
                {
                    MoePic.Models.Settings.Current.Rating = "o";
                    ratingText.Text = "ExplicitOnly";
                    ratingInfo.Text = "ExplicitOnly是只显示Explicit分级的图片的级别.";
                }
            }
        }

        bool LoadOver = false;

        private void limit_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (LoadOver)
            {
                MoePic.Models.Settings.Current.Limit = (int)e.NewValue;
                limitText.Text = MoePic.Models.Settings.Current.Limit.ToString();
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadOver = true;
        }

        private void onlyWifi_Checked(object sender, RoutedEventArgs e)
        {

            if (LoadOver)
            {
                MoePic.Models.Settings.Current.IsWifiOnly = true;
            }
        }

        private void onlyWifi_Unchecked(object sender, RoutedEventArgs e)
        {

            if (LoadOver)
            {
                MoePic.Models.Settings.Current.IsWifiOnly = false;
            }
        }

        private void peerNum_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (LoadOver)
            {
                MoePic.Models.Settings.Current.PeerNum = (int)peerNum.Value;
                peerNumText.Text = MoePic.Models.Settings.Current.PeerNum.ToString();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Models.HistoryHelp.Clear();
            Models.ToastService.Show(AppResources.ClearHistoryOK);
        }

        private void defaultImageType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LoadOver)
            {
                Models.Settings.Current.DefaultType = (Models.ImageType)defaultImageType.SelectedIndex;
                switch (Models.Settings.Current.DefaultType)
                {
                    case MoePic.Models.ImageType.Sample:
                        defaultImageTypeInfo.Text = AppResources.SampleImageInfo;
                        break;
                    case MoePic.Models.ImageType.PNG:
                        defaultImageTypeInfo.Text = AppResources.PNGImageInfo;
                        break;
                    case MoePic.Models.ImageType.JPG:
                        defaultImageTypeInfo.Text = AppResources.JpegImageInfo;
                        break;
                    default:
                        break;
                }
            }
        }

        private void password_Click(object sender, RoutedEventArgs e)
        {
            if (LoadOver)
            {
                if ((App.Current as App).IsTrial)
                {
                    Models.MessageBoxService.Show("应用密码不可用", "购买正式版以解锁应用密码功能?", true, new Controls.Command("购买", (s, a) =>
                    {
                        new Microsoft.Phone.Tasks.MarketplaceDetailTask()
                        {
                            ContentIdentifier = "58755b21-9c40-4c17-9d39-b1b73e595ffd"
                        }.Show();
                    }), new Controls.Command("取消", null));
                    password.IsChecked = false;
                }
                else
                {
                    if (Models.Settings.Current.Password == null)
                    {
                        Models.NavigationService.Navigate("PasswordPage.xaml", PasswordOperationType.Create);
                    }
                    else
                    {
                        Models.NavigationService.Navigate("PasswordPage.xaml", PasswordOperationType.Cancel);
                    }
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Models.NavigationService.Navigate("PasswordPage.xaml", PasswordOperationType.Change);
        }

        private void needPW_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LoadOver)
            {
                Models.Settings.Current.NeedPasswordActive = needPW.SelectedIndex == 0;
            }
        }

        private void addFavSnyc_Unchecked(object sender, RoutedEventArgs e)
        {
            if(LoadOver)
            {
                Models.Settings.Current.AddFavSnyc = false;
            }
        }

        private void addFavSnyc_Checked(object sender, RoutedEventArgs e)
        {
            if (LoadOver)
            {
                Models.Settings.Current.AddFavSnyc = true;
            }
        }

        private void themeColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LoadOver)
            {
                Models.Settings.Current.ThemeColor = (Models.ThemeColorsEnum)themeColor.SelectedIndex;
                (App.Current.Resources["ThemeColor"] as Models.ThemeColor).SetColor(Models.Settings.Current.ThemeColor);
            }
        }

        private void themeColor_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if((App.Current as App).IsTrial)
            {
                Models.MessageBoxService.Show("主题色不可用", "购买正式版以解锁主题色功能?", true, new Controls.Command("购买", (s, a) =>
                    {
                        new Microsoft.Phone.Tasks.MarketplaceDetailTask()
                        {
                            ContentIdentifier = "58755b21-9c40-4c17-9d39-b1b73e595ffd"
                        }.Show();
                    }), new Controls.Command("取消", null));
            }
            else
            {
                if (ThemeColorItems.Count > 5)
                {
                    Models.StatusBarService.HideStatusBar();
                }
            }
        }

        private void autoHideTags_Checked(object sender, RoutedEventArgs e)
        {
            if (LoadOver)
            {
                if ((App.Current as App).IsTrial)
                {
                    Models.MessageBoxService.Show("智能隐藏Tag不可用", "购买正式版以解锁智能隐藏Tag功能?", true, new Controls.Command("购买", (s, a) =>
                    {
                        new Microsoft.Phone.Tasks.MarketplaceDetailTask()
                        {
                            ContentIdentifier = "58755b21-9c40-4c17-9d39-b1b73e595ffd"
                        }.Show();
                    }), new Controls.Command("取消", null));
                    autoHideTags.IsChecked = false;
                }
                else
                {
                    Models.Settings.Current.AutoHideTags = true;
                }
            }
        }

        private void autoHideTags_Unchecked(object sender, RoutedEventArgs e)
        {
            if (LoadOver)
            {
                Models.Settings.Current.AutoHideTags = false;
            }
        }

        private void hideTags_Checked(object sender, RoutedEventArgs e)
        {
            if (LoadOver)
            {
                Models.Settings.Current.HideTags = true;
            }
        }

        private void hideTags_Unchecked(object sender, RoutedEventArgs e)
        {
            if (LoadOver)
            {
                Models.Settings.Current.HideTags = false;
            }
        }

        private void cdnEnable_Checked(object sender, RoutedEventArgs e)
        {
            if (LoadOver)
            {
                if ((App.Current as App).IsTrial)
                {
                    Models.MessageBoxService.Show("CDN不可用", "购买正式版以解锁CDN功能?", true, new Controls.Command("购买", (s, a) =>
                    {
                        new Microsoft.Phone.Tasks.MarketplaceDetailTask()
                        {
                            ContentIdentifier = "58755b21-9c40-4c17-9d39-b1b73e595ffd"
                        }.Show();
                    }), new Controls.Command("取消", null));
                    cdnEnable.IsChecked = false;
                }
                else
                {
                    Models.Settings.Current.EnableCDN = true;
                    if(Models.Settings.Current.Website == "K")
                    {
                        Models.MoebooruAPI.SetWebsiteK(); 
                    }
                    else
                    {
                        Models.MoebooruAPI.SetWebsiteY();
                    }
                }
            }
        }

        private void cdnEnable_Unchecked(object sender, RoutedEventArgs e)
        {
            if (LoadOver)
            {
                Models.Settings.Current.EnableCDN = false;
                if (Models.Settings.Current.Website == "K")
                {
                    Models.MoebooruAPI.SetWebsiteK();
                }
                else
                {
                    Models.MoebooruAPI.SetWebsiteY();
                }
            }
        }

        private void feedback_Checked(object sender, RoutedEventArgs e)
        {
            if (LoadOver)
            {
                Models.Settings.Current.FeedbackOn = true;
            }
        }

        private void feedback_Unchecked(object sender, RoutedEventArgs e)
        {
            if (LoadOver)
            {
                Models.Settings.Current.FeedbackOn = false;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var file = IsolatedStorageFile.GetUserStoreForApplication();
            if(file.FileExists("Feedback.log"))
            {
                using (var stream = file.OpenFile("Feedback.log", FileMode.Open))
                {
                    byte[] data = new Byte[stream.Length];
                    stream.Read(data, 0, data.Length);
                    var logdata = System.Text.Encoding.UTF8.GetString(data, 0, data.Length);
                    var info = Newtonsoft.Json.JsonConvert.DeserializeObject<ExceptionInfo>(logdata);

                    EmailComposeTask task = new EmailComposeTask();
                    task.To = "higan@live.cn";
                    task.Subject = String.Format("MoePic Ver.{0} 异常报告", App.GetVersion());
                    task.Body = info.ToString();
                    task.Show();
                }
                file.DeleteFile("Feedback.log");
            }
            else
            {
                Models.ToastService.Show("没有要反馈的异常.");
            }
            
        }
    }

    public class ThemeColorItem
    {
        public String Name { get; set; }
        public System.Windows.Media.SolidColorBrush SampleColor { get; set; }
    }
}