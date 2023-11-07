using Renci.SshNet;
using Renci.SshNet.Async;
using Renci.SshNet.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace E9361Debug.SshInterface
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

        public void ConnectToSshServer()
        {
            try
            {
                m_SshClient = new SshClient(m_HostIp, m_SshPort, m_UsrName, m_Passwd);
                m_SshClient.Connect();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool IsSshConnected => m_SshClient != null && m_SshClient.IsConnected;

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
            catch (Exception ex)
            {
                throw ex;
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
            catch (Exception ex)
            {
                throw ex;
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetSshMd5(string fileFullName)
        {
            try
            {
                if (m_SshClient == null || !m_SshClient.IsConnected)
                {
                    ConnectToSshServer();
                }

                string cmd = $"md5sum {fileFullName} | awk '{{print  $1}}'";
                using (SshCommand x = m_SshClient.RunCommand(cmd))
                {
                    return x.Result.Trim('\r').Trim('\n');
                }
            }
            catch (Exception ex)
            {
                throw ex;
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
            catch (Exception ex)
            {
                throw ex;
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
            catch (Exception ex)
            {
                throw ex;
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
                m_SshClient.Dispose();
                m_SshClient = null;
            }
        }

        public void ConnectToSftpServer()
        {
            try
            {
                m_SftpClient = new SftpClient(m_HostIp, m_SshPort, m_UsrName, m_Passwd);
                m_SftpClient.Connect();
            }
            catch (Exception ex)
            {
                throw ex;
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool IsSftpConnected => m_SftpClient != null && m_SftpClient.IsConnected;

        /// <summary>
        /// 上传文件至终端
        /// </summary>
        /// <param name="computerfile">要上传到终端的文件名</param>
        /// <param name="terminalfile">上传到终端后的文件名</param>
        public async Task UploadFileToTerminalAsync(string computerfile, string terminalfile)
        {
            if (m_SftpClient == null || !m_SftpClient.IsConnected)
            {
                return;
            }

            try
            {
                using (FileStream fs = File.OpenRead(computerfile))
                {
                    await m_SftpClient.UploadAsync(fs, terminalfile);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 将终端的文件下载至电脑
        /// </summary>
        /// <param name="terminalfile">终端中的文件，包含文件名</param>
        /// <param name="computerfile"> 电脑端的 文件路径，包含文件名</param>
        public async Task DownLoadFileFromTerminalAsync(string terminalfile, string computerfile)
        {
            try
            {
                if (m_SftpClient == null || !m_SftpClient.IsConnected)
                {
                    return;
                }

                using (FileStream fs = File.OpenWrite(computerfile))
                {
                    await m_SftpClient.DownloadAsync(terminalfile, fs);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}