using System;
using System.Windows.Forms;
using System.Drawing;

namespace ListBoxHelper.ext {
    public static class ListBoxHelper {
        // 初始化ListBox
        public static void Initstyle(this ListBox lbox) {
            lbox.DrawMode = DrawMode.OwnerDrawFixed;
            lbox.DrawItem += lbox.onDrawItem;
        }

        // 滚动到末尾
        public static void scrollToLast(this ListBox lbox) {
            lbox.TopIndex = lbox.Items.Count - 1;
            return;
        }

        // 给每一行加上序号
        public static void addIndexedItem(this ListBox lbox, string s) {
            lbox.Items.Add(string.Format("{0:d4} {1}", lbox.Items.Count + 1, s));
        }

        // event handler for drawitem
        // 以!或?开头的字符串，以红字显示.
        public static void onDrawItem(this ListBox lbox, object sender, DrawItemEventArgs e) {
            // drawbackground must run first, 
            // or it will be wierd when item is selected, 
            // and there will be no highlight on the selected item.
            if (e.Index < 0) {
                e.DrawBackground();
                e.DrawFocusRectangle();
                return;
            }

            e.DrawBackground();

            var control = (ListBox)sender;
            var item = control.Items[e.Index];
            Brush brush = null;

            var title = item.ToString();

            if (title.StartsWith("!") || title.StartsWith("?")) {
                brush = new SolidBrush(Color.Red);
                title = title.Substring(1);
            } else {
                brush = new SolidBrush(Color.Black);
            }
            e.Graphics.DrawString(title, control.Font, brush,
                e.Bounds, StringFormat.GenericDefault);
            e.DrawFocusRectangle();
        }
    }
}