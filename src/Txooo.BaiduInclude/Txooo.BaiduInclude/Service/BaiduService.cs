using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Txooo.BaiduInclude.Service
{
    public class BaiduService : ServiceBase
    {
        const string TOKEN = @"KtnGR7zBD8930TQr";

        Regex _containerReg = new Regex("<div class=\"result c-container \" id=\"1\"[\\s\\S]*?百度快照");
        Regex checkReg = new Regex(@"没有找到该URL。您可以直接访问|很抱歉，没有找到与");  //提交网址[\s\S]*反馈给我们
        Regex _urlReg = new Regex(@"(((^https?:(?:\/\/)?)(?:[-;:&=\+\$,\w]+@)?[A-Za-z0-9.-]+|(?:www.|[-;:&=\+\$,\w]+@)[A-Za-z0-9.-]+)((?:\/[\+~%\/.\w-_]*)?\??(?:[-\+=&;%@.\w_]*)#?(?:[\w]*))?)");


        HttpClient client = new HttpClient();
       

        /// <summary>
        /// 初始化一个 BaiduService 实例
        /// </summary>
        /// <param name="context"></param>
        public BaiduService(ServiceContext context) : base(context)
        {
            NetClient.GetAsync("http://www.baidu.com");

            client.DefaultRequestHeaders.Add("user-agent", "curl/7.12.1");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "text/plain");
            client.DefaultRequestHeaders.Host = "data.zz.baidu.com";
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Length", "83");
        }

        /// <summary>
        /// 检查是否收录
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<bool> CheckIsIncludeSync(string url)
        {
            var searchUrl = "http://www.baidu.com/s?wd=" + url;
            var response = await NetClient.GetAsync(searchUrl);
            try
            {
                response.EnsureSuccessStatusCode();
                string resultStr = await response.Content.ReadAsStringAsync();
                if (checkReg.IsMatch(resultStr))
                {
                    return false;
                }
                var includeHtmlContainer = _containerReg.Match(resultStr).Value;
                var domain = "";
                if (_urlReg.IsMatch(url))
                {
                    domain = _urlReg.Match(url).Groups[2].Value.Replace(_urlReg.Match(url).Groups[3].Value, "");
                }
                if (string.IsNullOrEmpty(includeHtmlContainer))
                {
                    return false;
                }
                if (includeHtmlContainer.IndexOf(url.Remove(url.Length - 1, 1)) > -1)
                {
                    return true;
                }
                if (!string.IsNullOrEmpty(domain) && includeHtmlContainer.IndexOf(domain) == -1)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                this.TxLogError(ex.Message, ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 提交收录
        /// </summary>
        /// <param name="urls"></param>
        /// <returns></returns>
        public async Task<bool> LinkSubmitSync(IEnumerable<string> urls)
        {
            //TODO:需要的话记录结果 但是需要登录
            var searchUrl = @"http://data.zz.baidu.com/urls?site={0}&token=KtnGR7zBD8930TQr";
            var domain = "www.txooo.com";
            var tempUrl = urls.FirstOrDefault();
            if (_urlReg.IsMatch(tempUrl))
            {
                domain = _urlReg.Match(tempUrl).Groups[2].Value.Replace(_urlReg.Match(tempUrl).Groups[3].Value, "");
            }

            var content = Encoding.Default.GetBytes(string.Join(Environment.NewLine, urls));
            var response = await client.PostAsync(string.Format(searchUrl, domain), new ByteArrayContent(content)); 

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                this.TxLogError(ex.Message, ex);
            }
            string resultStr = await response.Content.ReadAsStringAsync();
            TxLogHelper.GetLogger("提交百度记录结果").TxLogInfo(resultStr);
            return true;
        }
    }
}
