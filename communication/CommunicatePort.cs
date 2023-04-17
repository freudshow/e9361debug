using E9361App.Log;
using E9361App.Maintain;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Management;
using System.Text.RegularExpressions;
using System.Data;
using System.Threading.Tasks;
using System.Diagnostics;

namespace E9361App.Communication
{
    public enum PortTypeEnum
    {
        PortType_Error = -1,
        PortType_Serial = 0,
        PortType_Net_UDP_Client,
        PortType_Net_TCP_Client,
        PortType_Net_TCP_Server,
    }

    public interface AbstractPort
    {
        bool IsOpen();

        bool Open(object para);

        bool Close();

        bool Write(byte[] frame, int index, int len);

        Task<MaintainParseRes> ReadOneFrameAsync(long timeout);

        PortTypeEnum GetPortType();
    }

    public static class UsbDection
    {
        private static ManagementEventWatcher m_InsertWatchingObect = null;
        private static ManagementEventWatcher m_RemoveWatchingObect = null;
        private static ManagementScope m_Scope = new ManagementScope { Options = new ConnectionOptions { EnablePrivileges = true }, Path = new ManagementPath("root\\CIMV2") };

        public static void AddRemoveUSBHandler(EventArrivedEventHandler h)
        {
            try
            {
                if (m_RemoveWatchingObect == null)
                {
                    m_RemoveWatchingObect = new ManagementEventWatcher(m_Scope, new WqlEventQuery { EventClassName = "__InstanceDeletionEvent", WithinInterval = new TimeSpan(0, 0, 1), Condition = @"TargetInstance ISA 'Win32_USBControllerdevice'" });
                }

                m_RemoveWatchingObect.EventArrived += new EventArrivedEventHandler(h);
                m_RemoveWatchingObect.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                if (m_RemoveWatchingObect != null)
                    m_RemoveWatchingObect.Stop();
            }
        }

        public static void AddInsetUSBHandler(EventArrivedEventHandler h)
        {
            try
            {
                if (m_RemoveWatchingObect == null)
                {
                    m_InsertWatchingObect = new ManagementEventWatcher(m_Scope, new WqlEventQuery { EventClassName = "__InstanceCreationEvent", WithinInterval = new TimeSpan(0, 0, 1), Condition = @"TargetInstance ISA 'Win32_USBControllerdevice'" });
                }

                m_InsertWatchingObect.EventArrived += new EventArrivedEventHandler(h);
                m_InsertWatchingObect.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                if (m_InsertWatchingObect != null)
                    m_InsertWatchingObect.Stop();
            }
        }
    }

    public class UartPortPara
    {
        private string m_PortName;
        private int m_BaudRate = 9600;
        private StopBits m_StopBits = StopBits.One;
        private int m_DataBits = 8;
        private Parity m_Parity = Parity.None;
        private int m_ReadTimeout = 1000;
        private bool m_RtsEnable = true;

        public string PortName
        {
            get => m_PortName;
            set
            {
                m_PortName = value;
            }
        }

        public int BaudRate
        {
            get => m_BaudRate;
            set
            {
                m_BaudRate = value;
            }
        }

        public StopBits StopBits
        {
            get => m_StopBits;
            set
            {
                m_StopBits = value;
            }
        }

        public int DataBits
        {
            get => m_DataBits;
            set
            {
                m_DataBits = value;
            }
        }

        public Parity Parity
        {
            get => m_Parity;
            set
            {
                m_Parity = value;
            }
        }

        public int ReadTimeout
        {
            get => m_ReadTimeout;
            set
            {
                m_ReadTimeout = value;
            }
        }

        public bool RtsEnable
        {
            get => m_RtsEnable;
            set
            {
                m_RtsEnable = value;
            }
        }
    }

    public class UartPort : AbstractPort
    {
        private SerialPort m_SerialPort = null;
        private object m_Lock = new object();
        private List<byte> m_ReceiveBuffer = new List<byte>();
        private SRMessageSingleton m_MsgHandle = SRMessageSingleton.getInstance();
        private log4net.ILog m_LogError = log4net.LogManager.GetLogger("logerror");
        private static readonly Stopwatch m_StopWatch = new Stopwatch();

        public bool IsOpen()
        {
            return m_SerialPort != null && m_SerialPort.IsOpen;
        }

        public PortTypeEnum GetPortType()
        {
            return PortTypeEnum.PortType_Serial;
        }

