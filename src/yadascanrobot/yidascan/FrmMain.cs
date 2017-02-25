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

using commonhelper;
using ListBoxHelper.ext;

namespace yidascan {
    public partial class FrmMain : Form {
        NormalScan nscan1;
        private LableCodeBll lcb = new LableCodeBll();
        public static TaskQueues taskQ;
        bool isrun = false;
        CacheHelper cacheher = new CacheHelper();
        #region opc
        public static OPCParam opcParam = new OPCParam();

#if !DEBUG
        public static IOpcClient opcClient = GetOpcClient(true);
        public static IOpcClient ScannerOpcClient = GetOpcClient(true);
        public static IOpcClient RobotOpcClient = GetOpcClient(true);
#endif
#if DEBUG
        public static IOpcClient opcClient = GetOpcClient(false);
        public static IOpcClient ScannerOpcClient = GetOpcClient(false);
        public static IOpcClient RobotOpcClient = GetOpcClient(false);
#endif
        #endregion

        DataTable dtopc;

        RobotHelper robot;
        private bool robotRun = false;

        public static LogOpreate logOpt;

        private DateTime StartTime;

        public static decimal zStart = 0;

        // 用于锁定手动和自动扫描标签的处理工程。
        public object LOCK_CAMERA_PROCESS = new object();

        private int counter = 0;

        private IErpApi callErpApi;

