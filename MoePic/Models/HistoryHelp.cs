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
    public static class HistoryHelp
    {
        public static ObservableCollection<MoePost> PostHistory;
        public static ObservableCollection<String> SearchHistory;

        public static void AddPost(MoePost post)
        {
            if (PostHistory.Contains(post, new PostComparer()))
            {
                PostHistory.Remove(PostHistory.First((p) => {
                    if(PostComparer.GetHashCodeStatic(p) == PostComparer.GetHashCodeStatic(post))
                    {
                        return true;
                    }
                    return false;
                }));
            }
            if(PostHistory.Count >= 30)
            {
                PostHistory.Remove(PostHistory.Last());
            }
            PostHistory.Insert(0, post);
        }

        public static void AddSearch(String tag)
        {
            if (SearchHistory.Contains(tag))
            {
                SearchHistory.Remove(tag);
            }
            if (SearchHistory.Count >= 30)
            {
                SearchHistory.Remove(SearchHistory.Last());
            }
            SearchHistory.Insert(0, tag);
        }

        public static void Clear()
        {
            PostHistory.Clear();
            SearchHistory.Clear();
        }

        public static void Read()
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();

            if (file.FileExists("PostHistory.json"))
            {
                IsolatedStorageFileStream stream = file.OpenFile("PostHistory.json", System.IO.FileMode.Open);
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, (int)stream.Length);
                String jsonString = Encoding.UTF8.GetString(buffer, 0, (int)stream.Length);
                if (jsonString == "")
                {
                    PostHistory = new ObservableCollection<MoePost>();
                }
                else
                {
                    PostHistory = JsonConvert.DeserializeObject<ObservableCollection<MoePost>>(jsonString);
                }
                stream.Close();
            }
            else
            {
                PostHistory = new ObservableCollection<MoePost>();
            }

            if (file.FileExists("SearchHistory.json"))
            {
                IsolatedStorageFileStream stream = file.OpenFile("SearchHistory.json", System.IO.FileMode.Open);
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, (int)stream.Length);
                String jsonString = Encoding.UTF8.GetString(buffer, 0, (int)stream.Length);
                if (jsonString == "")
                {
                    SearchHistory = new ObservableCollection<String>();
                }
                else
                {
                    SearchHistory = JsonConvert.DeserializeObject<ObservableCollection<String>>(jsonString);
                }
                stream.Close();
            }
            else
            {
                SearchHistory = new ObservableCollection<String>();
            }
        }

        public static void Save()
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream stream = file.OpenFile("PostHistory.json", System.IO.FileMode.Create);
            byte[] buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(PostHistory));
            stream.Write(buffer, 0, buffer.Length);
            stream.Close();
            stream = file.OpenFile("SearchHistory.json", System.IO.FileMode.Create);
            buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(SearchHistory));
            stream.Write(buffer, 0, buffer.Length);
            stream.Close();
        }
    }
}
