using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.IO;

namespace Hamastar.Common.Net
{
    /// <summary>
    /// Network 的摘要描述
    /// </summary>
    public class Network
    {
        Process _Process;

        private string _HostPath;
        public string HostPath
        {
            get { return _HostPath; }
            set { _HostPath = value; }
        }

        private string _Account;
        public string Account
        {
            get { return _Account; }
            set { _Account = value; }
        }

        private string _Password;
        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }

        private bool _IsConnection = false;
        public bool IsConnection
        {
            get { return _IsConnection; }
            set { _IsConnection = value; }
        }


        public Network()
        {
            //
            // TODO: 在此加入建構函式的程式碼
            //
        }

        /// <summary>
        /// 網路磁碟機連線
        /// </summary>
        /// <returns></returns>
        public void Connection()
        {
            try
            {
                _Process = new Process();
                _Process.StartInfo.FileName = "net.exe";
                _Process.StartInfo.Arguments = @"use " + _HostPath + " " + _Password + " /user:" + _Account;
                _Process.StartInfo.CreateNoWindow = true;
                _Process.StartInfo.UseShellExecute = false;
                _Process.Start();
                _Process.WaitForExit();

                DirectoryInfo directory = new DirectoryInfo(@_HostPath);
                if (directory.Exists)
                {
                    _IsConnection = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 網路磁碟機斷線
        /// </summary>
        /// <param name="process"></param>
        public void Disconnection()
        {
            if (_Process != null)
            {
                try
                {
                    _Process.StartInfo.Arguments = @"use " + _HostPath + " /delete";
                    _Process.Start();
                    _Process.Close();

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    _Process.Dispose();
                }
            }
        }
    }
}