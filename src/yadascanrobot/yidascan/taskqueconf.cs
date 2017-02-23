using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Newtonsoft.Json;
using yidascan.DataAccess;

namespace yidascan {
    /// <summary>
    /// 用法：
    /// TaskQueConf<TaskQueues>.save(Application.StartupPath + "\\taskques.json", taskQue);
    /// or:
    /// var ques = TaskQueConf<TaskQueues>.load(Application.StartupPath + "\\taskques.json");
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class TaskQueConf<T> {
        /// <summary>
        /// 把对象保存到json文件中。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="obj"></param>
        public static void save(string path, T obj) {
            var s = JsonConvert.SerializeObject(obj);
            File.WriteAllText(path, s);
        }

        /// <summary>
        /// 从json文件中加载对象。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T load(string path) {
            var s = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(s);
        }
    }
}
