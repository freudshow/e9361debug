using E9361App.Communication;
using E9361App.Log;
using E9361App.Maintain;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace e9361debug.communication
{
    public enum NetMode
    {
        UdpClientMode = 0,
        TcpClientMode = 1,
        TcpServerMode = 2
    }

    public class TcpPara
    {
        private string m_ServerIP;
        private int m_ServerPort;
        private NetMode m_Mode;

        public string ServerIP
        {
            get { return m_ServerIP; }
            set { m_ServerIP = value; }
        }

        public int ServerPort
        {
            get { return m_ServerPort; }
            set { m_ServerPort = value; }
        }

        public NetMode Mode
        {
            get { return m_Mode; }
            set { m_Mode = value; }
        }
    }

    public class TcpClientNetPort : AbstractPort
    {
        private Socket m_Socket = null;
        private Thread m_Thread = null;
        private volatile bool m_ThreadRunning = false;
        private List<byte> m_ReceiveBuffer = new List<byte>();
        private TcpClient m_TcpClient = null;
        private NetworkStream m_Stream = null;
        private SRMessageSingleton m_msgHandle = SRMessageSingleton.getInstance();
        private log4net.ILog m_LogError = log4net.LogManager.GetLogger("logerror");
        private log4net.ILog loginfo = log4net.LogManager.GetLogger("loginfo");

        public bool IsOpen()
        {
            if (m_TcpClient != null && m_TcpClient.Connected)
            {
                return true;
            }

            return false;
        }

        public PortTypeEnum GetPortType()
        {
            return PortTypeEnum.PortType_Net;
        }

        public bool Open(object o)
        {
            TcpPara tcpPara = (TcpPara)o;
            if (tcpPara == null || tcpPara.Mode != NetMode.TcpClientMode)
            {
                throw new ArgumentException("Mode is not TcpClientMode");
            }

            if (m_TcpClient != null && m_TcpClient.Connected)
            {
                m_TcpClient.Close();
            }

            try
            {
                m_TcpClient = new TcpClient(tcpPara.ServerIP, tcpPara.ServerPort);

                if (m_TcpClient.Connected)
                {
                    m_Stream = m_TcpClient.GetStream();
                }

                if (m_Thread == null)
                {
                    m_Thread = new Thread(TcpClientRun);
                }

                m_Thread.Name = $"TcpClient-{tcpPara.ServerIP}:{tcpPara.ServerPort}";
                m_Thread.IsBackground = true;
                m_ThreadRunning = true;
                m_Thread.Start();

                return m_TcpClient.Connected;
            }
            catch (Exception ex)
            {
                m_LogError.Error(ex.Message);
                throw ex;
            }
        }

        public bool Close()
        {
            m_ThreadRunning = false;
            if (m_TcpClient == null)
            {
                return true;
            }

            m_Stream.Close();
            m_Stream = null;

            m_TcpClient.Close();
            m_TcpClient.Dispose();
            m_TcpClient = null;

            return m_TcpClient.Connected;
        }

        private void TcpClientRun()
        {
            while (m_ThreadRunning)
            {
                if (m_Stream == null)
                {
                    continue;
                }

                if (m_Stream.CanRead)
                {
                }
            }
        }

        public byte[] Read(int maxlen)
        {
            try
            {
                byte[] log = m_ReceiveBuffer.ToArray();
                int actualLen = Math.Min(log.Length, maxlen);
                byte[] res = new byte[actualLen];
                Array.Copy(log, 0, res, 0, actualLen);
                return res;
            }
            catch (Exception ex)
            {
                m_LogError.Error(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        ///读取缓冲区全部数据
        /// </summary>
        /// <returns></returns>
        public byte[] ReadAll()
        {
            return m_ReceiveBuffer.ToArray();
        }

        public bool Write(byte[] frame, int index, int len)
        {
            try
            {
                if (m_Stream == null)
                {
                    return false;
                }

                m_Stream.Write(frame, index, len);
                string msg = "发送报文: >>>" + MaintainProtocol.ByteArryToString(frame, index, len);
                m_msgHandle.AddSRMsg(SRMsgType.报文说明, msg);

                return true;
            }
            catch (Exception ex)
            {
                loginfo.Debug(ex.ToString());
                return false;
            }
        }

        public bool FlushAll()
        {
            try
            {
                m_ReceiveBuffer.Clear();
                return m_ReceiveBuffer.Count == 0;
            }
            catch (Exception ex)
            {
                m_LogError.Error(ex.Message);
                throw ex;
            }
        }
    }
}