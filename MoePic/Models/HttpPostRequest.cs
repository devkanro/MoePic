using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading.Tasks;

namespace MoePic.Models
{
    /// <summary>
    /// 提供对服务器发送 Post 数据的实现
    /// </summary>
    public class HttpPostRequest
    {
        HttpWebRequest httpWebRequest;
        int retryCount;

        /// <summary>
        /// 当Post失败时,得到的异常信息
        /// </summary>
        public WebException WebException;

        string url;
        bool retry;

        /// <summary>
        /// 以异步完成 Post 工作,返回null则为失败,可以查询 HttpPostRequest.WebException 字段获取异常信息.
        /// </summary>
        /// <param name="url">要进行发送的数据及其服务器地址</param>
        /// <param name="retry">是否要进行重试,将会重试3次</param>
        /// <returns>返回服务器返回的 Post 数据</returns>
        public async Task<String> PostDataAsync(String url, bool retry = true, bool throwEx = false, String method = "POST")
        {
        retry:
            this.url = url;
            this.retry = retry;
            try
            {
                httpWebRequest = HttpWebRequest.CreateHttp(url);
                httpWebRequest.Method = method;
                httpWebRequest.AllowReadStreamBuffering = true;
                Stream stream = null;
                WebResponse webResponse = null;
                webResponse = await httpWebRequest.GetResponseAsync();
                stream = webResponse.GetResponseStream();
                byte[] buff = new byte[stream.Length];
                stream.Read(buff, 0, (int)stream.Length);
                return Encoding.UTF8.GetString(buff, 0, (int)stream.Length);
            }
            catch (WebException webEx)
            {
                if (retry && retryCount < 3)
                {
                    retryCount++;
                    goto retry;
                }
                else
                {
                    WebException = webEx;
                    if (throwEx)
                    {
                        throw;
                    }
                    else
                    {

                        ToastService.Show(MoePic.Resources.AppResources.CanNotLink);
                        return "[]";
                    }
                }
            }
        }

        /// <summary>
        /// 以事件异步完成 Post 工作,Post 完成后将会触发 PostCompleted 事件,可以查询 HttpPostRequest.WebException 字段获取异常信息.
        /// </summary>
        /// <param name="url">要进行发送的数据及其服务器地址</param>
        /// <param name="retry">是否要进行重试,将会重试3次</param>
        public void PostData(String url, bool retry = true)
        {
            this.url = url;
            this.retry = retry;
            httpWebRequest = HttpWebRequest.CreateHttp(url);
            httpWebRequest.Method = "POST";
            httpWebRequest.AllowReadStreamBuffering = true;

            Object requestStatus = new object();
            IAsyncResult result = httpWebRequest.BeginGetResponse(new AsyncCallback(Callback), requestStatus);
        }

        void Callback(IAsyncResult result)
        {
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)httpWebRequest.EndGetResponse(result);
            }
            catch (WebException webEx)
            {
                if (retry && retryCount < 3)
                {
                    PostData(url, retry);
                    return;
                }
                else
                {
                    WebException = webEx;
                    PostCompleted(this, new PostCompletedEventArgs(null, PostResult.Fail, webEx));
                    return;
                }
            }
            Stream stream = response.GetResponseStream();
            byte[] buff = new byte[stream.Length];
            stream.Read(buff, 0, (int)stream.Length);
            PostCompleted(this, new PostCompletedEventArgs(Encoding.UTF8.GetString(buff, 0, (int)stream.Length), PostResult.Ok, null));
        }

        public delegate void PostCompletedEventHandler(object sender, PostCompletedEventArgs e);
        /// <summary>
        /// Post 任务完成时触发该事件
        /// </summary>
        public event PostCompletedEventHandler PostCompleted;
    }

    public class PostCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// 服务器返回的 Post 数据
        /// </summary>
        public String Data { get; private set; }
        /// <summary>
        /// Post 任务的结果
        /// </summary>
        public PostResult Result { get; private set; }
        /// <summary>
        /// Post 失败时的异常信息
        /// </summary>
        public WebException Exception { get; private set; }

        public PostCompletedEventArgs(String data, PostResult result, WebException ex)
        {
            Data = data;
            Result = result;
            Exception = ex;
        }
    }

    public enum PostResult
    {
        Ok, Fail
    }
}
