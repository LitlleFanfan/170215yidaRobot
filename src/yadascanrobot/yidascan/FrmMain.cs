using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using yidascan.DataAccess;
using ProduceComm;
using ProduceComm.Scanner;
using Newtonsoft.Json;
using ProduceComm.OPC;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Data.SqlClient;

using commonhelper;
using ListBoxHelper.ext;

namespace yidascan {
    public partial class FrmMain : Form {
        NormalScan nscan1;

        public static TaskQueues taskQ;
        bool isrun = false;
        CacheHelper cacheher;
        #region opc
        public static OPCParam opcParam = new OPCParam();

        public static IOpcClient opcClient;

        #endregion

        DataTable dtopc;

        IRobotJob robot;
        private bool robotRun = false;

        public static LogOpreate logOpt;

        private DateTime StartTime;

        // 用于锁定手动和自动扫描标签的处理工程。
        public object LOCK_CAMERA_PROCESS = new object();

        private int counter = 0;

        private IErpApi callErpApi;

        public FrmMain() {
            InitializeComponent();

#if !DEBUG
            btnTestPlc.Visible = false;
#endif
            logOpt = new ProduceComm.LogOpreate();

            try {
                // 显示效果不对，以后再说。
                InitListBoxes();

                timer_message.Enabled = true;

                var msgStart = string.Format("{0} V{1} 启动。",
                                  clsSetting.PRODUCT_NAME,
                                  Application.ProductVersion.ToString());
                logOpt.Write(msgStart, LogType.NORMAL);

                lblOpcIp.BackColor = Color.LightGreen;

                TaskQueues.onlog = (string msg, string group) => {
                    this.Invoke((Action)(() => {
                        logOpt.Write(msg, group);
                    }));
                };
            } catch (Exception ex) {
                logOpt.Write(string.Format("!启动失败.\n{0}", ex), LogType.NORMAL);
            }
        }

        private static TaskQueues loadconf(string fn = null) {
            try {
                var confpath = string.IsNullOrEmpty(fn)
                    ? Path.Combine(Application.StartupPath, TASKQUE_CONF)
                    : fn;
                return TaskQueConf<TaskQueues>.load(confpath);
            } catch (Exception ex) {
                logOpt.Write($"!加载队列文件异常: {ex}。");
                return null;
            }
        }

        // 运行效果不正确。无红字显示。
        private void InitListBoxes() {
            lsvBufferLog.Initstyle();
            lsvLog.Initstyle();
            lsvRobotStackLog.Initstyle();
            lsvAlarmLog.Initstyle();
        }

        private void ShowTaskState(bool running) {
            lbTaskState.BackColor = running ? Color.LightGreen : Color.Orange;
            lbTaskState.Text = running ? "任务启动" : "任务停止";
        }

        private void ShowTitle() {
            Text = $"{clsSetting.PRODUCT_NAME} V{Application.ProductVersion.ToString()}";
        }

        public void ShowTaskQ() {
            showLabelQue(taskQ.WeighQ, lsvWeigh);
            showLabelQue(taskQ.CacheQ, lsvCacheBefor);
            showLabelQue(taskQ.LableUpQ, lsvLableUp);
            showLabelQue(taskQ.CatchAQ, lsvCatch1);
            showLabelQue(taskQ.CatchBQ, lsvCatch2);
            showRobotQue(taskQ.RobotRollAQ, lsvRobotA);
            showRobotQue(taskQ.RobotRollBQ, lsvRobotB);
            showCachePosQue(taskQ.CacheSide);
        }

        private void FrmMain_Load(object sender, EventArgs e) {
            try {
                LayerShape.loadconf();

                taskQ = loadconf() ?? new TaskQueues();
                cacheher = new CacheHelper(taskQ.CacheSide);

                ShowTaskQ();

                StartOpcParam();
                PlcSlot.loadBadShapeLocation();

                opcClient = CreateOpcClient("");

                ShowTitle();
                ShowTaskState(false);
                RefreshRobotMenuState();
                cmbShiftNo.SelectedIndex = 0;

                SetRobotTip(false);
                SetButtonState(false);
                InitCfgView();
                LableCode.DeleteAllFinished();

            } catch (Exception ex) {
                logOpt.Write($"!初始化失败, {ex.ToString()}。\n{ex}", LogType.NORMAL);
            }
        }

        private void queryOpcParam() {
            try {
                dtopc = OPCParam.Query();
                dtopc.Columns.Remove("Class");
                dtopc.Columns.Add(new DataColumn("Value"));
            } catch (NullReferenceException ex) {
                throw new Exception("从数据库加载opc参数失败，请检查数据库连接");
            }
        }

        /// <summary>
        /// 在主程序启动时运行。
        /// </summary>
        private void StartOpcParam() {
            queryOpcParam();
            opcParam.Init();

            logOpt.Write(JsonConvert.SerializeObject(opcParam), LogType.NORMAL, LogViewType.OnlyFile);
            logOpt.Write($"板宽：{clsSetting.SplintWidth}间隙：{clsSetting.RollSep}边缘预留：{clsSetting.EdgeSpace}" +
                $"忽略偏差：{clsSetting.CacheIgnoredDiff}奇数层横放：{clsSetting.OddTurn}第一层放布Z：{clsSetting.InitHeigh}",
                LogType.NORMAL, LogViewType.OnlyFile);
        }

        private void setupOpcClient(IOpcClient c, string name) {
            if (c.Open(clsSetting.OPCServerIP)) {
                logOpt.Write($"{name}OPC client连接成功。", LogType.NORMAL);
                c.AddSubscription(dtopc); // 这个也写在plchelper比较好。
                PlcHelper.subscribe(c);
            } else {
                logOpt.Write($"!{name}OPC client务连接失败。", LogType.NORMAL);
            }
        }

        private IOpcClient CreateOpcClient(string name) {
            var client = GetOpcClient();
            setupOpcClient(client, name);            
            return client;
        }

