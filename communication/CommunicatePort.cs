using E9361App.Log;
using E9361App.Maintain;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

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

        bool close();

        byte[] Read(int readtimes, out int bufferlen);

        byte[] ContinueRead(int readtimes, out int bufferlen);

        bool Write(byte[] frame, int index, int len);

        PortTypeEnum GetPorType();
    }

    public class UartPort : AbstractPort
    {
        private SerialPort sp = null;
        private log4net.ILog logerror = log4net.LogManager.GetLogger("logerror");
        private log4net.ILog loginfo = log4net.LogManager.GetLogger("loginfo");

        //打印收发报文
        private SRMessageSingleton m_msgHandle = SRMessageSingleton.getInstance();

        public bool IsOpen()
        {
            return sp != null && sp.IsOpen;
        }

        public PortTypeEnum GetPorType()
        {
            return PortTypeEnum.PortType_Serial;
        }

        public bool Open(string com, int baud)
        {
            try
            {
                if (sp == null)
                {
                    sp = new SerialPort();
                    sp.PortName = com;
                    sp.BaudRate = baud;
                    sp.StopBits = StopBits.One;
                    sp.DataBits = 8;
                    sp.Parity = Parity.None;
                    sp.ReadTimeout = 1000;
                    sp.RtsEnable = true;
                    sp.Open();

                    return sp.IsOpen;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool close()
        {
            if (sp != null)
            {
                sp.Close();

                return !sp.IsOpen;
            }

            return true;
        }

        /*
         * 从串口读取一帧报文
         * @para frame 报文缓冲区
         * @para timeOut 超时时间
         * @return 此帧报文的长度
         */

        public int ReadOneFrame(byte[] frame, int msTimeOut)
        {
            try
            {
                if (sp.BytesToRead >= 5)
                {
                    int head = sp.ReadByte();
                    int frameSize = 0;
                    if (head == 0xAA)
                    {
                        int addrL = sp.ReadByte();
                        int addrH = sp.ReadByte();
                        //不校验地址
                        if (addrL > 0/*addrL == 0xff && addrH == 0xff*/)
                        {
                            int msgSizeL = sp.ReadByte();
                            int msgSizeH = sp.ReadByte();
                            int msgSize = (msgSizeH << 8) + msgSizeL;
                            //判断数据长度异常，清空串口buffer
                            if (msgSize > 2048)
                            {
                                loginfo.Error(string.Format("接收: 数据长度太大，大小 = {0:d} (最大 = 2048)\n", msgSize));

                                sp.DiscardInBuffer();
                                return -1;
                            }

                            int remainSize = msgSize + 1;
                            //一帧报文没传送完，等待
                            DateTime dt = DateTime.Now;
                            while (remainSize > sp.BytesToRead)
                            {
                                Thread.Sleep(20);
                                if ((DateTime.Now - dt).TotalMilliseconds > msTimeOut)
                                {
                                    break;
                                }
                            }
                            frame[0] = (byte)0xAA;
                            frame[1] = (byte)addrL;
                            frame[2] = (byte)addrH;
                            frame[3] = (byte)msgSizeL;
                            frame[4] = (byte)msgSizeH;

                            int readBytes = sp.Read(frame, 5, remainSize);
                            frameSize = readBytes + 5;
                            //读取帧超时，清空串口buffer
                            if (readBytes != remainSize)
                            {
                                loginfo.Error(string.Format("接收: 读取帧超时，大小 = {0:d} (期望 = {0:d})\n", readBytes, remainSize));
                                sp.DiscardInBuffer();
                                return -1;
                            }
                            int comRxLen = readBytes + 5;
                            string SndDataString = sp.PortName + "接收:";
                            for (int m = 0; m < comRxLen; m++)
                            {
                                SndDataString += string.Format("{0:X2} ", frame[m]);
                            }
                            loginfo.Debug(SndDataString);
                            return readBytes + 5;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log4net.ILog loginfo = log4net.LogManager.GetLogger("loginfo");
                loginfo.Error(ex.ToString());
            }
            return 0;
        }

        /// <summary>
        ///读取端口
        /// </summary>
        /// <param name="readtimes"></param>
        /// <param name="bufferlen"></param>
        /// <returns></returns>

        public byte[] Read(int readtimes, out int bufferlen)
        {
            bufferlen = 0;
            byte[] SerialReadBuffer = new byte[4096];

            List<byte> recvbuf = new List<byte>();
            if (sp == null)
            {
                return SerialReadBuffer;
            }

            if (!sp.IsOpen)
            {
                return SerialReadBuffer;
            }

            readtimes *= 100;//原每次延时为200ms，现在改为20ms，所以*10

            sp.DiscardInBuffer();
            int comRxLen = 0;

            /// 丢弃来自串行驱动程序的传输缓冲区的数据 清空缓冲区
            for (int ReadCount = 0; ReadCount < readtimes; ReadCount++)
            {
                if (sp.BytesToRead == 0)
                {
                    Thread.Sleep(2);
                    continue;
                }

                try
                {
                    comRxLen += sp.Read(SerialReadBuffer, comRxLen, sp.BytesToRead);
                }
                catch (TimeoutException)
                {
                    Console.WriteLine("Serial Read Overtime");
                }
            }

            bufferlen = comRxLen;
            string SndDataString = sp.PortName + "接收:" + MaintainProtocol.ByteArryToString(SerialReadBuffer, 0, bufferlen);

            return SerialReadBuffer;
        }

        public byte[] ContinueRead(int readtimes, out int bufferlen)
        {
            try
            {
                if (sp == null)
                {
                    bufferlen = 0;
                    return null;
                }

                if (!sp.IsOpen)
                {
                    bufferlen = 0;
                    return null;
                }

                Console.WriteLine(DateTime.Now.ToString("hh-mm-ss.fff") + "w");

                sp.DiscardInBuffer();/// 丢弃来自串行驱动程序的传输缓冲区的数据 清空缓冲区
                int ReadCount = 0;
                int comRxLen = 0;
                byte[] SerialReadBuffer = new byte[2048];
                for (ReadCount = 0; ReadCount < readtimes; ReadCount++)
                {
                    Console.WriteLine(DateTime.Now.ToString("hh-mm-ss.fff") + ReadCount.ToString());

                    if (sp.BytesToRead == 0)
                    {
                        Thread.Sleep(200);
                        continue;
                    }
                    try
                    {
                        comRxLen += sp.Read(SerialReadBuffer, comRxLen, sp.BytesToRead);
                        Console.WriteLine(DateTime.Now.ToString("hh-mm-ss.fff") + "接收长度" + comRxLen.ToString());
                    }
                    catch (TimeoutException)
                    {
                        Console.WriteLine("Serial Read Overtime");
                    }
                }
                bufferlen = comRxLen;
                string SndDataString = sp.PortName + "接收:";
                for (int m = 0; m < comRxLen; m++)
                {
                    SndDataString += string.Format("{0:X2} ", SerialReadBuffer[m]);
                }
                //loginfo.Debug(SndDataString);
                //Console.WriteLine(DateTime.Now.ToString("hh-mm-ss.fff") + SndDataString);
                return SerialReadBuffer;
            }
            catch (Exception ex)
            {
                loginfo.Error(ex.ToString());
                throw ex;
            }
        }

        public bool Write(byte[] frame, int start, int len)
        {
            if (sp != null && sp.IsOpen)
            {
                sp.Write(frame, start, len);

                string msg = sp.PortName + "发送报文：" + MaintainProtocol.ByteArryToString(frame, start, len);
                m_msgHandle.AddSRMsg(SRMsgType.报文说明, msg);

                return true;
            }

            return false;
        }
    }
}