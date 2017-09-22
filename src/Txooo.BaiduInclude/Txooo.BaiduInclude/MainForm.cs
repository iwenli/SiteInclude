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
        /// <summary>
        /// 设置是否允许关闭的标记位
        /// </summary>
        bool _shutdownFlag = false;

        Task[] tasks = null;
        ServiceContext _context;
        string _checkLogFormat = "连接[{0}]  {1}";
        TaskData _data;

        int _updateMaxCount = 50;

        string _maxDataCount = "500";
        string _isDesc = "false";
        string _maxThreadCount = "50";
        string _searchWhere = "";


        public MainForm()
        {
            InitializeComponent();
            Text = string.Format("{0} V{1} By:{2}", AppInfo.AssemblyTitle, AppInfo.AssemblyVersion, AppInfo.AUTHOR);

            Load += MainForm_Load;
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            GetConfig();
            //服务接入
            await InitServiceContext();
            InitStatusBar();

            tsBtnGoWithUrl.Click += TsBtnGoWithUrl_Click;
            tsTxtUrl.KeyUp += TsTxtUrl_KeyUp;

            tsBtnGo.Click += TsBtnGo_Click;
            Task.Run(() =>
             {
                 AppendLogWarning("当前未检查数据总数：{0} 条", _context.DbService.GetWaitCheakListCountSync().Result);
             });

        }

        async void TsBtnGo_Click(object sender, EventArgs e)
        {
            SaveConfig();
            tsBtnGo.Enabled = false;
            var cts = new CancellationTokenSource();
            if (_data.WaitForCheckTasks.Count > 0)
            {
                AppendLog("继续上次未完成任务，待检查数据{0}条...", _data.WaitForCheckTasks.Count);
                await Task.Delay(2000);
            }
            else
            {
                BeginOperation("开始提取待检查数据...", 0, true);
                var list = await _context.DbService.GetWaitCheakListSync(Convert.ToInt32(_maxDataCount)
                    , Convert.ToBoolean(_isDesc), _searchWhere);
                EndOperation(string.Format("提取结束，共提取 {0} 条记录", list.Count));
                _data.WaitForCheckTasks = new Queue<UrlInfo>(list);
            }
            var _taskCount = Convert.ToInt32(_maxThreadCount);
            if (_taskCount > _data.WaitForCheckTasks.Count)
            {
                _taskCount = _data.WaitForCheckTasks.Count;
            }
            BeginOperation("正在检查是否收录...", _data.WaitForCheckTasks.Count, true);
            tasks = new Task[_taskCount];
            for (int i = 0; i < _taskCount; i++)
            {
                tasks[i] = new Task(() => ChechIncludeTask(cts.Token), cts.Token, TaskCreationOptions.LongRunning);
                tasks[i].Start();
                AppendLogWarning("[全局]任务{0}启动...", i + 1);
            }
            //Task.Run(async () =>
            //{
            //    while (true)
            //    {
            //        if (ProductCache.WaitUploadList.Count == 0 && allCount == ProductCache.UploadFailList.Count + ProductCache.UploadSuccessList.Count)
            //        {
            //            CloseWithResult("商品上传完成...");
            //            break;
            //        }
            //        await Task.Delay(1500);
            //    }
            //});

            //捕捉窗口关闭事件
            //主要是给一个机会等待任务完成并把任务数据都保存
            FormClosing += (_s, _e) =>
           {
               if (_shutdownFlag)
                   return;

               _e.Cancel = !_shutdownFlag;
               AppendLog("[全局] 等待任务结束...");
               cts.Cancel();
               try
               {
                   if (tasks != null && tasks.Length > 0)
                   {
                       Task.WaitAll(tasks);
                   }
               }
               catch (Exception ex)
               {
                   this.TxLogError(ex.Message, ex);
               }
               _shutdownFlag = true;
               TaskContext.Instance.Save();
               Close();
           };

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
            var cleanupcount = 0;
            UrlInfo urlModel = null;
            //token是用来控制队列退出的
            while (!token.IsCancellationRequested)
            {
                urlModel = null;

                lock (_data.WaitForCheckTasks)
                {
                    if (_data.WaitForCheckTasks.Count > 0)
                    {
                        urlModel = _data.WaitForCheckTasks.Dequeue();
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
                if (urlModel.IsInclude)
                {
                    lock (_data.CheckSuccess)
                    {
                        _data.CheckSuccess.Add(urlModel.Id.ToString(), urlModel);
                        //CheckSuccessList.Add(urlModel);
                    }
                    lock (_data.WaitUpdateList)
                    {
                        _data.WaitUpdateList.Add(urlModel);
                        if (_data.WaitUpdateList.Count == _updateMaxCount)
                        {
                            _context.DbService.UpdateIncludeUrl(_data.WaitUpdateList, true);
                            _data.WaitUpdateList.Clear();
                        }
                    }
                }
                else
                {
                    lock (_data.CheckFail)
                    {
                        _data.CheckFail.Add(urlModel.Id.ToString(), urlModel);
                    }
                    lock (_data.WaitSendUrlsList)
                    {
                        _data.WaitSendUrlsList.Add(urlModel);
                        if (_data.WaitSendUrlsList.Count == _updateMaxCount)
                        {
                            _context.BaiduService.LinkSubmitSync(_data.WaitSendUrlsList.Select(m => m.Url));
                            _context.DbService.UpdateIncludeUrl(_data.WaitSendUrlsList, false);
                            _data.WaitSendUrlsList.Clear();
                        }
                    }

                }
                UpdateTaskProcess();
                if (cleanupcount++ > 20)
                {
                    //每20个任务后手动释放一下内存
                    cleanupcount = 0;
                    GC.Collect();
                    //保存任务数据，防止什么时候宕机了任务进度回滚太多
                    TaskContext.Instance.Save();
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
                stStatus.Text = string.Format("已检查:{0}个，共{1}个", stProgress.Value, stProgress.Maximum);
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
            BeginOperation("正在初始化配置信息...", 0, true);
            AppendLog("[全局] 正在初始化...");
            SaveConfig();
            TaskContext.Instance.Init();
            _data = TaskContext.Instance.Data;
            AppendLog("[全局] 初始化完成...");
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

        #region 设置相关
        void GetConfig()
        {
            _maxDataCount = AppConfig.GetItem("MaxDataCount") ?? "500";
            _isDesc = AppConfig.GetItem("IsDesc") ?? "false";
            _searchWhere = AppConfig.GetItem("SearchWhere") ?? "";
            _maxThreadCount = AppConfig.GetItem("MaxThreadCount") ?? "50";

            txtMaxDataCount.Text = _maxDataCount;
            txtMaxThreadCount.Text = _maxThreadCount;
            rchTxtSearchWhere.Text = _searchWhere;
            chkBoxIsDesc.Checked = _isDesc.Equals("true");
        }
        void SaveConfig()
        {
            _maxDataCount = txtMaxDataCount.Text;
            if (string.IsNullOrEmpty(_maxDataCount))
            {
                _maxDataCount = "0";
            }
            _isDesc = chkBoxIsDesc.Checked.ToString().ToLower();
            _searchWhere = rchTxtSearchWhere.Text;
            _maxThreadCount = txtMaxThreadCount.Text;
            if (string.IsNullOrEmpty(_maxDataCount))
            {
                _maxThreadCount = "50";
            }

            AppConfig.ModifyItem("MaxDataCount", _maxDataCount);
            AppConfig.ModifyItem("IsDesc", _isDesc);
            AppConfig.ModifyItem("SearchWhere", _searchWhere);
            AppConfig.ModifyItem("MaxThreadCount", _maxThreadCount);
        }
        #endregion
    }
}