        /// <summary>
        /// 启动机器人布卷队列等待。
        /// </summary>
        private void StartRobotJobATask() {
            // setup opc client.
            var RobotOpcClientA = CreateOpcClient("机器人队列A");

            Task.Factory.StartNew(() => {
                while (isrun) {
                    // 等待布卷
                    var r = RobotOpcClientA.ReadBool(PlcSlot.ITEM_CATCH_A);
                    if (r) {
                        // 加入机器人布卷队列。
                        var code = taskQ.GetCatchAQ();
                        if (code != null) {
                            RobotOpcClientA.Write(PlcSlot.ITEM_CATCH_A, false);

                            showLabelQue(taskQ.CatchAQ, lsvCatch1);
                            showRobotQue(taskQ.RobotRollAQ, lsvRobotA);
                        }
                    }
                    Thread.Sleep(1000);
                }
            });
            logOpt.Write("机器人抓料A布卷队列任务启动。", LogType.NORMAL);
        }

        /// <summary>
        /// 启动机器人布卷队列等待。
        /// </summary>
        private void StartRobotJobBTask() {
            var RobotOpcClientB = CreateOpcClient("机器人队列B");
            Task.Factory.StartNew(() => {
                while (isrun) {
                    // 等待布卷                        
                    var r = RobotOpcClientB.ReadBool(PlcSlot.ITEM_CATCH_B);
                    if (r) {
                        // 加入机器人布卷队列。
                        var code = taskQ.GetCatchBQ();
                        if (code != null) {
                            RobotOpcClientB.Write(PlcSlot.ITEM_CATCH_B, false);

                            showLabelQue(taskQ.CatchBQ, lsvCatch2);
                            showRobotQue(taskQ.RobotRollBQ, lsvRobotB);
                        }
                    }
                    Thread.Sleep(1000);
                }
            });
            logOpt.Write("机器人抓料B布卷队列任务启动。", LogType.NORMAL);
        }

        /// <summary>
        /// 去OPC的可放料信号。
        /// </summary>
        /// <param name="tolocation"></param>
        /// <returns></returns>
        [Obsolete("use plchelper.IsPanelAvailable instead.")]
        public static bool PanelAvailable(string tolocation) {
            try {
                lock (opcClient) {
                    var s = opcClient.ReadString(opcParam.BAreaPanelState[tolocation]);
                    return s == "2";
                }
            } catch (Exception ex) {
                logOpt.Write($"读交地状态信号异常 tolocation: {tolocation} opc:{JsonConvert.SerializeObject(opcParam.BAreaFloorFinish)} err:{ex}", LogType.ROBOT_STACK);
                return true;//临时
            }
        }

        public static PanelState GetPanelState(LableCode label, PanelInfo pinfo) {
            var state = PanelState.LessHalf;
            if (label.Floor >= pinfo.MaxFloor - 1) {
                state = PanelState.HalfFull;
            }
            if (pinfo.Status == 5 && LableCode.IsPanelLastRoll(pinfo.PanelNo, label.LCode)) {
                state = PanelState.Full;
            }
            return state;
        }

        private static IRobotJob GetRobot(string ip, string jobname) {
#if DEBUG
            return new FakeRobotJob(ip, jobname);
#else
            return new RobotHelper(ip, jobname);
#endif
        }

        private void StartRobotTask() {
            var RobotOpcClient = CreateOpcClient("机器人");
            try {
                logOpt.Write("机器人正在启动...", LogType.NORMAL);
                Task.Factory.StartNew(() => {
                    robot = GetRobot(clsSetting.RobotIP, clsSetting.JobName);
                    robot.setup(logOpt.Write, RobotOpcClient, opcParam);

                    if (robot.IsConnected()) {
                        this.Invoke((Action)(() => { lblRobot.BackColor = Color.LightGreen; }));

                        SetRobotTip(true);
                        logOpt.Write("开始机器人线程。", LogType.NORMAL);

                        robot.JobLoop(ref robotRun, lsvRobotA, lsvRobotB);

                        logOpt.Write("机器人启动正常。", LogType.NORMAL);
                    } else {
                        SetRobotTip(false, "机器人网络故障");
                        logOpt.Write("!机器人网络故障。", LogType.NORMAL);
                    }
                });
            } catch (Exception ex) {
                logOpt.Write($"机器人启动异常。{ex}", LogType.NORMAL);
            }
        }

        /// <summary>
        /// B区人工完成信号任务。
        /// </summary>
        private void BAreaUserFinalLayerTask() {
            var client = CreateOpcClient("B区手动完成");

            foreach (KeyValuePair<string, string> kv in opcParam.BAreaUserFinalLayer) {
                Task.Factory.StartNew(() => {
                    while (isrun) {
                        var signal = client.ReadString(kv.Value);

                        if (signal == "1") {
                            // kv.Key是交地。
                            var tolocation = kv.Key;

                            // 修改当前板号的属性。
                            var shiftno = createShiftNo();

                            var pf = LableCode.GetTolactionCurrPanelNo(tolocation, shiftno);
                            LableCode.SetMaxFloorAndFull(tolocation);
                            logOpt.Write($"{kv.Key}收到人工完成信号。", LogType.NORMAL, LogViewType.OnlyFile);

                            // 创建新的板信息。
                            var newPanel = PanelGen.NewPanelNo();

                            // 重新计算缓存区的布卷的坐标。
                            cacheher.ReCalculateCoordinate(newPanel, tolocation);

                            //处理满板信号
                            robot.NotifyOpcJobFinished(
                                LableCode.IsAllRollOnPanel(pf.PanelNo) ? PanelState.Full : PanelState.HalfFull,
                                tolocation);

                            // plc复位信号。
                            client.Write(kv.Value, "0");
                        }
                        Thread.Sleep(OPCClient.DELAY * 200);
                    }
                });
            }

            logOpt.Write("B区人工完成信号任务启动。", LogType.NORMAL);
        }

        void SetButtonState(bool isRun) {
            this.Invoke((EventHandler)(delegate {
                btnSet.Enabled = !isRun;
                btnRun.Enabled = !isRun;
                btnQuit.Enabled = !isRun;
                btnNewRun.Enabled = !isRun;

                dtpDate.Enabled = !isRun;
                cmbShiftNo.Enabled = !isRun;

                grbHandwork.Enabled = isRun;
                btnStop.Enabled = isRun;
            }));
        }

