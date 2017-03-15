using System;
using System.IO;

using Newtonsoft.Json;

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
        /// <param name="path">文件名</param>
        /// <param name="obj">要保存的对象</param>
        public static void save(string path, T obj) {
            var s = JsonConvert.SerializeObject(obj, Formatting.Indented);
            File.WriteAllText(path, s);
        }

        /// <summary>
        /// 从json文件中加载对象。
        /// </summary>
        /// <param name="path">文件名</param>
        /// <returns>加载失败会返回null</returns>
        public static T load(string path) {
            try {
                var s = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<T>(s);
            } catch (Exception) {
                return default(T);
            }
        }
    }
}
