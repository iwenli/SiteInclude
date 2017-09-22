using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Txooo.BaiduInclude.Service.Entities;
using Txooo.Data;
using Txooo.Data.Entity;

namespace Txooo.BaiduInclude.Service
{
    public class DbService : ServiceBase
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        const string CON_STR = @"Server=10.10.10.29;Database=TxoooBrands;UID=TxoooNewDataBaseDesignUser;Password=Tx)))NewSJCoolSJPassWORderAdmin;";

        /// <summary>
        /// 初始化一个 DbService 实例
        /// </summary>
        /// <param name="context"></param>
        public DbService(ServiceContext context) : base(context)
        {
            NetClient.GetAsync("http://www.baidu.com");
        }

        /// <summary>
        /// 获取待检查数据总数
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetWaitCheakListCountSync()
        {
            var _count = 0;
            await Task.Run(() =>
            {
                using (TxDataHelper helper = TxDataHelper.GetDataHelper(DatabaseType.Sql, CON_STR))
                {
                    string _sql = @"SELECT COUNT(1) FROM dbo.main_link WHERE baidu_included = 0";
                    _count = Convert.ToInt32(helper.SqlScalar(_sql) ?? "0");
                }
            });
            return _count;
        }


        /// <summary>
        /// 获取待检查URL集合
        /// </summary>
        /// <param name="top">取多少条，默认-1 表示全部</param>
        /// <param name="isDesc">是否按link降序</param>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public async Task<List<UrlInfo>> GetWaitCheakListSync(long top = -1, bool isDesc = true, string where = "")
        {
            var list = new List<UrlInfo>();
            await Task.Run(() =>
            {
                using (TxDataHelper helper = TxDataHelper.GetDataHelper(DatabaseType.Sql, CON_STR))
                {
                    string _sql = @" SELECT {0} id AS ID,link AS Url FROM [main_link] WHERE baidu_included = 0 {2} ORDER BY ID {1}";
                    var _dt = helper.SqlGetDataTable(string.Format(_sql, top == -1 ? "" : "TOP " + top.ToString(), isDesc ? "DESC" : "ASC", where));
                    list.AddRange(DataEntityHelper.GetEntityList<UrlInfo>(_dt));
                }
            });
            return list;
        }

        /// <summary>
        /// 更新到库
        /// </summary>
        /// <param name="list">待更新集合</param>
        /// <param name="isInclude">是否包含</param>
        /// <returns></returns>
        public bool UpdateIncludeUrl(List<UrlInfo> list, bool isInclude)
        {
            if (list.Count == 0) { return true; }
            using (TxDataHelper helper = TxDataHelper.GetDataHelper(DatabaseType.Sql, CON_STR))
            {
                string _sql = @"UPDATE [main_link] SET baidu_included = {0},is_run = 1,run_time = GETDATE() WHERE id IN ({1})";
                return helper.SqlExecute(string.Format(_sql, string.Join(",", list.Select(m => m.Id).ToArray()))) > 0;
            }
        }
    }
}