        public void Logger(string s) {
            logOpt.Write(s);
        }

        private void StartScanner() {
            const string CAMERA_1 = "1#相机";
            if (OpenPort(ref nscan1, CAMERA_1, FrmSet.pcfgScan1)) {
                nscan1.logger = Logger;
                nscan1.OnDataArrived = nscan_OnDataArrived;
                // 启动相机读取线程。

                var ScannerOpcClient = CreateOpcClient("相机");
                nscan1._StartJob(ScannerOpcClient);

                lblScanner.BackColor = Color.LightGreen;
                logOpt.Write(nscan1.name + "ok.");
            } else {
                ShowWarning("启动相机失败。");
                logOpt.Write($"!{nscan1.name}启动失败！", LogType.NORMAL);
            }
        }

        private void btnRun_Click(object sender, EventArgs e) {
            StartTime = DateTime.Now;
            dtpDate.Value = DateTime.Now;

            SetButtonState(true);
            logOpt.Write(string.Format("!系统流程开始运行"), LogType.NORMAL);

            // 产生一个IErpApi实例，如果参数是false，则生成模拟的接口。
            // 如果参数是true，则调用真正的web api接口。

            initErpApi();

            PanelGen.Init(dtpDate.Value);

            StartScanner();
            isrun = true;

            WeighTask();
            ACAreaFinishTask();
            BeforCacheTask_new();
            LableUpTask();

            StartRobotJobATask();
            StartRobotJobBTask();
            BAreaUserFinalLayerTask();

            if (chkUseRobot.Checked) {
                StartAllRobotTasks();
            } else {
                logOpt.Write("未使用机器人。", LogType.NORMAL);
            }

            // 焦点设在手工输入框。
            txtLableCode1.Focus();

            chkUseRobot.Enabled = false;
            ShowTaskState(isrun);
            RefreshRobotMenuState();
        }

        private void WeighTask() {
            const int TO_WEIGH = 1;
            const int SUCCESS = 0;
            const int FAIL = 2;

            var client = CreateOpcClient("称重");

            Task.Factory.StartNew(() => {
                while (isrun) {
                    try {
                        var signal = client.ReadInt(opcParam.ScanParam.GetWeigh);
                        if (signal == TO_WEIGH) {
                            var code = taskQ.GetWeighQ();

                            if (code != null) {
                                signal = NotifyWeigh(code.LCode, false) ? SUCCESS : FAIL;
                                logOpt.Write($"{code.LCode}称重API状态：{signal} 写OPC状态：{client.Write(opcParam.ScanParam.GetWeigh, signal)}");

                                showLabelQue(taskQ.WeighQ, lsvWeigh);
                                if (code.ToLocation.Substring(0, 1) == "B") {
                                    showLabelQue(taskQ.CacheQ, lsvCacheBefor);//加到缓存列表中显示
                                }
                            } else {
                                logOpt.Write($"称重信号无对应数据");
                            }
                        }
                    } catch (Exception ex) {
                        logOpt.Write($"!weigh task: {ex.ToString()}", LogType.NORMAL);
                    }

                    Thread.Sleep(OPCClient.DELAY * 200);
                }
            });

            logOpt.Write("称重任务启动。", LogType.NORMAL);
        }

        private void ACAreaFinishTask() {
            var opcclient = CreateOpcClient("AC区完成");
            foreach (var kv in opcParam.ACAreaPanelFinish) {
                Task.Factory.StartNew(() => {
                    while (isrun) {
                        try {
                            var signal = opcclient.ReadBool(kv.Value.Signal);
                            if (signal) {
                                var fullLable = PlcHelper.ReadCompleteLable(opcclient, kv.Value);

                                logOpt.Write($"{kv.Value.Signal} 收到完成信号。标签:{fullLable} 执行状态:{AreaAAndCFinish(fullLable)}", LogType.NORMAL);
                            }
                        } catch (Exception ex) {
                            logOpt.Write("!" + ex.Message);
                        }

                        opcclient.Write(kv.Value.Signal, 0);

                        Thread.Sleep(OPCClient.DELAY * 2000);
                    }
                });
            }

            logOpt.Write("AC区完成信号任务启动。", LogType.NORMAL);
        }

        private void BindQueue(LableCode code, LableCode outCacheLable, CacheResult cr) {
            try {
                switch (cr.state) {
                    case CacheState.Go:
                        lock (taskQ.LableUpQ) {
                            taskQ.LableUpQ.Enqueue(code);
                        }

                        showLabelQue(taskQ.CacheQ, lsvCacheBefor);
                        showLabelQue(taskQ.LableUpQ, lsvLableUp);
                        break;
                    case CacheState.Cache:
                        showLabelQue(taskQ.CacheQ, lsvCacheBefor);
                        showCachePosQue(taskQ.CacheSide);
                        break;
                    case CacheState.GetThenCache:
                        lock (taskQ.LableUpQ) {
                            taskQ.LableUpQ.Enqueue(outCacheLable);
                        }

                        showLabelQue(taskQ.LableUpQ, lsvLableUp);
                        showLabelQue(taskQ.CacheQ, lsvCacheBefor);
                        showCachePosQue(taskQ.CacheSide);
                        break;
                    case CacheState.CacheAndGet:
                        lock (taskQ.LableUpQ) {
                            taskQ.LableUpQ.Enqueue(outCacheLable);
                        }

                        showLabelQue(taskQ.CacheQ, lsvCacheBefor);
                        showCachePosQue(taskQ.CacheSide);
                        break;
                    case CacheState.GoThenGet:
                        lock (taskQ.LableUpQ) {
                            taskQ.LableUpQ.Enqueue(code);
                            taskQ.LableUpQ.Enqueue(outCacheLable);
                        }

                        showLabelQue(taskQ.CacheQ, lsvCacheBefor);
                        showLabelQue(taskQ.LableUpQ, lsvLableUp);
                        showCachePosQue(taskQ.CacheSide);
                        break;
                    case CacheState.GetThenGo:
                        lock (taskQ.LableUpQ) {
                            taskQ.LableUpQ.Enqueue(outCacheLable);
                            taskQ.LableUpQ.Enqueue(code);
                        }

                        showLabelQue(taskQ.CacheQ, lsvCacheBefor);
                        showLabelQue(taskQ.CacheQ, lsvCacheBefor);
                        showCachePosQue(taskQ.CacheSide);
                        break;
                    default:
                        break;
                }
            } catch (Exception ex) {
                logOpt.Write($"!{ex}", LogType.BUFFER);
                logOpt.Write($"!code: {code.LCode}, outlable: {outCacheLable}, state: {nameof(cr.state)}, {cr.state}");
            }
        }

