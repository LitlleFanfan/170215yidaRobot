﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProduceComm.Scanner {
    public class NormalScan {
        public string name { get; set; }

        public delegate void ErrEventHandler(Exception ex);
        public delegate void HostErrEventHandler(Exception ex);

        private bool stoped;

        private ICommunication icom;

        public Action<string> logger;
        public Action<IOpcClient, string, string, int> OnDataArrived;

        public NormalScan(string devicename, ICommunication _icom) {
            this.name = devicename;
            icom = _icom;
        }

        public bool Open() {
            var re = false;
            try {
                re = icom.Open();
            } catch (Exception) {
                re = false;
            }
            return re;
        }

        public void Close() {
            Thread.Sleep(1);
            icom.Close();
        }

        private string tryReadLine() {
            try {
                var data = icom.Read(1024);
                return Encoding.Default.GetString(data);
            } catch (Exception) {
                return string.Empty;
            }
        }

        public void _StartJob(IOpcClient client) {
            this.stoped = false;

            Task.Factory.StartNew(() => {
                while (!this.stoped) {
                    var s = tryReadLine();
                    if (!string.IsNullOrEmpty(s)) {
                        OnDataArrived(client, "", s, 1);
                    }
                    Thread.Sleep(1000);
                }
                logger?.Invoke("!扫描线程结束。");
            });
        }

        public void _StopJob() {
            this.stoped = true;
        }
    }
}
