using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Txooo.BaiduInclude.Service.Entities;

namespace Txooo.BaiduInclude.Service
{
    /// <summary>
    /// 任务数据
    /// </summary>
    public class TaskData
    {

        /// <summary>
        /// 等待检查的队列任务
        /// </summary>
        public Queue<UrlInfo> WaitForCheckTasks { get; set; }

        /// <summary>
        /// 已经收录的集合
        /// </summary>
        public Dictionary<string, UrlInfo> CheckSuccess { get; set; }

        /// <summary>
        /// 未收录的集合
        /// </summary>
        public Dictionary<string, UrlInfo> CheckFail { get; set; }

        /// <summary>
        /// 验证成功等待更新的List  满_updateMaxCount更新一次
        /// </summary>
        public List<UrlInfo> WaitUpdateList { set; get; }

        /// <summary>
        /// 等待提交百度的url集合
        /// </summary>
        public List<UrlInfo> WaitSendUrlsList { set; get; }

        public TaskData()
        {
            WaitForCheckTasks = new Queue<UrlInfo>();
            CheckSuccess = new Dictionary<string, UrlInfo>();
            CheckFail = new Dictionary<string, UrlInfo>();
            WaitUpdateList = new List<UrlInfo>();
            WaitSendUrlsList = new List<UrlInfo>();
        }
    }
}
