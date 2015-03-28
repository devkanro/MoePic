using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


using System.IO.IsolatedStorage;

using Newtonsoft.Json;



namespace MoePic.Models
{
    /// <summary>
    /// 提供运行时资源的简单访问
    /// </summary>
    public class RuntimeResources : DependencyObject
    {
        /// <summary>
        /// 显示名字格式
        /// </summary>
        public String NameFormat
        {
            get { return (String)GetValue(NameFormatProperty); }
            set { SetValue(NameFormatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NameFormat.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NameFormatProperty =
            DependencyProperty.Register("NameFormat", typeof(String), typeof(RuntimeResources), new PropertyMetadata(UpdataName));

        public string UserID { get; set; }

        public KeyValuePair<MoePic.Models.MoePost, MoePic.Models.ImageType> ImageInfo { get; set; }

        public int PixivID { get; set; }

        public DateTime FirstRunTime { get; set; }

        /// <summary>
        /// 名
        /// </summary>
        public String FirstName
        {
            get { return (String)GetValue(FirstNameProperty); }
            set { SetValue(FirstNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FirstName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FirstNameProperty =
            DependencyProperty.Register("FirstName", typeof(String), typeof(RuntimeResources), new PropertyMetadata(UpdataName));


        /// <summary>
        /// 姓
        /// </summary>
        public String LastName
        {
            get { return (String)GetValue(LastNameProperty); }
            set { SetValue(LastNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LastName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LastNameProperty =
            DependencyProperty.Register("LastName", typeof(String), typeof(RuntimeResources), new PropertyMetadata(UpdataName));



        /// <summary>
        /// 格式化显示字符
        /// </summary>
        public String Name
        {
            get 
            {
                if ((String)GetValue(NameProperty) != null)
                {
                    return (String)GetValue(NameProperty); 
                }
                else
                {
                    return Resources.AppResources.Me;
                }
            }
            private set { SetValue(NameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Name.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(String), typeof(RuntimeResources), null);

        public static void UpdataName(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if((d as RuntimeResources).FirstName != null && (d as RuntimeResources).LastName != null && (d as RuntimeResources).NameFormat!= null)
            {
                (d as RuntimeResources).Name = String.Format((d as RuntimeResources).NameFormat.Replace("{FN}", "{0}").Replace("{LN}", "{1}"), (d as RuntimeResources).FirstName, (d as RuntimeResources).LastName);
            }
        }




        public bool WaitingForWifi
        {
            get { return (bool)GetValue(WaitingForWifiProperty); }
            set { SetValue(WaitingForWifiProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WaitingForWifi.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WaitingForWifiProperty =
            DependencyProperty.Register("WaitingForWifi", typeof(bool), typeof(RuntimeResources), null);



        public bool WaitingForNetworking
        {
            get { return (bool)GetValue(WaitingForNetworkingProperty); }
            set { SetValue(WaitingForNetworkingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WaitingForNetworking.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WaitingForNetworkingProperty =
            DependencyProperty.Register("WaitingForNetworking", typeof(bool), typeof(RuntimeResources), null);

        


        public String AvatarUrl
        {
            get { return (String)GetValue(AvatarUrlProperty); }
            set { SetValue(AvatarUrlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AvatarUrl.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AvatarUrlProperty =
            DependencyProperty.Register("AvatarUrl", typeof(String), typeof(RuntimeResources), null);

        public Stack<MoePost> PostViewStack
        {
            get { return (Stack<MoePost>)GetValue(PostViewStackProperty); }
            set { SetValue(PostViewStackProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PostViewStack.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PostViewStackProperty =
            DependencyProperty.Register("PostViewStack", typeof(Stack<MoePost>), typeof(RuntimeResources),new PropertyMetadata(new Stack<MoePost>()));

        public Boolean IsLogin
        {
            get { return (Boolean)GetValue(IsLoginProperty); }
            set { SetValue(IsLoginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsLogin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLoginProperty =
            DependencyProperty.Register("IsLogin", typeof(Boolean), typeof(RuntimeResources), null);


        public static void Save()
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream stream = file.OpenFile("RuntimeResources.json", System.IO.FileMode.Create);
            byte[] buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(App.Current.Resources["RuntimeResources"]));
            stream.Write(buffer, 0, buffer.Length);
            stream.Close();
        }

        public static void Read()
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream stream = file.OpenFile("RuntimeResources.json", System.IO.FileMode.OpenOrCreate);

            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, (int)stream.Length);
            String jsonString = Encoding.UTF8.GetString(buffer, 0, (int)stream.Length);

            RuntimeResources rrs = JsonConvert.DeserializeObject<RuntimeResources>(jsonString);

            (App.Current.Resources["RuntimeResources"] as RuntimeResources).Name = rrs.Name;
            (App.Current.Resources["RuntimeResources"] as RuntimeResources).AvatarUrl = rrs.AvatarUrl;
            (App.Current.Resources["RuntimeResources"] as RuntimeResources).FirstName = rrs.FirstName;
            (App.Current.Resources["RuntimeResources"] as RuntimeResources).IsLogin = rrs.IsLogin;
            (App.Current.Resources["RuntimeResources"] as RuntimeResources).LastName = rrs.LastName;
            (App.Current.Resources["RuntimeResources"] as RuntimeResources).NameFormat = rrs.NameFormat;
            (App.Current.Resources["RuntimeResources"] as RuntimeResources).PostViewStack = rrs.PostViewStack;
            (App.Current.Resources["RuntimeResources"] as RuntimeResources).NavigateArgs = rrs.NavigateArgs;
            (App.Current.Resources["RuntimeResources"] as RuntimeResources).ArgsCount = rrs.ArgsCount;
            (App.Current.Resources["RuntimeResources"] as RuntimeResources).Pool = rrs.Pool;
            (App.Current.Resources["RuntimeResources"] as RuntimeResources).ImageInfo = rrs.ImageInfo;
            (App.Current.Resources["RuntimeResources"] as RuntimeResources).PixivID = rrs.PixivID;
            (App.Current.Resources["RuntimeResources"] as RuntimeResources).UserID = rrs.UserID;
            (App.Current.Resources["RuntimeResources"] as RuntimeResources).WaitingForNetworking = rrs.WaitingForNetworking;
            (App.Current.Resources["RuntimeResources"] as RuntimeResources).WaitingForWifi = rrs.WaitingForWifi;
            stream.Close();
        }

        public MoePool Pool { get; set; }

        public int ArgsCount = 0;
        public Dictionary<int, object> NavigateArgs = new Dictionary<int, object>();

        public bool RequestPassword = false;
    }

    public class BoolToVisibility : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if((bool)value)
            {
                return System.Windows.Visibility.Visible;
            }
            else
            {
                return System.Windows.Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((System.Windows.Visibility)value == System.Windows.Visibility.Visible)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
