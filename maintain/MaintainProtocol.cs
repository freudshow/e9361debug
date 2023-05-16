using E9361Debug.logical;
using System;
using System.Runtime.InteropServices;

namespace E9361Debug.Maintain
{
    public enum MaintainMainFuction
    {
        MaintainMainFuction_HeartJump = 0x00,
        MaintainMainFuction_FileManager = 0x01,
        MaintainMainFuction_SysInfo = 0x02,
        MaintainMainFuction_ReadRealData = 0x03,
        MaintainMainFuction_OperaAde9078 = 0x04,
        MaintainMainFuction_PortMonitor = 0x05,
        MaintainMainFuction_ParameterSet = 0X06,
        MaintainMainFuction_LookUpInfo = 0X07,
        MaintainMainFuction_SOEInfo = 0x08,
        MaintainMainFuction_YXOpera = 0x09,
        MaintainMainFuction_ReadTime = 0x0A,
        MaintainMainFuction_YKOpera = 0x0B,
        MaintainMainFuction_RealdataWatch = 0x0D,
        MaintainMainFuction_Gprs = 0x0E,
        MaintainMainFuction_Ade9078Mult = 0x0F,
        MaintainMainFuction_SSHPassThrough = 0x11
    };

    public enum RealDataTeleTypeEnum
    {
        Real_Data_TeleType_Invalid = -1,
        Real_Data_TeleType_YX = 0,
        Real_Data_TeleType_YC = 1,
        Real_Data_TeleType_YK = 2,
        Real_Data_TeleType_DD = 3,
        Real_Data_TeleType_Parameter = 4
    }

    public enum RealDataDataTypeEnum
    {
        Real_Data_type_Invalid = -1,
        Real_Data_type_Float = 0,
        Real_Data_type_Char = 1,
        Real_Data_type_Int = 2,
    }

    public enum YKOnOffEnum
    {
        YK_On = 0x51,
        YK_Off = 0x91
    }

    public enum YKOperateTypeEnum
    {
        YK_Operate_Preset = 1,
        YK_Operate_Actual = 2,
        YK_Operate_Cancel_Preset = 3,
    }

    public class MaintainParseRes
    {
        private byte m_MainFunc;
        private byte m_SubFucn;
        private int m_Start;
        private int m_Len;
        private byte[] m_Data;
        private byte[] m_Frame;

        public MaintainParseRes()
        { }

        public byte MainFunc
        {
            get { return m_MainFunc; }
            set { m_MainFunc = value; }
        }

        public byte SubFunc
        {
            get { return m_SubFucn; }
            set { m_SubFucn = value; }
        }

        public int Start
        {
            get { return m_Start; }
            set { m_Start = value; }
        }

        public int Len
        {
            get { return m_Len; }
            set { m_Len = value; }
        }

        public byte[] Data
        {
            get { return m_Data; }
            set { m_Data = value; }
        }

        public byte[] Frame
        {
            get { return m_Frame; }
            set { m_Frame = value; }
        }
    }

    public static class MaintainProtocol
    {
        private const byte m_startCode = 0xAA;
        private static ushort m_address = 0xFFFF;

        /// <summary>
        /// 报文的最小长度.
        /// 1字节起始码 + 2字节地址 + 2字节数据域长度 + 1字节主功能码 + 1字节子功能码 + 1字节校验值
        /// </summary>
        private const int m_minLen = sizeof(byte) + sizeof(ushort) + sizeof(ushort) + sizeof(byte) + sizeof(byte) + sizeof(byte);

        /// <summary>
        /// 报文去掉数据域的长度.
        /// 1字节起始码 + 2字节地址 + 2字节数据域长度 +  + 1字节校验值
        /// </summary>
        private const int m_removeDataLen = sizeof(byte) + sizeof(ushort) + sizeof(ushort) + sizeof(byte);

        /// <summary>
        /// 报文头的长度.
        /// 1字节起始码 + 2字节地址 + 2字节数据域长度
        /// </summary>
        private const int m_headLen = sizeof(byte) + sizeof(ushort) + sizeof(ushort);

