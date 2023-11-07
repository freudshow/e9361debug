using E9361Debug.Log;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace E9361Debug.Common
{
    public static class CommonClass
    {
        private static readonly log4net.ILog m_LogError = log4net.LogManager.GetLogger("logerror");

        /// <summary>
        /// 主版本号
        /// </summary>
        private static readonly string MainVersion = "v3";

        /// <summary>
        /// 次版本号
        /// </summary>
        private static readonly string SubVersion = "00";

        /// <summary>
        /// 补丁号
        /// </summary>
        private static readonly string PatchVersion = "01";

        /// <summary>
        /// 软件版本信息
        /// </summary>
        public static string Version
        {
            get
            {
                return $"{MainVersion}.{SubVersion}.{PatchVersion}";
            }
        }

        /// <summary>
        /// 验证一个整型值是否在
        /// enum中被定义
        /// </summary>
        /// <param name="t">enum类型</param>
        /// <param name="i">欲验证的整型值</param>
        /// <returns>true - 整型值被定义在enum中; false - 整型值未被定义在enum中</returns>
        public static bool IsInEnum(Type t, int i)
        {
            if (t == null)
            {
                return false;
            }

            return t.IsEnum && Enum.IsDefined(t, i);
        }

        /// <summary>
        /// 将字符串转为目标枚举类型
        /// </summary>
        /// <typeparam name="T">目标枚举类型</typeparam>
        /// <param name="s">枚举类型的成员名</param>
        /// <returns>目标枚举类型的值</returns>
        /// <exception cref="Exception">字符串不能为null或空字符串</exception>
        public static T GetEnumByString<T>(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                throw new Exception("argument can not be null or empty");
            }

            return (T)Enum.Parse(typeof(T), s, false);
        }

        /// <summary>
        /// 深度复制一个实例
        /// </summary>
        /// <typeparam name="T">可通过用法交由编译器推断, 或显式指出类型</typeparam>
        /// <param name="source">被复制实例</param>
        /// <returns>复制的实例</returns>
        public static T DeepClone<T>(T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }

        /// <summary>
        /// 读取一个文本文件的所有内容
        /// </summary>
        /// <param name="filename">完整文件名</param>
        /// <returns>文件的完整内容</returns>
        public static string ReadTextFile(string filename)
        {
            try
            {
                FileStream file = new FileStream(filename, FileMode.Open);
                StreamReader rd = new StreamReader(file);

                string s = rd.ReadToEnd();
                rd.Close();
                file.Close();

                return s;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 将字符串写入文本文件
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="s">欲写入的字符串</param>
        /// <returns>true - 写入成功; false - 写入失败</returns>
        public static bool WriteTextFile(string filename, string s)
        {
            try
            {
                StreamWriter sw = File.CreateText(filename);
                sw.WriteLine(s);
                sw.Flush();
                sw.Close();
                sw.Dispose();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 通过一个lambda表达式生成
        /// 一个委托函数, 这个委托函数
        /// 的输入参数是泛型的, 类型可以
        /// 是int, float, double, short
        /// 等等, 具体的类型视具体情况而定,
        /// 返回值是bool类型.
        /// 比如, 输入的lambda表达式为字符串:
        /// "(f)=>f>100", 这个lambda表达式只是
        /// 简单的计算一个变量是否大于100, f为lambda
        /// 表达式的参数. 则调用方法为
        /// Func<float, bool> comp = GetLambdaAsync<float>("(f)=>f>100");
        /// 本函数返回了1个委托Func<float, bool> comp,
        /// 使用comp(100.1), 就能判断其实参是否比100大.
        /// </summary>
        /// <typeparam name="T">泛型参数, 调用时指定</typeparam>
        /// <param name="s">lambda表达式的字符串</param>
        /// <returns>返回一个有1个形参, 1个bool返回值的委托</returns>
        public static async Task<Func<T, bool>> GetLambdaAsync<T>(string s)
        {
            return await CSharpScript.EvaluateAsync<Func<T, bool>>(s);
        }

        public static DataTable GetPortNames()
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

                return dt;
            }
            catch (Exception ex)
            {
                m_LogError.Error($"{FileFunctionLine.GetFilePath()}{FileFunctionLine.GetFunctionName()}{FileFunctionLine.GetLineNumber()}{ex.Message}");
                throw ex;
            }
        }

        public static string GetComputerFileMd5(string fileName)
        {
            FileStream file = new FileStream(Path.GetFullPath(fileName), FileMode.Open);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] pcMd5 = md5.ComputeHash(file);
            file.Close();
            file.Dispose();
            string pcString = BitConverter.ToString(pcMd5).Replace("-", "");

            return pcString;
        }

        public static string ExecDosCmd(string Command)
        {
            Command = Command.Trim().TrimEnd('&') + "&exit";
            using (Process p = new Process())
            {
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.UseShellExecute = false;        //是否使用操作系统shell启动
                p.StartInfo.RedirectStandardInput = true;   //接受来自调用程序的输入信息
                p.StartInfo.RedirectStandardOutput = true;  //由调用程序获取输出信息
                p.StartInfo.RedirectStandardError = true;   //重定向标准错误输出
                p.StartInfo.CreateNoWindow = true;          //不显示程序窗口
                p.Start();//启动程序
                          //向cmd窗口写入命令
                p.StandardInput.WriteLine(Command);
                p.StandardInput.AutoFlush = true;
                //获取cmd窗口的输出信息
                StreamReader reader = p.StandardOutput;//截取输出流
                StreamReader error = p.StandardError;//截取错误信息
                string str = reader.ReadToEnd() + error.ReadToEnd();
                p.WaitForExit();//等待程序执行完退出进程
                p.Close();
                return str;
            }
        }

        /// <summary>
        /// 将输入的报文字符串格式化为
        /// 正规格式, 并输出字节串
        /// </summary>
        /// <param name="inputString">输入的报文字符串</param>
        /// <param name="outputSting">格式化后的正规字符串</param>
        /// <param name="outputByteArray">输出的字节串</param>
        /// <returns>True: 输入的报文字符串格式正确; False: 输入的报文字符串格式错误</returns>
        public static bool PrepareInputString(string inputString, out string outputSting, out byte[] outputByteArray)
        {
            List<byte> list = new List<byte>();
            string s = inputString;
            bool matched = false;

            outputSting = "";
            string[] charsToRemove = new string[] { " ", ",", "\r", "\n", "\t" };
            foreach (string c in charsToRemove)
            {
                s = s.Replace(c, string.Empty);
            }

            Regex r0 = new Regex("^([0-9a-fA-F]{2})*[0-9a-fA-F]{2}$");
            Regex r1 = new Regex("(0x[0-9a-fA-F]{1,2})+");

            if (r0.IsMatch(s))
            {
                int len = s.Length / 2;

                for (int i = 0; i < len; i++)
                {
                    byte b = (byte)Convert.ToInt32(s.Substring(2 * i, 2), 16);
                    outputSting += string.Format("{0:X2} ", b);
                    list.Add(b);
                }

                matched = true;
            }
            else if (r1.IsMatch(s))
            {
                string[] sa = s.Split(new String[] { "0x" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < sa.Length; i++)
                {
                    byte b = (byte)Convert.ToInt32(sa[i], 16);
                    outputSting += string.Format("{0:X2} ", b);
                    list.Add(b);
                }

                matched = true;
            }

            if (matched)
            {
                outputByteArray = list.ToArray();
            }
            else
            {
                outputByteArray = null;
            }

            outputSting = outputSting.Trim();

            return matched;
        }

        public static void InverseArray(byte[] buf)
        {
            byte b;
            int bufSize = buf.Length;
            for (int i = 0; i < bufSize / 2; i++)
            {
                b = buf[i];
                buf[i] = buf[bufSize - i - 1];
                buf[bufSize - i - 1] = b;
            }
        }
    }
}