        public FrmMain() {
            InitializeComponent();
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

        private static TaskQueues loadconf() {
            try {
                var confpath = Path.Combine(Application.StartupPath, TASKQUE_CONF);
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

        private void showQ(ListView view, Queue<LableCode> labels) {
            foreach (var item in labels) {
                QueuesView.Add(view, $"{item.LCode} {item.ToLocation}");
            }
        }

        private void showQ1(ListView view, Queue<RollPosition> rolls) {
            foreach (var item in rolls) {
                QueuesView.Add(view, $"{item.LabelCode} {item.ToLocation}");
            }
        }

        private void showCacheq(IList<LableCode> cache) {
            for (var i = 0; i < cache.Count; i++) {
                if (cache[i] == null) { continue; }
                var side = i + 1;
                var str = $"{cache[i].LCode} {cache[i].ToLocation} {cache[i].Diameter.ToString().PadRight(4, ' ')} {side}";
                if (side <= 5) {
                    lsvCacheQ1.Items[i].Text = str;
                } else if (side >= 6 && side <= 10) {
                    lsvCacheQ2.Items[i - 5].Text = str;
                } else if (side >= 11 && side <= 15) {
                    lsvCacheQ3.Items[i - 10].Text = str;
                } else if (side >= 16 && side <= 20) {
                    lsvCacheQ4.Items[i - 15].Text = str;
                }
            }
        }

        private void ShowTaskQ() {
            showQ(lsvCacheBefor, taskQ.CacheQ);
            showQ(lsvCatch1, taskQ.CatchAQ);
            showQ(lsvCatch2, taskQ.CatchBQ);
            showQ(lsvLableUp, taskQ.LableUpQ);
            showQ1(lsvRobotA, taskQ.RobotRollAQ);
            showQ1(lsvRobotB, taskQ.RobotRollBQ);
            showQ(lsvWeigh, taskQ.WeighQ);
            showCacheq(taskQ.CacheSide);
        }

        private void FrmMain_Load(object sender, EventArgs e) {
            try {
                QueuesView.f = this;

                taskQ = loadconf() ?? new TaskQueues();
                ShowTaskQ();

                StartOpc();
                PlcHelper.subscribe(RobotOpcClient);

                ShowTitle();
                ShowTaskState(false);
                RefreshRobotMenuState();
                cmbShiftNo.SelectedIndex = 0;

                SetRobotTip(false);
                SetButtonState(false);
                InitCfgView();
                LableCode.DeleteAllFinished();

            } catch (Exception ex) {
                logOpt.Write($"!初始化失败。\n{ex}", LogType.NORMAL);
            }
        }

        private void initOpcParam() {
            dtopc = OPCParam.Query();
            dtopc.Columns.Remove("Class");
            dtopc.Columns.Add(new DataColumn("Value"));
        }

        /// <summary>
        /// 在主程序启动时运行。
        /// </summary>
        private void StartOpc() {
            initOpcParam();

            setupOpcClient(opcClient, "扫描");
            setupOpcClient(ScannerOpcClient, "相机");
            setupOpcClient(RobotOpcClient, "机器人");

            opcParam.Init();

            logOpt.Write(JsonConvert.SerializeObject(opcParam), LogType.NORMAL, LogViewType.OnlyFile);
        }

        private void setupOpcClient(IOpcClient c, string name) {
            if (c.Open(clsSetting.OPCServerIP)) {
                logOpt.Write($"{name}OPC client连接成功。", LogType.NORMAL);
                c.AddSubscription(dtopc);
            } else {
                logOpt.Write($"!{name}OPC client务连接失败。", LogType.NORMAL);
            }
        }

        /// <summary>
        /// 启动机器人布卷队列等待。
        /// </summary>
        private void StartRobotJobATask() {
            Task.Factory.StartNew(() => {
                while (isrun) {
                    lock (RobotOpcClient) {
                        // 等待布卷
                        var r = RobotOpcClient.ReadBool(PlcSlot.ITEM_CATCH_A);
                        if (r) {
                            // 加入机器人布卷队列。
                            var code = taskQ.GetCatchAQ();
                            if (code != null) {
                                RobotOpcClient.Write(PlcSlot.ITEM_CATCH_A, false);

                                QueuesView.Move(lsvCatch1, lsvRobotA);
                            }
                        }
                    }
                    Thread.Sleep(5000);
                }
            });
            logOpt.Write("机器人抓料A布卷队列任务启动。", LogType.NORMAL);
        }

        /// <summary>
        /// 启动机器人布卷队列等待。
        /// </summary>
        private void StartRobotJobBTask() {
            Task.Factory.StartNew(() => {
                while (isrun) {
                    lock (RobotOpcClient) {
                        // 等待布卷                        
                        var r = RobotOpcClient.ReadBool(PlcSlot.ITEM_CATCH_B);
                        if (r) {
                            // 加入机器人布卷队列。
                            var code = taskQ.GetCatchBQ();
                            if (code != null) {
                                RobotOpcClient.Write(PlcSlot.ITEM_CATCH_B, false);

                                QueuesView.Move(lsvCatch2, lsvRobotB);
                            }
                        }
                    }
                    Thread.Sleep(5000);
                }
            });
            logOpt.Write("机器人抓料B布卷队列任务启动。", LogType.NORMAL);
        }

        /// <summary>
        /// 去OPC的可放料信号。
        /// </summary>
        /// <param name="tolocation"></param>
        /// <returns></returns>
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

        private void StartRobotTask() {
            try {
                logOpt.Write("机器人正在启动...", LogType.NORMAL);
                Task.Factory.StartNew(() => {
                    robot = new RobotHelper(clsSetting.RobotIP, clsSetting.JobName);
                    robot.setup(logOpt.Write, RobotOpcClient, opcParam);

                    if (robot.IsConnected()) {
                        lblRobot.BackColor = Color.LightGreen;

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

        private void BAreaUserFinalLayerTask() {
            foreach (KeyValuePair<string, string> kv in opcParam.BAreaUserFinalLayer) {
                Task.Factory.StartNew(() => {
                    while (isrun) {
                        lock (opcClient) {
                            var signal = opcClient.ReadString(kv.Value);

                            if (signal == "1") {
                                LableCode.SetMaxFloor(kv.Key);
                                logOpt.Write(string.Format("{0} 收到人工完成信号。", kv.Key), LogType.NORMAL, LogViewType.OnlyFile);
                                opcClient.Write(kv.Value, "0");
                            }
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
                nscan1.OnDataArrived = nscan1_OnDataArrived;
                // 启动相机读取线程。
                nscan1._StartJob();
                lblScanner.BackColor = Color.LightGreen;
                logOpt.Write(nscan1.name + "ok.");
            } else {
                lblScanner2.BackColor = System.Drawing.Color.Gray;
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

#if !DEBUG
            initErpApi(true);
#endif
#if DEBUG
            initErpApi(false);
#endif

            PanelGen.Init(dtpDate.Value);

            StartScanner();
            isrun = true;

            WeighTask();
            ACAreaFinishTask();
            BeforCacheTask();
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

            Task.Factory.StartNew(() => {
                while (isrun) {
                    try {
                        lock (opcClient) {
                            var getWeight = opcClient.ReadInt(opcParam.ScanParam.GetWeigh);
                            if (getWeight == TO_WEIGH) {
                                var code = taskQ.GetWeighQ();

                                if (code != null) {
                                    getWeight = NotifyWeigh(code.LCode, false) ? SUCCESS : FAIL;
                                    logOpt.Write($"{code.LCode}称重API状态：{getWeight} 写OPC状态：{opcClient.Write(opcParam.ScanParam.GetWeigh, getWeight)}");

                                    #region 临时称重信号问题排除
                                    Thread.Sleep(20);
                                    logOpt.Write($"当前OPC称重信号状态：{ opcClient.ReadInt(opcParam.ScanParam.GetWeigh)}");
                                    #endregion

                                    QueuesView.Remove(lsvWeigh);
                                    if (code.ToLocation.Substring(0, 1) == "B") {
                                        QueuesView.Add(lsvCacheBefor, string.Format("{0} {1}", code.LCode, code.ToLocation));//加到缓存列表中显示
                                    }
                                }
                            }
                        }
                    } catch (Exception ex) {
                        logOpt.Write("!" + ex.ToString(), LogType.NORMAL);
                    }

                    Thread.Sleep(OPCClient.DELAY * 200);
                }
            });

            logOpt.Write("称重任务启动。", LogType.NORMAL);
        }

        private void ACAreaFinishTask() {
            foreach (var kv in opcParam.ACAreaPanelFinish) {
                Task.Factory.StartNew(() => {
                    while (isrun) {
                        lock (opcClient) {
                            try {
                                var signal = opcClient.ReadBool(kv.Value.Signal);
                                if (signal) {
                                    var fullLable = ReadCompleteLable(opcClient, kv.Value);

                                    logOpt.Write(string.Format("{0} 收到完成信号。标签:{1}", kv.Value.Signal, fullLable), LogType.NORMAL);

                                    logOpt.Write(string.Format("执行状态:{0}",
                                        AreaAAndCFinish(fullLable)), LogType.NORMAL);
                                }
                            } catch (Exception ex) {
                                logOpt.Write("!" + ex.Message);
                            }

                            opcClient.Write(kv.Value.Signal, 0);

                        }
                        Thread.Sleep(OPCClient.DELAY * 2000);
                    }
                });
            }

            logOpt.Write("AC区完成信号任务启动。", LogType.NORMAL);
        }

        /// <summary>
        /// 读取完整标签号码。
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        private static string ReadCompleteLable(IOpcClient client, LCodeSignal slot) {
            const int MAX_LEN = 6;
            var lable1 = client.ReadString(slot.LCode1);
            var lable2 = client.ReadString(slot.LCode2);
            return lable1.PadLeft(MAX_LEN, '0') + lable2.PadLeft(MAX_LEN, '0');
        }

        /// <summary>
        /// 读PLC的CacheParam.BeforCacheStatus值。
        /// 如果读到的不是bool值，会弹出异常。
        /// </summary>
        /// <param name="client">OPCClient实例</param>
        /// <param name="param">OPCParam实例</param>
        /// <returns></returns>
        private static bool ReadBeforeCacheStatus(IOpcClient client, OPCParam param) {
            return client.ReadBool(param.CacheParam.BeforCacheStatus);
        }

        private void BeforCacheTask() {
            logOpt.Write("缓存任务启动。", LogType.NORMAL);

            Task.Factory.StartNew(() => {
                while (isrun) {
                    lock (RobotOpcClient) {
                        try {
                            if (PlcHelper.ReadCacheSignal(RobotOpcClient)) {
                                var code = taskQ.CacheQ.Peek();

                                if (code != null) {
                                    var lc = LableCode.QueryByLCode(code.LCode);

                                    if (lc == null) {
                                        logOpt.Write(string.Format("!{0}标签找不到", code.LCode), LogType.BUFFER);
                                    } else {
                                        // 检查重复计算。
                                        if (string.IsNullOrEmpty(lc.PanelNo)) {
                                            // 板号以前没算过。                                            

                                            // 计算位置
                                            LableCode outCacheLable = null;
                                            var msg = "";
                                            var cState = lcb.AreaBCalculate(callErpApi,
                                                lc,
                                                string.Format("{0}{1}",
                                                        dtpDate.Value.ToString(clsSetting.LABEL_CODE_DATE_FORMAT),
                                                        cmbShiftNo.SelectedIndex.ToString()),
                                                out outCacheLable, out msg); //计算位置

                                            // new method.
                                            //var rt = lcb.AreaBCalculatePro(callErpApi,
                                            //    lc,
                                            //    string.Format("{0}{1}",
                                            //            dtpDate.Value.ToString(clsSetting.LABEL_CODE_DATE_FORMAT),
                                            //            cmbShiftNo.SelectedIndex.ToString()),
                                            //    taskQ.CacheQ); //计算位置
                                            //msg = rt.message;
                                            // var cState = rt.state;
                                            // end of new method.

                                            logOpt.Write(msg, LogType.BUFFER);

                                            var cr = cacheher.WhenRollArrived(cState, lc, outCacheLable);

                                            logOpt.Write($"1 {JsonConvert.SerializeObject(cr)}", LogType.BUFFER);

                                            BindQueue(code, lc, outCacheLable, cr);
                                            taskQ.CacheQ.Dequeue();

                                            if (cr.state == CacheState.CacheAndGet || cr.state== CacheState.GetThenCache) {
                                                if ((cr.savepos < 11 && cr.getpos < 11) || (cr.savepos > 10 && cr.getpos > 10)) {
                                                    cr.state = CacheState.GetThenCache;
                                                } else {
                                                    cr.state = CacheState.CacheAndGet;
                                                }
                                            }
                                            logOpt.Write($"2 {JsonConvert.SerializeObject(cr)}", LogType.BUFFER);

                                            PlcHelper.WriteCacheJob(RobotOpcClient, cr.state, cr.savepos, cr.getpos);
                                        } else {
                                            logOpt.Write(string.Format("!{0}标签重复。", code.LCode), LogType.BUFFER);
                                        }
                                    }

                                }
                            }
                        } catch (Exception ex) {
                            logOpt.Write(string.Format("!{0}", ex), LogType.BUFFER);
                        }
                    }
                    Thread.Sleep(OPCClient.DELAY * 200);
                }
            });
        }

        private void BindQueue(LableCode code, LableCode lc, LableCode outCacheLable, CacheResult cr) {
            switch (cr.state) {
                case CacheState.Go:
                    taskQ.LableUpQ.Enqueue(code);

                    QueuesView.Move(lsvCacheBefor, lsvLableUp);
                    break;
                case CacheState.Cache:
                    QueuesView.Remove(lsvCacheBefor);
                    CachePosViewSave(lc, cr);
                    break;
                case CacheState.GetThenCache:
                case CacheState.CacheAndGet:
                    taskQ.LableUpQ.Enqueue(outCacheLable);

                    QueuesView.Remove(lsvCacheBefor);
                    CachePosViewSave(lc, cr);
                    CachePosViewGet(cr);
                    break;
                case CacheState.GoThenGet:
                    taskQ.LableUpQ.Enqueue(code);
                    taskQ.LableUpQ.Enqueue(outCacheLable);

                    QueuesView.Move(lsvCacheBefor, lsvLableUp);
                    CachePosViewGet(cr);
                    break;
                case CacheState.GetThenGo:
                    taskQ.LableUpQ.Enqueue(outCacheLable);
                    taskQ.LableUpQ.Enqueue(code);

                    CachePosViewGet(cr);
                    QueuesView.Move(lsvCacheBefor, lsvLableUp);
                    break;
                default:
                    break;
            }
        }

        private void CachePosViewSave(LableCode lc, CacheResult cr) {
            this.Invoke((Action)(() => {
                var str = $"{lc.LCode} {lc.ToLocation} {lc.Diameter.ToString().PadRight(4, ' ')} {cr.savepos}";
                if (cr.savepos <= 5) {
                    lsvCacheQ1.Items[cr.savepos - 1].Text = str;
                } else if (cr.savepos >= 6 && cr.savepos <= 10) {
                    lsvCacheQ2.Items[cr.savepos - 1 - 5].Text = str;
                } else if (cr.savepos >= 11 && cr.savepos <= 15) {
                    lsvCacheQ3.Items[cr.savepos - 1 - 10].Text = str;
                } else if (cr.savepos >= 16 && cr.savepos <= 20) {
                    lsvCacheQ4.Items[cr.savepos - 1 - 15].Text = str;
                }
            }));
            taskQ.CacheSide[cr.savepos - 1] = lc;
        }
        private void CachePosViewGet(CacheResult cr) {
            this.Invoke((Action)(() => {
                var str = $"                      {cr.getpos}";
                if (cr.getpos <= 5) {
                    QueuesView.Add(lsvLableUp, lsvCacheQ1.Items[cr.getpos - 1].Text.Substring(0, 16));
                    lsvCacheQ1.Items[cr.getpos - 1].Text = str;
                } else if (cr.getpos >= 6 && cr.getpos <= 10) {
                    QueuesView.Add(lsvLableUp, lsvCacheQ2.Items[cr.getpos - 1 - 5].Text.Substring(0, 16));
                    lsvCacheQ2.Items[cr.getpos - 1 - 5].Text = str;
                } else if (cr.getpos >= 11 && cr.getpos <= 15) {
                    QueuesView.Add(lsvLableUp, lsvCacheQ3.Items[cr.getpos - 1 - 10].Text.Substring(0, 16));
                    lsvCacheQ3.Items[cr.getpos - 1 - 10].Text = str;
                } else if (cr.getpos >= 16 && cr.getpos <= 20) {
                    QueuesView.Add(lsvLableUp, lsvCacheQ4.Items[cr.getpos - 1 - 15].Text.Substring(0, 16));
                    lsvCacheQ4.Items[cr.getpos - 1 - 15].Text = str;
                }
            }));
            taskQ.CacheSide[cr.getpos - 1] = null;
        }

        private string createShiftNo() {
            var d1 = dtpDate.Value.ToString(clsSetting.LABEL_CODE_DATE_FORMAT);
            var d2 = cmbShiftNo.SelectedIndex.ToString();
            return $"{d1}{d2}";
        }

        //private void BeforCacheTask_new() {
        //    logOpt.Write("缓存任务启动。", LogType.NORMAL);

        //    Task.Factory.StartNew(() => {
        //        while (isrun) {
        //            Thread.Sleep(OPCClient.DELAY * 200);

        //            lock (opcClient) {
        //                try {
        //                    if (PlcHelper.ReadCacheSignal(opcClient)) {
        //                        var lc = taskQ.GetCacheQ();

        //                        if (lc == null) {
        //                            logOpt.Write($"!缓存队列没有标签", LogType.BUFFER);
        //                            continue;
        //                        }

        //                        // 检查重复计算。???
        //                        if (string.IsNullOrEmpty(lc.PanelNo)) {
        //                            logOpt.Write($"!{lc.LCode} 标签重复。", LogType.BUFFER);
        //                            continue;
        //                        }

        //                        // 计算位置, lc和cache队列里比较。
        //                        var calResultt = lcb.AreaBCalculatePro(callErpApi,
        //                            lc,
        //                            createShiftNo(), taskQ.CacheQ); //计算位置

        //                        if (calResultt.message != "") {
        //                            logOpt.Write(calResultt.message, LogType.BUFFER);
        //                        }

        //                        // 显示缓存计算信息
        //                        var msg = lcb.ShowCacheOperationInfo(calResultt.CodeToCache, calResultt.CodeFromCache, calResultt.state);
        //                        logOpt.Write(msg, LogType.BUFFER);

        //                        // 确定缓存操作动作
        //                        var cacheJobState = cacheher.WhenRollArrived(calResultt.state, calResultt.CodeToCache, calResultt.CodeFromCache);
        //                        logOpt.Write(JsonConvert.SerializeObject(cacheJobState), LogType.BUFFER);

        //                        // 发出机械手缓存动作指令
        //                        PlcHelper.WriteCacheJob(opcClient, cacheJobState.state, cacheJobState.savepos, cacheJobState.getpos);

        //                        // 更新界面显示
        //                        QueuesView.Move(lsvCacheBefor, lsvLableUp);
        //                    }
        //                } catch (Exception ex) {
        //                    logOpt.Write($"!{ex.ToString()}", LogType.BUFFER);
        //                }
        //            }
        //        }
        //    });
        //}

        /// <summary>
        /// 2期代码。
        /// </summary>
        private void LableUpTask() {
            logOpt.Write("标签朝上任务启动。", LogType.NORMAL);

            Task.Factory.StartNew(() => {
                while (isrun) {
                    lock (RobotOpcClient) {
                        try {
                            var r = RobotOpcClient.ReadBool(PlcSlot.LABEL_UP_SIGNAL);

                            if (r) {
                                var code = taskQ.GetLableUpQ(); if (code != null) {
                                    logOpt.Write(string.Format("收到标签朝上来料信号。号码: {0}", code.LCode), LogType.BUFFER);

                                    // ???未完成

                                    RobotOpcClient.Write(PlcSlot.LABEL_UP_SIGNAL, false);

                                    #region 临时称重信号问题排除
                                    Thread.Sleep(20);
                                    logOpt.Write($"当前OPC标签朝上来料信号状态：{ RobotOpcClient.ReadBool(PlcSlot.LABEL_UP_SIGNAL)}");
                                    #endregion

                                    QueuesView.Move(lsvLableUp, int.Parse(code.ParseLocationNo()) < 6 ? lsvCatch1 : lsvCatch2);
                                }
                            }
                        } catch (Exception ex) {
                            logOpt.Write(string.Format("!{0}", ex), LogType.BUFFER);
                        }
                    }
                    Thread.Sleep(OPCClient.DELAY * 200);
                }
            });
        }

        void nscan1_OnDataArrived(string type, string code) {
            nscan_OnDataArrived(type, code, 1);
        }

        void nscan2_OnDataArrived(string type, string code) {
            nscan_OnDataArrived(type, code, 2);
        }

        void nscan_OnDataArrived(string type, string code, int scanNo) {
            if (code == "ERROR" || code.Length < 12) { return; }

            // 条码请取前面12位,有些扫描器会扫出13位是因为把后面的识别码也读出来了.
            // 摘自2016年9月10日(星期六) 下午2:37邮件:答复: 答复: 9月9号夜班布卷扫描枪PC连接不上ERP说明
            code = code.Substring(0, 12);

            // wait for opc available.
            // must use try/finally block to release this mutex.
            lock (LOCK_CAMERA_PROCESS) {
                ScanLableCode(code, scanNo, false);
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
            switch (FrmSet.pcfgScan2.CommunicationType) {
                case "Network":
                    lblScanner2.Text = string.Format("Scanner2:{0}/{1}",
                        FrmSet.pcfgScan2.IPAddr, FrmSet.pcfgScan2.IPPort);
                    break;
                case "SerialPort":
                    lblScanner2.Text = string.Format("Scanner2:{0}/{1}",
                        FrmSet.pcfgScan2.ComPort, FrmSet.pcfgScan2.BaudRate);
                    break;
                default:
                    lblScanner2.Text = string.Format("Scanner2:Unknown");
                    break;
            }

            lblOpcIp.Text = string.Format("OPC server ip:{0}", clsSetting.OPCServerIP);
            lblRobot.Text = string.Format("Robot:{0}/{1}", clsSetting.RobotIP, clsSetting.JobName);
        }

        private void SetRobotTip(bool run, string msg = "") {
            if (run) {
                lbRobotState.ForeColor = Color.Green;
                lbRobotState.Text = string.IsNullOrEmpty(msg) ? "机器人已启动" : msg;
                lblRobot.BackColor = Color.LightGreen;
            } else {
                lbRobotState.ForeColor = Color.Red;
                lbRobotState.Text = string.IsNullOrEmpty(msg) ? "机器人未启动" : msg;
                lblRobot.BackColor = Color.LightGray;
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

                await Task.Run(() => {
                    // waiting for mutex available.
                    lock (LOCK_CAMERA_PROCESS) {
                        ScanLableCode(code, 0, true);
                    }
                });

                ResetLabelInput();

            } else if (txtLableCode1.Text.Length > 12) {
                txtLableCode1.Text = txtLableCode1.Text.Substring(0, 12);
            }
        }

        private void ScanLableCode(string code, int scanNo, bool handwork) {
            ShowWarning(code, false);

#if !DEBUG
            Thread.Sleep(40);
            //PLC已将布卷勾走
            if (ScannerOpcClient.ReadBool(opcParam.ScanParam.PlcPushAside)) {
                ScannerOpcClient.Write(opcParam.ScanParam.PlcPushAside, 0);
                logOpt.Write($"采集超时，号码{code}被勾走。");
                return;
            }
#endif

            var tolocation = string.Empty;

            var t = TimeCount.TimeIt(() => {
                tolocation = GetLocation(code, handwork);
            });
            logOpt.Write($"取交地耗时:　{t}ms");

            if (string.IsNullOrEmpty(tolocation)) {
                ScannerOpcClient.Write(opcParam.ScanParam.PushAside, 1);
                return;
            }

            var lc = new LableCode(code, tolocation, handwork);
            var clothsize = new ClothRollSize();

            logOpt.Write("等待SizeState信号。");

            t = TimeCount.TimeIt(() => {
                while (isrun) {
                    var f = ScannerOpcClient.ReadBool(opcParam.ScanParam.SizeState);
                    if (f) { break; }

                    Thread.Sleep(OPCClient.DELAY);
                }
            });
            logOpt.Write($"等尺寸信号耗时:{t}ms", LogType.NORMAL);

            t = TimeCount.TimeIt(() => {
                clothsize.getFromOPC(ScannerOpcClient, opcParam);
                ScannerOpcClient.Write(opcParam.ScanParam.SizeState, false);
            });

            lc.SetSize(clothsize.diameter, clothsize.length);
            logOpt.Write($"{lc.Size_s()}, 耗时: {t}ms");

            while (isrun) {
                // 等待可写信号为false。
                var f = ScannerOpcClient.ReadBool(opcParam.ScanParam.ScanState);
                if (!f) { break; }

                Thread.Sleep(OPCClient.DELAY);
            }

            t = TimeCount.TimeIt(() => {
                // write area and locationno.
                ScannerOpcClient.Write(opcParam.ScanParam.ToLocationArea, clsSetting.AreaNo[lc.ParseLocationArea()]);
                ScannerOpcClient.Write(opcParam.ScanParam.ToLocationNo, lc.ParseLocationNo());
                // write label.
                ScannerOpcClient.Write(opcParam.ScanParam.ScanLable1, lc.CodePart1());
                ScannerOpcClient.Write(opcParam.ScanParam.ScanLable2, lc.CodePart2());
                // write camera no. and set state true.
                ScannerOpcClient.Write(opcParam.ScanParam.ScanState, true);
            });
            logOpt.Write($"写OPC耗时: {t}ms", LogType.NORMAL);

#if !DEBUG
            Thread.Sleep(40);
            //PLC已将布卷勾走
            if (ScannerOpcClient.ReadBool(opcParam.ScanParam.PlcPushAside)) {
                ScannerOpcClient.Write(opcParam.ScanParam.PlcPushAside, 0);
                logOpt.Write($"采集超时，号码{code}被勾走。");
                return;
            }
#endif

            lock (taskQ.WeighQ) {
                taskQ.WeighQ.Enqueue(lc);
            }
            QueuesView.Add(lsvWeigh, string.Format("{0} {1}", lc.LCode, lc.ToLocation));

            try {
                if (LableCode.Add(lc)) {
                    ViewAddLable(lc);
                    counter += 1;
                    RefreshCounter();
                } else {
                    logOpt.Write($"!扫描号码{lc.LCode}存数据库失败。");
                    ShowWarning($"扫描号码{lc.LCode}存数据库失败。", true);
                }
            } catch (Exception ex) {
                logOpt.Write($"!扫描号码过程异常: {ex}");
            }
        }

        private bool AreaAAndCFinish(string lCode) {
            var lc = LableCode.QueryByLCode(lCode);
            if (lc == null) { return false; }

            LableCodeBll.GetPanelNo(lc, "");
            LableCode.Update(lc);
            LableCode.SetPanelNo(lCode);

            string msg;
            var re = lcb.NotifyPanelEnd(callErpApi, lc.PanelNo, out msg);
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

        private string GetLocation(string code, bool handwork) {
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

        /// <summary>
        /// 等待为OPC允许删除信号，然后删除号码，最后信号复位。
        /// </summary>
        /// <param name="fullLabelCode">长度12位的号码</param>
        public void WriteDeletedLabelToOpc(string fullLabelCode) {
            //等PLC做了此功能再放出来
            //string signal = OPCRead(opcParam.DeleteLCode.Signal).ToString();

            //while (isrun && bool.Parse(signal)) {
            //    signal = OPCRead(opcParam.DeleteLCode.Signal).ToString();
            //    Thread.Sleep(OPCClient.DELAY);
            //}
            lock (opcClient) {
                opcClient.Write(opcParam.DeleteLCode.LCode1, fullLabelCode.Substring(0, 6));
                opcClient.Write(opcParam.DeleteLCode.LCode2, fullLabelCode.Substring(6, 6));
                opcClient.Write(opcParam.DeleteLCode.Signal, true);
            }
        }

        private void btnReset_Click(object sender, EventArgs e) {
            lock (opcClient) {
                opcClient.Write(opcParam.ScanParam.SizeState, false);
                opcClient.Write(opcParam.ScanParam.ScanLable1, "");
                opcClient.Write(opcParam.ScanParam.ScanLable2, "");
                opcClient.Write(opcParam.ScanParam.ToLocationArea, clsSetting.AreaNo["A07".Substring(0, 1)]);
                opcClient.Write(opcParam.ScanParam.ToLocationNo, "A07".Substring(1, 2));
                logOpt.Write("传送复位。", LogType.NORMAL);
                opcClient.Write(opcParam.ScanParam.ScanState, true);
            }
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
        /// 发送报警信号到OPC。
        /// </summary>
        /// <param name="value">报警信号的值。</param>
        private void AlarmToOPC(object value) {
            lock (opcClient) {
                opcClient.Write(opcParam.None.ERPAlarm, value);
            }
        }

        /// <summary>
        /// ERP故障
        /// </summary>
        /// <param name="erpAlarm"></param>
        /// <param name="opcClient">opc client</param>
        /// <param name="opcParam">opc param</param>
        public static void ERPAlarm(IOpcClient opcClient, OPCParam opcParam, ERPAlarmNo erpAlarm) {
            try {
                opcClient.Write(opcParam.None.ERPAlarm, (int)erpAlarm);
            } catch (Exception ex) {
                logOpt.Write("!OPC写信号失败: " + ex.ToString());
            }
        }

        /// <summary>
        /// 发送机器人报警信号给OPC。
        /// </summary>
        /// <param name="value">报警信号的值。</param>
        private void RobotAlarmToOpc(object value) {
            lock (opcClient) {
                opcClient.Write(opcParam.None.RobotAlarmSlot, value);
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
            StopAllRobotTasks();
            RefreshRobotMenuState();
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

        private bool doit() {
            MessageBox.Show("doit");
            return false;
        }

        private void btnTestPlc_Click(object sender, EventArgs e) {
            using (var w = new wtestplc()) {
                w.ShowDialog();
            }
        }

        private void initErpApi(bool isRealWebApi) {
            if (isRealWebApi) {
                callErpApi = new CallWebApi();
            } else {
                callErpApi = new FakeWebApi();
            }
        }

        private static IOpcClient GetOpcClient(bool isRealOpc) {
            if (isRealOpc) {
                return new OPCClient();
            } else {
                // logOpt.ViewInfo("!模拟opc client.", LogViewType.OnlyForm);
                return new FakeOpcClient(opcParam);
            }
        }

        private void btnSignalWeigh_Click(object sender, EventArgs e) {
            SignalGen.startTimerWeigh();
        }

        private void btnSignalCache_Click(object sender, EventArgs e) {
            SignalGen.startTimerCache();
        }

        private void btnSignalLabelUp_Click(object sender, EventArgs e) {
            SignalGen.startTimerLabelUp();
        }

        private void btnSignalItemCatchA_Click(object sender, EventArgs e) {
            SignalGen.startTimerItemCatchA();
        }

        private void btnSignalItemCatchB_Click(object sender, EventArgs e) {
            SignalGen.startTimerItemCatchB();
        }

        private void btnStartAllSignals_Click(object sender, EventArgs e) {
            SignalGen.startTimerWeigh();
            Thread.Sleep(100);
            SignalGen.startTimerCache();
            Thread.Sleep(100);
            SignalGen.startTimerLabelUp();
            Thread.Sleep(100);
            SignalGen.startTimerItemCatchA();
            Thread.Sleep(100);
            SignalGen.startTimerItemCatchB();
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

        private void btnClearAllRunningData_Click(object sender, EventArgs e) {
            if (taskQ != null) {
                taskQ.clearAll();
                clearAllTaskViews();

                showQ(lsvCacheBefor, taskQ.CacheQ);
                showQ(lsvCatch1, taskQ.CatchAQ);
                showQ(lsvCatch2, taskQ.CatchBQ);
                showQ(lsvLableUp, taskQ.LableUpQ);
                showQ1(lsvRobotA, taskQ.RobotRollAQ);
                showQ1(lsvRobotB, taskQ.RobotRollBQ);
                showQ(lsvWeigh, taskQ.WeighQ);
                showCacheq(taskQ.CacheSide);
            }
        }
    }
}
