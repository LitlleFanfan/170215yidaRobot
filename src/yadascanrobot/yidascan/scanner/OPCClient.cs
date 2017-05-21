using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using yidascan;

namespace ProduceComm.OPC {
    public class OPCClient : IOpcClient {
        public delegate void ErrEventHandler(Exception ex);

        public event ErrEventHandler OnError;

        Opc.Server[] servers;
        Opc.Da.Server m_server = null;//定义数据存取服务器
        Dictionary<string, Opc.Da.Subscription> groups = new Dictionary<string, Opc.Da.Subscription>();//定义组对象（订阅者）

        public static int DELAY = 5;

        public bool Connected {
            get {
                //return true;
                return m_server != null && m_server.IsConnected;
            }
        }

        public bool Open(string mAddr) {
            servers = new OpcCom.ServerEnumerator().GetAvailableServers(Opc.Specification.COM_DA_20, mAddr, null);
            if (servers != null && servers.Count() > 0) {
                m_server = (Opc.Da.Server)servers[0];
                m_server.Connect();
                return true;
            } else {
                clsSetting.loger.Error("没有有效的OPC服务！");
                OnError(new Exception("没有有效的OPC服务！"));
            }
            return false;
        }

        public void AddSubscription(System.Data.DataTable p) {
            foreach (System.Data.DataRow pi in p.Rows) {
                string value1 = pi["Code"].ToString();
                if (!string.IsNullOrEmpty(value1)) {
                    AddSubscription(value1);
                }
            }
        }
        public bool AddSubscription(string code) {
            try {
                Opc.Da.SubscriptionState groupstate = new Opc.Da.SubscriptionState();//定义组（订阅者）状态，相当于OPC规范中组的参数
                groupstate.Name = code;//组名
                groupstate.ServerHandle = null;//服务器给该组分配的句柄。
                groupstate.ClientHandle = Guid.NewGuid().ToString();//客户端给该组分配的句柄。
                groupstate.Active = true;//激活该组。
                groupstate.UpdateRate = 500;//刷新频率为1秒。
                groupstate.Deadband = 0;// 死区值，设为0时，服务器端该组内任何数据变化都通知组。
                groupstate.Locale = null;//不设置地区值。  

                Opc.Da.Subscription group = (Opc.Da.Subscription)m_server.CreateSubscription(groupstate);//创建组
                Opc.Da.Item item = new Opc.Da.Item();
                item.ClientHandle = Guid.NewGuid().ToString();//客户端给该数据项分配的句柄。
                item.ItemPath = null; //该数据项在服务器中的路径。
                item.ItemName = code; //该数据项在服务器中的名字。
                group.AddItems(new Opc.Da.Item[] { item });

                groups.Add(code, group);
                return true;
            } catch (Exception ex) {
                clsSetting.loger.Error(string.Format("{0}添加订阅失败！{1}", code, ex));
                OnError(new Exception(string.Format("{0}添加订阅失败！", code), ex));
                return false;
            }
        }

        public bool Write(string code, object value) {
            if (string.IsNullOrEmpty(code)) {
                yidascan.FrmMain.logOpt.Write($"!项目为空");
                OnError(new Exception("项目为空！"));
                return false;
            }
            try {
                if (!groups.Keys.Contains(code)) {
                    clsSetting.loger.Error(string.Format("{0}未添加订阅！", code));
                    OnError(new Exception(string.Format("{0}未添加订阅！", code)));
                    return false;
                }
                Opc.Da.ItemValue iv = new Opc.Da.ItemValue((Opc.ItemIdentifier)groups[code].Items[0]);
                iv.Value = value;
                groups[code].Write(new Opc.Da.ItemValue[] { iv });
                return true;
            } catch (Exception ex) {
                yidascan.FrmMain.logOpt.Write(string.Format("!{0}写入失败！{1}", code, ex));
                //OnError(new Exception("写入失败！", ex));
                return false;
            }
        }

