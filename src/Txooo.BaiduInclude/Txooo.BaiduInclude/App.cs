using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Txooo.BaiduInclude.Common;

namespace Txooo.BaiduInclude
{
    static class App
    {
        static System.Threading.Mutex _mutex;

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (CanRun())
            {
                try
                {
                    Application.Run(new MainForm());
                }
                catch (Exception ex)
                {
                    TxLogHelper.GetLogger("app").TxLogError(ex.Message,ex);
                }
            }
        }


        /// <summary>
        /// 是否可以启动窗体
        /// 暂无运行的窗体  &&  没有需要更新的版本  && 网络
        /// </summary>
        /// <returns></returns>
        static bool CanRun()
        {
            bool canRun;
            Attribute guid_attr = Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(GuidAttribute));
            string guid = ((GuidAttribute)guid_attr).Value;
            _mutex = new System.Threading.Mutex(true, guid, out canRun);
            if (!canRun)
            {
                MessageBox.Show("已经在运行了!", AppInfo.AssemblyTitle,
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                canRun = IsConnect();
                if (!canRun)
                {
                    MessageBox.Show("网络异常，请检查网络是否连接!", AppInfo.AssemblyTitle,
                                                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    //不开启自动更新
                    //canRun = AppUpdater.CheckUpdateTask().Result == null;
                }
            }
            return canRun;
        }

        #region 联网校验
        private const int INTERNET_CONNECTION_MODEM = 1;
        private const int INTERNET_CONNECTION_LAN = 2;
        [DllImport("winInet.dll ")]
        private static extern bool InternetGetConnectedState(
        ref int dwFlag,
        int dwReserved
        );
        /// <summary>
        /// 是否联网
        /// </summary>
        static bool IsConnect()
        {
            Int32 dwFlag = new int();
            bool result = true;
            if (!InternetGetConnectedState(ref dwFlag, 0))
            {
                TxLogHelper.GetLogger("netWork").TxLogInfo("网络连接已断开...");
                result = false;
            }
            else if ((dwFlag & INTERNET_CONNECTION_MODEM) != 0)
            {
                TxLogHelper.GetLogger("netWork").TxLogInfo("网络已连接[调治解调器]...");
            }
            else if ((dwFlag & INTERNET_CONNECTION_LAN) != 0)
            {
                TxLogHelper.GetLogger("netWork").TxLogInfo("网络已连接[网卡]...");
            }
            return result;
        }
        #endregion
    }
}