        private static readonly byte[] _auchCRCHi = new byte[]//crc高位表
            {
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
            0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
            0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
            0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
            0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
            0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
            0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
            0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
            0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
            0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
            0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
            0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
            0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
            0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
            0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
            0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
            0x80, 0x41, 0x00, 0xC1, 0x81, 0x40
            };

        private static readonly byte[] _auchCRCLo = new byte[]//crc低位表
            {
            0x00, 0xC0, 0xC1, 0x01, 0xC3, 0x03, 0x02, 0xC2, 0xC6, 0x06,
            0x07, 0xC7, 0x05, 0xC5, 0xC4, 0x04, 0xCC, 0x0C, 0x0D, 0xCD,
            0x0F, 0xCF, 0xCE, 0x0E, 0x0A, 0xCA, 0xCB, 0x0B, 0xC9, 0x09,
            0x08, 0xC8, 0xD8, 0x18, 0x19, 0xD9, 0x1B, 0xDB, 0xDA, 0x1A,
            0x1E, 0xDE, 0xDF, 0x1F, 0xDD, 0x1D, 0x1C, 0xDC, 0x14, 0xD4,
            0xD5, 0x15, 0xD7, 0x17, 0x16, 0xD6, 0xD2, 0x12, 0x13, 0xD3,
            0x11, 0xD1, 0xD0, 0x10, 0xF0, 0x30, 0x31, 0xF1, 0x33, 0xF3,
            0xF2, 0x32, 0x36, 0xF6, 0xF7, 0x37, 0xF5, 0x35, 0x34, 0xF4,
            0x3C, 0xFC, 0xFD, 0x3D, 0xFF, 0x3F, 0x3E, 0xFE, 0xFA, 0x3A,
            0x3B, 0xFB, 0x39, 0xF9, 0xF8, 0x38, 0x28, 0xE8, 0xE9, 0x29,
            0xEB, 0x2B, 0x2A, 0xEA, 0xEE, 0x2E, 0x2F, 0xEF, 0x2D, 0xED,
            0xEC, 0x2C, 0xE4, 0x24, 0x25, 0xE5, 0x27, 0xE7, 0xE6, 0x26,
            0x22, 0xE2, 0xE3, 0x23, 0xE1, 0x21, 0x20, 0xE0, 0xA0, 0x60,
            0x61, 0xA1, 0x63, 0xA3, 0xA2, 0x62, 0x66, 0xA6, 0xA7, 0x67,
            0xA5, 0x65, 0x64, 0xA4, 0x6C, 0xAC, 0xAD, 0x6D, 0xAF, 0x6F,
            0x6E, 0xAE, 0xAA, 0x6A, 0x6B, 0xAB, 0x69, 0xA9, 0xA8, 0x68,
            0x78, 0xB8, 0xB9, 0x79, 0xBB, 0x7B, 0x7A, 0xBA, 0xBE, 0x7E,
            0x7F, 0xBF, 0x7D, 0xBD, 0xBC, 0x7C, 0xB4, 0x74, 0x75, 0xB5,
            0x77, 0xB7, 0xB6, 0x76, 0x72, 0xB2, 0xB3, 0x73, 0xB1, 0x71,
            0x70, 0xB0, 0x50, 0x90, 0x91, 0x51, 0x93, 0x53, 0x52, 0x92,
            0x96, 0x56, 0x57, 0x97, 0x55, 0x95, 0x94, 0x54, 0x9C, 0x5C,
            0x5D, 0x9D, 0x5F, 0x9F, 0x9E, 0x5E, 0x5A, 0x9A, 0x9B, 0x5B,
            0x99, 0x59, 0x58, 0x98, 0x88, 0x48, 0x49, 0x89, 0x4B, 0x8B,
            0x8A, 0x4A, 0x4E, 0x8E, 0x8F, 0x4F, 0x8D, 0x4D, 0x4C, 0x8C,
            0x44, 0x84, 0x85, 0x45, 0x87, 0x47, 0x46, 0x86, 0x82, 0x42,
            0x43, 0x83, 0x41, 0x81, 0x80, 0x40
            };

