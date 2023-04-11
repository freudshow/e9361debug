using E9361App.Communication;
using E9361App.Log;
using E9361App.Maintain;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;

namespace E9361App.Communication
{
    public enum NetMode
    {
        UdpClientMode = 0,
        TcpClientMode = 1,
        TcpServerMode = 2
    }

    public class NetPara
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
        private Thread m_Thread = null;
        private volatile bool m_ThreadRunning = false;
        private List<byte> m_ReceiveBuffer = new List<byte>();
        private byte[] m_TempBuffer = new byte[1024];
        private TcpClient m_TcpClient = null;
        private NetworkStream m_Stream = null;
        private volatile object m_Lock = new object();
        private SRMessageSingleton m_MsgHandle = SRMessageSingleton.getInstance();
        private log4net.ILog m_LogError = log4net.LogManager.GetLogger("logerror");
        private log4net.ILog m_LogInfo = log4net.LogManager.GetLogger("loginfo");
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
            NetPara para = (NetPara)o;
            if (para == null || para.Mode != NetMode.TcpClientMode)
            {
                throw new ArgumentException("Mode is not TcpClientMode");
            }

            if (m_TcpClient != null && m_TcpClient.Connected)
            {
                m_TcpClient.Close();
            }

            try
            {
                m_TcpClient = new TcpClient(para.ServerIP, para.ServerPort);

                if (m_TcpClient.Connected)
                {
                    m_Stream = m_TcpClient.GetStream();
                }

                if (m_Thread == null)
                {
                    m_Thread = new Thread(TcpClientRun);
                }

                m_Thread.Name = $"TcpClient-{para.ServerIP}:{para.ServerPort}";
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
            lock (m_Lock)
            {
                m_ThreadRunning = false;
                if (m_TcpClient != null)
                {
                    return true;
                }

                m_Stream.Close();
                m_Stream.Dispose();
                m_Stream = null;

                m_TcpClient.Close();
                m_TcpClient.Dispose();
                m_TcpClient = null;
            }

            return true;
        }

