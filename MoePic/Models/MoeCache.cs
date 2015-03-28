using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

using Newtonsoft.Json;

namespace MoePic.Models
{
    /// <summary>
    /// 提供对应用缓存的简单访问
    /// </summary>
    public class MoeCache
    {
        private MoeCache()
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            if(file.DirectoryExists("Cache"))
            {

            }

        }

        private static MoeCache _Current = new MoeCache();

        public MoeCache Current { get { return _Current; } }

        
    }

    /// <summary>
    /// 表示一个缓存文件
    /// </summary>
    public class MoeCacheFile
    {

    }

    /// <summary>
    /// 表示一个缓存图片
    /// </summary>
    public class MoeCacheImage : MoeCacheFile
    {

    }

}
