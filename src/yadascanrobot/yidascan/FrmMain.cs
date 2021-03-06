﻿using System;
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
        bool isrun;
        CacheHelper cacheher;
        #region opc
        public static OPCParam opcParam = new OPCParam();

        public static IOpcClient opcScan;
        public static IOpcClient opcErp;
        public static IOpcClient opcWeigh;
        public static IOpcClient opcBUFL;
        public static IOpcClient opcACF;
        public static IOpcClient opcRobot;

        #endregion
        IRobotJob robot;
        public static bool robotRun;

        public static LogOpreate logOpt;

        private DateTime StartTime;

        // 用于锁定手动和自动扫描标签的处理工程。
        public object LOCK_CAMERA_PROCESS = new object();

        private IErpApi callErpApi;

        private readonly Counter counter = new Counter();

        public FrmMain() {
            InitializeComponent();

            logOpt = new ProduceComm.LogOpreate();

            try {
                locationLbls = new Dictionary<string, Label> { { "B01", lbl1 }, { "B02", lbl2 }, { "B03", lbl3 }, { "B04", lbl4 },
                    { "B05", lbl5 }, {"B06",lbl6 },{"B07",lbl7 }, {"B08",lbl8 }, {"B09",lbl9 }, {"B10",lbl10 },{"B11", lbl11 } };
                // 显示效果不对，以后再说。
                InitListBoxes();

                timer_message.Enabled = true;

                var msgStart = $"{clsSetting.PRODUCT_NAME} V{Application.ProductVersion.ToString()}启动。";
                logOpt.Write(msgStart, LogType.NORMAL);

                lblOpcIp.BackColor = Color.LightGreen;

                TaskQueues.onlog = (string msg, string group) => {
                    this.Invoke((Action)(() => {
                        logOpt.Write(msg, group);
                    }));
                };
            } catch (Exception ex) {
                logOpt.Write($"!启动失败, {ex}", LogType.NORMAL);
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
            lsvWarn.Initstyle();
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
#if !DEBUG
            btnSignalGen.Visible = false;
#endif

            try {
                LayerShape.loadconf();

                TaskQueues.lochelper = loadLocationConf() ?? loadDefaultLocationConf();
                taskQ = loadconf() ?? new TaskQueues();
                cacheher = new CacheHelper(taskQ.CacheSide);

                if (taskQ.PanelNoFrefix == null) {
                    taskQ.PanelNoFrefix = DateTime.Now;
                }
                dtpDate.Value = taskQ.PanelNoFrefix;

                ShowTaskQ();

                ShowTitle();
                ShowTaskState(false);
                RefreshRobotMenuState();

                SetRobotTip(false);
                SetButtonState(false);
                InitCfgView();

                // 检查板号是否用尽
                try {
                    var lastpanelno = PanelDb.GetLatestPanelNo();
                    if (!string.IsNullOrEmpty(lastpanelno) && CommonHelper.IsPanelnoMax(lastpanelno)) {
                        var msg = "板号用尽，请清料，并开启新任务。";
                        CommonHelper.Warn(msg);
                        logOpt.Write($"!{msg}");
                    }
                } catch (Exception ex) {
                    logOpt.Write($"{ex}");
                }


#if DEBUG
                SignalGen.Init();
#endif

            } catch (Exception ex) {
                logOpt.Write($"!来源: {nameof(FrmMain_Load)}, 初始化失败: {ex}", LogType.NORMAL);
            }
        }

        /// <summary>
        /// 运行时，写程序主要参数。
        /// </summary>
        private static void LogParam() {
            logOpt.Write($"板宽：{clsSetting.SplintWidth}间隙：{clsSetting.RollSep}边缘预留：{clsSetting.EdgeSpace}" +
                $"忽略偏差：{clsSetting.CacheIgnoredDiff}奇数层横放：{clsSetting.OddTurn}第一层放布Z：{clsSetting.InitHeigh}",
                LogType.NORMAL);
        }

        public static IOpcClient CreateOpcClient(string name) {
            var client = GetOpcClient();
            if (client.Open(clsSetting.OPCServerIP)) {
                logOpt.Write($"OPC client {name}连接成功。", LogType.NORMAL);
            } else {
                logOpt.Write($"!OPC client {name}连接失败。", LogType.NORMAL);
            }
            return client;
        }
        public static void OpcClientClose(IOpcClient client, string name) {
            try {
                client.Close();
                logOpt.Write($"OPC client {name}连接关闭。", LogType.NORMAL);
            } catch {
                logOpt.Write($"!OPC client {name}连接关闭失败。", LogType.NORMAL);
            }
        }

        /// <summary>
        /// 启动机器人布卷队列等待。
        /// </summary>
        private void StartRobotJobTask() {
            var RobotCarryOpcClient = CreateOpcClient("机器人抓料队列");
            opcParam.RobotCarryParam = new OPCRobotCarryParam(RobotCarryOpcClient);

            Task.Factory.StartNew(() => {
                while (isrun) {
                    // 等待布卷
                    var r = opcParam.RobotCarryParam.PlcSnA.ReadSN(RobotCarryOpcClient);
                    if (r) {
                        // 加入机器人布卷队列。
                        var code = taskQ.GetCatchAQ();
                        if (code != null) {
                            RobotCarryOpcClient.TryWrite(opcParam.RobotCarryParam.RobotCarryA, false);
                            opcParam.RobotCarryParam.PlcSnA.WriteSN(RobotCarryOpcClient);

                            showLabelQue(taskQ.CatchAQ, lsvCatch1);
                            showRobotQue(taskQ.RobotRollAQ, lsvRobotA);
                        }
                    }
                    Thread.Sleep(1000);

                    r = opcParam.RobotCarryParam.PlcSnB.ReadSN(RobotCarryOpcClient);
                    if (r) {
                        // 加入机器人布卷队列。
                        var code = taskQ.GetCatchBQ();
                        if (code != null) {
                            RobotCarryOpcClient.TryWrite(opcParam.RobotCarryParam.RobotCarryB, false);
                            opcParam.RobotCarryParam.PlcSnB.WriteSN(RobotCarryOpcClient);

                            showLabelQue(taskQ.CatchBQ, lsvCatch2);
                            showRobotQue(taskQ.RobotRollBQ, lsvRobotB);
                        }
                    }
                    Thread.Sleep(1000);
                }
                OpcClientClose(opcRobot, "机器人抓料队列");
            });
        }

        public static PanelState GetPanelState(LableCode label, PanelInfo pinfo) {
            var state = PanelState.LessHalf;
            if (label.Floor >= pinfo.MaxFloor - 1) {
                state = PanelState.HalfFull;
            }
            if (pinfo.Status == (int)LableState.PanelFill && label.Floor == pinfo.MaxFloor && label.Status == (int)LableState.FloorLastRoll) {
                state = PanelState.Full;
            }
            return state;
        }

        private IRobotJob GetRobot(string ip, string jobname) {
#if DEBUG
            return new FakeRobotJob(ip, jobname);
#else
            return new RobotHelper(callErpApi, ip, jobname);
#endif
        }

        private void StartRobotTask() {
            try {
                logOpt.Write("!机器人正在启动...", LogType.NORMAL);
                opcRobot = CreateOpcClient("机器人");
                opcParam.RobotParam = new OPCRobotParam(opcRobot);
                opcParam.InitBadShapeLocations(opcRobot);
                opcParam.InitBAreaFloorFinish(opcRobot);
                opcParam.InitBAreaPanelFinish(opcRobot);
                opcParam.InitBAreaPanelState(opcRobot);

                Task.Factory.StartNew(() => {
                    robot = GetRobot(clsSetting.RobotIP, clsSetting.JobName);
                    robot.setup(SetRobotTip, logOpt.Write, opcRobot, opcParam);

                    if (robot.IsConnected()) {
                        // logOpt.Write("开始机器人线程...", LogType.NORMAL);
                        SetRobotTip(true);
                        logOpt.Write("机器人启动正常。", LogType.NORMAL);

                        robot.JobLoop(ref robotRun, lsvRobotA, lsvRobotB);

                    } else {
                        SetRobotTip(false, "机器人网络故障");
                        logOpt.Write("!机器人网络故障。", LogType.NORMAL);
                    }
                });
            } catch (Exception ex) {
                logOpt.Write($"!来源: {nameof(StartRobotTask)}机器人启动异常。{ex}", LogType.NORMAL);
            }
        }

        /// <summary>
        /// 读两次以防止误信号。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="slot"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        private static string safeTryRead(IOpcClient client, string slot, int delay) {
            var s1 = "";
            var s2 = "";
            lock (client) {
                s1 = client.TryReadString(slot);
            }
            Thread.Sleep(delay);
            lock (client) {
                s2 = client.TryReadString(slot);
            }

            return s1 == s2 ? s1 : "";
        }

        /// <summary>
        /// B区人工满板任务
        /// </summary>
        private void StartBAreaUserFinalLayerTask() {
            opcBUFL = CreateOpcClient("B区手动完成");
            opcParam.InitBAreaUserFinalLayer(opcBUFL);

            // for should be within while loop.
            Task.Factory.StartNew(() => {
                const string SIGNAL_ON = "1";
                const string SIGNAL_OFF = "0";
                while (isrun) {
                    foreach (KeyValuePair<string, string> kv in opcParam.BAreaUserFinalLayer) {
                        try {
                            var signal = safeTryRead(opcBUFL, kv.Value, 1000);

                            if (signal == SIGNAL_ON) {
                                // kv.Key是交地。
                                var reallocation = kv.Key;

                                // 取实际交地对应板号
                                var panelNo = "";
                                lock (TaskQueues.LOCK_LOCHELPER) {
                                    panelNo = TaskQueues.lochelper.lookupPanel(reallocation);
                                }

                                if (string.IsNullOrEmpty(panelNo)) {// plc复位信号。
                                    logOpt.Write($"!实际交地{kv.Key} 收到人工完成信号, 放弃, 原因: 交地无板号。", LogType.ROBOT_STACK);
                                    lock (opcBUFL) {
                                        opcBUFL.TryWrite(kv.Value, SIGNAL_OFF);
                                    }
                                    continue;
                                }

                                var pf = LableCode.GetPanel(panelNo);

                                if (pf == null) {
                                    logOpt.Write($"!实际交地{kv.Key} 收到人工完成信号, 放弃, 板号：{panelNo}  数据库中无对应板号。", LogType.ROBOT_STACK);
                                    lock (opcBUFL) {
                                        opcBUFL.TryWrite(kv.Value, SIGNAL_OFF);
                                    }
                                    continue;
                                }
                                logOpt.Write($"!实际交地{kv.Key} 收到人工完成信号。板号：{panelNo} ERP交地：{pf.ToLocation}", LogType.ROBOT_STACK);

                                LableCode.SetMaxFloorAndFull(panelNo);

                                // 创建新的板信息。
                                var newPanel = PanelGen.NewPanelNo();

                                // 重新计算缓存区的布卷的坐标。
                                lock (TaskQueues.LOCK_LOCHELPER) {
                                    cacheher.ReCalculateCoordinate(newPanel, pf.ToLocation);
                                }

                                //处理满板信号
                                var panelfull = areAllRollsOnBoard(pf.PanelNo, pf.ToLocation);

                                logOpt.Write($"!布卷是否全部上垛: {panelfull}");

                                robot.NotifyOpcJobFinished(pf.PanelNo, pf.ToLocation, reallocation, panelfull);

                                // plc复位信号。
                                lock (opcBUFL) {
                                    opcBUFL.TryWrite(kv.Value, SIGNAL_OFF);
                                }

                                logUnboardRolls(); // 未上板的布卷号码。
                            } // end of if

                            Thread.Sleep(OPCClient.DELAY * 20);
                        } catch (Exception ex) {
                            logOpt.Write($"!处理满板信号时发生异常, {ex}", LogType.ROBOT_STACK);
                        }
                    } // end of foreach.
                    Thread.Sleep(OPCClient.DELAY * 400);
                } // end of while
                logOpt.Write($"!人工满板任务停止", LogType.ROBOT_STACK);
            });
        }

        void SetButtonState(bool isRun) {
            this.Invoke((EventHandler)(delegate {
                btnSet.Enabled = !isRun;
                btnRun.Enabled = !isRun;
                btnQuit.Enabled = !isRun;
                btnNewRun.Enabled = !isRun;

                dtpDate.Enabled = !isRun;

                grbHandwork.Enabled = isRun;
                btnStop.Enabled = isRun;

                // 删除号码菜单项
                btnDeleteCodeFromQueueAndDb.Enabled = !isRun;

                // 关闭opc连接进程
                btnRestartOpcServer.Enabled = !isRun;
            }));
        }

        private void StartScannerTask() {
            const string CAMERA_1 = "1#相机";
            if (OpenPort(ref nscan1, CAMERA_1, FrmSet.pcfgScan1)) {
                nscan1.logger = (string x) => { logOpt.Write(x, LogType.NORMAL); };
                nscan1.OnDataArrived = nscan_OnDataArrived;
                // 启动相机读取线程。

                opcScan = CreateOpcClient("相机");
                opcParam.ScanParam = new OPCScanParam(opcScan);

                nscan1._StartJob(opcScan);

                lblScanner.BackColor = Color.LightGreen;
                logOpt.Write(nscan1.name + "ok.");
            } else {
                ShowWarning("启动相机失败。");
                logOpt.Write($"!{nscan1.name}启动失败！", LogType.NORMAL);
            }
        }

        private void logCounter(string msg) {
            lock (counter) {
                logOpt.Write($"计数器{msg}: {counter.total(string.Empty)}");
            }
        }

        private void btnRun_Click(object sender, EventArgs e) {
            StartTime = DateTime.Now;

            SetButtonState(true);
            logOpt.Write(string.Format("!系统流程开始运行"), LogType.NORMAL);
            logCounter("初值");

            setupOpcErp();
            initErpApi();
            PanelGen.Init(dtpDate.Value);

            StartScannerTask();
            isrun = true;

            StartWeighTask();
            StartACAreaFinishTask();

            StartBeforCacheTask();
            StartLableUpTask();
            StartRobotJobTask();
            StartBAreaUserFinalLayerTask();

            StartAllRobotTasks();

            StartPanelEndTask(); // 自由板位，监听板准备好。
            StartDeadPanelDetectTask(); // 死板检测。

            StartHeartBeatingTask(); // 心跳包任务。

            LogParam();
            // 焦点设在手工输入框。
            txtLableCode1.Focus();

            ShowTaskState(isrun);
            RefreshRobotMenuState();
        }

        string lastweighLable = string.Empty;
        private void StartWeighTask() {
            const int SUCCESS = 0;
            const int FAIL = 2;

            opcWeigh = CreateOpcClient("称重");
            opcParam.WeighParam = new OPCWeighParam(opcWeigh);

            Task.Factory.StartNew(() => {
                while (isrun) {
                    Thread.Sleep(OPCClient.DELAY * 200);

                    try {
                        var signal = false;
                        lock (opcWeigh) {
                            signal = opcParam.WeighParam.PlcSn.ReadSN(opcWeigh);
                        }
                        if (signal) {
                            var code = taskQ.GetWeighQ();
                            var re = FAIL;
                            var codeFromPlc = "";

                            lock (opcWeigh) {
                                codeFromPlc = PlcHelper.GetLabelCodeWhenWeigh(opcWeigh, opcParam);
                            }
                            if (codeFromPlc == lastweighLable) {
                                // 复位
                                lock (opcWeigh) {
                                    opcWeigh.TryWrite(opcParam.WeighParam.GetWeigh, 0);
                                    opcParam.WeighParam.PlcSn.WriteSN(opcWeigh);
                                }
                                logOpt.Write($"称重复位, 原因: 重复称重。plc标签{codeFromPlc}");
                                continue;
                            }

                            if (code != null) {
                                lastweighLable = codeFromPlc;
                                if (codeFromPlc == code.LCode) {
                                    re = ErpHelper.NotifyWeigh(callErpApi, code.LCode) ? SUCCESS : FAIL;

                                    lock (opcWeigh) {
                                        var wstate = opcWeigh.TryWrite(opcParam.WeighParam.GetWeigh, re);
                                        opcParam.WeighParam.PlcSn.WriteSN(opcWeigh);
                                    }

                                    if (re != SUCCESS) {
                                        logOpt.Write($"!{code.LCode}称重失败, plc:{codeFromPlc}");
                                    } else {
                                        logOpt.Write($"{code.LCode}称重成功");
                                    }
                                }
                                showLabelQue(taskQ.WeighQ, lsvWeigh);

                                if (code.ToLocation.Substring(0, 1) == Area.B) {
                                    showLabelQue(taskQ.CacheQ, lsvCacheBefor);//加到缓存列表中显示
                                }
                            }
                        }
                    } catch (Exception ex) {
                        logOpt.Write($"!来源: {nameof(StartWeighTask)} {ex}", LogType.NORMAL);
                    }
                }

                logOpt.Write("称重任务停止。", LogType.NORMAL);
            });

            logOpt.Write("称重任务启动。", LogType.NORMAL);
        }

        private void StartACAreaFinishTask() {
            opcACF = CreateOpcClient("AC区完成");
            opcParam.InitACAreaFinish(opcACF);
            foreach (var kv in opcParam.ACAreaPanelFinish) {
                Task.Factory.StartNew(() => {
                    while (isrun) {
                        try {
                            var signal = false;
                            lock (opcACF) {
                                signal = opcACF.TryReadBool(kv.Value.Signal);
                                if (signal) {
                                    var fullLable = PlcHelper.ReadCompleteLable(opcACF, kv.Value);
                                    var ok = AreaAAndCFinish(fullLable);
                                    opcACF.TryWrite(kv.Value.Signal, 0);

                                    logOpt.Write($"{kv.Key}收到完成信号。标签:{fullLable} 执行状态:{ok}", LogType.NORMAL);
                                }
                            }
                        } catch (Exception ex) {
                            logOpt.Write($"!来源: {nameof(StartACAreaFinishTask)}, {ex}");
                        }

                        Thread.Sleep(OPCClient.DELAY * 2000);
                    }
                });
            }

            logOpt.Write("AC区完成信号任务启动。", LogType.NORMAL);
        }

        private void BindQueue(LableCode code, LableCode outCacheLable, PlcCacheResult cr) {
            try {
                switch (cr.state) {
                    case CacheState.Go:
                        lock (TaskQueues.LOCK_LOCHELPER) {
                            taskQ.LableUpQ.Enqueue(code);
                        }

                        showLabelQue(taskQ.CacheQ, lsvCacheBefor);
                        showLabelQue(taskQ.LableUpQ, lsvLableUp);
                        break;
                    case CacheState.Cache:
                        showLabelQue(taskQ.CacheQ, lsvCacheBefor);
                        showCachePosQue(taskQ.CacheSide);
                        break;
                    case CacheState.CacheAndGet:
                    case CacheState.GetThenCache:
                        lock (TaskQueues.LOCK_LOCHELPER) {
                            taskQ.LableUpQ.Enqueue(outCacheLable);
                        }

                        showLabelQue(taskQ.LableUpQ, lsvLableUp);
                        showLabelQue(taskQ.CacheQ, lsvCacheBefor);
                        showCachePosQue(taskQ.CacheSide);
                        break;
                    case CacheState.GoThenGet:
                        lock (TaskQueues.LOCK_LOCHELPER) {
                            taskQ.LableUpQ.Enqueue(code);
                            taskQ.LableUpQ.Enqueue(outCacheLable);
                        }

                        showLabelQue(taskQ.CacheQ, lsvCacheBefor);
                        showLabelQue(taskQ.LableUpQ, lsvLableUp);
                        showCachePosQue(taskQ.CacheSide);
                        break;
                    case CacheState.GetThenGo:
                        lock (TaskQueues.LOCK_LOCHELPER) {
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
                logOpt.Write($"!来源: {nameof(BindQueue)}, {ex}", LogType.BUFFER);
                logOpt.Write($"!来源: {nameof(BindQueue)}, code: {code.LCode}, outlable: {outCacheLable}, state: {nameof(cr.state)}, {cr.state}");
            }
        }

        private string createShiftNo() {
            var d1 = dtpDate.Value.ToString(clsSetting.LABEL_CODE_DATE_FORMAT);
            return $"{d1}";
        }

        private void StartBeforCacheTask() {
            var CacheOpcClient = CreateOpcClient("缓存位");
            opcParam.CacheParam = new OPCBeforCacheParam(CacheOpcClient);
            Task.Factory.StartNew(() => {
                while (isrun) {
                    Thread.Sleep(OPCClient.DELAY * 200);
                    try {
                        var r = opcParam.CacheParam.PlcSn.ReadSN(CacheOpcClient);
                        if (r) {
                            lock (TaskQueues.LOCK_LOCHELPER) {
                                if (taskQ.CacheQ.Count == 0) continue;
                            }

                            var peek = string.Empty;
                            lock (TaskQueues.LOCK_LOCHELPER) {
                                try {
                                    var foo = taskQ.CacheQ.Peek();
                                    peek = foo.LCode;
                                } catch (Exception ex) {
                                    logOpt.Write($"来源: {nameof(StartBeforCacheTask)}, {ex}");
                                }
                            }

                            if (string.IsNullOrEmpty(peek)) { continue; }

                            var lc = LableCode.QueryByLCode(peek);

                            if (lc == null) {
                                logOpt.Write($"!来源: {nameof(StartBeforCacheTask)}, 缓存队列没有标签", LogType.BUFFER);
                                continue;
                            }

                            // 检查重复计算。???
                            if (!string.IsNullOrEmpty(lc.PanelNo)) {
                                logOpt.Write($"!来源: {nameof(StartBeforCacheTask)}, {lc.LCode} 标签重复。", LogType.BUFFER);
                                continue;
                            }

                            // 显示缓存位状态
                            logCachedlables(lc.ToLocation);

                            // 计算位置, lc和cache队列里比较。
                            var calResult = LableCodeBllPro.AreaBCalculate(lc,
                                createShiftNo(),
                                taskQ.GetBeforCacheLables(lc),
                                x => { logOpt.Write(x, LogType.BUFFER); }
                                );

                            // 确定缓存操作动作
                            var cacheJobState = cacheher.WhenRollArrived(calResult.state, calResult.CodeCome, calResult.CodeFromCache);
                            // 这个为什么不放在whenRollArrived里面呢?
                            if (cacheJobState.state == CacheState.CacheAndGet || cacheJobState.state == CacheState.GetThenCache) {
                                if (CacheHelper.isInSameCacheChannel(cacheJobState.getpos, cacheJobState.savepos)) {
                                    // 在同一侧
                                    cacheJobState.state = CacheState.GetThenCache;  // action 3.
                                } else {
                                    cacheJobState.state = CacheState.CacheAndGet;   // action 6
                                }
                            }
                            logOpt.Write($"{calResult.CodeCome.ToLocation} {JsonConvert.SerializeObject(cacheJobState)}" +
                                $" 来料标签：{calResult.CodeCome.LCode} {calResult.CodeCome.Diameter} " +
                                $"取出标签：{calResult.CodeFromCache?.LCode} {calResult.CodeFromCache?.Diameter}  " +
                                $"{calResult.message}", LogType.BUFFER);

                            lock (TaskQueues.LOCK_LOCHELPER) {
                                taskQ.CacheQ.Dequeue();
                            }

                            // 更新界面显示
                            BindQueue(lc, calResult.CodeFromCache, cacheJobState);

                            // 发出机械手缓存动作指令
                            PlcHelper.WriteCacheJob(CacheOpcClient, opcParam, cacheJobState.state, cacheJobState.savepos, cacheJobState.getpos, lc.LCode);

                            // 检查缓存位状况
                            logCache(lc.ToLocation);

#if DEBUG
                            if (Math.Abs(lc.Cx + lc.Cy) > 1000) {
                                throw new Exception($"!{lc.LCode}布卷坐标超界");
                            }
#endif
                        }
                    } catch (Exception ex) {
                        logOpt.Write($"!来源: {nameof(StartBeforCacheTask)}, {ex}", LogType.BUFFER);
                    }
                }
                OpcClientClose(CacheOpcClient, "缓存位");
            });
        }

        private static void logCache(string loc) {
            IEnumerable<string> morelocs;
            lock (TaskQueues.LOCK_LOCHELPER) {
                // 超出正常数量的交地清单。
                morelocs = taskQ.ValidateCacheSide();
            }

            if (morelocs != null && morelocs.Count() > 0) {
                var loclist = string.Join(",", morelocs);
                logOpt.Write($"!缓存位数据异常，同一交地有多余物料: {loclist}。", LogType.BUFFER);
                lock (TaskQueues.LOCK_LOCHELPER) {
                    var q = taskQ.CacheSide.Where(x => x.labelcode != null && x.labelcode.ToLocation == loc);
                    foreach (var item in q) {
                        logOpt.Write($"!{item.id}: {item.labelcode.LCode}, {item.labelcode.ToLocation}, {item.labelcode.PanelNo}");
                    }
                }
            }
        }

        /// <summary>
        /// 2期代码。
        /// </summary>
        private void StartLableUpTask() {
            var LabelUpOpcClient = CreateOpcClient("标签朝上");
            opcParam.LableUpParam = new OPCLableUpParam(LabelUpOpcClient);

            Task.Factory.StartNew(() => {
                while (isrun) {
                    try {
                        var r = opcParam.LableUpParam.PlcSn.ReadSN(LabelUpOpcClient);
                        if (r) {
                            var code = taskQ.GetLableUpQ();
                            if (code != null) {
                                logOpt.Write($"标签朝上来料信号。号码: {code.LCode}", LogType.ROLL_QUEUE);

                                if (string.IsNullOrEmpty(code.RealLocation)) {
                                    var msg = $"!来源: {nameof(StartLableUpTask)}, 真实交地是空值: {code.LCode}/{code.ToLocation}";
                                    logOpt.Write(msg, LogType.ROLL_QUEUE);
                                    throw new Exception(msg);
                                }

                                // 写plc直径和分道号。1~5号板走1道， 其他走2道。
                                PlcHelper.WriteLabelUpData(LabelUpOpcClient, opcParam, code.Diameter,
                                    int.Parse(LableCode.ParseRealLocationNo(code.RealLocation)) < 6 ? RollCatchChannel.channel_1 : RollCatchChannel.channel_2);

                                showLabelQue(taskQ.LableUpQ, lsvLableUp);
                                showCachePosQue(taskQ.CacheSide);

                                if (int.Parse(LableCode.ParseRealLocationNo(code.RealLocation)) < 6) {
                                    showLabelQue(taskQ.CatchAQ, lsvCatch1);
                                } else {
                                    showLabelQue(taskQ.CatchBQ, lsvCatch2);
                                }
                            } else {
#if !DEBUG
                                logOpt.Write($"!标签朝上处来料信号无对应数据，忽略", LogType.ROLL_QUEUE);
#endif
                            }
                        }
                    } catch (Exception ex) {
                        logOpt.Write($"!来源: {nameof(StartLableUpTask)}, {ex}", LogType.ROLL_QUEUE);
                    }
                    Thread.Sleep(OPCClient.DELAY * 200);
                }
                OpcClientClose(LabelUpOpcClient, "标签朝上");
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
                clsSetting.loger.Error($"!来源: {nameof(OpenPort)}, {ex}");
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
            if (robotRun) {
                robotRun = false;

                Thread.Sleep(1000);

                try {
                    robot.Dispose();
                } catch (Exception ex) {
                    logOpt.Write($"!来源: {nameof(StopAllRobotTasks)}, {ex}");
                }

                OpcClientClose(opcRobot, "机器人");

                logOpt.Write("!机器人任务停止。", LogType.NORMAL);
                SetRobotTip(false, "机器人停止");
            }
        }

        private void StopAllJobs() {
            isrun = false;

            Thread.Sleep(500);

            StopScanner(nscan1);
            StopAllRobotTasks();
        }

        private void btnStop_Click(object sender, EventArgs e) {
            if (!CommonHelper.Confirm("确定停止吗?")) { return; }

            logCounter("末值");

            StopAllJobs();

            OpcClientClose(opcWeigh, "称重");
            OpcClientClose(opcScan, "相机");
            OpcClientClose(opcBUFL, "B区手动完成");
            OpcClientClose(opcACF, "AC区完成");
            OpcClientClose(opcErp, "其它报警");

            SetButtonState(false);
            ShowTaskState(isrun);
            RefreshRobotMenuState();

            // 保存自由板位状态。
            var p = Path.Combine(Application.StartupPath, LOCATION_CONF);
            TaskQueues.lochelper.SaveConf(p);

            // 保存缓存位状态。
            saveTaskQ();

            resetOpcProc(); // 关闭opc连接进程。

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

        public void SetRobotTip(bool state, string msg = "") {
            if (state) {
                this.Invoke((Action)(() => {
                    lbRobotState.ForeColor = Color.Green;
                    lbRobotState.Text = string.IsNullOrEmpty(msg) ? "机器人准备好" : msg;
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


                await Task.Run(() => {
                    var handScannerClient = CreateOpcClient("相机手动");
                    new OPCScanParam(handScannerClient);

                    // waiting for mutex available.
                    lock (LOCK_CAMERA_PROCESS) {
                        ScanLableCode(handScannerClient, code, 0, true);
                    }
                    OpcClientClose(handScannerClient, "相机手动");
                });

                ResetLabelInput();

            } else if (txtLableCode1.Text.Length > 12) {
                txtLableCode1.Text = txtLableCode1.Text.Substring(0, 12);
            }
        }

        private void ScanLableCode(IOpcClient client, string code, int scanNo, bool handwork) {
            LableCode lc = null;
            try {
                ShowWarning(code, false);
#if !DEBUG
                lock (client) {
                    if (PlcHelper.IsPushedAside(client, opcParam)) {
                        logOpt.Write($"!采集超时, 收到勾料信号, {code}被勾走。");
                        return;
                    }
                }
#endif
                var tolocation = string.Empty;

                var t = TimeCount.TimeIt(() => {
                    tolocation = ErpHelper.GetLocationAndLength(callErpApi, code, ShowWarning);
                });
                logOpt.Write($"取交地耗时: {t}ms");

                var str = tolocation.Split('|');
                if (string.IsNullOrEmpty(tolocation) || string.IsNullOrEmpty(str[0])) {
                    lock (client) {
                        PlcHelper.PushAside(client, opcParam);
                    }
                    logOpt.Write($"!{code}交地无效，发出勾料信号。");
                    return;
                }

                lc = new LableCode(code, str[0], decimal.Parse(str[1]), handwork);
                var clothsize = new ClothRollSize();

                t = TimeCount.TimeIt(() => {
                    while (isrun) {
                        lock (client) {
                            var f = client.TryReadBool(opcParam.ScanParam.SizeState);
                            if (f) { break; }
                        }

                        logOpt.Write("等待SizeState信号。");
                        Thread.Sleep(OPCClient.DELAY);
                    }
                });
                logOpt.Write($"等尺寸信号耗时: {t}ms", LogType.NORMAL);

                t = TimeCount.TimeIt(() => {
                    lock (client) {
                        clothsize.getFromOPC(client, opcParam);
                        client.TryWrite(opcParam.ScanParam.SizeState, false);
                    }
                });

                const int MIN_DIAMETER = 30;
                if (clothsize.diameter < MIN_DIAMETER) {
                    // 小于30mm的布卷，则勾走。
                    lock (client) {
                        PlcHelper.PushAside(client, opcParam);
                    }
                    logOpt.Write($"布卷直径{clothsize.diameter}, 小于{MIN_DIAMETER}mm, 勾走。");
                    return;
                }

                lc.SetSize(clothsize.diameter, clothsize.length);
                logOpt.Write($"{lc.Size_s()}, 耗时: {t}ms");

                while (isrun) {
                    // 等待可写信号为false。
                    lock (client) {
                        var f = client.TryReadBool(opcParam.ScanParam.ScanState);
                        if (!f) { break; }
                    }
                    Thread.Sleep(OPCClient.DELAY);
                }

                var status = false;
                t = TimeCount.TimeIt(() => {
                    // here should use PlcHelper.WriteScanResult
                    lock (client) {
                        // write area and locationno.
                        client.TryWrite(opcParam.ScanParam.ToLocationArea, clsSetting.AreaNo[lc.ParseLocationArea()]);
                        client.TryWrite(opcParam.ScanParam.ToLocationNo, lc.ParseLocationNo());
                        // write label.
                        client.TryWrite(opcParam.ScanParam.ScanLable1, lc.CodePart1());
                        client.TryWrite(opcParam.ScanParam.ScanLable2, lc.CodePart2());
                        // write camera no. and set state true.
                        status = client.TryWrite(opcParam.ScanParam.ScanState, true);
                    }
                });
                logOpt.Write($"写OPC耗时: {t}ms", LogType.NORMAL);

                if (!status) {//来料信号复位失败
                    lock (client) {
                        PlcHelper.PushAside(client, opcParam);
                    }
                    return;
                }

#if !DEBUG
                lock (client) {
                    if (PlcHelper.IsPushedAside(client, opcParam)) {
                        logOpt.Write($"!收到勾料信号, {code}被勾走。");
                        return;
                    }
                }
#endif
            } catch (Exception ex) {
                logOpt.Write($"!扫描号码{code}过程异常,来源: {nameof(ScanLableCode)}, {ex}");
                PlcHelper.PushAside(client, opcParam);
            }

            if (LableCode.Add(lc)) {
                ViewAddLable(lc);

                updateCounter(lc.ToLocation);
                udpatePriority(lc.ToLocation);

                RefreshCounter();

                lock (taskQ.WeighQ) {
                    taskQ.WeighQ.Enqueue(lc);
                }
                showLabelQue(taskQ.WeighQ, lsvWeigh);
            } else {
                logOpt.Write($"!扫描号码{lc.LCode}存数据库失败。");
                ShowWarning($"扫描号码{lc.LCode}存数据库失败。", true);
                PlcHelper.PushAside(client, opcParam);
            }
        }

        private void updateCounter(string location) {
            lock (counter) {
                counter.inc(location);
            }
        }

        private void udpatePriority(string location) {
            if (location.StartsWith("B")) {
                var hot = Counter.LOW;
                lock (counter) {
                    hot = counter.hotareab(location);
                }
                lock (TaskQueues.LOCK_LOCHELPER) {
                    if (hot == Counter.LOW) {
                        TaskQueues.lochelper.setPriority(location, Priority.LOW);
                    } else if (hot == Counter.MEDIUM) {
                        TaskQueues.lochelper.setPriority(location, Priority.MEDIUM);
                    } else if (hot == Counter.HIGH) {
                        TaskQueues.lochelper.setPriority(location, Priority.HIGH);
                    }
                }
            }
        }

        private bool AreaAAndCFinish(string lCode) {
            var lc = LableCode.QueryByLCode(lCode);
            if (lc == null) { return false; }

            try {
                LableCodeBllPro.GetPanelNo(lc, "");
                LableCode.Update(lc);
                LableCode.SetPanelNo(lCode);
                LableCode.DeleteFinishPanel(lCode);
            } catch (Exception ex) {
                logOpt.Write($"!AC区交地{lc.ToLocation}完成信号处理异常, {ex}");
            }

            string msg;
            var re = ErpHelper.NotifyPanelEnd(callErpApi, lc.PanelNo, lc.ToLocation, out msg);
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
                lock (counter) {
                    lblCount.Text = counter.total(string.Empty).ToString();
                }
            }));
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
        private const string LOCATION_CONF = "location.json";
        private const string LOCATION_DEFAULT_CONF = "location_default.json";


        private static void saveTaskQ() {
            var path = Path.Combine(Application.StartupPath, TASKQUE_CONF);
            TaskQueConf<TaskQueues>.save(path, taskQ);
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e) {
            if (btnStop.Enabled) {
                MessageBox.Show(string.Format("正在运行无法关闭软件！"));
                e.Cancel = true;
            } else {
                try {
                    saveTaskQ();
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
            openLocationWin();
            refreshRealLocState();
        }

        #region LISTBOX_MESSAGES

        private MessageItem last = null;
        private void showWarningMessages(ListBox lbx, IEnumerable<MessageItem> warnmessages) {
            foreach (var msg in warnmessages) {
                // 连续的重复消息更新显示，非重复消息增加显示。
                if (last != null && msg.Text == last.Text && msg.Group == last.Group) {
                    lbx.Items[0] = msg;
                } else {
                    lbx.Items.Insert(0, msg);
                }
                last = msg;
            }

            // 显示的总条数超过1000条。
            limitShowCount(lbx);
        }

        private void showNormalMessages(IEnumerable<MessageItem> msgs) {
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
                limitShowCount(box);
            }
        }

        private static void limitShowCount(ListBox lbx) {
            const int MAX = 1000;
            var len = lbx.Items.Count;
            if (len > MAX) {
                lbx.Items.RemoveAt(len - 1);
            }
        }

        #endregion

        private void timer_message_Tick(object sender, EventArgs e) {
            var msgs = logOpt.msgCenter.GetAll();
            if (msgs == null) { return; }

            var normals = msgs.Where(x => !x.IsWarning());
            showNormalMessages(normals);

            var warnmsgs = msgs.Where(x => x.IsWarning());
            showWarningMessages(lsvWarn, warnmsgs);
        }

        #region reallocation State
        static Dictionary<string, Label> locationLbls = new Dictionary<string, Label>();
        public static void SetReallocationState(string locationNo, LocationState pstate, Priority p, string panelNo) {
            if (p == Priority.DISABLE) {
                locationLbls[locationNo].BackColor = Color.Gray;
            } else {
                switch (pstate) {
                    case LocationState.FULL:
                        locationLbls[locationNo].BackColor = Color.Red;
                        break;
                    case LocationState.BUSY:
                        locationLbls[locationNo].BackColor = TaskQueues.lochelper.isMapped(locationNo)
                             ? Color.Orange : Color.LightGray;

                        var pi = LableCode.GetPanel(panelNo);
                        if (pi != null) {
                            locationLbls[locationNo].Text = pi.CurrFloor.ToString();
                        } else {
                            locationLbls[locationNo].Text = "?";
                        }
                        break;
                    default:
                        locationLbls[locationNo].BackColor = Color.LightGreen;
                        locationLbls[locationNo].Text = string.Empty;
                        break;
                }
            }
        }
        #endregion

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

        private void RefreshRobotMenuState() {
            btnStartRobot.Enabled = !robotRun;
            btnStopRobot.Enabled = robotRun;
        }

        private void btnStartRobot_Click(object sender, EventArgs e) {
            logOpt.Write("!手动启动机器人。");
            StartAllRobotTasks();
            RefreshRobotMenuState();
        }

        private void btnStopRobot_Click(object sender, EventArgs e) {
            if (CommonHelper.Confirm("确定停止机器人任务吗?")) {
                logOpt.Write("!手动停止机器人。");
                StopAllRobotTasks();
                RefreshRobotMenuState();
            }
        }

        private void btnWeighReset_Click(object sender, EventArgs e) {
            const int TO_WEIGH = 1;
            // 称重复位。
            var signal = 0;
            lock (opcWeigh) {
                signal = opcWeigh.TryReadInt(opcParam.WeighParam.GetWeigh);
            }
            if (signal == TO_WEIGH) {
                lock (opcWeigh) {
                    opcWeigh.TryWrite(opcParam.WeighParam.GetWeigh, 0);
                }
                logOpt.Write("手动称重复位", LogType.NORMAL, LogViewType.Both);
            } else {
                logOpt.Write("手动称重复位 状态正确无需复位", LogType.NORMAL, LogViewType.Both);
            }
        }

        private void btnBrowsePanels_Click(object sender, EventArgs e) {
            using (var w = new WRollBrowser()) {
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
            var client = new OPCClient {
                ResetServerOnDisconnect = resetOpcProc
            };
            return client;
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

        private static void showLabelQue(Queue<LableCode> que, ListView view) {
            try {
                List<string> lst;
                lock (TaskQueues.LOCK_LOCHELPER) {
                    lst = que.Select(x => x.brief()).Reverse().ToList();
                }
                showListInView(lst, view);
            } catch (Exception ex) {
                logOpt.Write($"!来源: {nameof(showLabelQue)}, {ex}");
            }
        }

        public static void showRobotQue(Queue<RollPosition> que, ListView view) {
            try {
                List<string> lst;
                lock (TaskQueues.LOCK_LOCHELPER) {
                    lst = que.Select(x => x.brief()).Reverse().ToList();
                }
                showListInView(lst, view);
            } catch (Exception ex) {
                logOpt.Write($"!来源: {nameof(showRobotQue)}, {ex}");
            }
        }

        private void ClearAllRunningData() {
            if (taskQ != null) {
                taskQ.PanelNoFrefix = DateTime.Now;
                dtpDate.Value = taskQ.PanelNoFrefix;

                lock (TaskQueues.LOCK_LOCHELPER) {
                    logOpt.Write("!使用默认板位");
                    TaskQueues.lochelper = LocationHelper.LoadRealDefaultPriority();
                    TaskQueues.lochelper.ResetVirtualPriority();
                }

                taskQ.clearAll();
                clearAllTaskViews();
                ShowTaskQ();
            }
        }

        private void btnNewRun_Click(object sender, EventArgs e) {
            if (CommonHelper.Confirm("启动新任务会清除缓存位数据和板位数据，请确认!")) {
                logOpt.Write("启动新任务。");
                LableCode.SetAllPanelsFinished();
                ClearAllRunningData();
                resetLocationIcons();
                logOpt.Write("所有板设置为完成状态；清除所有队列。");//AC区无法设置完成。
                btnRun_Click(sender, e);
            } else {
                logOpt.Write("!放弃启动新任务。");
            }
        }

        private void btnLoadTaskq_Click(object sender, EventArgs e) {
            using (var dlg = new OpenFileDialog()) {
                dlg.InitialDirectory = Application.StartupPath;
                dlg.Filter = "Json Files(*.json)|*.json";
                dlg.ShowDialog();

                clearAllTaskViews();
                taskQ = loadconf(dlg.FileName) ?? new TaskQueues();
                cacheher = new CacheHelper(taskQ.CacheSide);
                ShowTaskQ();
            }
        }

        private void btnSaveTaskq_Click(object sender, EventArgs e) {
            using (var dlg = new SaveFileDialog()) {
                dlg.InitialDirectory = Application.StartupPath;
                dlg.Filter = "Json Files(*.json)|*.json";
                dlg.ShowDialog();

                var path = string.IsNullOrEmpty(dlg.FileName)
                    ? Path.Combine(Application.StartupPath, TASKQUE_CONF)
                    : dlg.FileName;

                TaskQueConf<TaskQueues>.save(path, taskQ);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e) {
            using (var w = new wfind()) {
                w.ShowDialog();
            }
        }

        private void btnCodeFromBarea_Click(object sender, EventArgs e) {
            if (isrun) {
                commonhelper.CommonHelper.Warn("生产线正在运行，不能删除号码。");
                return;
            }

            using (var w = new wdeletebcode()) {
                w.wmain = this;
                w.ShowDialog();
            }
        }

        private void openLocationWin() {
            using (var w = new wloc()) {
                w.setRunState(isrun);
                w.setdata(TaskQueues.lochelper);
                w.ShowDialog();
            }
        }

        // 监听板准备好信号
        private void StartPanelEndTask() {
            Task.Run(() => {
                while (isrun) {
                    try {
                        IEnumerable<string> keys;
                        lock (TaskQueues.LOCK_LOCHELPER) {
                            keys = TaskQueues.lochelper.RealLocations
                            .Where(x => x.state == LocationState.FULL)
                            .Select(x => x.realloc)
                            .ToList();
                        }

                        foreach (var item in keys) {
                            var ok = robot.PanelAvailable(item);
                            if (ok) {
                                logOpt.Write($"交地{item}板就位。");
                                lock (TaskQueues.LOCK_LOCHELPER) {
                                    TaskQueues.lochelper.OnReady(item);
                                }
                            }
                        }
                    } catch (Exception ex) {
                        logOpt.Write($"!来源: {nameof(StartPanelEndTask)}, {ex}");
                    }

                    Thread.Sleep(1000 * 5);
                }
            });
        }

        private void btnOpenLogDir_Click(object sender, EventArgs e) {
            openDirectory(Application.StartupPath, "log");
        }

        #region LOCATION_CONF
        // !! 此函数应当在TaskQueues加载后，立即执行。
        private static LocationHelper loadlocconf(string conffile) {
            try {
                var p = Path.Combine(Application.StartupPath, conffile);
                return LocationHelper.LoadConf(p);
            } catch (FileNotFoundException) {
                logOpt.Write($"!来源: {nameof(loadDefaultLocationConf)}. 没找到板位文件{conffile}，使用默认配置。");
                return null;
            } catch (Exception ex) {
                logOpt.Write($"!来源: {nameof(loadDefaultLocationConf)}. 加载配置异常: {ex}。");
                return null;
            }
        }

        private static LocationHelper loadDefaultLocationConf() {
            return loadlocconf(LOCATION_DEFAULT_CONF);
        }

        // !! 此函数应当在TaskQueues加载后，立即执行。
        private static LocationHelper loadLocationConf() {
            return loadlocconf(LOCATION_CONF);
        }
        #endregion

        private void btnSetPriority_Click(object sender, EventArgs e) {
            using (var w = new wpriority()) {
                w.locs = TaskQueues.lochelper;
                w.ShowDialog();
            }
        }

        private void btnSignalGen_Click(object sender, EventArgs e) {
            using (var w = new wtestpanel()) {
                w.ShowDialog();
            }
        }

        private static void resetLocationIcons() {
            foreach (var item in locationLbls) {
                item.Value.BackColor = Color.LightGreen;
            }
        }

        private void mnuContextCopy_Click(object sender, EventArgs e) {
            ListView view = null;

            var views = new List<ListView> { lsvLableCode, lsvWeigh, lsvCacheBefor, lsvCacheQ1, lsvCacheQ2,
            lsvCacheQ3, lsvCacheQ4, lsvLableUp, lsvCatch1, lsvCatch2, lsvRobotA, lsvRobotB};

            var cacheviews = new List<ListView> { lsvCacheQ1, lsvCacheQ2, lsvCacheQ3, lsvCacheQ4 };

            view = views.FirstOrDefault(x => x.Focused);

            if (view == null) { return; }

            if (view.SelectedItems.Count > 0) {
                var item = view.SelectedItems[0].Text.Trim();

                var slices = item.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (cacheviews.Contains(view) && (slices.Length > 1)) {
                    Clipboard.SetText(slices[1]);
                } else if (slices.Length > 0) {
                    Clipboard.SetText(slices[0]);
                } else {
                    Clipboard.SetText(item);
                }
            }
        }

        private void refreshRealLocState() {
            foreach (var item in TaskQueues.lochelper.RealLocations) {
                SetReallocationState(item.realloc, item.state, item.priority, item.panelno);
            }
        }

        private void tRefreshRealLocState_Tick(object sender, EventArgs e) {
            refreshRealLocState();
        }

        private static void logCachedlables(string loc) {
            logOpt.Write("-----cached lables-----");
            lock (TaskQueues.LOCK_LOCHELPER) {
                var q = taskQ.CacheSide.Where(x => x.labelcode != null && x.labelcode.ToLocation == loc);
                foreach (var item in q) {
                    logOpt.Write($"id: {item.id}, lable: {item.labelcode.brief()}", LogType.BUFFER);
                }
            }
        }

        private static void resetprocess(string procname, string args) {
            try {
                var p = Process.GetProcessesByName(procname).FirstOrDefault();
                if (p != null) {
                    var path = p.MainModule.FileName;
                    p.Kill();
                    p.WaitForExit(2000);
                    logOpt.Write($"关闭opc进程完成: {procname}");

                    var pnew = Process.Start(path, args);
                    Thread.Sleep(2000);
                    logOpt.Write($"启动opc进程完成: {procname}");
                } else {
                    logOpt.Write($"!进程没有找到: {procname}");
                }
            } catch (Exception ex) {
                logOpt.Write($"!关闭opc进程失败: {procname}, {ex}");
            }
        }

        private static void resetOpcProc() {
            resetprocess("OPCDAServer", "-Embedding"); // 名字末尾不包括.exe
        }

        private void btnRestartOpcServer_Click(object sender, EventArgs e) {
            if (!isrun) {
                resetOpcProc();
            }
        }

        private static void setupOpcErp() {
            opcErp = CreateOpcClient("其它报警");
            opcParam.None = new NoneOpcParame(opcErp);
        }

        private static void logQue(IEnumerable<string> q, string title) {
            if (q.Count() > 0) {
                foreach (var item in q) {
                    logOpt.Write($"{title}, {item}", LogType.ROBOT_STACK);
                }
            }
        }

        /// <summary>
        /// /// 按照实际名义判断线上是否还有未上垛的布卷。
        /// </summary>
        /// <param name="panelno">板号</param>
        /// <param name="toloc">名义交地</param>
        /// <returns></returns>
        private static bool areAllRollsOnBoard(string panelno, string toloc) {
            lock (TaskQueues.LOCK_LOCHELPER) {
                var v = taskQ.LableUpQ.Count(x => x.ToLocation == toloc && x.PanelNo == panelno)
                + taskQ.CatchAQ.Count(x => x.ToLocation == toloc && x.PanelNo == panelno)
                + taskQ.CatchBQ.Count(x => x.ToLocation == toloc && x.PanelNo == panelno)
                + taskQ.RobotRollAQ.Count(x => x.ToLocation == toloc && x.PanelNo == panelno)
                + taskQ.RobotRollBQ.Count(x => x.ToLocation == toloc && x.PanelNo == panelno);
                return v == 0;
            }
        }

        /// <summary>
        /// 按照实际交地判断线上是否还有未上垛的布卷。
        /// </summary>
        /// <param name="panelno">板号</param>
        /// <param name="realloc">实际交地</param>
        /// <returns></returns>
        private static bool areAllRollsOnBoardEx(string panelno, string realloc) {
            var v = taskQ.LableUpQ.Count(x => x.RealLocation == realloc && x.PanelNo == panelno)
            + taskQ.CatchAQ.Count(x => x.RealLocation == realloc && x.PanelNo == panelno)
            + taskQ.CatchBQ.Count(x => x.RealLocation == realloc && x.PanelNo == panelno)
            + taskQ.RobotRollAQ.Count(x => x.RealLocation == realloc && x.PanelNo == panelno)
            + taskQ.RobotRollBQ.Count(x => x.RealLocation == realloc && x.PanelNo == panelno);
            return v == 0;
        }

        private static void logUnboardRolls() {
            logOpt.Write("------未上垛布卷号码------", LogType.ROBOT_STACK);
            lock (TaskQueues.LOCK_LOCHELPER) {
                logQue(taskQ.LableUpQ.Select(x => x.detail()), "标签朝上队列");
                logQue(taskQ.CatchAQ.Select(x => x.detail()), "抓料队列A");
                logQue(taskQ.CatchBQ.Select(x => x.detail()), "抓料队列B");
                logQue(taskQ.RobotRollAQ.Select(x => x.detail()), "机器人队列A");
                logQue(taskQ.RobotRollBQ.Select(x => x.detail()), "机器人队列B");
            }
        }

        private void btnLogQueues_Click(object sender, EventArgs e) {
            logUnboardRolls();
        }

        private void StartDeadPanelDetectTask() {
            Task.Run(() => {
                FrmMain.logOpt.Write("死板检测任务启动。");
                var opcDeadPanelCheck = CreateOpcClient("死板检测");
                while (isrun) {
                    try {
                        IEnumerable<RealLoc> q;
                        lock (TaskQueues.LOCK_LOCHELPER) {
                            // 失去交地对应，状态忙，并且没有未上垛布卷的。
                            q = TaskQueues.lochelper.RealLocations
                            .Where(x => !TaskQueues.lochelper.isMapped(x.realloc) && x.state == LocationState.BUSY && areAllRollsOnBoardEx(x.panelno, x.realloc));
                        }

                        foreach (var item in q) {
                            lock (opcDeadPanelCheck) {
                                robot.NotifyOpcJobFinished(item.panelno, "", item.realloc, true);
                            }
                            FrmMain.logOpt.Write($"死板检测任务检测到满板{item.realloc}。");
                        }

                    } catch (Exception ex) {
                        FrmMain.logOpt.Write($"来源: {nameof(StartDeadPanelDetectTask)}, {ex}");
                    }

                    Thread.Sleep(5 * 1000); // 5s.
                }

                OpcClientClose(opcDeadPanelCheck, "死板检测");
                FrmMain.logOpt.Write("死板检测任务停止。");
            });
        }

        private void StartHeartBeatingTask() {
            const int interval = 500; // 0.5s.

            const string sourcepc = "heartbeat-pc";
            const string sourceplc = "heartbeat-plc";

            Task.Run(() => {
                logOpt.Write($"!心跳任务启动");
                var signal = 0; // 1 or 0;

                const int MAX = 10;
                var signals = new Queue<int>();

                var client = CreateOpcClient("心跳");
                opcParam.InitHeartBeat(client);

                var slotpc = opcParam.HeartBeat[sourcepc];
                var slotplc = opcParam.HeartBeat[sourceplc];

                logOpt.Write($"pc心跳源: {slotpc}");
                logOpt.Write($"plc心跳源: {slotplc}");

                while (isrun) {
                    try {
                        signal = signal == 1 ? 0 : 1;
                        PlcHelper.HeartBeat(client, slotpc, signal);
                        var ret = PlcHelper.HeartBeatBack(client, slotplc);

                        signals.Enqueue(ret);
                        if (signals.Count > MAX) {
                            signals.Dequeue();

                            var c = signals.Count(x => x == 1);
                            if (c == 0 || c >= MAX) {
                                logOpt.Write($"!plc心跳似乎停住了");
                            }
                        }
                    } catch (Exception ex) {
                        logOpt.Write($"!心跳包异常, 来源: {nameof(StartHeartBeatingTask)}, {ex}");
                    }

                    Thread.Sleep(interval);
                }

                signals.Clear();
                OpcClientClose(client, "心跳");
                logOpt.Write($"!心跳任务结束");
            });
        }
    }
}
