using System;
using System.Runtime.InteropServices;

namespace E9361App.Maintain
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

        public byte SubFucn
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

    public class MaintainResEventArgs : EventArgs
    {
        public readonly MaintainParseRes m_Res;

        public MaintainResEventArgs(MaintainParseRes res)
        {
            m_Res = res;
        }
    }

    public delegate void MaintainResEventHander(object sender, MaintainResEventArgs e);

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

            byte[] b = new byte[len];
            Array.Copy(buf, start, b, 0, len);

            return BitConverter.ToString(b).Replace('-', ' ');
        }

        public static void ComposeFrame(byte mainFunc, byte subFunc, byte[] data, out byte[] frameBuf)
        {
            //data len except mainFunc + subFunc
            ushort dataLen = 0;

            //data length
            if (data != null && data.Length > 0)
            {
                dataLen += (ushort)(data.Length);
            }

            int pos = 0;
            frameBuf = new byte[m_minLen + dataLen];

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
        }

        /// <summary>
        /// 查找一帧报文
        /// </summary>
        /// <param name="buf">报文缓冲区</param>
        /// <param name="start">报文起始位置</param>
        /// <param name="len">一帧报文长度, 不包括无用的字符</param>
        /// <param name="mainFunc">主功能码</param>
        /// <param name="subFunc">子功能码</param>
        /// <returns></returns>
        public static bool FindOneFrame(byte[] buf, out int start, out int len, out byte mainFunc, out byte subFunc, out byte[] data)
        {
            start = -1;
            len = 0;
            mainFunc = 0;
            subFunc = 0;
            data = null;

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
            len = dataLen + m_removeDataLen;

            if (dataLen < 2 || buf.Length < (start + len))
            {
                len = 0;
                return false;
            }

            byte chk = GetXor(buf, start + Marshal.SizeOf(m_startCode), Marshal.SizeOf(m_address) + Marshal.SizeOf(dataLen) + dataLen);

            if (buf[start + Marshal.SizeOf(m_startCode) + Marshal.SizeOf(m_address) + Marshal.SizeOf(dataLen) + dataLen] != chk)
            {
                len = 0;
                return false;
            }

            mainFunc = buf[start + 5];
            subFunc = buf[start + 6];

            int dataBufLen = dataLen - Marshal.SizeOf(mainFunc) - Marshal.SizeOf(subFunc);
            data = new byte[dataBufLen];
            Array.Copy(buf, start + m_headLen + Marshal.SizeOf(mainFunc) + Marshal.SizeOf(subFunc), data, 0, dataBufLen);

            return true;
        }

        public static void SetAddress(ushort address)
        {
            m_address = address;
        }

        public static void GetResetFrame(out byte[] resetFrame)
        {
            byte mainFunc = (byte)MaintainMainFuction.MaintainMainFuction_ParameterSet;
            byte subFunc = 0x01;
            byte[] data = new byte[1];
            data[0] = 0x00;
            ComposeFrame(mainFunc, subFunc, data, out resetFrame);
        }
    }
}