        private string createShiftNo() {
            var d1 = dtpDate.Value.ToString(clsSetting.LABEL_CODE_DATE_FORMAT);
            var d2 = cmbShiftNo.SelectedIndex.ToString();
            return $"{d1}{d2}";
        }

        private void BeforCacheTask_new() {
            logOpt.Write("!新版缓存计算任务启动。", LogType.NORMAL);

            var CacheOpcClient = CreateOpcClient("缓存位");

            Task.Factory.StartNew(() => {
                while (isrun) {
                    Thread.Sleep(OPCClient.DELAY * 200);

                    try {
                        if (PlcHelper.ReadCacheSignal(CacheOpcClient)) {
                            if (taskQ.CacheQ.Count == 0) continue;

                            var lc = taskQ.CacheQ.Peek();
                            lc = LableCode.QueryByLCode(lc.LCode);

                            if (lc == null) {
                                logOpt.Write($"!缓存队列没有标签", LogType.BUFFER);
                                continue;
                            }

                            // 检查重复计算。???
                            if (!string.IsNullOrEmpty(lc.PanelNo)) {
                                logOpt.Write($"!{lc.LCode} 标签重复。", LogType.BUFFER);
                                continue;
                            }

                            var t = TimeCount.TimeIt(() => {
                                // 计算位置, lc和cache队列里比较。
                                var calResult = LableCodeBllPro.AreaBCalculate(CacheOpcClient,
                                    callErpApi,
                                    lc,
                                    createShiftNo(),
                                    taskQ.GetBeforCacheLables(lc),
                                    x => { logOpt.Write(x, LogType.BUFFER); }
                                    );

                                // 确定缓存操作动作
                                var cacheJobState = cacheher.WhenRollArrived(calResult.state, calResult.CodeCome, calResult.CodeFromCache);
                                logOpt.Write($"{calResult.CodeCome.ToLocation} {JsonConvert.SerializeObject(cacheJobState)}" +
                                    $" 来料标签：{calResult.CodeCome.LCode} {calResult.CodeCome.Diameter} " +
                                    $"取出标签：{calResult.CodeFromCache?.LCode} {calResult.CodeFromCache?.Diameter}  " +
                                    $"{calResult.message}", LogType.BUFFER);

                                lock (taskQ.CacheQ) {
                                    taskQ.CacheQ.Dequeue();
                                }

                                // 这个为什么不放在whenRollArrived里面呢?
                                if (cacheJobState.state == CacheState.CacheAndGet || cacheJobState.state == CacheState.GetThenCache) {
                                    if (CacheHelper.isInSameCacheChannel(cacheJobState.getpos, cacheJobState.savepos)) {
                                        // 在同一侧
                                        cacheJobState.state = CacheState.GetThenCache;  // action 3.
                                    } else {
                                        cacheJobState.state = CacheState.CacheAndGet;   // action 6
                                    }
                                }

                                // 更新界面显示
                                BindQueue(lc, calResult.CodeFromCache, cacheJobState);

                                var ts = TimeCount.TimeIt(() => {
                                    // 发出机械手缓存动作指令
                                    PlcHelper.WriteCacheJob(CacheOpcClient, cacheJobState.state, cacheJobState.savepos, cacheJobState.getpos);
                                });

                                logOpt.Write($"计算缓存写OPC耗时:　{ts}ms", LogType.BUFFER);
                            });
                            logOpt.Write($"计算缓存总耗时:　{t}ms", LogType.BUFFER);
                        }
                    } catch (Exception ex) {
                        logOpt.Write($"!{ex.ToString()}", LogType.BUFFER);
                    }
                }
            });
        }

        /// <summary>
        /// 2期代码。
        /// </summary>
        private void LableUpTask() {
            logOpt.Write("标签朝上任务启动。", LogType.NORMAL);

            var LabelUpOpcClient = CreateOpcClient("标签朝上");

            Task.Factory.StartNew(() => {
                while (isrun) {
                    try {
                        var r = LabelUpOpcClient.ReadBool(PlcSlot.LABEL_UP_SIGNAL);

                        if (r) {
                            var code = taskQ.GetLableUpQ(); if (code != null) {
                                logOpt.Write(string.Format("收到标签朝上来料信号。号码: {0}", code.LCode), LogType.ROLL_QUEUE);

                                // 写plc直径和分道号。1~5号板走1道， 其他走2道。
                                PlcHelper.WriteLabelUpData(LabelUpOpcClient, code.Diameter, int.Parse(code.ParseLocationNo()) < 6 ? RollCatchChannel.channel_1 : RollCatchChannel.channel_2);

                                showLabelQue(taskQ.LableUpQ, lsvLableUp);
                                showCachePosQue(taskQ.CacheSide);
                                if (int.Parse(code.ParseLocationNo()) < 6) {
                                    showLabelQue(taskQ.CatchAQ, lsvCatch1);
                                } else {
                                    showLabelQue(taskQ.CatchBQ, lsvCatch2);
                                }
                            }
                        }
                    } catch (Exception ex) {
                        logOpt.Write($"!{ex}", LogType.ROLL_QUEUE);
                    }
                    Thread.Sleep(OPCClient.DELAY * 200);
                }
            });
        }

