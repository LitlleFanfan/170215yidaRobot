using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yidascan {
    public class MessageItem {
        private const string FMT_MESSAGE = "{0}[{1}] [{2}] {3}";
        private const string FMT_DATE = "yyyy-MM-dd HH:mm:ss";

        public DateTime Timestamp { get; set; }
        public string Text { get; set; }
        public string Group { get; set; }

        public MessageItem(string message, string group) {
            this.Timestamp = DateTime.Now;
            this.Text = message;
            this.Group = group;
        }

        public override string ToString() {
            var prefix = "";
            if (IsWarning()) {
                prefix = this.Text.Substring(0, 1);                
            }

            return string.Format(FMT_MESSAGE,
                prefix,
                Timestamp.ToString(FMT_DATE),
                Group,
                Text);
        }

        public bool IsWarning() {
            return !string.IsNullOrEmpty(Text) 
                && (Text.StartsWith("!") || Text.StartsWith("?"));
        }
    }

    public class MessageCenter {
        private readonly Queue<MessageItem> que;

        public MessageCenter() {
            que = new Queue<MessageItem>();
        }

        /// <summary>
        /// 消息加入消息中心队列。
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="group">可选消息分组</param>
        public void Push(string message, string group = "normal") {
            if (string.IsNullOrWhiteSpace(message)) { return; }

            lock (que) {
                que.Enqueue(new MessageItem(message, group));
            }
        }

        /// <summary>
        /// 从消息中心取所有消息。
        /// </summary>
        /// <returns>返回List对象，如果没有消息，List的长度为0，</returns>
        public List<MessageItem> GetAll() {
            List<MessageItem> lst;
            lock (que) {
                lst = que.ToList();
                que.Clear();
            }
            return lst;
        }

        /// <summary>
        /// 清空消息队列。
        /// </summary>
        public void Clear() {
            lock (que) {
                que.Clear();
            } // end of lock.
        }
    }
}
