using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Txooo.BaiduInclude.Service.Entities
{
    /// <summary>
    /// 带查询网址实体
    /// </summary>
    public class UrlInfo
    {

        long _id = 0;
        /// <summary>
        /// 唯一索引
        /// </summary>
        public long Id { get { return _id; } set { _id = value; } }

        string _url = string.Empty;
        /// <summary>
        /// 网址
        /// </summary>
        public string Url { get { return _url; } set { _url = value; } }

        bool _isInclude = false;
        /// <summary>
        /// 是否收录
        /// </summary>
        public bool IsInclude { get { return _isInclude; } set { _isInclude = value; } }
    }
}
