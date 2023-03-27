using ControlzEx.Standard;
using E9361App.Log;
using E9361App.Maintain;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Media.TextFormatting;

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

        bool Open(String Com, int Braude);

        bool Close();

        byte[] Read(int readtimes);

        bool Write(byte[] frame, int index, int len);

        bool FlushAll();

        PortTypeEnum GetPortType();
    }

    public class UartPort : AbstractPort
    {
        private SerialPort m_SerialPort = null;
        private List<byte> m_ReceiveBuffer = new List<byte>();
        private SRMessageSingleton m_MsgHandle = SRMessageSingleton.getInstance();
        private Thread m_Thread;
        private object m_Lock = new object();

        public bool IsOpen()
        {
            return m_SerialPort != null && m_SerialPort.IsOpen;
        }

        public PortTypeEnum GetPortType()
        {
            return PortTypeEnum.PortType_Serial;
        }

        public bool Open(string com, int baud)
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

                m_SerialPort.PortName = com;
                m_SerialPort.BaudRate = baud;
                m_SerialPort.StopBits = StopBits.One;
                m_SerialPort.DataBits = 8;
                m_SerialPort.Parity = Parity.None;
                m_SerialPort.ReadTimeout = 1000;
                m_SerialPort.RtsEnable = true;
                m_SerialPort.Open();

                if (m_Thread == null)
                {
                    m_Thread = new Thread(RunPort);
                }

                m_Thread.Name = m_SerialPort.PortName;
                m_Thread.IsBackground = true;
                m_SerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
                m_Thread.Start(this);

                return m_SerialPort.IsOpen;
            }
            catch
            {
                return false;
            }
        }

        private void RunPort()
        {
            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        public bool Close()
        {
            if (m_SerialPort != null)
            {
                m_SerialPort.Close();

                return !m_SerialPort.IsOpen;
            }

            return true;
        }

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (!IsOpen())
            {
                return;
            }

            byte[] receivedData = new byte[m_SerialPort.BytesToRead];
            m_SerialPort.Read(receivedData, 0, m_SerialPort.BytesToRead);
            m_ReceiveBuffer.AddRange(receivedData);
        }

        /// <summary>
        ///读取端口
        /// </summary>
        /// <param name="readtimes"></param>
        /// <param name="bufferlen"></param>
        /// <returns></returns>
        public byte[] Read(int readtimes)
        {
            byte[] log = m_ReceiveBuffer.ToArray();
            string msg = m_SerialPort.PortName + "接收报文:<<<" + MaintainProtocol.ByteArryToString(log, 0, log.Length);
            m_MsgHandle.AddSRMsg(SRMsgType.报文说明, msg);

            return log;
        }

        public bool Write(byte[] frame, int start, int len)
        {
            if (m_SerialPort == null || !m_SerialPort.IsOpen)
            {
                return false;
            }

            m_SerialPort.Write(frame, start, len);

            string msg = m_SerialPort.PortName + "发送报文:>>>" + MaintainProtocol.ByteArryToString(frame, start, len);
            m_MsgHandle.AddSRMsg(SRMsgType.报文说明, msg);

            return true;
        }

        public bool FlushAll()
        {
            m_ReceiveBuffer.Clear();
            return true;
        }
    }
}