        /// <summary>
        /// 计算CRC-16校验值
        /// </summary>
        /// <param name="buffer">报文缓冲区</param>
        /// <param name="start">起始位置</param>
        /// <param name="end">结束位置</param>
        /// <returns></returns>
        public static ushort CRC16(byte[] buf, int start, int len)
        {
            int end = start + len;
            if (buf == null)
            {
                throw new ArgumentOutOfRangeException("buf");
            }

            if (start >= buf.Length)
            {
                throw new ArgumentOutOfRangeException("start");
            }

            if (end > buf.Length)
            {
                throw new ArgumentOutOfRangeException("len");
            }

            byte crcHi = 0xff; // 高位初始化
            byte crcLo = 0xff; // 低位初始化
            for (int i = start; i < end; i++)
            {
                int crcIndex = crcHi ^ buf[i]; //查找crc表值

                crcHi = (byte)(crcLo ^ _auchCRCHi[crcIndex]);
                crcLo = _auchCRCLo[crcIndex];
            }

            return (ushort)(crcHi << 8 | crcLo);
        }

        public static byte GetXor(byte[] buf, int start, int len)
        {
            int end = start + len;
            if (buf == null)
            {
                throw new ArgumentOutOfRangeException("buf");
            }

            if (start >= buf.Length)
            {
                throw new ArgumentOutOfRangeException("start");
            }

            if (end > buf.Length)
            {
                throw new ArgumentOutOfRangeException("len");
            }

            byte x = 0;
            for (int i = start; i < end; i++)
            {
                x ^= buf[i];
            }

            return x;
        }

