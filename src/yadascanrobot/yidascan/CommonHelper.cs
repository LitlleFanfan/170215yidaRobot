using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace commonhelper {
    public static class CommonHelper {
        /// <summary>
        /// 显示用户确认窗口。
        /// </summary>
        /// <param name="question">窗口提示正文。</param>
        /// <returns>用户按下Yes按钮 ，则返回true, 否则返回false。</returns>
        public static bool Confirm(string question) {
            var r = MessageBox.Show(question, "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            return r == DialogResult.Yes;
        }

        public static void Warn(string s) {
            MessageBox.Show(s, "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// 关闭指定名字的进程。但不会关闭本进程。        
        /// </summary>
        /// <param name="name">进程名，注意，名字不含扩展名，不含路径。</param>
        public static void KillInstance(string name) {
            foreach (var process in Process.GetProcessesByName(name)) {
                if (process.Id != Process.GetCurrentProcess().Id) {
                    process.Kill();
                }
            }
        }

        public static void InvokeEx(this Control c, Action p) {
            c.Invoke((Delegate)p);
        }

        /// <summary>
        /// 拆分形如"key=value"这样的字符串。
        /// 失败会弹出
        /// </summary>ArgumentException.
        /// <param name="s">待拆分的字符串。</param>
        /// <returns>KeyValuePair<key, pair>变量。</key></returns>
        public static KeyValuePair<string, string> splitExp(string s) {
            var sep = new char[] { '=' };
            var lst = s.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            if (lst != null && lst.Length == 2) {
                return new KeyValuePair<string, string>(lst[0], lst[1]);
            } else {
                throw new ArgumentException($"invalid key-value pair string: {s}");
            }
        }

        public static Dictionary<string, string> parseLines(string s) {
            var sep = new string[] {"\r\n" };
            var lines = s.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            var dic = new Dictionary<string, string>();
            foreach(var l in lines) {
                try {
                    var kv = splitExp(l);
                    if (dic.ContainsKey(kv.Key)) {
                        dic[kv.Key] = kv.Value;
                    } else {
                        dic.Add(kv.Key, kv.Value);
                    }                    
                } catch {}
            }
            return dic;
        }

        public static Int64 foldersize(string fullpath) {
            try {
                return new DirectoryInfo(fullpath)
                    .GetFiles("*.*", SearchOption.AllDirectories)
                    .Sum(file => file.Length);
            } catch {
                return 0;
            }
        }
    }
}