        /// <summary>
        /// 解析返回结果，如果任意一项为false，则结果为false。
        /// 详细信息写入日志。
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public static Boolean ParseResult(Opc.IdentifiedResult[] rt, Action<string> log) {
            var r = true;
            if (rt == null) {
                return false;
            } else {
                foreach (var item in rt) {
                    r = r && item.ResultID.Succeeded();
                    var s = JsonConvert.SerializeObject(item);

                    FrmMain.logOpt.Write($"{item.ResultID.Code} {item.ResultID.Name} {item.ResultID.Succeeded()}", "normal", LogViewType.OnlyFile);
                }
                return r;
            }
        }

        /// <summary>
        /// 返回结果用ParseResult函数解析。
        /// </summary>
        /// <param name="code"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Set(string code, object value) {
            if (string.IsNullOrEmpty(code)) {
                yidascan.FrmMain.logOpt.Write($"!项目 {code} 为空！");
                return false;
            }

            try {
                if (!groups.Keys.Contains(code)) {
                    yidascan.FrmMain.logOpt.Write("!{code}未添加订阅！");
                    return false;
                }

                var key = (Opc.ItemIdentifier)groups[code].Items[0];
                var iv = new Opc.Da.ItemValue(key) {
                    Value = value
                };
                var rt = groups[code].Write(new Opc.Da.ItemValue[] { iv });

                return ParseResult(rt, (x) => { FrmMain.logOpt.Write(x); });
            } catch (Exception ex) {
                yidascan.FrmMain.logOpt.Write($"!{code}写入失败: {ex}");
                return false;
            }
        }

        public object Read(string code) {
            if (string.IsNullOrEmpty(code)) {
                yidascan.FrmMain.logOpt.Write($"!项目为空");
                OnError(new Exception("项目为空！"));
                return null;
            }
            try {
                if (!groups.Keys.Contains(code)) {
                    clsSetting.loger.Error(string.Format("{0}未添加订阅！", code));
                    OnError(new Exception(string.Format("{0}未添加订阅！", code)));
                    return null;
                }
                Opc.Da.ItemValueResult[] values = groups[code].Read(groups[code].Items);

                if (values[0].Quality.Equals(Opc.Da.Quality.Good)) {
                    return values[0].Value;
                }
                return null;
            } catch (Exception ex) {
                yidascan.FrmMain.logOpt.Write(string.Format("!{0}读取失败！{1}", code, ex));
                OnError(new Exception("读取失败！", ex));
                return null;
            }
        }

        public int ReadInt(string slot) {
            var val = Read(slot);
            try {
                return val != null ? int.Parse(val.ToString().Trim()) : 0;
            } catch (Exception ex) {
                yidascan.FrmMain.logOpt.Write(string.Format("!{0}读取失败!{1} {2}", slot, val, ex));
                return 0;
            }
        }

        public string ReadString(string slot) {
            var val = Read(slot);
            try {
                return val != null ? val.ToString() : string.Empty;
            } catch (Exception ex) {
                yidascan.FrmMain.logOpt.Write(string.Format("!{0}读取失败!{1} {2}", slot, val, ex));
                return string.Empty;
            }
        }

        public bool ReadBool(string slot) {
            var val = Read(slot);
            try {
                return val != null ? bool.Parse(val.ToString().Trim()) : false;
            } catch (Exception ex) {
                yidascan.FrmMain.logOpt.Write(string.Format("!{0}读取失败!{1} {2}", slot, val, ex));
                return false;
            }
        }

        public decimal ReadDecimal(string slot) {
            var val = Read(slot);
            try {
                return val != null ? decimal.Parse(val.ToString().Trim()) : 0;
            } catch (Exception ex) {
                yidascan.FrmMain.logOpt.Write(string.Format("!{0}读取失败!{1} {2}", slot, val, ex));
                return 0;
            }
        }

        public void Close() {
            try {
                m_server.Disconnect();
            } catch (Exception ex) {
                clsSetting.loger.Error("关闭连接失败！", ex);
                OnError(new Exception("关闭连接失败！", ex));
            }
        }
    }
}
