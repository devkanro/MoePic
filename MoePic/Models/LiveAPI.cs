using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Media.Imaging;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Live;
using Newtonsoft.Json;

namespace MoePic.Models
{
    /// <summary>
    /// 提供对 Microsoft Account 的访问支持
    /// </summary>
    public static class LiveAPI
    {
        static string[] scopes = new string[] { "wl.signin", "wl.basic", "wl.offline_access", "wl.skydrive", "wl.skydrive_update", "wl.birthday" };
        static LiveAuthClient authClient;
        static LiveConnectClient liveClient;

        /// <summary>
        /// 获取或设置客户端ID
        /// </summary>
        public static String ClientID
        {
            get;
            set;
        }

        public static void Logout()
        {
            authClient.Logout();
            (App.Current.Resources["RuntimeResources"] as RuntimeResources).IsLogin = false;
        }

        /// <summary>
        /// 初始化 LiveAPI
        /// </summary>
        /// <param name="clientID"></param>
        public static async Task Initialize(string clientID)
        {
            ClientID = clientID;
            authClient = new LiveAuthClient(ClientID);
            try
            {
                LiveLoginResult loginResult = await authClient.InitializeAsync(scopes);
                if(loginResult.Status == LiveConnectSessionStatus.Connected)
                {
                    liveClient = new LiveConnectClient(loginResult.Session);
                    string name = await GetMe();
                    await GetProfilePicture();
                    (App.Current.Resources["RuntimeResources"] as RuntimeResources).IsLogin = true;
                }
                else
                {
                    (App.Current.Resources["RuntimeResources"] as RuntimeResources).IsLogin = false;
                }
            }
            catch (LiveAuthException)
            {
                (App.Current.Resources["RuntimeResources"] as RuntimeResources).IsLogin = false;
            }
        }

        public static async Task<bool> CreateFolder(String name, String description)
        {
            try
            {
                var folderData = new Dictionary<string, object>();
                folderData.Add("name", name);
                folderData.Add("description", description);
                LiveOperationResult operationResult = await liveClient.PostAsync("me/skydrive", folderData);
                return true;
            }
            catch (LiveConnectException ex)
            {
                return false;
            }
        }

