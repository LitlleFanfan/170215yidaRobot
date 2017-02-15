using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace ProduceComm
{
    public class FileIo
    {
        StreamReader sr;
        StreamWriter sw;

        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <param name="text">内容</param>
        /// <returns>是否成功</returns>
        public bool WriterFile(string filePath, string[] text)
        {
            try
            {
                sw = new StreamWriter(filePath, false, Encoding.Default);

                //开始写
                foreach (string s in text)
                {
                    sw.WriteLine(s);
                }

                sw.Close();
                return true;
            }
            catch (Exception ex)
            {
                try
                {
                    sw.Close();
                }
                catch
                {
                }
                return false;
            }
        }

        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <param name="text">内容</param>
        /// <returns>是否成功</returns>
        public bool WriterFileAppend(string filePath, string[] text)
        {
            try
            {
                sw = new StreamWriter(filePath, true, Encoding.UTF8);

                //开始写
                foreach (string s in text)
                {
                    sw.WriteLine(s);
                }

                sw.Close();
                return true;
            }
            catch (Exception ex)
            {
                try
                {
                    sw.Close();
                }
                catch
                {
                }
                return false;
            }
        }

        /// <summary>
        /// 读文件
        /// </summary>
        /// <param name="filename">文件名</param>SHOWW
        /// <returns>集合</returns>
        public List<string> ReaderFiles(string filePath)
        {
            try
            {
                List<string> lst = new List<string>();

                sr = new StreamReader(filePath, Encoding.Default);
                //开始读取

                string ss = sr.ReadLine();
                while (ss != null)
                {
                    lst.Add(ss);

                    ss = sr.ReadLine();
                }

                sr.Close();
                return lst;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>是否成功</returns>
        public bool DeleteFile(string filePath)
        {
            try
            {
                File.Delete(filePath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>是否成功</returns>
        public bool CreateFile(string filePath)
        {
            try
            {
                File.Create(filePath);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public static void CreateFiles(string path)
        {
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }
        }
    }
}
