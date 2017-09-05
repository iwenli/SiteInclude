using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Txooo.BaiduInclude.Service
{
    /// <summary>
    /// 服务上下文
    /// </summary>
    public class ServiceContext
    {
        public ServiceContext()
        {
            Session = new Session();
            BaiduService = new BaiduService(this);
            DbService = new DbService(this);
        }

        /// <summary>
        /// 获得当前的会话状态
        /// </summary>
        public Session Session { get; private set; }

        /// <summary>
        /// 百度相关服务
        /// </summary>
        public BaiduService BaiduService { get; private set; }

        /// <summary>
        /// 数据库操作服务
        /// </summary>
        public DbService DbService { get; private set; }
    }
}
