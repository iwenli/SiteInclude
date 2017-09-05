using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Txooo.BaiduInclude.Common;
using Txooo.BaiduInclude.Service;
using Txooo.BaiduInclude.Service.Entities;

namespace Txooo.BaiduInclude
{
    public partial class MainForm : Form
    {
        ServiceContext _context;
        string _checkLogFormat = "连接[{0}]  {1}";
        int _updateMaxCount = 50;

        /// <summary>
        /// 等待验证的队列
        /// </summary>
        public Queue<UrlInfo> WaitCheckQueue { set; get; }

        /// <summary>
        /// 验证失败的List
        /// </summary>
        public List<UrlInfo> CheckFailList { set; get; }

        /// <summary>
        /// 验证成功的List
        /// </summary>
        public List<UrlInfo> CheckSuccessList { set; get; }
        /// <summary>
        /// 验证成功等待更新的List  满_updateMaxCount更新一次
        /// </summary>
        public List<UrlInfo> WaitUpdateList { set; get; }

        /// <summary>
        /// 等待提交百度的url集合
        /// </summary>
        public List<string> WaitSendUrlsList { set; get; }

        public MainForm()
        {
            InitializeComponent();
            Text = string.Format("{0} V{1} By:{2}", AppInfo.AssemblyTitle, AppInfo.AssemblyVersion, AppInfo.AUTHOR);

            Load += MainForm_Load;
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            //服务接入
            await InitServiceContext();
            InitStatusBar();

            tsBtnGoWithUrl.Click += TsBtnGoWithUrl_Click;
            tsTxtUrl.KeyUp += TsTxtUrl_KeyUp;

            tsBtnGo.Click += TsBtnGo_Click;
        }

        async void TsBtnGo_Click(object sender, EventArgs e)
        {
            var cts = new CancellationTokenSource();

            BeginOperation("正在提取待检查数据...", 0, true);
            try
            {
                var maxDataCount = ConfigurationManager.AppSettings["MaxDataCount"] ?? "1000";
                var list = await _context.DbService.GetWaitCheakListSync(Convert.ToInt32(maxDataCount));
                EndOperation(string.Format("提取结束，共提取 {0} 条记录", list.Count));
                await Task.Delay(1200);
                if (list.Count > 0)
                {
                    WaitCheckQueue = new Queue<UrlInfo>(list);
                    BeginOperation("正在检查是否收录...", list.Count, true);
                    await ChechIncludeTask(cts.Token);
                    //list = await CheckIsIncludeSync(list);
                    // EndOperation();
                    await Task.Run(async () =>
                    {
                        _context.DbService.UpdateIncludeUrl(WaitUpdateList);
                        WaitUpdateList.Clear();

                        await _context.BaiduService.LinkSubmitSync(WaitSendUrlsList);
                        WaitSendUrlsList.Clear();
                        EndOperation(string.Format("共处理{0}条，成功{1}条，失败{2}条", list.Count, CheckSuccessList.Count, CheckFailList.Count));
                    });
                }
            }
            catch (Exception ex)
            {
                this.TxLogError(ex.Message, ex);
            }
            
        }

