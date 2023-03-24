using System;
using System.Collections.Generic;
using System.IO;
using Renci.SshNet;
using Renci.SshNet.Common;
using System.Threading;
using System.Windows;

namespace E9361App.SshInterface
{
    internal class SshClientClass
    {
        private string m_UsrName = "root";
        private string m_Passwd = "123456";
        private string m_HostIp = "192.168.0.232";
        private int m_SshPort = 22;

        private SshClient m_SshClient;
        private SftpClient m_SftpClient;
        private ShellStream m_ShellStream;
        private PasswordConnectionInfo connectionInfo;
        private SshClient client;

        public SshClientClass()
        {
        }

        ~SshClientClass()
        {
            if (m_SshClient != null)
            {
                m_SshClient.Disconnect();
                m_SshClient.Dispose();
            }

            if (m_SftpClient != null)
            {
                m_SftpClient.Disconnect();
                m_SftpClient.Dispose();
            }
        }

        public SshClientClass(string ip, int port, string username, string pwd)
        {
            m_UsrName = username;
            m_Passwd = pwd;
            m_HostIp = ip;
            m_SshPort = port;
        }

        public bool Isconnected()
        {
            try
            {
                if (!m_SshClient.IsConnected)
                {
                    Console.WriteLine("服务器不可达");
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("与服务器连接失败");
                return false;
            }
        }

        public void ConnectToSshServer()
        {
            try
            {
                m_SshClient = new SshClient(m_HostIp, m_SshPort, m_UsrName, m_Passwd);
                m_SshClient.Connect();
            }
            catch (Exception err)
            {
                MessageBox.Show("与服务器连接失败");
            }
        }

        public bool SSHConnected => m_SshClient != null && m_SshClient.IsConnected;

        private void CreateShell()
        {
            Dictionary<TerminalModes, uint> terminalMode = new Dictionary<TerminalModes, uint>();
            terminalMode.Add(TerminalModes.ECHO, 0);
            m_ShellStream = m_SshClient.CreateShellStream("vt-100", 65536, 65536, 65536, 65536, 262144, terminalMode);
        }

        public void ExecShellCmd(string cmd)
        {
            try
            {
                if (m_SshClient == null || !m_SshClient.IsConnected)
                {
                    return;
                }

                if (m_ShellStream == null)
                {
                    CreateShell();
                }

                m_ShellStream.WriteLine(cmd);
                m_ShellStream.Flush();
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public void ExpectResult(string res)
        {
            try
            {
                if (m_SshClient == null || !m_SshClient.IsConnected)
                {
                    return;
                }

                if (m_ShellStream == null)
                {
                    CreateShell();
                }

                m_ShellStream.Expect(res);
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public string ExecCmd(string cmd)
        {
            try
            {
                if (m_SshClient == null || !m_SshClient.IsConnected)
                {
                    return null;
                }

                using (SshCommand x = m_SshClient.RunCommand(cmd))
                {
                    return x.Result;
                }
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public string ReadShellStreamResult()
        {
            try
            {
                if (m_SshClient == null || !m_SshClient.IsConnected)
                {
                    return null;
                }

                if (m_ShellStream == null)
                {
                    CreateShell();
                }

                string tmpstring = m_ShellStream.Read();
                m_ShellStream.Flush();
                return tmpstring;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public string ContinueReadShellStreamResult(int a)
        {
            try
            {
                if (m_SshClient == null || !m_SshClient.IsConnected)
                {
                    return null;
                }

                if (m_ShellStream == null)
                {
                    CreateShell();
                }

                string tmpstring = m_ShellStream.Read();

                Thread.Sleep(a * 1000);//延时

                tmpstring += m_ShellStream.Read();

                return tmpstring;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public void DisConnectSSH()
        {
            if (m_ShellStream != null)
            {
                m_ShellStream.Close();
                m_ShellStream.Dispose();
                m_ShellStream = null;
            }

            if (m_SshClient != null && m_SshClient.IsConnected)
            {
                m_SshClient.Disconnect();
            }
        }

        public void ConnectToSftpServer()
        {
            try
            {
                m_SftpClient = new SftpClient(m_HostIp, m_SshPort, m_UsrName, m_Passwd);
                m_SftpClient.Connect();
            }
            catch (Exception err)
            {
            }
        }

        public void DisConnectSftp()
        {
            try
            {
                if (m_SftpClient != null && m_SftpClient.IsConnected)
                {
                    m_SftpClient.Disconnect();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool SftpConnected => m_SftpClient != null && m_SftpClient.IsConnected;

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="srcfile">要上传到终端的文件名</param>
        /// <param name="destfile">上传到终端后的文件名</param>
        public void UploadFile(string srcfile, string destfile)
        {
            try
            {
                if (m_SftpClient == null || !m_SftpClient.IsConnected)
                {
                    return;
                }

                FileStream fs = File.Open(srcfile, FileMode.Open);

                m_SftpClient.UploadFile(fs, destfile);
                fs.Close();
                fs.Dispose();
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        /// <summary>
        /// 将终端的文件上传 至 电脑端
        /// </summary>
        /// <param name="srcfile">终端中的文件，包含文件名</param>
        /// <param name="destfile"> 电脑端的 文件路径，包含文件名</param>
        public void DownLoadFile(string srcfile, string destfile)
        {
            try
            {
                if (m_SftpClient == null || !m_SftpClient.IsConnected)
                {
                    return;
                }

                FileStream fs = File.Open(destfile, FileMode.OpenOrCreate);

                m_SftpClient.DownloadFile(srcfile, fs);
                fs.Close();
                fs.Dispose();
            }
            catch (Exception err)
            {
                throw err;
            }
        }
    }
}