using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

using commonhelper;

namespace yidascan {
    static class Program {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main() {
            var created = true;
            var mutex = new Mutex(true, "ABCDEFG_MUTEX_ROBOT_CONTROL_YIDA_X", out created);

            // 在创建mutex之后，执行此代码。
            if (!created) {
                CommonHelper.Warn("已经有一个程序在运行。");
                // Application.Exit();
                return;
            }

            try {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FrmMain());
            } finally {
                mutex.ReleaseMutex();
            }
        }
    }
}