        /// <summary>
        /// 将byte数组转为字符串
        /// </summary>
        /// <param name="buf">byte数组</param>
        /// <param name="start">数组的起始位置</param>
        /// <param name="len">要转换的数组长度</param>
        /// <returns>转换好的字符串</returns>
        /// <exception cref="ArgumentOutOfRangeException">数组为空或者索引值不合法</exception>
        public static string ByteArryToString(byte[] buf, int start, int len)
        {
            int end = start + len;
            if (buf == null)
            {
                throw new ArgumentOutOfRangeException("buf");
            }

            if (start >= buf.Length)
            {
                throw new ArgumentOutOfRangeException("start");
            }

            if (end > buf.Length)
            {
                throw new ArgumentOutOfRangeException("len");
            }

            try
            {
                byte[] b = new byte[len];
                Array.Copy(buf, start, b, 0, len);

                return BitConverter.ToString(b).Replace('-', ' ');
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 组织一帧维护规约报文
        /// </summary>
        /// <param name="mainFunc">主功能码</param>
        /// <param name="subFunc">子功能码</param>
        /// <param name="data">去掉主功能码和子功能码后的数据域</param>
        /// <param name="frameBuf">输出的完整的报文缓冲区</param>
        public static byte[] ComposeFrame(byte mainFunc, byte subFunc, byte[] data)
        {
            //data len except mainFunc + subFunc
            ushort dataLen = 0;
            if (data != null && data.Length > 0)
            {
                dataLen += (ushort)(data.Length);
            }

            int pos = 0;
            byte[] frameBuf = new byte[m_minLen + dataLen];

            //start code
            frameBuf[pos++] = m_startCode;

            //address
            Array.Copy(BitConverter.GetBytes(m_address), 0, frameBuf, pos, sizeof(ushort));
            pos += sizeof(ushort);

            //data len
            dataLen += (sizeof(byte) + sizeof(byte));
            Array.Copy(BitConverter.GetBytes(dataLen), 0, frameBuf, pos, sizeof(ushort));
            pos += sizeof(ushort);

            //function code
            frameBuf[pos++] = mainFunc;
            frameBuf[pos++] = subFunc;

            //data
            if (data != null && data.Length > 0)
            {
                Array.Copy(data, 0, frameBuf, pos, data.Length);
                pos += data.Length;
            }

            //chksum
            frameBuf[pos++] = GetXor(frameBuf, 1, pos - 1);

            return frameBuf;
        }

        /// <summary>
        /// 查找一帧完整的报文
        /// </summary>
        /// <param name="buf">报文缓冲区</param>
        /// <param name="res">如果查找到完整报文, 将解析结果存入res; 否则res返回null</param>
        /// <returns>true-查找到一帧完整的报文; false-未查找到一帧完整的报文</returns>
        public static bool FindOneFrame(byte[] buf, out MaintainParseRes res)
        {
            int start = -1;
            res = null;

            if (buf == null || buf.Length <= m_minLen)
            {
                return false;
            }

            int delta;
            ushort addr;
            bool foundStart = false;
            for (int i = 0; i < buf.Length; i += delta)
            {
                if (buf[i] == m_startCode)
                {
                    if ((buf.Length - i) > sizeof(ushort))
                    {
                        byte[] addrArray = new byte[sizeof(ushort)];
                        Array.Copy(buf, i + 1, addrArray, 0, sizeof(ushort));
                        addr = BitConverter.ToUInt16(addrArray, 0);
                        if (addr == m_address || addr == 0xFFFF)
                        {
                            foundStart = true;
                            start = i;
                            break;
                        }
                        else
                        {
                            delta = Marshal.SizeOf(m_address) + 1;
                        }
                    }
                    else
                    {
                        delta = 1;
                    }
                }
                else
                {
                    delta = 1;
                }
            }

            if (!foundStart)
            {
                return false;
            }

            if ((buf.Length - start) < m_headLen)
            {
                return false;
            }

            ushort dataLen = BitConverter.ToUInt16(buf, start + Marshal.SizeOf(m_startCode) + Marshal.SizeOf(m_address));
            int len = dataLen + m_removeDataLen;

            if (dataLen < 2 || buf.Length < (start + len))
            {
                return false;
            }

            byte chk = GetXor(buf, start + Marshal.SizeOf(m_startCode), Marshal.SizeOf(m_address) + Marshal.SizeOf(dataLen) + dataLen);

            if (buf[start + Marshal.SizeOf(m_startCode) + Marshal.SizeOf(m_address) + Marshal.SizeOf(dataLen) + dataLen] != chk)
            {
                return false;
            }

            res = new MaintainParseRes
            {
                MainFunc = buf[start + 5],
                SubFunc = buf[start + 6],
                Start = start,
                Len = len
            };

            int dataBufLen = dataLen - Marshal.SizeOf(res.MainFunc) - Marshal.SizeOf(res.SubFunc);
            res.Data = new byte[dataBufLen];
            Array.Copy(buf, start + m_headLen + Marshal.SizeOf(res.MainFunc) + Marshal.SizeOf(res.SubFunc), res.Data, 0, dataBufLen);

            res.Frame = new byte[len];
            Array.Copy(buf, start, res.Frame, 0, len);

            return true;
        }

        /// <summary>
        /// 终端的地址, 默认为0xFFFF
        /// </summary>
        /// <param name="address">终端的地址</param>
        public static void SetAddress(ushort address)
        {
            m_address = address;
        }

        /// <summary>
        /// 组一个重启终端帧
        /// </summary>
        /// <param name="outframe">组合好的报文</param>
        public static byte[] GetResetFrame()
        {
            byte mainFunc = (byte)MaintainMainFuction.MaintainMainFuction_ParameterSet;
            byte subFunc = 0x01;
            byte[] data = new byte[1];
            data[0] = 0x00;
            return ComposeFrame(mainFunc, subFunc, data);
        }

        /// <summary>
        /// 组一个读取终端时间帧
        /// </summary>
        /// <param name="outframe">组合好的报文</param>
        public static byte[] GetTerminalTime()
        {
            byte mainFunc = (byte)MaintainMainFuction.MaintainMainFuction_ReadTime;
            byte subFunc = 0x01;
            byte[] data = new byte[1];
            data[0] = 0x00;
            return ComposeFrame(mainFunc, subFunc, data);
        }

        /// <summary>
        /// 解析终端返回的读取时间的报文
        /// </summary>
        /// <param name="frame">一帧完整的时间应答报文</param>
        /// <returns>报文里的时间, DateTime格式</returns>
        public static DateTime ParseTerminalTime(byte[] frame)
        {
            byte[] b = new byte[7];
            Array.Copy(frame, 7, b, 0, 7);

            int year = (int)(b[6] & 0x7F) + 2000;
            int month = (int)(b[5] & 0x0F);
            int day = (int)(b[4] & 0x1F);
            int hour = (int)(b[3] & 0x1F);
            int minute = (int)(b[2] & 0x3F);
            int second = (int)((b[1] << 8) + b[0]) / 1000;
            int milisec = (int)((b[1] << 8) + b[0]) % 1000;

            return new DateTime(year, month, day, hour, minute, second, milisec);
        }

        /// <summary>
        /// 读取实时库中同一遥测/遥信类型的,
        /// 且同一数据类型的, 连续数量的数值
        /// </summary>
        /// <param name="startIdx">起始实时库号</param>
        /// <param name="count">连续读取的实时库数据项数量</param>
        /// <param name="teleType">遥测/遥信类型, 0 - 遥信, 1 - 遥测, 2 - 遥控, 3 - 电度, 4 - 参数</param>
        /// <param name="dataType">数据类型, 0 - float, 1 - char</param>
        /// <param name="outframe">输出的读取报文</param>
        public static byte[] GetContinueRealDataBaseValue(RealDatabaseCmdParameters param)
        {
            byte mainFunc = (byte)MaintainMainFuction.MaintainMainFuction_RealdataWatch;
            byte subFunc = 0x01;
            int datalen = sizeof(ushort) + sizeof(byte) + sizeof(byte) + sizeof(byte) + sizeof(byte);
            byte[] data = new byte[datalen];

            int pos = 0;
            byte[] start = BitConverter.GetBytes(param.RealDataBaseNo);
            Array.Copy(start, 0, data, pos, sizeof(ushort));
            pos += sizeof(ushort);

            data[pos++] = (byte)param.TeleType;//遥测/遥信类型
            data[pos++] = (byte)param.DataType;//数据类型
            data[pos++] = param.DataItemCount;//数据项数量
            data[pos++] = 0x00;//预留

            return ComposeFrame(mainFunc, subFunc, data);
        }

        public static ContinueRealData ParseContinueRealDataValue(byte[] frame)
        {
            if (frame == null || frame.Length < m_minLen)
            {
                return null;
            }

            if (!FindOneFrame(frame, out MaintainParseRes res))
            {
                return null;
            }

            if (res.MainFunc != (byte)MaintainMainFuction.MaintainMainFuction_RealdataWatch || res.SubFunc != 0x01)
            {
                return null;
            }

            if (res.Data == null || res.Data.Length < 6)
            {
                return null;
            }

            ContinueRealData realData = new ContinueRealData();

            int pos = 0;
            realData.StartIdx = BitConverter.ToUInt16(res.Data, 0);
            pos += sizeof(ushort);

            realData.TeleType = (RealDataTeleTypeEnum)res.Data[pos++];
            realData.DataType = (RealDataDataTypeEnum)res.Data[pos++];
            realData.IsValid = res.Data[pos++] == 0x00;
            realData.ByteCount = res.Data[pos++];

            if (!realData.IsValid)
            {
                return null;
            }

            int delta = 0;
            switch (realData.DataType)
            {
                case RealDataDataTypeEnum.Real_Data_type_Float:
                case RealDataDataTypeEnum.Real_Data_type_Int:
                    delta = 4;
                    break;

                case RealDataDataTypeEnum.Real_Data_type_Char:
                    delta = 1;
                    break;

                case RealDataDataTypeEnum.Real_Data_type_Invalid:
                default:
                    delta = 0;
                    break;
            }

            if (delta <= 0 || res.Data.Length < realData.ByteCount + 6)
            {
                return null;
            }

            int regCount = (realData.ByteCount / delta);
            realData.RealDataArray = new RealDataType[regCount];

            pos = 6;
            for (int i = 0; i < regCount; i++)
            {
                realData.RealDataArray[i] = new RealDataType();
                switch (realData.DataType)
                {
                    case RealDataDataTypeEnum.Real_Data_type_Float:
                        realData.RealDataArray[i].FloatValue = BitConverter.ToSingle(res.Data, pos);
                        break;

                    case RealDataDataTypeEnum.Real_Data_type_Char:
                        realData.RealDataArray[i].CharValue = (sbyte)res.Data[pos];
                        break;

                    case RealDataDataTypeEnum.Real_Data_type_Int:
                        realData.RealDataArray[i].IntValue = BitConverter.ToInt32(res.Data, pos);
                        break;

                    case RealDataDataTypeEnum.Real_Data_type_Invalid:
                    default:
                        break;
                }

                pos += delta;
            }

            return realData;
        }

        /// <summary>
        /// 获取遥控预置合的报文
        /// </summary>
        /// <param name="YKNo">遥控号</param>
        /// <returns>输出的报文</returns>
        public static byte[] GetYKPresetOn(byte YKNo)
        {
            byte mainFunc = (byte)MaintainMainFuction.MaintainMainFuction_YKOpera;
            byte subFunc = (byte)YKOperateTypeEnum.YK_Operate_Preset;
            int datalen = sizeof(byte) + sizeof(byte);
            byte[] data = new byte[datalen];
            data[0] = YKNo;
            data[1] = (byte)YKOnOffEnum.YK_On;

            return ComposeFrame(mainFunc, subFunc, data);
        }

        /// <summary>
        /// 获取遥控预置撤销的报文
        /// </summary>
        /// <param name="YKNo">遥控号</param>
        /// <returns>输出的报文</returns>
        public static byte[] GetYKPresetOff(byte YKNo)
        {
            byte mainFunc = (byte)MaintainMainFuction.MaintainMainFuction_YKOpera;
            byte subFunc = (byte)YKOperateTypeEnum.YK_Operate_Cancel_Preset;
            int datalen = sizeof(byte);
            byte[] data = new byte[datalen];
            data[0] = YKNo;

            return ComposeFrame(mainFunc, subFunc, data);
        }

        /// <summary>
        /// 获取遥控分/合的报文
        /// </summary>
        /// <param name="YKNo">遥控号</param>
        /// <returns>输出的报文</returns>
        public static byte[] GetYKOnOff(byte YKNo, YKOnOffEnum op)
        {
            byte mainFunc = (byte)MaintainMainFuction.MaintainMainFuction_YKOpera;
            byte subFunc = (byte)YKOperateTypeEnum.YK_Operate_Actual;
            int datalen = sizeof(byte) + sizeof(byte);
            byte[] data = new byte[datalen];
            data[0] = YKNo;
            data[1] = (byte)op;

            return ComposeFrame(mainFunc, subFunc, data);
        }

        public static byte ParseYKResult(byte[] frame)
        {
            byte result = 1;
            if (frame == null || frame.Length < 10)
            {
                return result;
            }

            return frame[8];
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 16, CharSet = CharSet.Ansi)]
    public class RealDataType
    {
        [FieldOffset(0)] public float FloatValue;
        [FieldOffset(0)] public sbyte CharValue;
        [FieldOffset(0)] public int IntValue;
        [FieldOffset(0)] public Double DoubleValue;
        [FieldOffset(0)] public long LongValue;
    }

    public class ContinueRealData
    {
        public ushort StartIdx;
        public RealDataTeleTypeEnum TeleType;
        public RealDataDataTypeEnum DataType;
        public bool IsValid;
        public byte ByteCount;

        public RealDataType[] RealDataArray;
    }
}