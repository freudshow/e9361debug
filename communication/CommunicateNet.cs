using E9361App.Communication;
using E9361App.Log;
using E9361App.Maintain;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Net.Sockets;
using System.Threading;

namespace E9361App.Communication
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
        private TcpPara m_TcpPara = new TcpPara();
        private Socket m_Socket = null;
        private Thread m_Thread = null;
        private volatile bool m_ThreadRunning = false;
        private List<byte> m_ReceiveBuffer = new List<byte>();
        private byte[] m_TempBuffer = new byte[1024];
        private TcpClient m_TcpClient = null;
        private NetworkStream m_Stream = null;
        private SRMessageSingleton m_MsgHandle = SRMessageSingleton.getInstance();
        private log4net.ILog m_LogError = log4net.LogManager.GetLogger("logerror");
        private log4net.ILog loginfo = log4net.LogManager.GetLogger("loginfo");
        public MaintainResEventHander MaintainResHander = null;

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
            m_TcpPara = (TcpPara)o;
            if (m_TcpPara == null || m_TcpPara.Mode != NetMode.TcpClientMode)
            {
                throw new ArgumentException("Mode is not TcpClientMode");
            }

            if (m_TcpClient != null && m_TcpClient.Connected)
            {
                m_TcpClient.Close();
            }

            try
            {
                m_TcpClient = new TcpClient(m_TcpPara.ServerIP, m_TcpPara.ServerPort);

                if (m_TcpClient.Connected)
                {
                    m_Stream = m_TcpClient.GetStream();
                }

                if (m_Thread == null)
                {
                    m_Thread = new Thread(TcpClientRun);
                }

                m_Thread.Name = $"TcpClient-{m_TcpPara.ServerIP}:{m_TcpPara.ServerPort}";
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
                    int len = m_Stream.Read(m_TempBuffer, 0, m_TempBuffer.Length);
                    if (len > 0)
                    {
                        byte[] readbuf = new byte[len];
                        Array.Copy(m_TempBuffer, 0, readbuf, 0, len);
                        m_ReceiveBuffer.AddRange(readbuf);
                    }
                }

                byte[] b = m_ReceiveBuffer.ToArray();

                if (b != null)
                {
                    byte mainFunc;
                    byte subFucn;
                    int start;
                    int len;
                    byte[] data;

                    bool find = MaintainProtocol.FindOneFrame(b, out start, out len, out mainFunc, out subFucn, out data);

                    if (find)
                    {
                        byte[] frame = new byte[len];
                        Array.Copy(b, start, frame, 0, len);

                        MaintainParseRes res = new MaintainParseRes
                        {
                            MainFunc = mainFunc,
                            SubFucn = subFucn,
                            Start = start,
                            Len = len,
                            Data = data,
                            Frame = frame
                        };

                        string msg = m_Thread.Name + "接收报文:<<<" + MaintainProtocol.ByteArryToString(frame, start, len);
                        m_MsgHandle.AddSRMsg(SRMsgType.报文说明, msg);

                        if (MaintainResHander != null)
                        {
                            MaintainResEventArgs e = new MaintainResEventArgs(res);
                            MaintainResHander(this, e);
                        }

                        m_ReceiveBuffer.RemoveRange(0, start + len);
                    }
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
                m_MsgHandle.AddSRMsg(SRMsgType.报文说明, msg);

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