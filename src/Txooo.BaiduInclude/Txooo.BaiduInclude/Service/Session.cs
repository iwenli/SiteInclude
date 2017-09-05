using System.Net.Http;

namespace Txooo.BaiduInclude.Service
{
    /// <summary>
    /// 表示当前的登录会话，以及一些必须的状态信息。
    /// </summary>
    public class Session
    {
        public Session()
        {
            NetClient = new HttpClient()
            {
                MaxResponseContentBufferSize = 1024 * 1024,
                Timeout = new System.TimeSpan(0, 0, 10)
            };
        }

        /// <summary>
        /// 获得当前使用的网络对象，每个网络对象都是会话关联的。
        /// </summary>
        public HttpClient NetClient { get; private set; }
    }
}