        void nscan_OnDataArrived(IOpcClient client, string type, string code, int scanNo) {
            if (code == "ERROR" || code.Length < 12) { return; }

            // 条码请取前面12位,有些扫描器会扫出13位是因为把后面的识别码也读出来了.
            // 摘自2016年9月10日(星期六) 下午2:37邮件:答复: 答复: 9月9号夜班布卷扫描枪PC连接不上ERP说明
            code = code.Substring(0, 12);

            // wait for opc available.
            // must use try/finally block to release this mutex.
            lock (LOCK_CAMERA_PROCESS) {
                ScanLableCode(client, code, scanNo, false);
            }
        }

        private static bool OpenPort(ref NormalScan nscan, string scannerName, CommunicationCfg cfg) {
            try {
                switch ((CommunicationType)Enum.Parse(typeof(CommunicationType), cfg.CommunicationType, true)) {
                    case CommunicationType.Network:
                        nscan = new NormalScan(scannerName, new TcpIpManage(cfg.IPAddr, int.Parse(cfg.IPPort)));
                        break;
                    case CommunicationType.SerialPort:
                        nscan = new NormalScan(scannerName, new SerialPortManage(cfg.ComPort, int.Parse(cfg.BaudRate)));
                        break;
                }
                return nscan.Open();
            } catch (Exception ex) {
                clsSetting.loger.Error(string.Format("!{0}", ex.ToString()));
                return false;
            }
        }

        private static void StopScanner(NormalScan scanner) {
            if (scanner == null) { return; }
            try {
                scanner._StopJob();
                Thread.Sleep(500);
                scanner.Close();
                logOpt.Write("关闭相机：" + scanner.name);
            } catch (Exception ex) {
                var msg = $"!{scanner.name}关闭失败。\n{ex}";
                logOpt.Write(msg, LogType.NORMAL);
            }
        }

        private void StartAllRobotTasks() {
            robotRun = true;
            StartRobotTask();
        }

        private void StopAllRobotTasks() {
            robotRun = false;

            Thread.Sleep(1000);

            try {
                if (chkUseRobot.Checked) {
                    robot.Dispose();
                }
            } catch (Exception ex) {
                logOpt.Write("!" + ex.ToString());
            }

            logOpt.Write("机器人任务停止。", LogType.NORMAL);
            SetRobotTip(false, "机器人停止");
        }

        private void StopAllJobs() {
            isrun = false;

            Thread.Sleep(500);

            StopScanner(nscan1);
            StopAllRobotTasks();
        }

        private void btnStop_Click(object sender, EventArgs e) {
            if (!CommonHelper.Confirm("确定停止吗?")) { return; }

            StopAllJobs();

            chkUseRobot.Enabled = true;
            SetButtonState(false);
            ShowTaskState(isrun);
            RefreshRobotMenuState();

            logOpt.Write("停止操作完成。", LogType.NORMAL);
        }

        private void btnSet_Click(object sender, EventArgs e) {
            using (var w = new FrmSet()) {
                if (w.ShowDialog() == DialogResult.OK) {
                    InitCfgView();
                }
            }
        }

        private void InitCfgView() {
            switch (FrmSet.pcfgScan1.CommunicationType) {
                case "Network":
                    lblScanner.Text = string.Format("Scanner1:{0}/{1}",
                        FrmSet.pcfgScan1.IPAddr, FrmSet.pcfgScan1.IPPort);
                    break;
                case "SerialPort":
                    lblScanner.Text = string.Format("Scanner1:{0}/{1}",
                        FrmSet.pcfgScan1.ComPort, FrmSet.pcfgScan1.BaudRate);
                    break;
                default:
                    lblScanner.Text = string.Format("Scanner1:Unknown");
                    break;
            }

            lblOpcIp.Text = string.Format("OPC server ip:{0}", clsSetting.OPCServerIP);
            lblRobot.Text = string.Format("Robot:{0}/{1}", clsSetting.RobotIP, clsSetting.JobName);
        }

        private void SetRobotTip(bool run, string msg = "") {
            if (run) {
                this.Invoke((Action)(() => {
                    lbRobotState.ForeColor = Color.Green;
                    lbRobotState.Text = string.IsNullOrEmpty(msg) ? "机器人已启动" : msg;
                    lblRobot.BackColor = Color.LightGreen;
                }));
            } else {
                this.Invoke((Action)(() => {
                    lbRobotState.ForeColor = Color.Red;
                    lbRobotState.Text = string.IsNullOrEmpty(msg) ? "机器人未启动" : msg;
                    lblRobot.BackColor = Color.LightGray;
                }));
            }
        }

        private void btnQuit_Click(object sender, EventArgs e) {
            logOpt.Write("程序正常退出。", LogType.NORMAL);
            Close();
        }

        private void btnToWeigh_Click(object sender, EventArgs e) {
            NotifyWeigh("");
        }

