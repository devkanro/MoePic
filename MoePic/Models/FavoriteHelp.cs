using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;

using Newtonsoft.Json;

namespace MoePic.Models
{
    public static class FavoriteHelp
    {
        static FavoriteHelp()
        {
            AddFavWeb += FavoriteHelp_AddFavWeb;
            DelFavWeb += FavoriteHelp_DelFavWeb;
        }

        public static ObservableCollection<MoePost> FavoriteList;
        public static ObservableCollection<MoeTag> FavoriteTagList;

        public static void ReadFavorite()
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();

            if (file.FileExists("FavoriteList.json"))
            {
                IsolatedStorageFileStream stream = file.OpenFile("FavoriteList.json", System.IO.FileMode.Open);
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, (int)stream.Length);
                String jsonString = Encoding.UTF8.GetString(buffer, 0, (int)stream.Length);
                if (jsonString == "")
                {
                    FavoriteList = new ObservableCollection<MoePost>();
                }
                else
                {
                    FavoriteList = JsonConvert.DeserializeObject<ObservableCollection<MoePost>>(jsonString);
                }
                stream.Close();
            }
            else
            {
                FavoriteList = new ObservableCollection<MoePost>();
            }

            if (file.FileExists("FavoriteTagList.json"))
            {
                IsolatedStorageFileStream stream = file.OpenFile("FavoriteTagList.json", System.IO.FileMode.Open);
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, (int)stream.Length);
                String jsonString = Encoding.UTF8.GetString(buffer, 0, (int)stream.Length);
                if (jsonString == "")
                {
                    FavoriteTagList = new ObservableCollection<MoeTag>();
                }
                else
                {
                    FavoriteTagList = JsonConvert.DeserializeObject<ObservableCollection<MoeTag>>(jsonString);
                }
                stream.Close();
            }
            else
            {
                    FavoriteTagList = new ObservableCollection<MoeTag>();
            }
        }

        public static void SaveFavorite()
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream stream = file.OpenFile("FavoriteList.json", System.IO.FileMode.Create);
            byte[] buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(FavoriteList));
            stream.Write(buffer, 0, buffer.Length);
            stream.Close();
            stream = file.OpenFile("FavoriteTagList.json", System.IO.FileMode.Create);
            buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(FavoriteTagList));
            stream.Write(buffer, 0, buffer.Length);
            stream.Close();
        }

        public static bool AddFavorite(MoePost post)
        {
            if (FavoriteList.Contains(post, new PostComparer()))
            {
                return false;
            }
            else
            {
                if(Settings.Current.AddFavSnyc)
                {
                    AddFavWeb(null, post);
                }
                FavoriteList.Insert(0, post);
                return true;
            }
        }

        static async void FavoriteHelp_DelFavWeb(object sender, MoePost e)
        {
            
            if(e.preview_url.Contains("yande"))
            {
                if(Settings.Current.UserY != null)
                {
                    if (!await MoebooruAPI.DelFav((int)e.id, MoebooruAPI.Yandere))
                    {
                        ToastService.Show(new Uri(e.preview_url), Resources.AppResources.SnycDelWebFail);
                    }
                }
            }
            else
            {
                if (Settings.Current.UserK != null)
                {
                    if (!await MoebooruAPI.DelFav((int)e.id, MoebooruAPI.Konachan))
                    {
                        ToastService.Show(new Uri(e.preview_url), Resources.AppResources.SnycDelWebFail);
                    }
                }
            }
        }

        static async void FavoriteHelp_AddFavWeb(object sender, MoePost e)
        {
            if (e.preview_url.Contains("yande"))
            {
                if (Settings.Current.UserY != null)
                {
                    if (!await MoebooruAPI.AddFav((int)e.id, MoebooruAPI.Yandere))
                    {
                        ToastService.Show(new Uri(e.preview_url), Resources.AppResources.SnycAddWebFail);
                    }
                }
            }
            else
            {
                if (Settings.Current.UserK != null)
                {
                    if (!await MoebooruAPI.AddFav((int)e.id, MoebooruAPI.Konachan))
                    {
                        ToastService.Show(new Uri(e.preview_url), Resources.AppResources.SnycAddWebFail);
                    }
                }
            }
        }

        static event EventHandler<MoePost> AddFavWeb;
        static event EventHandler<MoePost> DelFavWeb;

        public static bool DelFavorite(MoePost post)
        {
            if (FavoriteList.Contains(post, new PostComparer()))
            {
                FavoriteList.Remove(FavoriteList.First((p) =>
                {
                    if (PostComparer.GetHashCodeStatic(post) == PostComparer.GetHashCodeStatic(p))
                    {
                        return true;
                    }
                    return false;
                }));
                if (Settings.Current.AddFavSnyc)
                {
                    DelFavWeb(null, post);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool AddTag(MoeTag tag)
        {
            if(FavoriteTagList.Contains(tag,new TagComparer()))
            {
                return false;
            }
            else
            {
                FavoriteTagList.Add(tag);
                return true;
            }
        }

        public static bool DelTag(MoeTag tag)
        {
            if (FavoriteTagList.Contains(tag, new TagComparer()))
            {
                FavoriteTagList.Remove(FavoriteTagList.First((t) =>
                {
                    if (TagComparer.EqualsStatic(tag, t))
                    {
                        return true;
                    }
                    return false;
                }));
                return true;
            }
            else
            {
                FavoriteTagList.Add(tag);
                return false;
            }
        }

        public static bool ContainsTag(MoeTag tag)
        {
            return FavoriteTagList.Contains(tag, new TagComparer());
        }

        public static bool ContainsPost(MoePost post)
        {
            return FavoriteList.Contains(post, new PostComparer());
        }

    }

    public class PostComparer : IEqualityComparer<MoePost>
    {
        public bool Equals(MoePost post1,MoePost post2)
        {
            if (GetHashCode(post1) == GetHashCode(post2))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(MoePost post)
        {
            return (int)(post.id + (post.sample_url.Contains("yande") ? 0x10000000 : 0x00000000));
        }

        public static int GetHashCodeStatic(MoePost post)
        {
            return (int)(post.id + (post.sample_url.Contains("yande") ? 0x10000000 : 0x00000000));
        }
    }

    public class TagComparer : IEqualityComparer<MoeTag>
    {
        public bool Equals(MoeTag tag1,MoeTag tag2)
        {
            if (tag1.id == tag2.id && tag1.name == tag2.name)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool EqualsStatic(MoeTag tag1, MoeTag tag2)
        {
            if (tag1.id == tag2.id && tag1.name == tag2.name)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(MoeTag tag)
        {
            return (int)tag.id;
        }
    }
}