        public static async Task<bool> UploadFile(String path,String name,Stream stream)
        {
            try
            {
                string folder = await GetFolderIdFormPath(path);
                if(folder != null)
                {
                    LiveOperationResult operationResult = await liveClient.UploadAsync(
                        folder,
                        name,
                        stream,
                        OverwriteOption.Overwrite);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (LiveConnectException ex)
            {
                return false;
            }
        }

        public static async Task<bool> UploadFileUseId(String folderId, String name, Stream stream)
        {
            try
            {
                LiveOperationResult operationResult = await liveClient.UploadAsync(
                    folderId,
                    name,
                    stream,
                    OverwriteOption.Overwrite);

                return true;
            }
            catch (LiveConnectException ex)
            {
                return false;
            }
        }

        public static async Task<List<LiveFile>> GetFilesListFormId(string id)
        {
            try
            {
                LiveOperationResult operationResult = await liveClient.GetAsync(String.Format("{0}/files", id));
                if(id != null)
                {
                    LiveFilesInfo fileList = JsonConvert.DeserializeObject<LiveFilesInfo>(operationResult.RawResult);
                    return fileList.data;
                }
                else
                {
                    return null;
                }
            }
            catch (LiveConnectException ex)
            {
                return null;
            }
        }

        public static async Task<String> GetFolderIdFormPath(String path)
        {
            try
            {
                List<String> pathInfo = path.Split('/').ToList();
                String id = null;
                foreach (var item in pathInfo)
                {
                    if(id == null)
                    {
                        LiveOperationResult operationResult = await liveClient.GetAsync("me/skydrive/files");
                        LiveFilesInfo fileList = JsonConvert.DeserializeObject<LiveFilesInfo > (operationResult.RawResult);
                        if( fileList.data.Find((f) =>
                        {
                            if(f.name == item)
                            {
                                id = f.id;
                                return true;
                            }
                            return false;
                        }) == null)
                        {
                            return null;
                        }
                    }
                    else
                    {
                        LiveOperationResult operationResult = await liveClient.GetAsync(String.Format("{0}/files",id));
                        LiveFilesInfo fileList = JsonConvert.DeserializeObject<LiveFilesInfo>(operationResult.RawResult);
                        if(fileList.data.Find((f) =>
                        {
                            if (f.name == item)
                            {
                                id = f.id;
                                return true;
                            }
                            return false;
                        }) == null)
                        {
                            return null;
                        }
                    }
                }

                return id;
            }
            catch (LiveConnectException ex)
            {
                return null;
            }
        }

        public static async Task<List<LiveFile>> GetFilesListFormPath(String path)
        {
            try
            {
                String id = await GetFolderIdFormPath(path);
                LiveOperationResult result = await liveClient.GetAsync(String.Format("{0}/files", id));
                return JsonConvert.DeserializeObject<LiveFilesInfo>(result.RawResult).data;
            }
            catch (LiveConnectException ex)
            {
                return null;
            }
        }

        public static async Task<Stream> DownloadFile(String name,String path)
        {
            try
            {
                List<LiveFile> fileList = await GetFilesListFormPath(path);
                if(fileList != null)
                {
                    var downloadResult = await liveClient.DownloadAsync(fileList.Find((f) => { return f.name == name ? true : false; }).id);
                    return downloadResult.Stream;
                }
                else
                {
                    return null;
                }
            }
            catch (LiveConnectException ex)
            {

                return null;
            }
        }


        public static async Task<Stream> DownloadFileUseId(String id, String name)
        {
            try
            {
                List<LiveFile> fileList = await GetFilesListFormId(id);
                if (fileList != null)
                {
                    String fileId = fileList.Find((f) => { return f.name == name ? true : false; }).id;
                    var downloadResult = await liveClient.DownloadAsync(String.Format("{0}/content", fileId));

                    return downloadResult.Stream;
                }
                else
                {
                    return null;
                }
            }
            catch (LiveConnectException ex)
            {

                return null;
            }
        }


        public static async Task GetProfilePicture()
        {
            try
            {
                LiveOperationResult operationResult = await liveClient.GetAsync("me/picture");
                ProfilePictureResult result = Newtonsoft.Json.JsonConvert.DeserializeObject<ProfilePictureResult>(operationResult.RawResult);
                (App.Current.Resources["RuntimeResources"] as RuntimeResources).AvatarUrl = result.location.Replace(":UserTileStatic", "");
            }
            catch (LiveConnectException e)
            {

            }
        }

        public class ProfilePictureResult
        {
            public String location { get; set; }
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> LoginAsync()
        {
            try
            {
                if ((App.Current.Resources["RuntimeResources"] as RuntimeResources).IsLogin == false)
                {
                    authClient = new LiveAuthClient(ClientID);
                    LiveLoginResult loginResult = await authClient.LoginAsync(scopes);
                    if (loginResult.Status == LiveConnectSessionStatus.Connected)
                    {
                        liveClient = new LiveConnectClient(loginResult.Session);
                        string name = await GetMe();
                        await GetProfilePicture();
                        (App.Current.Resources["RuntimeResources"] as RuntimeResources).IsLogin = true;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {

                return false;
            }
        }


        private static async Task<String> GetMe()
        {
            try
            {
                LiveOperationResult operationResult = await liveClient.GetAsync("me");

                dynamic properties = operationResult.Result;
                (App.Current.Resources["RuntimeResources"] as RuntimeResources).FirstName = properties.first_name;
                (App.Current.Resources["RuntimeResources"] as RuntimeResources).LastName = properties.last_name;
                (App.Current.Resources["RuntimeResources"] as RuntimeResources).UserID = properties.id;
                return  properties.first_name + " " + properties.last_name;
            }
            catch (LiveConnectException e)
            {
                return null;
            }
        }
    }

    public class LiveFilesInfo
    {
        public List<LiveFile> data;
    }

    public class LiveFile
    {
        public String id;
        public String name;
        public String description;
        public String parent_id;
        public String type;
    }
}