        /// <summary>
        /// 手工称重函数。界面按钮调用此函数。
        /// 成功返回true, 失败返回false.
        /// </summary>
        /// <param name="handwork"></param>
        /// <param name="code">标签号码</param>
        /// <returns></returns>
        [Obsolete("应使用erphelper.NotifyErp函数")]
        private bool NotifyWeigh(string code, bool handwork = true) {
            try {
                var re = callErpApi.Post(clsSetting.ToWeight,
                    new Dictionary<string, string>() { { "Fabric_Code", code } });

                var msg = string.Format("{0} {1}称重{2}", code, (handwork ? "手工" : "自动"), JsonConvert.SerializeObject(re));
                logOpt.Write(msg, LogType.NORMAL, LogViewType.OnlyFile);

                if (re["ERPState"] == "OK") {
                    var re1 = JsonConvert.DeserializeObject<DataTable>(re["Data"]);
                    return (re1.Rows[0]["result"].ToString().ToUpper() != "FAIL");
                } else {
                    ERPAlarm(opcClient, opcParam, ERPAlarmNo.COMMUNICATION_ERROR);
                    return false;
                }
            } catch (Exception ex) {
                logOpt.Write(string.Format("!称重调用webapi异常: {0}", ex));
                return false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e) {
            if (isrun) {
                var span = DateTime.Now - StartTime;
                lblTimer.Text = span.ToString(@"dd\.hh\:mm\:ss");
            }
        }

        /// <summary>
        /// read scanned code from input box.
        /// </summary>
        /// <returns></returns>
        private string GetLabelFromInput() {
            var code = txtLableCode1.Text.Trim();
            return code.Length >= 12
                ? code.Substring(0, 12)
                : string.Empty;
        }

        /// <summary>
        /// 复位标签输入栏。
        /// </summary>
        private void ResetLabelInput() {
            // reset the code input box.
            txtLableCode1.Text = string.Empty;
            txtLableCode1.Enabled = true;
            txtLableCode1.Focus();
        }

        private async void txtLableCode1_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == '\r') {
                var code = GetLabelFromInput();
                if (string.IsNullOrEmpty(code)) { return; }

                txtLableCode1.Enabled = false;

                var handScannerClient = CreateOpcClient("相机手动");

                await Task.Run(() => {
                    // waiting for mutex available.
                    lock (LOCK_CAMERA_PROCESS) {
                        ScanLableCode(handScannerClient, code, 0, true);
                    }
                });

                ResetLabelInput();

            } else if (txtLableCode1.Text.Length > 12) {
                txtLableCode1.Text = txtLableCode1.Text.Substring(0, 12);
            }
        }

        private void ScanLableCode(IOpcClient client, string code, int scanNo, bool handwork) {
            ShowWarning(code, false);

#if !DEBUG
            Thread.Sleep(40);
            //PLC已将布卷勾走
            if (client.ReadBool(opcParam.ScanParam.PlcPushAside)) {
                client.Write(opcParam.ScanParam.PlcPushAside, 0);
                logOpt.Write($"采集超时，号码{code}被勾走。");
                return;
            }
#endif

            var tolocation = string.Empty;

            var t = TimeCount.TimeIt(() => {
                tolocation = GetLocationAndLength(code, handwork);
            });
            var str = tolocation.Split('|');

            if (string.IsNullOrEmpty(tolocation) || string.IsNullOrEmpty(str[0])) {
                PlcHelper.PushAside(client, opcParam);
                return;
            }

            var lc = new LableCode(code, str[0], decimal.Parse(str[1]), handwork);
            var clothsize = new ClothRollSize();

            t = TimeCount.TimeIt(() => {
                while (isrun) {
                    var f = client.ReadBool(opcParam.ScanParam.SizeState);
                    if (f) { break; }

                    logOpt.Write("等待SizeState信号。");
                    Thread.Sleep(OPCClient.DELAY);
                }
            });

            t = TimeCount.TimeIt(() => {
                clothsize.getFromOPC(client, opcParam);
                client.Write(opcParam.ScanParam.SizeState, false);
            });

            const int MIN_DIAMETER = 30;
            if (clothsize.diameter < MIN_DIAMETER) {
                // 小于30mm的布卷，则勾走。
                PlcHelper.PushAside(client, opcParam);
                logOpt.Write($"布卷直径{clothsize.diameter}, 小于{MIN_DIAMETER}mm, 勾走。");
                return;
            }

            lc.SetSize(clothsize.diameter, clothsize.length);

            while (isrun) {
                // 等待可写信号为false。
                var f = client.ReadBool(opcParam.ScanParam.ScanState);
                if (!f) { break; }

                Thread.Sleep(OPCClient.DELAY);
            }

            var status = false;
            t = TimeCount.TimeIt(() => {
                // write area and locationno.
                client.Write(opcParam.ScanParam.ToLocationArea, clsSetting.AreaNo[lc.ParseLocationArea()]);
                client.Write(opcParam.ScanParam.ToLocationNo, lc.ParseLocationNo());
                // write label.
                client.Write(opcParam.ScanParam.ScanLable1, lc.CodePart1());
                client.Write(opcParam.ScanParam.ScanLable2, lc.CodePart2());
                // write camera no. and set state true.
                status = client.Write(opcParam.ScanParam.ScanState, true);
            });

#if !DEBUG
            Thread.Sleep(40);
            //PLC已将布卷勾走
            if (client.ReadBool(opcParam.ScanParam.PlcPushAside)) {
                client.Write(opcParam.ScanParam.PlcPushAside, 0);
                logOpt.Write($"采集超时，号码{code}被勾走。");
                return;
            }
#endif

            try {
                if (status) {
                    if (LableCode.Add(lc)) {
                        ViewAddLable(lc);
                        counter += 1;
                        RefreshCounter();

                        lock (taskQ.WeighQ) {
                            taskQ.WeighQ.Enqueue(lc);
                        }
                        showLabelQue(taskQ.WeighQ, lsvWeigh);
                    } else {
                        logOpt.Write($"!扫描号码{lc.LCode}存数据库失败。");
                        ShowWarning($"扫描号码{lc.LCode}存数据库失败。", true);
                    }
                }
            } catch (Exception ex) {
                logOpt.Write($"!扫描号码过程异常: {ex}");
            }
        }

        private bool AreaAAndCFinish(string lCode) {
            var lc = LableCode.QueryByLCode(lCode);
            if (lc == null) { return false; }

            LableCodeBllPro.GetPanelNo(lc, "");
            LableCode.Update(lc);
            LableCode.SetPanelNo(lCode);

            string msg;
            var re = LableCodeBllPro.NotifyPanelEnd(callErpApi, lc.PanelNo, out msg);
            logOpt.Write(string.Format("{0} {1}", lc.ToLocation, msg), LogType.NORMAL);

            return re;
        }

        private static void SetupPanel(LableCode lc, PanelInfo pinfo) {
            lc.PanelNo = pinfo.PanelNo;
            lc.Floor = pinfo.CurrFloor;
            lc.FloorIndex = 0;
            lc.Coordinates = "";
        }

