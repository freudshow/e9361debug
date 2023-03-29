using E9361App.Log;
using E9361App.Maintain;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.Management;
using System.Text.RegularExpressions;
using System.Data;
using System.Web.UI.WebControls.WebParts;

namespace E9361App.Communication
{
    public enum PortTypeEnum
    {
        PortType_Serial = 0,
        PortType_Net,
    }

    public interface AbstractPort
    {
        bool IsOpen();

        bool Open(object para);

        bool Close();

        byte[] Read(int maxlen);

        byte[] ReadAll();

        bool Write(byte[] frame, int index, int len);

        bool FlushAll();

        PortTypeEnum GetPortType();
    }

    public delegate void USBDetectionEventHander(object sender, EventArgs e);

    public class UsbDection
    {
        private static ManagementEventWatcher watchingObect = null;
        private static WqlEventQuery watcherQuery;
        private static ManagementScope scope;

        public UsbDection()
        {
            scope = new ManagementScope("root\\CIMV2");
            scope.Options.EnablePrivileges = true;
        }

        public static void AddRemoveUSBHandler(USBDetectionEventHander h)
        {
            try
            {
                USBWatcherSetUp("__InstanceDeletionEvent");
                watchingObect.EventArrived += new EventArrivedEventHandler(h);
                watchingObect.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                if (watchingObect != null)
                    watchingObect.Stop();
            }
        }

        public static void AddInsetUSBHandler(USBDetectionEventHander h)
        {
            try
            {
                USBWatcherSetUp("__InstanceCreationEvent");
                watchingObect.EventArrived += new EventArrivedEventHandler(h);
                watchingObect.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                if (watchingObect != null)
                    watchingObect.Stop();
            }
        }

        private static void USBWatcherSetUp(string eventType)
        {
            watcherQuery = new WqlEventQuery();
            watcherQuery.EventClassName = eventType;
            watcherQuery.WithinInterval = new TimeSpan(0, 0, 2);
            watcherQuery.Condition = @"TargetInstance ISA 'Win32_USBControllerdevice'";
            watchingObect = new ManagementEventWatcher(scope, watcherQuery);
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
        private List<byte> m_ReceiveBuffer = new List<byte>();
        private SRMessageSingleton m_MsgHandle = SRMessageSingleton.getInstance();
        private Thread m_Thread;
        private volatile object m_Lock = new object();
        private volatile bool m_ThreadRunning = false;
        private log4net.ILog m_LogError = log4net.LogManager.GetLogger("logerror");

        public MaintainResEventHander MaintainResHander = null;

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
                if (m_SerialPort == null)
                {
                    m_SerialPort = new SerialPort();
                }

                if (m_SerialPort.IsOpen)
                {
                    m_SerialPort.Close();
                }

                UartPortPara upara = (UartPortPara)para;
                m_SerialPort.PortName = upara.PortName;
                m_SerialPort.BaudRate = upara.BaudRate;
                m_SerialPort.StopBits = upara.StopBits;
                m_SerialPort.DataBits = upara.DataBits;
                m_SerialPort.Parity = upara.Parity;
                m_SerialPort.ReadTimeout = upara.ReadTimeout;
                m_SerialPort.RtsEnable = upara.RtsEnable;
                m_SerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
                m_SerialPort.Open();

                if (m_Thread == null)
                {
                    m_Thread = new Thread(PortRun);
                }

                m_Thread.Name = m_SerialPort.PortName;
                m_Thread.IsBackground = true;
                m_ThreadRunning = true;
                m_Thread.Start();

                return m_SerialPort.IsOpen;
            }
            catch (Exception ex)
            {
                m_LogError.Error(ex.Message);
                throw ex;
            }
        }

        private void PortRun()
        {
            try
            {
                while (m_ThreadRunning)
                {
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

        public bool Close()
        {
            try
            {
                m_ThreadRunning = false;
                if (m_SerialPort != null)
                {
                    m_SerialPort.Close();

                    return !m_SerialPort.IsOpen;
                }

                return true;
            }
            catch (Exception ex)
            {
                m_LogError.Error(ex.Message);
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
                    byte[] receivedData = new byte[len];
                    m_SerialPort.Read(receivedData, 0, len);
                    m_ReceiveBuffer.AddRange(receivedData);
                }
            }
            catch (Exception ex)
            {
                m_LogError.Error(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        ///读取缓冲区数据
        /// </summary>
        /// <param name="maxlen">读取的最大长度</param>
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
                m_LogError.Error(ex.Message);
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
                m_LogError.Error(ex.Message);
                throw ex;
            }
        }
    }
}