        public bool Open(object para)
        {
            try
            {
                UartPortPara upara = para as UartPortPara;
                if (upara == null)
                {
                    return false;
                }

                if (m_SerialPort == null)
                {
                    m_SerialPort = new SerialPort();
                }

                if (m_SerialPort.IsOpen)
                {
                    m_SerialPort.Close();
                }

                m_SerialPort.PortName = upara.PortName;
                m_SerialPort.BaudRate = upara.BaudRate;
                m_SerialPort.StopBits = upara.StopBits;
                m_SerialPort.DataBits = upara.DataBits;
                m_SerialPort.Parity = upara.Parity;
                m_SerialPort.ReadTimeout = upara.ReadTimeout;
                m_SerialPort.RtsEnable = upara.RtsEnable;
                m_SerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
                m_SerialPort.Open();

                return m_SerialPort.IsOpen;
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
                if (m_SerialPort != null)
                {
                    m_SerialPort.Close();

                    return !m_SerialPort.IsOpen;
                }

                return true;
            }
            catch (Exception ex)
            {
                m_LogError.Error($"{FileFunctionLine.GetFilePath()}{FileFunctionLine.GetFunctionName()}{FileFunctionLine.GetLineNumber()}{ex.Message}");
                throw ex;
            }
        }

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                lock (m_Lock)
                {
                    if (!IsOpen())
                    {
                        return;
                    }

                    int len = m_SerialPort.BytesToRead;
                    if (len > 0)
                    {
                        byte[] receivedData = new byte[len];
                        m_SerialPort.Read(receivedData, 0, len);
                        m_ReceiveBuffer.AddRange(receivedData);
                    }
                }
            }
            catch (Exception ex)
            {
                m_LogError.Error($"{FileFunctionLine.GetFilePath()}{FileFunctionLine.GetFunctionName()}{FileFunctionLine.GetLineNumber()}{ex.Message}");
                throw ex;
            }
        }

        public bool Write(byte[] frame, int start, int len)
        {
            if (m_SerialPort == null || !m_SerialPort.IsOpen)
            {
                return false;
            }

            try
            {
                m_SerialPort.Write(frame, start, len);

                string msg = "发送报文:" + MaintainProtocol.ByteArryToString(frame, start, len);
                m_MsgHandle.AddSRMsg(SRMsgType.报文说明, msg);

                return true;
            }
            catch (Exception ex)
            {
                m_LogError.Error($"{FileFunctionLine.GetFilePath()}{FileFunctionLine.GetFunctionName()}{FileFunctionLine.GetLineNumber()}{ex.Message}");
                throw ex;
            }
        }

        public async Task<MaintainParseRes> ReadOneFrameAsync(long timeout)
        {
            m_ReceiveBuffer.Clear();
            m_StopWatch.Reset();
            m_StopWatch.Start();
            MaintainParseRes res = null;

            try
            {
                while (m_StopWatch.ElapsedMilliseconds < timeout)
                {
                    byte[] b = m_ReceiveBuffer.ToArray();
                    if (b != null)
                    {
                        bool found = MaintainProtocol.FindOneFrame(b, out int start, out int len, out byte mainFunc, out byte subFucn, out byte[] data);

                        if (found)
                        {
                            byte[] frame = new byte[len];
                            Array.Copy(b, start, frame, 0, len);

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

        public DataTable GetPortNames()
        {
            try
            {
                System.Data.DataTable dt = new System.Data.DataTable();
                dt.Columns.Add("Name");
                dt.Columns.Add("Description");
                System.Data.DataRow dr;
                //{4d36e978-e325-11ce-bfc1-08002be10318}为设备类别port（端口（COM&LPT））的GUID
                string sql = "SELECT * FROM Win32_PnPEntity WHERE ClassGuid=\"{4d36e978-e325-11ce-bfc1-08002be10318}\"";

                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", sql);
                ManagementObjectCollection mc = searcher.Get();
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    if (queryObj != null)
                    {
                        string name = queryObj.GetPropertyValue("Name").ToString();
                        Regex re = new Regex("COM\\d+");
                        Match m = re.Match(name);
                        if (name != null && m != null && m.Success)
                        {
                            dr = dt.NewRow();
                            dr["Name"] = m.Value;
                            dr["Description"] = name;
                            dt.Rows.Add(dr);
                        }
                    }
                }

                dr = dt.NewRow();
                dr["Name"] = "Net";
                dr["Description"] = "Net";
                dt.Rows.Add(dr);
                return dt;
            }
            catch (Exception ex)
            {
                m_LogError.Error($"{FileFunctionLine.GetFilePath()}{FileFunctionLine.GetFunctionName()}{FileFunctionLine.GetLineNumber()}{ex.Message}");
                throw ex;
            }
        }
    }

    public class CommunicationPort
    {
        private AbstractPort m_AbstractPort;

        public CommunicationPort(PortTypeEnum portType)
        {
            switch (portType)
            {
                case PortTypeEnum.PortType_Serial:
                    m_AbstractPort = new UartPort();
                    break;

                case PortTypeEnum.PortType_Net_UDP_Client:
                    m_AbstractPort = new UdpClientNetPort();
                    break;

                case PortTypeEnum.PortType_Net_TCP_Client:
                    m_AbstractPort = new TcpClientNetPort();
                    break;

                case PortTypeEnum.PortType_Net_TCP_Server:
                    break;

                default:
                    break;
            }
        }

        public bool IsOpen()
        {
            return m_AbstractPort != null && m_AbstractPort.IsOpen();
        }

        public PortTypeEnum GetPortType()
        {
            return m_AbstractPort == null ? PortTypeEnum.PortType_Error : m_AbstractPort.GetPortType();
        }

        public bool Open(object para)
        {
            return para != null && m_AbstractPort != null && m_AbstractPort.Open(para);
        }

        public bool Close()
        {
            return m_AbstractPort != null && m_AbstractPort.Close();
        }

        public bool Write(byte[] frame, int start, int len)
        {
            return m_AbstractPort != null && m_AbstractPort.Write(frame, start, len);
        }

        public async Task<MaintainParseRes> ReadOneFrameAsync(long timeout)
        {
            if (m_AbstractPort == null)
            {
                return null;
            }

            return await m_AbstractPort.ReadOneFrameAsync(timeout);
        }
    }
}