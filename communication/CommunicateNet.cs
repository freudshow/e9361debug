using E9361App.Log;
using E9361App.Maintain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace E9361App.Communication
{
    public class NetPara
    {
        private string m_ServerIP;
        private int m_ServerPort;
        private PortTypeEnum m_Mode;

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

        public PortTypeEnum Mode
        {
            get { return m_Mode; }
            set { m_Mode = value; }
        }
    }

    public class TcpClientNetPort : AbstractPort
    {
        private List<byte> m_ReceiveBuffer = new List<byte>();
        private byte[] m_TempBuffer = new byte[1024];
        private TcpClient m_TcpClient = null;
        private NetworkStream m_Stream = null;
        private SRMessageSingleton m_MsgHandle = SRMessageSingleton.getInstance();
        private log4net.ILog m_LogError = log4net.LogManager.GetLogger("logerror");
        private Stopwatch m_StopWatch = new Stopwatch();

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
            return PortTypeEnum.PortType_Net_UDP_Client;
        }

        public bool Open(object o)
        {
            NetPara netPara = (NetPara)o;
            if (netPara == null)
            {
                throw new ArgumentException("NULL Parameter");
            }

            if (netPara.Mode != PortTypeEnum.PortType_Net_TCP_Client)
            {
                throw new ArgumentException("Mode is not TcpClientMode");
            }

            try
            {
                if (IsOpen())
                {
                    Close();
                }

                m_TcpClient = new TcpClient(netPara.ServerIP, netPara.ServerPort);

                if (m_TcpClient.Connected)
                {
                    m_Stream = m_TcpClient.GetStream();
                }

                return m_TcpClient.Connected;
            }
            catch (Exception ex)
            {
                m_LogError.Error($"{FileFunctionLine.GetFilePath()}{FileFunctionLine.GetFunctionName()}{FileFunctionLine.GetLineNumber()}{ex.Message}");
                throw ex;
            }
        }

        public bool Close()
        {
            try
            {
                if (m_TcpClient != null)
                {
                    m_TcpClient.Close();
                    m_TcpClient.Dispose();
                    m_TcpClient = null;
                }

                if (m_Stream != null)
                {
                    m_Stream.Close();
                    m_Stream.Dispose();
                    m_Stream = null;
                }

                return true;
            }
            catch (Exception ex)
            {
                m_LogError.Error($"{FileFunctionLine.GetFilePath()}{FileFunctionLine.GetFunctionName()}{FileFunctionLine.GetLineNumber()}{ex.Message}");
                throw ex;
            }
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
                m_LogError.Error($"{FileFunctionLine.GetFilePath()}{FileFunctionLine.GetFunctionName()}{FileFunctionLine.GetLineNumber()}{ex.Message}");
                return false;
            }
        }

        public async Task<MaintainParseRes> ReadOneFrame(long timeout)
        {
            if (m_TcpClient == null || m_Stream == null)
            {
                return null;
            }

            m_ReceiveBuffer.Clear();
            m_StopWatch.Reset();
            m_StopWatch.Start();
            MaintainParseRes res = null;

            try
            {
                while (m_StopWatch.ElapsedMilliseconds < timeout)
                {
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

                    byte[] buf = m_ReceiveBuffer.ToArray();
                    if (buf != null)
                    {
                        bool found = MaintainProtocol.FindOneFrame(buf, out int start, out int len, out byte mainFunc, out byte subFucn, out byte[] data);

                        if (found)
                        {
                            byte[] frame = new byte[len];
                            Array.Copy(buf, start, frame, 0, len);

                            res = new MaintainParseRes
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
                            m_ReceiveBuffer.RemoveRange(0, start + len);
                            break;
                        }
                    }

                    await Task.Delay(100);
                }
            }
            catch (Exception ex)
            {
                m_LogError.Error($"{FileFunctionLine.GetFilePath()}{FileFunctionLine.GetFunctionName()}{FileFunctionLine.GetLineNumber()}{ex.Message}");
                throw ex;
            }

            return res;
        }
    }

    public class UdpClientNetPort : AbstractPort
    {
        private string remoteIp = "";

        private IPEndPoint m_RemoteIPEndPoint = null;
        private UdpClient m_UdpClient = null;

        private SRMessageSingleton m_MsgHandle = SRMessageSingleton.getInstance();
        private log4net.ILog m_LogError = log4net.LogManager.GetLogger("logerror");

        private List<byte> m_ReceiveBuffer = new List<byte>();
        private static readonly Stopwatch m_StopWatch = new Stopwatch();

        public bool IsOpen()
        {
            return m_UdpClient != null;
        }

        public PortTypeEnum GetPortType()
        {
            return PortTypeEnum.PortType_Net_UDP_Client;
        }

        public bool Open(object o)
        {
            try
            {
                NetPara netPara = (NetPara)o;
                if (netPara == null)
                {
                    throw new ArgumentException("NULL Parameter");
                }

                if (netPara.Mode != PortTypeEnum.PortType_Net_UDP_Client)
                {
                    throw new ArgumentException("Mode is not UdpClientMode");
                }

                if (IsOpen())
                {
                    Close();
                }

                if (m_UdpClient == null)
                {
                    m_UdpClient = new UdpClient();
                }

                m_UdpClient.Connect(IPAddress.Parse(netPara.ServerIP), netPara.ServerPort);

                return true;
            }
            catch (Exception ex)
            {
                m_LogError.Error($"{FileFunctionLine.GetFilePath()}{FileFunctionLine.GetFunctionName()}{FileFunctionLine.GetLineNumber()}{ex.Message}");
                throw ex;
            }
        }

        public bool Close()
        {
            try
            {
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

                return true;
            }
            catch (Exception ex)
            {
                m_LogError.Error($"{FileFunctionLine.GetFilePath()}{FileFunctionLine.GetFunctionName()}{FileFunctionLine.GetLineNumber()}{ex.Message}");
                throw ex;
            }
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

                string msg = remoteIp + "发送报文:>>>" + MaintainProtocol.ByteArryToString(frame, 0, len);
                m_MsgHandle.AddSRMsg(SRMsgType.报文说明, msg);

                return true;
            }
            catch (Exception ex)
            {
                m_LogError.Error($"{FileFunctionLine.GetFilePath()}{FileFunctionLine.GetFunctionName()}{FileFunctionLine.GetLineNumber()}{ex.Message}");
                throw ex;
            }
        }

        public async Task<MaintainParseRes> ReadOneFrame(long timeout)
        {
            m_ReceiveBuffer.Clear();
            m_StopWatch.Reset();
            m_StopWatch.Start();
            MaintainParseRes res = null;

            try
            {
                while (m_StopWatch.ElapsedMilliseconds < timeout)
                {
                    if (m_UdpClient == null)
                    {
                        return null;
                    }

                    if (m_UdpClient.Available > 0)
                    {
                        byte[] b = m_UdpClient.Receive(ref m_RemoteIPEndPoint);
                        m_ReceiveBuffer.AddRange(b);
                    }

                    byte[] buf = m_ReceiveBuffer.ToArray();
                    if (buf != null)
                    {
                        bool found = MaintainProtocol.FindOneFrame(buf, out int start, out int len, out byte mainFunc, out byte subFucn, out byte[] data);

                        if (found)
                        {
                            byte[] frame = new byte[len];
                            Array.Copy(buf, start, frame, 0, len);

                            res = new MaintainParseRes
                            {
                                MainFunc = mainFunc,
                                SubFucn = subFucn,
                                Start = start,
                                Len = len,
                                Data = data,
                                Frame = frame
                            };

                            string msg = "接收报文: + MaintainProtocol.ByteArryToString(frame, start, len)";
                            m_MsgHandle.AddSRMsg(SRMsgType.报文说明, msg);
                            m_ReceiveBuffer.RemoveRange(0, start + len);
                            break;
                        }
                    }

                    await Task.Delay(100);
                }
            }
            catch (Exception ex)
            {
                m_LogError.Error($"{FileFunctionLine.GetFilePath()}{FileFunctionLine.GetFunctionName()}{FileFunctionLine.GetLineNumber()}{ex.Message}");
                throw ex;
            }

            return res;
        }
    }
}