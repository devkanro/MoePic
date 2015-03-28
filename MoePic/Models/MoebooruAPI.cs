using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MoePic.Models
{
    /// <summary>
    /// 对 Moebooru 的API进行封装,调用
    /// </summary>
    public static class MoebooruAPI
    {
        public static string Konachan
        {
            get
            {
                return Settings.Current.EnableCDN ? "http://moepic.sinaapp.com" : "http://konachan.com";
            }
        }

        public static string Yandere
        {
            get
            {
                return Settings.Current.EnableCDN ? "http://yandere.sinaapp.com" : "https://yande.re";
            }
        }

        static String _website = Konachan;
        public static String partWebsite;
        public static string userY;
        public static string passwordHashY;
        public static string userK;
        public static string passwordHashK;

        

        static Dictionary<String, String> Cache = new Dictionary<string, String>();

        static int PageY { get; set; }
        static int PageK { get; set; }

        public static List<MoePost> SafeMode(List<MoePost> list)
        {
            switch (Settings.Current.Rating)
            {
                case "s":
                    return list.FindAll((p) =>
                    {
                        if (p.rating == "s")
                        {
                            return true;
                        }
                        return false;
                    }).ToList();
                case "q":
                    return list.FindAll((p) =>
                    {
                        if(p.rating != "e")
                        {
                            return true;
                        }
                        return false;
                    }).ToList();
                case "e":
                    return list;
                case "o":
                    return list.FindAll((p) =>
                    {
                        if (p.rating == "e")
                        {
                            return true;
                        }
                        return false;
                    }).ToList();
                default:
                    break;
            }
            return list;
        }

        /// <summary>
        /// 筛选指定Post,剔除已删除Post
        /// </summary>
        /// <param name="postList"></param>
        /// <returns></returns>
        public static List<MoePost> ScreenPost(List<MoePost> postList)
        {
            return postList.FindAll((post) =>
            {
                if (post.status != "deleted")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });
        }

        /// <summary>
        /// 设置当前网址为Y站
        /// </summary>
        public static void SetWebsiteY()
        {
            _website = Yandere;
        }

        /// <summary>
        /// 设置当前网址为K站
        /// </summary>
        public static void SetWebsiteK()
        {
            _website = Konachan;
        }

        /// <summary>
        /// 以提供的帐号密码登录Y站,返回是否成功登录
        /// </summary>
        /// <param name="user">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>登录结果</returns>
        public async static Task<bool> LoginY(string user, string password)
        {
            string passwordHash = SHA1.GetSHA1("choujin-steiner--" + password + "--");
            HttpPostRequest httpPostRequest = new HttpPostRequest();
            try
            {
                String PostsData = await httpPostRequest.PostDataAsync(String.Format("{2}/post/vote.json?id=3&score=0&login={0}&password_hash={1}", user, passwordHash,Yandere));
            }
            catch (System.Net.WebException ex)
            {
                return false;
            }
            userY = user;
            passwordHashY = passwordHash;
            return true;
        }

        public async static Task<bool> LoginY()
        {
            HttpPostRequest httpPostRequest = new HttpPostRequest();
            try
            {
                String PostsData = await httpPostRequest.PostDataAsync(String.Format("{2}/post/vote.json?id=3&score=0&login={0}&password_hash={1}", userY, passwordHashY, Yandere));
            }
            catch (System.Net.WebException ex)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 以提供的帐号密码登录K站,返回是否成功登录
        /// </summary>
        /// <param name="user">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>登录结果</returns>
        public async static Task<bool> LoginK(string user, string password)
        {
            string passwordHash = SHA1.GetSHA1("So-I-Heard-You-Like-Mupkids-?--" + password + "--");
            HttpPostRequest httpPostRequest = new HttpPostRequest();
            try
            {
                String PostsData = await httpPostRequest.PostDataAsync(String.Format("{2}/post/vote.json?id=3&score=0&login={0}&password_hash={1}", user, passwordHash,Konachan), true, true);
            }
            catch (System.Net.WebException ex)
            {
                return false;
            }
            userK = user;
            passwordHashK = passwordHash;
            return true;
        }

        public async static Task<bool> LoginK()
        {
            HttpPostRequest httpPostRequest = new HttpPostRequest();
            try
            {
                String PostsData = await httpPostRequest.PostDataAsync(String.Format("{2}/post/vote.json?id=3&score=0&login={0}&password_hash={1}", userK, passwordHashK, Konachan), true, true);
            }
            catch (System.Net.WebException ex)
            {
                return false;
            }
            return true;
        }

        public async static Task<bool> AddFav(int id,String website = null)
        {
            HttpPostRequest httpPostRequest = new HttpPostRequest();
            try
            {
                string web = website == null ? _website : website;
                if(web.Contains("yande"))
                {
                    String PostsData = await httpPostRequest.PostDataAsync(String.Format("{3}/post/vote.json?id={0}&score=3&login={1}&password_hash={2}", id, userY, passwordHashY,Yandere), true, true);
                }
                else
                {
                    String PostsData = await httpPostRequest.PostDataAsync(String.Format("{3}/post/vote.json?id={0}&score=3&login={1}&password_hash={2}", id, userK, passwordHashK,Konachan), true, true);
                }
            }
            catch (System.Net.WebException ex)
            {
                return false;
            }
            return true;
        }


        public async static Task<bool> DelFav(int id, String website = null)
        {
            HttpPostRequest httpPostRequest = new HttpPostRequest();
            try
            {
                string web = website == null ? _website : website;
                if (web.Contains("yande"))
                {
                    String PostsData = await httpPostRequest.PostDataAsync(String.Format("{3}/post/vote.json?id={0}&score=0&login={1}&password_hash={2}", id, userY, passwordHashY, Yandere), true, true);
                }
                else
                {
                    String PostsData = await httpPostRequest.PostDataAsync(String.Format("{3}/post/vote.json?id={0}&score=0&login={1}&password_hash={2}", id, userK, passwordHashK, Konachan), true, true);
                }
            }
            catch (System.Net.WebException ex)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 根据提供的页数,获取Post列表
        /// </summary>
        /// <param name="page">页数</param>
        /// <param name="limit">单页Post数</param>
        /// <param name="rating">分级</param>
        /// <returns>得到的Post列表</returns>
        public async static Task<List<MoePost>> GetPostsFromPage(int page = 1, int limit = 16, String rating = "s", String website = null)
        {
            if ((website == null ? _website : website).Contains("yande"))
            {
                PageY = page;
            }
            else
            {
                PageK = page;
            }
            String ratingInfo = "";
            switch (rating)
            {
                case "s":
                    ratingInfo = "rating:s";
                    break;
                case "q":
                    ratingInfo = "-rating:explicit";
                    break;
                case "e":
                    ratingInfo = "";
                    break;
                case "o":
                    ratingInfo = "rating:e";
                    break;
            }
            HttpPostRequest httpPostRequest = new HttpPostRequest();
            String PostsData = await httpPostRequest.PostDataAsync(String.Format("{3}/post.json?page={0}&limit={1}&tags={2}", page, limit, ratingInfo, website == null ? _website : website));
            return ScreenPost(JsonConvert.DeserializeObject<List<MoePost>>(PostsData));
        }


        /// <summary>
        /// 根据提供的最大Post编号,获取Post列表
        /// </summary>
        /// <param name="maxPost">最大Post编号</param>
        /// <param name="limit">单页Post数</param>
        /// <param name="rating">分级</param>
        /// <returns>得到的Post列表</returns>
        public async static Task<List<MoePost>> GetPostsFromMax(int maxPost, int page = 1, int limit = 16, String rating = "s", String website = null)
        {
            String ratingInfo = "";
            switch (rating)
            {
                case "s":
                    ratingInfo = "rating:s";
                    break;
                case "q":
                    ratingInfo = "-rating:explicit";
                    break;
                case "e":
                    ratingInfo = "";
                    break;
                case "o":
                    ratingInfo = "rating:e";
                    break;
            }
            HttpPostRequest httpPostRequest = new HttpPostRequest();
            String PostsData = await httpPostRequest.PostDataAsync(String.Format("{2}/post.json?page={4}&limit={0}&tags=id:<{1}+{3}", limit, maxPost, website == null ? _website : website, ratingInfo,page));
            return ScreenPost(JsonConvert.DeserializeObject<List<MoePost>>(PostsData));
        }

        /// <summary>
        /// 根据提供的最小Post编号,获取Post列表
        /// </summary>
        /// <param name="minPost">最小Post编号</param>
        /// <param name="limit">单页Post数</param>
        /// <param name="rating">分级</param>
        /// <returns>得到的Post列表</returns>
        public async static Task<List<MoePost>> GetPostsFromMin(int minPost, int page = 1, int limit = 16, String rating = "s", String website = null)
        {
            String ratingInfo = "";
            switch (rating)
            {
                case "s":
                    ratingInfo = "rating:s";
                    break;
                case "q":
                    ratingInfo = "-rating:explicit";
                    break;
                case "e":
                    ratingInfo = "";
                    break;
                case "o":
                    ratingInfo = "rating:e";
                    break;
            }
            HttpPostRequest httpPostRequest = new HttpPostRequest();
            String PostsData = await httpPostRequest.PostDataAsync(String.Format("{2}/post.json?page={4}&limit={0}&tags=id:>{1}+{3}", limit, minPost, website == null ? _website : website, ratingInfo,page));
            return ScreenPost(JsonConvert.DeserializeObject<List<MoePost>>(PostsData));
        }

        /// <summary>
        /// 得到随机排序的Posts
        /// </summary>
        /// <param name="limit">分页</param>
        /// <param name="rating">分级</param>
        /// <param name="website"></param>
        /// <returns></returns>
        public async static Task<List<MoePost>> GetPostsRandom(int limit = 16,String rating = "s", String website = null)
        {
            String ratingInfo = "";
            switch (rating)
            {
                case "s":
                    ratingInfo = "rating:s";
                    break;
                case "q":
                    ratingInfo = "-rating:explicit";
                    break;
                case "e":
                    ratingInfo = "";
                    break;
                case "o":
                    ratingInfo = "rating:e";
                    break;
            }
            HttpPostRequest httpPostRequest = new HttpPostRequest();
            String PostsData = await httpPostRequest.PostDataAsync(String.Format("{0}/post.json?limit={1}&tags=order:random+{2}", website == null ? _website : website, limit, ratingInfo));
            return ScreenPost(JsonConvert.DeserializeObject<List<MoePost>>(PostsData));
        }

        /// <summary>
        /// 根据所提供的标签,页数,获取Post列表
        /// </summary>
        /// <param name="tags">标签</param>
        /// <param name="page">页数</param>
        /// <param name="limit">单页Post数</param>
        /// <param name="rating">分级</param>
        /// <returns>得到的Post列表</returns>
        public async static Task<List<MoePost>> GetPostsFromTags(String tags, int page = 1, int limit = 16, String rating = "s", String website = null)
        {
            String ratingInfo = "";
            switch (rating)
            {
                case "s":
                    ratingInfo = "rating:s";
                    break;
                case "q":
                    ratingInfo = "-rating:explicit";
                    break;
                case "e":
                    ratingInfo = "";
                    break;
                case "o":
                    ratingInfo = "rating:e";
                    break;
            }
            HttpPostRequest httpPostRequest = new HttpPostRequest();
            String PostsData = await httpPostRequest.PostDataAsync(String.Format("{3}/post.json?page={0}&limit={1}&tags={2}+{4}", page, limit, tags, website == null ? _website : website, ratingInfo));
            return ScreenPost(JsonConvert.DeserializeObject<List<MoePost>>(PostsData));
        }

        /// <summary>
        /// 根据所提供的ID搜索图片
        /// </summary>
        /// <param name="id">图片ID</param>
        /// <param name="website"></param>
        /// <returns></returns>
        public async static Task<MoePost> GetPostFormID(int id, String website = null)
        {
            HttpPostRequest httpPostRequest = new HttpPostRequest();
            String PostData = await httpPostRequest.PostDataAsync(String.Format("{0}/post.json?tags=id:{1}",website == null? _website : website,id));
            List<MoePost> postlist = ScreenPost(JsonConvert.DeserializeObject<List<MoePost>>(PostData));
            return postlist.Count > 0 ? postlist[0] : null;
        }

        /// <summary>
        /// 根据所提供的父ID搜索子图片
        /// </summary>
        /// <param name="id">父图片ID</param>
        /// <param name="page">页数</param>
        /// <param name="limit">单页</param>
        /// <param name="rating">分级</param>
        /// <param name="website"></param>
        /// <returns></returns>
        public async static Task<List<MoePost>> GetPostsFormParents(int id, int page = 1, int limit = 16, String rating = "s", String website = null)
        {
            String ratingInfo = "";
            switch (rating)
            {
                case "s":
                    ratingInfo = "rating:s";
                    break;
                case "q":
                    ratingInfo = "-rating:explicit";
                    break;
                case "e":
                    ratingInfo = "";
                    break;
                case "o":
                    ratingInfo = "rating:e";
                    break;
            }
            HttpPostRequest httpPostRequest = new HttpPostRequest();
            String PostData = await httpPostRequest.PostDataAsync(String.Format("{0}/post.json?tags=parent:{1}+{2}&page={3}&limit={4}", website == null ? _website : website, id, ratingInfo, page, limit));
            return ScreenPost(JsonConvert.DeserializeObject<List<MoePost>>(PostData));
        }

        /// <summary>
        /// 更新Post信息,不需要更新的信息为null
        /// </summary>
        /// <param name="id">需要更新的Post的编号</param>
        /// <param name="tags">Tag</param>
        /// <param name="file">文件流</param>
        /// <param name="rating">图片分级</param>
        /// <param name="source">图片源</param>
        /// <param name="isRatingLock">是否允许其他用户更改分级</param>
        /// <param name="isNoteLock">是否允许其他用户更改Note</param>
        /// <param name="parentId">设定父Post</param>
        /// <returns>是否更新成功</returns>
        public async static Task<bool> UpdatePost(int id, string tags = null, string file = null, string rating = null, string source = null, Nullable<bool> isRatingLock = null, Nullable<bool> isNoteLock = null, Nullable<int> parentId = null, String website = null)
        {
            String post;
            if ((website == null ? _website : website).Contains("yande"))
            {
                post = String.Format("{0}/post/update.json?login={1}&password_hash={2}&id={3}", website == null ? _website : website, userY, passwordHashY, id);
            }
            else
            {
                post = String.Format("{0}/post/update.json?login={1}&password_hash={2}&id={3}", website == null ? _website : website, userK, passwordHashK, id);
            }

            if (tags != null)
            {
                post += String.Format("&post[tags]={0}", tags);
            }
            if (file != null)
            {
                post += String.Format("&post[file]={0}", file);
            }
            if (rating != null)
            {
                post += String.Format("&post[rating]={0}", rating);
            }
            if (source != null)
            {
                post += String.Format("&post[source]={0}", source);
            }
            if (isRatingLock != null)
            {
                post += String.Format("&post[is_rating_locked]={0}", isRatingLock);
            }
            if (isNoteLock != null)
            {
                post += String.Format("&post[is_note_locked]={0}", isNoteLock);
            }
            if (parentId != null)
            {
                post += String.Format("&post[parent_id]={0}", parentId);
            }

            HttpPostRequest httpPostRequest = new HttpPostRequest();
            String PostsData = await httpPostRequest.PostDataAsync(post);

            //处理返回数据

            return true;
        }

        /// <summary>
        /// 删除一个Post,需要有操作权限
        /// </summary>
        /// <param name="id">要删除的id</param>
        /// <returns>是否成功操作</returns>
        public async static Task<bool> DestroyPost(int id, String website = null)
        {
            if ((website == null ? _website : website).Contains("yande"))
            {
                HttpPostRequest httpPostRequest = new HttpPostRequest();
                String PostsData = await httpPostRequest.PostDataAsync(String.Format("{0}/post/destroy.json?login={1}&password_hash={2}&id={3}", website == null ? _website : website, userY, passwordHashY, id));

                //处理返回数据

                return true;
            }
            else
            {
                HttpPostRequest httpPostRequest = new HttpPostRequest();
                String PostsData = await httpPostRequest.PostDataAsync(String.Format("{0}/post/destroy.json?login={1}&password_hash={2}&id={3}", website == null ? _website : website, userK, passwordHashK, id));

                //处理返回数据

                return true;
            }
        }

        /// <summary>
        /// 根据所提供的Post编号,获取评论列表
        /// </summary>
        /// <param name="postId">Post编号</param>
        /// <returns>得到的评论列表</returns>
        public async static Task<List<MoeComment>> GetCommentsFormPost(long postId, String website = null)
        {
            HttpPostRequest httpPostRequest = new HttpPostRequest();
            String PostsData = await httpPostRequest.PostDataAsync(String.Format("{0}/comment.json?post_id={1}", website == null ? _website : website, postId));
            return JsonConvert.DeserializeObject<List<MoeComment>>(PostsData);
        }

        /// <summary>
        /// 根据所提供的评论编号,获取评论列表
        /// </summary>
        /// <param name="id">评论编号</param>
        /// <returns>得到的评论列表</returns>
        public async static Task<List<MoeComment>> GetCommentFormId(long id, String website = null)
        {
            HttpPostRequest httpPostRequest = new HttpPostRequest();
            String PostsData = await httpPostRequest.PostDataAsync(String.Format("{0}/comment/show.json?id={1}", website == null ? _website : website, id));
            return JsonConvert.DeserializeObject<List<MoeComment>>(PostsData);
        }

        /// <summary>
        /// 添加一个评论
        /// </summary>
        /// <param name="postId">Post编号</param>
        /// <param name="body">评论内容</param>
        /// <param name="anonymous">是否为匿名消息</param>
        /// <returns>返回创建结果</returns>
        public async static Task<bool> CreateComment(long postId, string body, bool anonymous, String website = null)
        {

            if ((website == null ? _website : website).Contains("yande"))
            {
                HttpPostRequest httpPostRequest = new HttpPostRequest();
                String PostsData = await httpPostRequest.PostDataAsync(String.Format("{0}/comment/create.json?comment[post_id]={1}&comment[body]={2}&login={3}&password_hash={4}{5}", _website, postId, body, userY, passwordHashY, anonymous ? "&comment[anonymous]=1" : ""));

                return true;
            }
            else
            {
                HttpPostRequest httpPostRequest = new HttpPostRequest();
                String PostsData = await httpPostRequest.PostDataAsync(String.Format("{0}/comment/create.json?comment[post_id]={1}&comment[body]={2}&login={3}&password_hash={4}{5}", _website, postId, body, userK, passwordHashK, anonymous ? "&comment[anonymous]=1" : ""));

                return true;
            }
        }

        /// <summary>
        /// 根据提供的ID搜索Tag
        /// </summary>
        /// <param name="id">Tag的id</param>
        /// <param name="limit">单页tag数</param>
        /// <param name="page">页数</param>
        /// <param name="order">排序方式</param>
        /// <returns>返回所得到的tag</returns>
        public async static Task<List<MoeTag>> GetTagsFormId(long id, int limit = 16, int page = 1, TagOrder order = TagOrder.name, String website = null)
        {
            string uri = String.Format("{0}/tag.json?id={1}&limit={2}&page={3}&order={4}", website == null ? _website : website, id, limit, page, order.ToString());
            String PostsData = GetCache(uri);
            if (PostsData == null)
            {
                HttpPostRequest httpPostRequest = new HttpPostRequest();
                PostsData = await httpPostRequest.PostDataAsync(uri);
                SetCache(uri, PostsData);
            }
            return JsonConvert.DeserializeObject<List<MoeTag>>(PostsData);
        }

        /// <summary>
        /// 根据提供的名字搜索Tag
        /// </summary>
        /// <param name="name">要搜索的名字</param>
        /// <param name="limit">单页tag数</param>
        /// <param name="page">页数</param>
        /// <param name="order">排序方式</param>
        /// <returns></returns>
        public async static Task<List<MoeTag>> GetTagsFormName(string name, int limit = 16, int page = 1, TagOrder order = TagOrder.name, String website = null)
        {
            string uri = String.Format("{0}/tag.json?name={1}&limit={2}&page={3}&order={4}", website == null ? _website : website, name, limit, page, order.ToString());
            String PostsData = GetCache(uri);
            if (PostsData == null)
            {
                HttpPostRequest httpPostRequest = new HttpPostRequest();
                PostsData = await httpPostRequest.PostDataAsync(uri);
                SetCache(uri, PostsData);
            }
            return JsonConvert.DeserializeObject<List<MoeTag>>(PostsData);
        }
        /// <summary>
        /// 根据提供的最小ID搜索Tag
        /// </summary>
        /// <param name="minId">最小ID</param>
        /// <param name="limit">单页tag数</param>
        /// <param name="page">页数</param>
        /// <param name="order">排序方式</param>
        /// <returns></returns>
        public async static Task<List<MoeTag>> GetTagsFormMinId(long minId, int limit = 16, int page = 1, TagOrder order = TagOrder.name, String website = null)
        {
            string uri = String.Format("{0}/tag.json?after_id={1}limit={2}&page={3}&order={4}", website == null ? _website : website, minId, limit, page, order.ToString());
            String PostsData = GetCache(uri);
            if(PostsData == null)
            {
                HttpPostRequest httpPostRequest = new HttpPostRequest();
                PostsData = await httpPostRequest.PostDataAsync(uri);
                SetCache(uri, PostsData);
            }
            return JsonConvert.DeserializeObject<List<MoeTag>>(PostsData);
        }

        /// <summary>
        /// 根据提供的Tag搜索相关Tag
        /// </summary>
        /// <param name="tag">父Tag</param>
        /// <param name="type">Tag的种类</param>
        /// <returns></returns>
        public async static Task<List<MoeTag>> GetTagsFormRelated(string tag, TagType? type = null, String website = null)
        {
            HttpPostRequest httpPostRequest = new HttpPostRequest();
            String PostsData = await httpPostRequest.PostDataAsync(String.Format("{0}/tag/related.json?tags={1}&type={2}", website == null ? _website : website, tag, type == null ? "" : ((int)(type.Value)).ToString()));
            return JsonConvert.DeserializeObject<List<MoeTag>>(PostsData);
        }

        /// <summary>
        /// 根据所提供的Post编号,获取Note列表
        /// </summary>
        /// <param name="postId">Post编号</param>
        /// <returns>得到的Note列表</returns>
        public async static Task<List<MoeNote>> GetNotesFormPost(long postId, String website = null)
        {
            HttpPostRequest httpPostRequest = new HttpPostRequest();
            String PostsData = await httpPostRequest.PostDataAsync(String.Format("{0}/note.json?post_id={1}", website == null ? _website : website, postId));
            return JsonConvert.DeserializeObject<List<MoeNote>>(PostsData);
        }

        public static string GetCache(string uri)
        {
            if(Cache.ContainsKey(uri))
            {
                return Cache[uri];
            }
            return null;
        }

        public static void SetCache(string uri,string content)
        {
            if (!Cache.ContainsKey(uri))
            {
                Cache.Add(uri, content);
            }
        }

        /// <summary>
        /// 获取当前最热门图片
        /// </summary>
        /// <param name="period">范围</param>
        /// <returns></returns>
        public async static Task<List<MoePost>> GetRankingPost(Period period = Period.d, String website = null)
        {
            HttpPostRequest httpPostRequest = new HttpPostRequest();
            String PostsData = await httpPostRequest.PostDataAsync(String.Format("{0}/post/popular_recent.json?period=1{1}", website == null ? _website : website, period.ToString()));
            return SafeMode(ScreenPost(JsonConvert.DeserializeObject<List<MoePost>>(PostsData)));
        }

        public async static Task<List<MoeUser>> GetUsersFormName(String name, String website = null)
        {
            HttpPostRequest httpPostRequest = new HttpPostRequest();
            String PostsData = await httpPostRequest.PostDataAsync(String.Format("{0}/user.json?name={1}", website == null ? _website : website, name));
            return JsonConvert.DeserializeObject<List<MoeUser>>(PostsData);
        }

        public async static Task<MoeUser> GetUsersFormId(int id, String website = null)
        {
            HttpPostRequest httpPostRequest = new HttpPostRequest();
            String PostsData = await httpPostRequest.PostDataAsync(String.Format("{0}/user.json?id={1}", website == null ? _website : website, id));
            var result = JsonConvert.DeserializeObject<List<MoeUser>>(PostsData);
            if(result.Count != 0)
            {

                return result[0];
            }
            else
            {
                return null;
            }
        }

        public async static Task<List<MoePool>> GetPoolFormPage(int page = 1,int limit = 16,String website = null)
        {
            HttpPostRequest httpPostRequest = new HttpPostRequest();
            String PostsData = await httpPostRequest.PostDataAsync(String.Format("{0}/pool.json?page={1}&limit={2}", website == null ? _website : website, page, limit));
            var pools = JsonConvert.DeserializeObject<List<MoePool>>(PostsData);
            var result = new List<MoePool>();

            IEnumerable<System.Threading.Tasks.Task<MoePool>> tasks = from pool in pools select GetPoolFormId(pool.id,website);
            var runResult = await Task.WhenAll(tasks);

            foreach (var item in runResult)
            {
                if(item.post_count > 0)
                {
                    item.posts = SafeMode(ScreenPost(item.posts));
                    if (item.posts.Count > 0)
                    {
                        result.Add(item);
                    }
                }
            }
            return result;
        }

        public async static Task<MoePool> UpdataPool(MoePool pool,String website = null)
        {
            HttpPostRequest httpPostRequest = new HttpPostRequest();
            String PostsData = await httpPostRequest.PostDataAsync(String.Format("{0}/pool/show.json?id={1}", website == null ? _website : website, pool.id));
            var result = JsonConvert.DeserializeObject<MoePool>(PostsData);
            pool.posts = SafeMode(ScreenPost(result.posts));
            return result;
        }

        public async static Task<MoePool> GetPoolFormId(int id, String website = null)
        {
            HttpPostRequest httpPostRequest = new HttpPostRequest();
            String PostsData = await httpPostRequest.PostDataAsync(String.Format("{0}/pool/show.json?id={1}", website == null ? _website : website, id));
            var result = JsonConvert.DeserializeObject<MoePool>(PostsData);
            return result;
        }

        public async static Task<List<MoePool>> GetPoolFormName(String name, int page = 1, int limit = 16, String website = null)
        {
            HttpPostRequest httpPostRequest = new HttpPostRequest();
            String PostsData = await httpPostRequest.PostDataAsync(String.Format("{0}/pool.json?page={1}&limit={2}&query={3}", website == null ? _website : website, page, limit, name));
            var result = JsonConvert.DeserializeObject<List<MoePool>>(PostsData).FindAll((p) =>
            {
                if (p.post_count > 0)
                {
                    return true;
                }
                return false;
            });
            return result;
        }

        public static DateTime TrueTime { get; set; }

        public async static Task<List<MoePost>> GetDayRankingPost(DateTime date, String website = null)
        {
            HttpPostRequest httpPostRequest = new HttpPostRequest();
            String PostsData = await httpPostRequest.PostDataAsync(String.Format("{0}/post/popular_by_day.json?day={1}&month={2}&year={3}", website == null ? _website : website, date.Day, date.Month, date.Year));
            if (PostsData == "[]")
            {
                return await GetRankingPost(Period.d, website);
            }
            return SafeMode(ScreenPost(JsonConvert.DeserializeObject<List<MoePost>>(PostsData)));
        }

        public async static Task<List<MoePost>> GetWeekRankingPost(DateTime date, String website = null)
        {
            HttpPostRequest httpPostRequest = new HttpPostRequest();
            String PostsData = await httpPostRequest.PostDataAsync(String.Format("{0}/post/popular_by_week.json?day={1}&month={2}&year={3}", website == null ? _website : website, date.Day, date.Month, date.Year));
            if (PostsData == "[]")
            {
                return await GetRankingPost(Period.w, website);
            }
            return SafeMode(ScreenPost(JsonConvert.DeserializeObject<List<MoePost>>(PostsData)));
        }

        public async static Task<List<MoePost>> GetMonthRankingPost(DateTime date, String website = null)
        {
            HttpPostRequest httpPostRequest = new HttpPostRequest();
            String PostsData = await httpPostRequest.PostDataAsync(String.Format("{0}/post/popular_by_month.json?day={1}&month={2}&year={3}", website == null ? _website : website, date.Day, date.Month, date.Year));
            if (PostsData == "[]")
            {
                return await GetRankingPost(Period.m, website);
            }
            return SafeMode(ScreenPost(JsonConvert.DeserializeObject<List<MoePost>>(PostsData)));
        }


        public static String GetUserAvatar(int userID, String website = null)
        {
            return String.Format("{0}/data/avatars/{1}.jpg", website == null ? _website : website, userID);
        }
    }

    public enum Period
    {
        d,
        w,
        m,
        y
    }

    public enum TagOrder
    {
        date,
        count,
        name
    }

    public enum TagType
    {
        General = 0, artist = 1, copyright = 3, character = 4, circle=5, faults = 6
    }

    /// <summary>
    /// 表示一个 Moebooru 所使用的Post
    /// </summary>
    public class MoePost
    {
        public long id { get; set; }
        public string tags { get; set; }
        public long? creator_id { get; set; }
        public String author { get; set; }
        public long change { get; set; }
        public String source { get; set; }
        public long score { get; set; }
        public String md5 { get; set; }
        public long file_size { get; set; }
        public String file_url { get; set; }
        public bool is_shown_in_index { get; set; }
        public String preview_url { get; set; }
        public int preview_width { get; set; }
        public int preview_height { get; set; }
        public int actual_preview_width { get; set; }
        public int actual_preview_height { get; set; }
        public String sample_url { get; set; }
        public int sample_width { get; set; }
        public int sample_height { get; set; }
        public long sample_file_size { get; set; }
        public String jpeg_url { get; set; }
        public int jpeg_width { get; set; }
        public int jpeg_height { get; set; }
        public long jpeg_file_size { get; set; }
        public string rating { get; set; }
        public bool has_children { get; set; }
        public String parent_id { get; set; }
        public String status { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public bool is_held { get; set; }
        public MoeDetail flag_detail { get; set; }
    }

    public class MoeDetail
    {
        public long post_id { get; set; }
        public String reason { get; set; }
        public DateTime created_at { get; set; }
        public String user_id { get; set; }
        public String flagged_by { get; set; }
    }

    /// <summary>
    /// 表示 Moebooru 所使用的一个 Comment
    /// </summary>
    public class MoeComment
    {
        public long id { get; set; }
        public DateTime created_at { get; set; }
        public long post_id { get; set; }
        public string creator { get; set; }
        public long? creator_id { get; set; }
        public string body { get; set; }
    }

    /// <summary>
    /// 表示 Moebooru 所使用的一个Tag
    /// </summary>
    public class MoeTag
    {
        public long id{ get; set; }
        public string name { get; set; }
        public long count { get; set; }
        public int type { get; set; }
        public bool ambiguous{ get; set; }

    }

    /// <summary>
    /// 表示 Moebooru 说实用的一个Note
    /// </summary>
    public class MoeNote
    {
        public long id{ get; set; }
        public DateTime created_at{ get; set; }
        public DateTime updated_at{ get; set; }
        public long creator_id{ get; set; }
        public int x{ get; set; }
        public int y{ get; set; }
        public int width{ get; set; }
        public int height{ get; set; }
        public bool is_active{ get; set; }
        public long post_id{ get; set; }
        public String body{ get; set; }
        public int version{ get; set; }
    }

    /// <summary>
    /// 表示 Moebooru 的一个用户
    /// </summary>
    public class MoeUser
    {
        public String name { get; set; }
        public String[] blacklisted_tags { get; set; }
        public int id { get; set; }
    }

    /// <summary>
    /// 表示 Moebooru 所使用的一个Pool
    /// </summary>
    public class MoePool
    {
        public int id { get; set; }
        public String name { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public int? user_id { get; set; }
        public bool is_public { get; set; }
        public int post_count { get; set; }
        public String description { get; set; }
        public List<MoePost> posts { get; set; }
    }
}