        private void ViewAddLable(LableCode lc) {
            lsvLableCode.Invoke((Action)(() => {
                var msg = string.Format("[{0}] 标签: {1} 交地: {2}", DateTime.Now, lc.LCode, lc.ToLocation);
                lsvLableCode.Items.Insert(0, msg);

                var count = lsvLableCode.Items.Count;
                if (count > 1000) {
                    lsvLableCode.Items.RemoveAt(count - 1);
                }
            }));
        }

        /// <summary>
        /// 根据dtview的行数，来刷新计数器的值。
        /// </summary>
        public void RefreshCounter() {
            this.Invoke((Action)(() => {
                lblCount.Text = counter.ToString();
            }));
        }

        /// <summary>
        ///  [交地]|[长度]
        /// </summary>
        /// <param name="code"></param>
        /// <param name="handwork"></param>
        /// <returns>[交地]|[长度]</returns>
        private string GetLocationAndLength(string code, bool handwork) {
            var re = string.Empty;
            var dt = LableCode.QueryByLCode(code);
            if (dt != null) {
                var msg = string.Format("{0}重复扫描{1}", (handwork ? "手工" : "自动"), code);
                logOpt.Write("!" + msg, LogType.NORMAL);
                ShowWarning("重复扫码");
            } else {
                Dictionary<string, string> str;
                try {
                    str = callErpApi.Post(clsSetting.GetLocation, new Dictionary<string, string>()
                    { { "Bar_Code", code } });
                    var res = JsonConvert.DeserializeObject<DataTable>(str["Data"].ToString());
                    if (str["ERPState"] == "OK") {
                        if (res.Rows.Count > 0 && res.Rows[0]["LOCATION"].ToString() != "Fail") {
                            re = res.Rows[0]["LOCATION"].ToString();
                            logOpt.Write(string.Format("{0}扫描{1}交地{2}。{3}",
                                (handwork ? "手工" : "自动"), code, re, str["Data"]), LogType.NORMAL);
                        } else {
                            ShowWarning("取交地失败");
                            ERPAlarm(opcClient, opcParam, ERPAlarmNo.TO_LOCATION_ERROR);
                            logOpt.Write(string.Format("!{0}{1}获取交地失败。{2}",
                                (handwork ? "手工" : "自动"), code, JsonConvert.SerializeObject(str)), LogType.NORMAL);
                        }
                    } else {
                        ShowWarning("取交地失败");
                        ERPAlarm(opcClient, opcParam, ERPAlarmNo.COMMUNICATION_ERROR);
                        logOpt.Write(string.Format("!{0}{1}获取交地失败。{2}",
                            (handwork ? "手工" : "自动"), code, JsonConvert.SerializeObject(str)), LogType.NORMAL);
                    }
                } catch (Exception ex) {
                    logOpt.Write("!" + ex.Message, LogType.NORMAL);
                }
            }
            return re;
        }

        private void ShowWarning(string msg, bool isError = true) {
            this.Invoke((Action)(() => {
                lbTaskState.Text = msg;
                lbTaskState.BackColor = isError
                    ? Color.Red
                    : Color.Green;
                lbTaskState.ForeColor = Color.White;
            }));
        }

        private void txtLableCode1_Enter(object sender, EventArgs e) {
            txtLableCode1.Text = string.Empty;
        }

        private void txtLableCode1_Leave(object sender, EventArgs e) {
            txtLableCode1.Text = "请将光标放置到这里扫描";
        }