        #region 输入连接验证
        void TsTxtUrl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Control || e.KeyCode == Keys.Enter)
            {
                CheckWithInputUrl();
            }
        }

        void TsBtnGoWithUrl_Click(object sender, EventArgs e)
        {
            CheckWithInputUrl();
        }

        async void CheckWithInputUrl()
        {
            BeginOperation("正在处理，请稍等...", 0, true);
            await CheckIsIncludeSync(tsTxtUrl.Text);
            EndOperation();
        }
        #endregion

        #region 验证url是否收录

        async Task ChechIncludeTask(CancellationToken token)
        {
            UrlInfo urlModel = null;
            //token是用来控制队列退出的
            while (!token.IsCancellationRequested)
            {
                urlModel = null;

                lock (WaitCheckQueue)
                {
                    if (WaitCheckQueue.Count > 0)
                    {
                        urlModel = WaitCheckQueue.Dequeue();
                    }
                }
                //如果没有任务，则退出
                if (urlModel == null)
                {
                    break;
                }

                try
                {
                    urlModel.IsInclude = await _context.BaiduService.CheckIsIncludeSync(urlModel.Url);
                    AppendLogWarning(_checkLogFormat, urlModel.Url, urlModel.IsInclude ? "已收录" : "未收录");

                }
                catch (Exception ex)
                {
                    AppendLogError("处理{0} 异常:{1}", urlModel.Url, ex.Message);
                    this.TxLogError(string.Format("处理{0} 异常:{1}", urlModel.Url, ex.Message), ex);
                }
                UpdateTaskProcess();
                if (urlModel.IsInclude)
                {
                    lock (CheckSuccessList)
                    {
                        CheckSuccessList.Add(urlModel);
                    }
                    lock (WaitUpdateList)
                    {
                        WaitUpdateList.Add(urlModel);
                        if (WaitUpdateList.Count == _updateMaxCount)
                        {
                            _context.DbService.UpdateIncludeUrl(WaitUpdateList);
                            WaitUpdateList.Clear();
                        }
                    }
                }
                else
                {
                    lock (CheckFailList)
                    {
                        CheckFailList.Add(urlModel);
                    }
                    lock (WaitSendUrlsList)
                    {
                        WaitSendUrlsList.Add(urlModel.Url);
                        if (WaitSendUrlsList.Count == _updateMaxCount)
                        {
                            _context.BaiduService.LinkSubmitSync(WaitSendUrlsList);
                            WaitSendUrlsList.Clear();
                        }
                    }

                }
            }
        }

        async Task<List<UrlInfo>> CheckIsIncludeSync(List<UrlInfo> list)
        {
            Parallel.For(0, list.Count, async (index) =>
            {
                list[index].IsInclude = await CheckIsIncludeSync(list[index].Url);
            });
            await Task.Delay(10);
            return list;
        }

        async Task<bool> CheckIsIncludeSync(string url)
        {
            var result = false;
            try
            {
                result = await _context.BaiduService.CheckIsIncludeSync(url);
                AppendLogWarning(_checkLogFormat, url, result ? "已收录" : "未收录");
            }
            catch (Exception ex)
            {
                AppendLogError("处理{0} 异常:{1}", url, ex.Message);
                this.TxLogError(string.Format("处理{0} 异常:{1}", url, ex.Message), ex);
            }
            //if (!result)
            //{
            //    WaitSendUrlsList.Add(url);
            //    await _context.BaiduService.LinkSubmitSync(WaitSendUrlsList);
            //    WaitSendUrlsList.Clear();
            //}
            UpdateTaskProcess();
            return result;
        }
        #endregion

        #region 服务接入

        /// <summary>
        /// 更新进度
        /// </summary>
        void UpdateTaskProcess()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    UpdateTaskProcess();
                }));
                return;
            }
            else
            {
                stProgress.Value = stProgress.Value + 1;
            }
            if (stProgress.Value == stProgress.Maximum)
            {
                EndOperation();
            }
        }
        /// <summary>
        /// 初始化状态栏
        /// </summary>
        void InitStatusBar()
        {
            //绑定链接处理
            foreach (var label in st.Items.OfType<ToolStripStatusLabel>().Where(s => s.IsLink && s.Tag != null))
            {
                label.Click += (s, e) =>
                {
                    try
                    {
                        Process.Start((s as ToolStripStatusLabel).Tag.ToString());
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, "错误：无法打开网址，错误信息：" + ex.Message + "。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                };
            }
        }

        /// <summary>
        /// 初始化服务状态
        /// </summary>
        async Task InitServiceContext()
        {
            _context = new ServiceContext();
            CheckFailList = new List<UrlInfo>();
            CheckSuccessList = new List<UrlInfo>();
            WaitUpdateList = new List<UrlInfo>();
            WaitSendUrlsList = new List<string>();

            BeginOperation("正在初始化配置信息...", 0, true);
            await Task.Delay(200);
            EndOperation();
        }
        /// <summary>
        /// 表示开始一个操作
        /// </summary>
        /// <param name="opName">当前操作的名称</param>
        /// <param name="maxItemsCount">当前操作如果需要显示进度，那么提供任务总数；不提供则为跑马灯等待</param>
        /// <param name="disableForm">是否禁用当前窗口的操作</param>
        void BeginOperation(string opName = "正在操作，请稍等...", int maxItemsCount = 100, bool disableForm = false)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    BeginOperation(opName, maxItemsCount, disableForm);
                }));
                return;
            }
            stStatus.Text = opName;
            AppendLog(stStatus.Text);
            stProgress.Visible = true;
            stProgress.Value = 0;
            stProgress.Maximum = maxItemsCount > 0 ? maxItemsCount : 100;
            stProgress.Style = maxItemsCount > 0 ? ProgressBarStyle.Blocks : ProgressBarStyle.Marquee;
            if (disableForm)
            {
                tsBtnGoWithUrl.Enabled = tsBtnGo.Enabled = false;
            }
        }

        /// <summary>
        /// 操作结束
        /// </summary>
        void EndOperation(string opName = "就绪.")
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    EndOperation(opName);
                }));
                return;
            }
            AppendLog(opName);
            stStatus.Text = opName;
            stProgress.Visible = false;
            tsBtnGoWithUrl.Enabled = tsBtnGo.Enabled = true;
        }
        #endregion

        #region 日志相关
        /// <summary>
        /// 追加警告
        /// </summary>
        /// <param name="txtLog"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        protected void AppendLogWarning(string message, params object[] args)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action(() =>
                {
                    AppendLog(Color.Violet, message, args);
                }));
                return;
            }
            AppendLog(Color.Violet, message, args);
        }
        /// <summary>
        /// 追加错误信息
        /// </summary>
        /// <param name="txtLog"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        protected void AppendLogError(string message, params object[] args)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action(() =>
                {
                    AppendLog(Color.Red, message, args);
                }));
                return;
            }
            AppendLog(Color.Red, message, args);
        }
        /// <summary>
        /// 添加日志 定义颜色
        /// </summary>
        /// <param name="fontColor"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void AppendLog(Color fontColor, string message, params object[] args)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action(() =>
                {
                    AppendLog(fontColor, message, args);
                }));
                return;
            }
            txtLog.SelectionColor = fontColor;
            AppendLog(message, args);
        }
        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        protected void AppendLog(string message, params object[] args)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action(() =>
                {
                    AppendLog(message, args);
                }));
                return;
            }
            string timeL = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            txtLog.AppendText(timeL + " => ");
            if (args == null || args.Length == 0)
            {
                txtLog.AppendText(message);
            }
            else
            {
                txtLog.AppendText(string.Format(message, args));
            }
            txtLog.AppendText(Environment.NewLine);
            txtLog.ScrollToCaret();
        }
        #endregion
    }
}
