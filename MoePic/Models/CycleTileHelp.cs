using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;

namespace MoePic.Models
{
    public class CycleTileHelp
    {
        /**
         * 
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            int count = 0;
            foreach (var item in Models.FavoriteHelp.FavoriteList)
            {
                WebClient web = new WebClient();
                web.OpenReadCompleted += web_OpenReadCompleted;
                web.OpenReadAsync(new Uri(item.sample_url));
                count++;
                if (count == 9)
                {
                    break;
                }
            }
        }


        List<Uri> uris = new List<Uri>();

        void web_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            var stream = file.OpenFile(String.Format("Shared/ShellContent/{0}.jpg", uris.Count), FileMode.Create);
            e.Result.CopyTo(stream);
            stream.Close();
            e.Result.Close();

            uris.Add(new Uri(String.Format("isostore:/Shared/ShellContent/{0}.jpg", uris.Count), UriKind.Absolute));

            if (uris.Count == 9)
            {
                CycleTileData tile = new CycleTileData();
                tile.CycleImages = uris;
                tile.SmallBackgroundImage = new Uri("/Assets/SquareTile71x71.png", UriKind.Relative);
                tile.Title = "MoePic";
                ShellTile.Create(new Uri("/SplashScreen.xaml", UriKind.Relative), tile, true);
                Models.ToastService.Show("已添加磁贴.");
            }
        }
         */
    }
}