        private const string TASKQUE_CONF = "taskq.json";

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e) {
            if (btnStop.Enabled) {
                MessageBox.Show(string.Format("正在运行无法关闭软件！"));
                e.Cancel = true;
            } else {
                try {
                    var path = Path.Combine(Application.StartupPath, TASKQUE_CONF);
                    TaskQueConf<TaskQueues>.save(path, taskQ);

                    opcClient.Close();
                } catch (Exception ex) {
                    logOpt.Write("!关闭OPC异常。\n" + ex, LogType.NORMAL);
                }
            }
        }

        private static void openDirectory(string basedir, string dir) {
            var path = Path.Combine(basedir, dir);
            Process.Start(path);
        }

        private void btnLog_Click(object sender, EventArgs e) {
            openDirectory(Application.StartupPath, "log");
        }

        private void btnReset_Click(object sender, EventArgs e) {

        }

        private void timer_message_Tick(object sender, EventArgs e) {
            var msgs = logOpt.msgCenter.GetAll();

            if (msgs == null) { return; }

            foreach (var msg in msgs) {
                var box = lsvLog;

                if (msg.Group == LogType.BUFFER) {
                    box = lsvBufferLog;
                } else if (msg.Group == LogType.ROBOT_STACK) {
                    box = lsvRobotStackLog;
                } else if (msg.Group == LogType.ROLL_QUEUE) {
                    box = lsvAlarmLog;
                }

                box.Items.Insert(0, msg);

                // 显示的总条数超过1000条。
                var len = box.Items.Count;
                if (len > 1000) {
                    box.Items.RemoveAt(len - 1);
                }
            }
        }

        private void btnHelp_Click(object sender, EventArgs e) {
            var path = Path.Combine(Application.StartupPath, @"help\index.html");
            Process.Start(path);
        }

        private void toolStripButton1_Click(object sender, EventArgs e) {
            using (var w = new WinDeleteLabel {
                mainwin = this
            }) {
                w.ShowDialog();
            }
        }

        /// <summary>
        /// ERP故障
        /// </summary>
        /// <param name="erpAlarm"></param>
        /// <param name="opcClient">opc client</param>
        /// <param name="opcParam">opc param</param>
        [Obsolete("应使用lchelper.ERPAlarm函数")]
        public static void ERPAlarm(IOpcClient opcClient, OPCParam opcParam, ERPAlarmNo erpAlarm) {
            try {
                opcClient.Write(opcParam.None.ERPAlarm, (int)erpAlarm);
            } catch (Exception ex) {
                logOpt.Write("!OPC写信号失败: " + ex.ToString());
            }
        }

        private void RefreshRobotMenuState() {
            btnStartRobot.Enabled = !robotRun;
            btnStopRobot.Enabled = robotRun;
        }

        private void btnStartRobot_Click(object sender, EventArgs e) {
            chkUseRobot.Checked = true;

            StartAllRobotTasks();
            RefreshRobotMenuState();
        }

        private void btnStopRobot_Click(object sender, EventArgs e) {
            if (CommonHelper.Confirm("确定停止机器人任务吗?")) {
                StopAllRobotTasks();
                RefreshRobotMenuState();
            }
        }

        private void btnWeighReset_Click(object sender, EventArgs e) {
            // 称重复位。
            opcClient.Write(opcParam.ScanParam.GetWeigh, 0);
        }

        private void btnBrowsePanels_Click(object sender, EventArgs e) {
            using (var w = new WRollBrowser()) {
                w.ShowDialog();
            }
        }

        private void btnTestPlc_Click(object sender, EventArgs e) {
            using (var w = new wtestplc()) {
                w.ShowDialog();
            }
        }

        private void initErpApi() {
#if DEBUG
            callErpApi = new FakeWebApi();
#else
            callErpApi = new CallWebApi();
#endif
        }

        private static IOpcClient GetOpcClient() {
#if DEBUG
            return new FakeOpcClient(opcParam);
#else
            return new OPCClient();
#endif
        }

        private void btnSignalWeigh_Click(object sender, EventArgs e) {
#if DEBUG
            SignalGen.startTimerWeigh();
#endif
        }

        private void btnSignalCache_Click(object sender, EventArgs e) {
#if DEBUG
            SignalGen.startTimerCache();
#endif
        }

        private void btnSignalLabelUp_Click(object sender, EventArgs e) {
#if DEBUG
            SignalGen.startTimerLabelUp();
#endif
        }

        private void btnSignalItemCatchA_Click(object sender, EventArgs e) {
#if DEBUG
            SignalGen.startTimerItemCatchA();
#endif
        }

        private void btnSignalItemCatchB_Click(object sender, EventArgs e) {
#if DEBUG
            SignalGen.startTimerItemCatchB();
#endif
        }

        private void btnStartAllSignals_Click(object sender, EventArgs e) {
#if DEBUG
            SignalGen.startTimerWeigh();
            Thread.Sleep(100);
            SignalGen.startTimerCache();
            Thread.Sleep(100);
            SignalGen.startTimerLabelUp();
            Thread.Sleep(100);
            SignalGen.startTimerItemCatchA();
            Thread.Sleep(100);
            SignalGen.startTimerItemCatchB();
#endif
        }

        private void clearAllTaskViews() {
            var views = new List<ListView> {
                lsvCacheBefor,
                lsvCatch1,
                lsvCatch2,
                lsvLableUp,
                lsvRobotA,
                lsvRobotB,
                lsvWeigh,
                lsvCacheQ1,
                lsvCacheQ2,
                lsvCacheQ3,
                lsvCacheQ4
            };

            foreach (var view in views) {
                view.Items.Clear();
            }
        }

        private static void showListInView(IList<string> lst, ListView view) {
            var l = new List<ListViewItem>();
            foreach (var item in lst) {
                l.Add(new ListViewItem(item));
            }

            view.Invoke((EventHandler)(delegate {
                view.Items.Clear();
                view.Items.AddRange(l.ToArray());
            }));
        }

        private void showCachePosQue(CachePos[] que) {
            if (que == null) { return; }
            List<string> lst;
            lock (que) {
                lst = que.Select(x => x.brief()).ToList();
            }
            var lst1 = lst.Take(5).ToList();
            var lst2 = lst.Skip(5).Take(5).ToList();
            var lst3 = lst.Skip(10).Take(5).ToList();
            var lst4 = lst.Skip(15).Take(5).ToList();

            showListInView(lst1, lsvCacheQ1);
            showListInView(lst2, lsvCacheQ2);
            showListInView(lst3, lsvCacheQ3);
            showListInView(lst4, lsvCacheQ4);
        }

        private void showLabelQue(Queue<LableCode> que, ListView view) {
            List<string> lst;
            lock (que) {
                lst = que.Select(x => x.brief()).Reverse().ToList();
            }
            showListInView(lst, view);
        }

        public static void showRobotQue(Queue<RollPosition> que, ListView view) {
            List<string> lst;
            lock (que) {
                lst = que.Select(x => x.brief()).Reverse().ToList();
            }
            showListInView(lst, view);
        }

        private void ClearAllRunningData() {
            if (taskQ != null) {
                taskQ.clearAll();
                clearAllTaskViews();
                ShowTaskQ();
            }
        }

        private void btnNewRun_Click(object sender, EventArgs e) {
            logOpt.Write("启动新任务。");
            LableCode.SetAllPanelsFinished();
            ClearAllRunningData();
            logOpt.Write("所有板设置为完成状态；清除所有队列。");//AC区无法设置完成。
            btnRun_Click(sender, e);
        }

        private void btnLoadTaskq_Click(object sender, EventArgs e) {
            using (var dlg = new OpenFileDialog()) {
                dlg.ShowDialog();

                clearAllTaskViews();
                taskQ = loadconf() ?? new TaskQueues();
                cacheher = new CacheHelper(taskQ.CacheSide);
                ShowTaskQ();
            }
        }

        private void btnSaveTaskq_Click(object sender, EventArgs e) {
            using (var dlg = new SaveFileDialog()) {
                dlg.ShowDialog();

                var path = string.IsNullOrEmpty(dlg.FileName)
                    ? Path.Combine(Application.StartupPath, TASKQUE_CONF)
                    : dlg.FileName;

                TaskQueConf<TaskQueues>.save(path, taskQ);
            }
        }

        private void btnSelfTest_Click(object sender, EventArgs e) {
            logOpt.Write("--- 自检开始 ---");
            var t = new SelfTest(FrmSet.pcfgScan1, (s) => {
                logOpt.Write(s);
                Application.DoEvents();
            });
            try {
                this.Cursor = Cursors.WaitCursor;
                t.run();
            } finally {
                t.close();
                this.Cursor = Cursors.Default;
                logOpt.Write("--- 自检结束 ---");
            }
        }
    }
}