        private void TcpClientRun()
        {
            try
            {
                while (m_ThreadRunning)
                {
                    lock (m_Lock)
                    {
                        if (m_Stream == null)
                        {
                            continue;
                        }
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
                        bool find = MaintainProtocol.FindOneFrame(b, out int start, out int len, out byte mainFunc, out byte subFucn, out byte[] data);

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

                            string msg = "接收报文:" + MaintainProtocol.ByteArryToString(frame, start, len);
                            m_MsgHandle.AddSRMsg(SRMsgType.报文说明, msg);

                            if (MaintainResHander != null)
                            {
                                MaintainResEventArgs e = new MaintainResEventArgs(res);
                                MaintainResHander(this, e);
                            }

                            m_ReceiveBuffer.RemoveRange(0, start + len);
                        }
                    }

                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                m_LogError.Error(ex.Message);
                throw ex;
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
                string msg = "发送报文:" + MaintainProtocol.ByteArryToString(frame, index, len);
                m_MsgHandle.AddSRMsg(SRMsgType.报文说明, msg);

                return true;
            }
            catch (Exception ex)
            {
                m_LogError.Error(ex.Message);
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

    public class UdpPort : AbstractPort
    {
        private string remoteIp = "";

        private IPEndPoint m_RemoteIPEndPoint = null;
        private UdpClient m_UdpClient = null;
        private Thread m_Thread;
        private volatile object m_Lock = new object();
        private volatile bool m_ThreadRunning = false;

        private SRMessageSingleton m_MsgHandle = SRMessageSingleton.getInstance();
        private log4net.ILog m_LogError = log4net.LogManager.GetLogger("logerror");
        private log4net.ILog m_LogInfo = log4net.LogManager.GetLogger("loginfo");

        private List<byte> m_ReceiveBuffer = new List<byte>();
        public MaintainResEventHander MaintainResHander = null;

        public bool IsOpen()
        {
            return m_UdpClient != null;
        }

        public PortTypeEnum GetPortType()
        {
            return PortTypeEnum.PortType_Net;
        }

        public bool Open(object o)
        {
            try
            {
                NetPara netPara = (NetPara)o;
                if (netPara == null || netPara.Mode != NetMode.UdpClientMode)
                {
                    throw new ArgumentException("Mode is not UdpClientMode");
                }

                if (m_UdpClient == null)
                {
                    m_UdpClient = new UdpClient();
                }

                m_UdpClient.Connect(IPAddress.Parse(netPara.ServerIP), netPara.ServerPort);

                if (m_Thread == null)
                {
                    m_Thread = new Thread(UdpClientRun);
                }

                m_ThreadRunning = true;
                m_Thread.Name = $"UdpClient-{netPara.ServerIP}:{netPara.ServerPort}";
                m_Thread.IsBackground = true;
                m_Thread.Start();

                return true;
            }
            catch (Exception ex)
            {
                m_LogError.Error(ex.Message);
                throw ex;
            }
        }

        public bool Close()
        {
            lock (m_Lock)
            {
                m_ThreadRunning = false;
                if (m_UdpClient != null)
                {
                    m_UdpClient.Close();
                    m_UdpClient.Dispose();
                    m_UdpClient = null;
                }

                if (m_RemoteIPEndPoint != null)
                {
                    m_RemoteIPEndPoint = null;
                }

                if (m_Thread != null)
                {
                    m_Thread.Abort();
                    m_Thread = null;
                }
            }

            return true;
        }

        private void UdpClientRun()
        {
            while (m_ThreadRunning)
            {
                lock (m_Lock)
                {
                    if (m_UdpClient == null)
                    {
                        continue;
                    }
                }

                if (m_UdpClient.Available > 0)
                {
                    byte[] b = m_UdpClient.Receive(ref m_RemoteIPEndPoint);
                    m_ReceiveBuffer.AddRange(b);
                }

                byte[] buf = m_ReceiveBuffer.ToArray();
                if (buf != null)
                {
                    bool find = MaintainProtocol.FindOneFrame(buf, out int start, out int len, out byte mainFunc, out byte subFucn, out byte[] data);

                    if (find)
                    {
                        byte[] frame = new byte[len];
                        Array.Copy(buf, start, frame, 0, len);

                        MaintainParseRes res = new MaintainParseRes
                        {
                            MainFunc = mainFunc,
                            SubFucn = subFucn,
                            Start = start,
                            Len = len,
                            Data = data,
                            Frame = frame
                        };

                        string msg = "接收报文:" + MaintainProtocol.ByteArryToString(frame, start, len);
                        m_MsgHandle.AddSRMsg(SRMsgType.报文说明, msg);

                        if (MaintainResHander != null)
                        {
                            MaintainResEventArgs e = new MaintainResEventArgs(res);
                            MaintainResHander(this, e);
                        }

                        m_ReceiveBuffer.RemoveRange(0, start + len);
                    }
                }

                Thread.Sleep(100);
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

        public byte[] ReadAll()
        {
            return m_ReceiveBuffer.ToArray();
        }

        public bool Write(byte[] frame, int start, int len)
        {
            try
            {
                if (m_UdpClient == null)
                {
                    return false;
                }

                byte[] b = new byte[len];
                Array.Copy(frame, start, b, 0, len);
                m_UdpClient.Send(frame, len);

                string msg = remoteIp + "发送报文：" + MaintainProtocol.ByteArryToString(frame, 0, len);
                m_MsgHandle.AddSRMsg(SRMsgType.报文说明, msg);

                return true;
            }
            catch (Exception ex)
            {
                m_LogError.Debug(ex.ToString());
                throw ex;
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

        public override string ToString()
        {
            return m_Thread.Name;
        }
    }
}