using E9361Debug.Common;
using E9361Debug.Communication;
using E9361Debug.Log;
using E9361Debug.logical;
using E9361Debug.Maintain;
using E9361Debug.Mqtt;
using E9361Debug.SshInterface;
using E9361Debug.Windows;
using MQTTnet.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace E9361Debug.Logical
{
    public enum CommandType
    {
        Cmd_Type_Invalid = -1,                      //无效
        Cmd_Type_MaintainFrame = 0,                 //维护规约的二进制命令
        Cmd_Type_Shell,                             //通过ssh或者串口, 发送bash命令
        Cmd_Type_Mqtt,                              //通过mqtt发送报文
        Cmd_Type_MaintainReadRealDataBase,          //通过维护规约, 读取实时库的值
        Cmd_Type_MaintainWriteRealDataBaseYK,       //通过维护规约, 遥控操作
        Cmd_Type_SftpFileTransfer,                  //通过sftp规约传输文件
        Cmd_Type_DelaySomeTime,                     //单纯的等待
        Cmd_Type_WindowsCommand,                    //执行Windows的命令
        Cmd_Type_Manual_Operate,                    //手动让用户操作
        Cmd_Type_ADC_Adjust,                        //交采整定
        Cmd_Type_Console,                           //维护口检测
        Cmd_Type_SetSerial,                         //设置终端序列号或者ESN号等
    }

    public enum ResultTypeEnum
    {
        Result_Type_Invalid = -1,
        Result_Type_Int32 = 0,
        Result_Type_Double,
        Result_Type_Boolean,
        Result_Type_Positive_Infinity,
        Result_Type_Negtive_Infinity,
        Result_Type_Byte_Array,
        Result_Type_String,
    }

    public enum ResultSignEnum
    {
        Result_Sign_Invalid = -1,
        Result_Sign_Equal = 0,
        Result_Sign_Greater_Than,
        Result_Sign_Less_Than,
        Result_Sign_Interval,
        Result_Sign_Regex,
        Result_Sign_Lambda
    }

    public enum ResultInfoType
    {
        ResultInfo_Logs = 0,
        ResultInfo_Result,
        ResultInfo_Exception
    }

    public enum CheckIsPassed
    {
        Check_Init = 0,
        Check_Start,
        Check_Is_Passed,
        Check_Not_passed,
    }

    public class PropertyChangedClass : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class CheckItems : PropertyChangedClass
    {
        private int m_Seq;
        private CommandType m_CmdType;
        private string m_CmdParam;
        private ResultTypeEnum m_ResultType;
        private string m_ResultValue;
        private ResultSignEnum m_ResultSign;
        private string m_Description;
        private bool m_IsEnable;
        private int m_TimeOut;
        private string m_ChildTableName;
        private ObservableCollection<CheckItems> m_Children;
        private int m_Depth;
        private CheckItems m_Father;
        private Brush m_BackgroundColor;
        private CheckIsPassed m_CheckIsPassed = CheckIsPassed.Check_Init;

        public int Seq
        {
            get => m_Seq;
            set
            {
                m_Seq = value; OnPropertyChanged(nameof(Seq));
            }
        }

        public CommandType CmdType
        {
            get => m_CmdType;
            set
            {
                m_CmdType = value; OnPropertyChanged(nameof(CmdType));
            }
        }

        public string CmdParam
        {
            get => m_CmdParam;
            set
            {
                m_CmdParam = value; OnPropertyChanged(nameof(CmdParam));
            }
        }

        public ResultTypeEnum ResultType
        {
            get => m_ResultType;
            set
            {
                m_ResultType = value; OnPropertyChanged(nameof(ResultType));
            }
        }

        public string ResultValue
        {
            get => m_ResultValue;
            set
            {
                m_ResultValue = value; OnPropertyChanged(nameof(ResultValue));
            }
        }

        public ResultSignEnum ResultSign
        {
            get => m_ResultSign;
            set
            {
                m_ResultSign = value; OnPropertyChanged(nameof(ResultSign));
            }
        }

        public string Description
        {
            get => m_Description;
            set
            {
                m_Description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public bool IsEnable
        {
            get => m_IsEnable;
            set
            {
                m_IsEnable = value;

                if (!value)
                {
                    CheckIsPassed = CheckIsPassed.Check_Init;
                }

                if (Children != null && Children.Count > 0)
                {
                    foreach (CheckItems item in Children)
                    {
                        item.IsEnable = value;
                    }
                }

                OnPropertyChanged(nameof(IsEnable));
            }
        }

        public int TimeOut
        {
            get => m_TimeOut;
            set
            {
                m_TimeOut = value;
                OnPropertyChanged(nameof(TimeOut));
            }
        }

        public int Depth
        {
            get => m_Depth;
            private set
            {
                m_Depth = value;
            }
        }

        public CheckItems Father
        {
            get => m_Father;
            private set
            {
                m_Father = value;
                Depth = value.Depth + 1;
            }
        }

        public Brush BackgroundColor
        {
            get => m_BackgroundColor;
            set
            {
                m_BackgroundColor = value;
                OnPropertyChanged(nameof(BackgroundColor));
            }
        }

        public CheckIsPassed CheckIsPassed
        {
            get => m_CheckIsPassed;
            set
            {
                m_CheckIsPassed = value;
                switch (value)
                {
                    case CheckIsPassed.Check_Init:
                        BackgroundColor = Brushes.LightGray;
                        break;

                    case CheckIsPassed.Check_Start:
                        BackgroundColor = Brushes.Yellow;
                        break;

                    case CheckIsPassed.Check_Is_Passed:
                        BackgroundColor = Brushes.Green;
                        break;

                    case CheckIsPassed.Check_Not_passed:
                        BackgroundColor = Brushes.Red;
                        break;

                    default:
                        break;
                }
            }
        }

        public string ChildTableName
        {
            get => m_ChildTableName;
            set
            {
                m_ChildTableName = value;
                if (!string.IsNullOrEmpty(value))
                {
                    DataTable dt = DataBaseLogical.GetCheckItemsByTableName(value);

                    if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
                    {
                        Children = new ObservableCollection<CheckItems>();
                        foreach (DataRow dr in dt.Rows)
                        {
                            CheckItems item = new CheckItems
                            {
                                Seq = Convert.ToInt32(dr["seq"]),
                                CmdType = dr["cmdType"] == DBNull.Value ? CommandType.Cmd_Type_Invalid : (CommandType)Convert.ToInt32(dr["cmdType"]),
                                CmdParam = dr["cmdParam"] == DBNull.Value ? "" : dr["cmdParam"].ToString(),
                                ResultType = dr["resultType"] == DBNull.Value ? ResultTypeEnum.Result_Type_Invalid : (ResultTypeEnum)Convert.ToInt32(dr["resultType"]),
                                ResultValue = dr["resultValue"] == DBNull.Value ? "" : dr["resultValue"].ToString(),
                                ResultSign = dr["resultSign"] == DBNull.Value ? ResultSignEnum.Result_Sign_Invalid : (ResultSignEnum)Convert.ToInt32(dr["resultSign"]),
                                Description = dr["description"] == DBNull.Value ? "" : dr["description"].ToString(),
                                IsEnable = dr["isEnable"] == DBNull.Value ? false : Convert.ToInt32(dr["isEnable"]) == 1 ? true : false,
                                TimeOut = dr["timeout"] == DBNull.Value ? 3 : Convert.ToInt32(dr["timeout"]),
                                Father = this,
                                ChildTableName = dr["childTableName"].ToString()
                            };

                            Children.Add(item);
                        }
                    }
                }

                OnPropertyChanged(nameof(ChildTableName));
                OnPropertyChanged(nameof(Children));
            }
        }

        public ObservableCollection<CheckItems> Children
        {
            get => m_Children;
            set
            {
                m_Children = value;
                OnPropertyChanged(nameof(Children));
            }
        }
    }

    public class CheckProcess
    {
        private static SshClientClass m_SshClass;
        private static MqttHelper m_MqttHelper;
        private static SRMessageSingleton m_MsgHandle = SRMessageSingleton.getInstance();
        private static readonly log4net.ILog m_LogError = log4net.LogManager.GetLogger("logerror");

        /// <summary>
        /// 读取终端时间
        /// </summary>
        /// <param name="port">维护规约端口</param>
        /// <param name="callbackOutput">用于输出信息的回调函数</param>
        /// <returns>无</returns>
        public async Task ReadTimeAsync(CommunicationPort port, Action<ResultInfoType, bool, string> callbackOutput)
        {
            try
            {
                byte[] b = MaintainProtocol.GetTerminalTime();
                port.Write(b, 0, b.Length);
                MaintainParseRes res = await port.ReadOneFrameAsync(5000);

                if (res != null)
                {
                    callbackOutput?.Invoke(ResultInfoType.ResultInfo_Logs, true, $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}: {MaintainProtocol.ByteArryToString(res.Frame, 0, res.Frame.Length)}, {MaintainProtocol.ParseTerminalTime(res.Frame).ToString("yyyy-MM-dd HH:mm:ss")}\n");
                }
                else
                {
                    callbackOutput?.Invoke(ResultInfoType.ResultInfo_Result, false, "连接超时\n");
                }
            }
            catch (Exception ex)
            {
                callbackOutput?.Invoke(ResultInfoType.ResultInfo_Exception, false, $"异常:{ex.Message}\n");
            }
        }

        /// <summary>
        /// 判断从终端读取的结果是否符合预期
        /// </summary>
        /// <typeparam name="S">从终端读取的结果的类型</typeparam>
        /// <typeparam name="T">预期的结果的类型</typeparam>
        /// <param name="result">从终端读取的结果</param>
        /// <param name="target">预期的结果</param>
        /// <param name="sign">符号的枚举</param>
        /// <returns>结果符合预期, 返回true; 不符合预期, 返回false</returns>
        public static async Task<bool> JudgeResultBySignAsync<S, T>(S result, T target, ResultSignEnum sign)
        {
            bool testResult = false;

            switch (sign)
            {
                case ResultSignEnum.Result_Sign_Equal://相等
                    Type t = typeof(T);
                    Type s = typeof(S);
                    if (s.FullName == "System.String" && t.FullName == "System.String")
                    {
                        testResult = result.ToString() == target.ToString();
                    }
                    else
                    {
                        Func<S, bool> equalEvaluate = await CommonClass.GetLambdaAsync<S>($"(x)=>x=={target}");
                        testResult = equalEvaluate(result);
                    }

                    break;

                case ResultSignEnum.Result_Sign_Greater_Than://大于等于
                    Func<S, bool> greaterEvaluate = await CommonClass.GetLambdaAsync<S>($"(x)=>x>={target}");
                    testResult = greaterEvaluate(result);
                    break;

                case ResultSignEnum.Result_Sign_Less_Than://小于等于
                    Func<S, bool> lessEvaluate = await CommonClass.GetLambdaAsync<S>($"(x)=>x<={target}");
                    testResult = lessEvaluate(result);
                    break;

                case ResultSignEnum.Result_Sign_Interval://区间
                    string[] bounds = target.ToString().Split(',');
                    Func<S, bool> intervalEvaluate = await CommonClass.GetLambdaAsync<S>($"(x)=>x>={bounds[0]}&&x<={bounds[1]}");
                    testResult = intervalEvaluate(result);
                    break;

                case ResultSignEnum.Result_Sign_Regex://正则表达式
                    Regex re = new Regex(target.ToString(), RegexOptions.Compiled);
                    if (result != null && !string.IsNullOrEmpty(result.ToString()))
                    {
                        testResult = re.IsMatch(result.ToString());
                    }

                    break;

                case ResultSignEnum.Result_Sign_Lambda://lambda表达式
                    Func<S, bool> lambdaEvaluate = await CommonClass.GetLambdaAsync<S>(target.ToString());
                    testResult = lambdaEvaluate(result);
                    break;

                case ResultSignEnum.Result_Sign_Invalid://符号无效, 意即符号不生效, 只进行一项操作而不对比结果,
                    testResult = true;
                    break;

                default:
                    testResult = false;
                    break;
            }

            return testResult;
        }

        public static async Task<bool> CheckOneItemAsync(Dictionary<PortUseTypeEnum, CommunicationPort> port, CheckItems c, Action<ResultInfoType, bool, string, int> callbackOutput)
        {
            bool res = true;
            if (c == null)
            {
                return true;
            }

            callbackOutput?.Invoke(ResultInfoType.ResultInfo_Logs, res, $"[{c.Description}], 检测开始\n", c.Depth);

            string checkstr = "";
            if (c.Children != null)
            {
                bool isEnable = false;
                foreach (CheckItems item in c.Children)
                {
                    if (item.IsEnable)
                    {
                        c.CheckIsPassed = CheckIsPassed.Check_Init;
                        isEnable = true;
                        c.CheckIsPassed = CheckIsPassed.Check_Start;
                        res &= await CheckOneItemAsync(port, item, callbackOutput);
                    }
                }

                checkstr = res ? "" : "不";
                callbackOutput?.Invoke(ResultInfoType.ResultInfo_Result, res, $"[{c.Description}], 检测{checkstr}通过\n", c.Depth);
                if (isEnable)
                {
                    c.CheckIsPassed = res ? CheckIsPassed.Check_Is_Passed : CheckIsPassed.Check_Not_passed;
                }
            }
            else
            {
                c.CheckIsPassed = CheckIsPassed.Check_Init;

                switch (c.CmdType)
                {
                    case CommandType.Cmd_Type_Invalid:
                        break;

                    case CommandType.Cmd_Type_Shell:
                        res = await CheckShellCmdAsync(port, c, callbackOutput);
                        break;

                    case CommandType.Cmd_Type_Mqtt:
                        res = await CheckMqttCmdAsync(port, c, callbackOutput);
                        break;

                    case CommandType.Cmd_Type_MaintainFrame:
                        break;

                    case CommandType.Cmd_Type_MaintainReadRealDataBase:
                        res = await ReadRealDatabaseAsync(port, c, callbackOutput);
                        break;

                    case CommandType.Cmd_Type_MaintainWriteRealDataBaseYK:
                        res = await CheckYXYKAsync(port, c, callbackOutput);
                        break;

                    case CommandType.Cmd_Type_SftpFileTransfer:
                        res = await CheckSftpFileTransferAsync(port, c, callbackOutput);
                        break;

                    case CommandType.Cmd_Type_DelaySomeTime:
                        res = true;
                        await Task.Delay(c.TimeOut);
                        break;

                    case CommandType.Cmd_Type_WindowsCommand:
                        res = await CheckWindowsCommandAsync(port, c, callbackOutput);
                        break;

                    case CommandType.Cmd_Type_Manual_Operate:
                        res = await CheckManualAsync(port, c, callbackOutput);
                        break;

                    case CommandType.Cmd_Type_ADC_Adjust:
                        res = await CheckADESet(port, c, callbackOutput);
                        break;

                    case CommandType.Cmd_Type_Console:
                        res = await CheckConsoleAsync(port, c, callbackOutput);
                        break;

                    default:
                        break;
                }

                checkstr = res ? "" : "不";
                callbackOutput?.Invoke(ResultInfoType.ResultInfo_Result, res, $"[{c.Description}], 检测{checkstr}通过\n", c.Depth);
                c.CheckIsPassed = res ? CheckIsPassed.Check_Is_Passed : CheckIsPassed.Check_Not_passed;
            }

            return res;
        }

        private static async Task<bool> ReadRealDatabaseAsync(Dictionary<PortUseTypeEnum, CommunicationPort> portDict, CheckItems c, Action<ResultInfoType, bool, string, int> callbackOutput)
        {
            bool testResult = true;
            try
            {
                byte[] b = MaintainProtocol.GetContinueRealDataBaseValue(JsonConvert.DeserializeObject<RealDatabaseCmdParameters>(c.CmdParam));
                MaintainParseRes res = null;

                CommunicationPort port = portDict[PortUseTypeEnum.Maintaince];

                for (int i = 0; i < 3 && res == null; i++)
                {
                    port.Write(b, 0, b.Length);
                    res = await port.ReadOneFrameAsync(c.TimeOut > 0 ? c.TimeOut : 3000);
                }

                if (res == null)
                {
                    return false;
                }

                callbackOutput?.Invoke(ResultInfoType.ResultInfo_Logs, true, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}: {MaintainProtocol.ByteArryToString(res.Frame, 0, res.Frame.Length)}\n", c.Depth);

                ContinueRealData data = MaintainProtocol.ParseContinueRealDataValue(res.Frame);

                if (data == null || !data.IsValid || data.RealDataArray == null || data.RealDataArray.Length <= 0)
                {
                    return false;
                }

                callbackOutput?.Invoke(ResultInfoType.ResultInfo_Logs, true, $"Values[{data.RealDataArray.Length}]: ", c.Depth);

                string valuesStr = "";
                foreach (var item in data.RealDataArray)
                {
                    switch (data.DataType)
                    {
                        case RealDataDataTypeEnum.Real_Data_type_Float:
                            valuesStr += $"{item.FloatValue}, ";
                            testResult &= await JudgeResultBySignAsync(item.FloatValue, c.ResultValue, c.ResultSign);
                            break;

                        case RealDataDataTypeEnum.Real_Data_type_Char:
                            valuesStr += $"{item.CharValue}, ";
                            testResult &= await JudgeResultBySignAsync(item.CharValue, c.ResultValue, c.ResultSign);
                            break;

                        case RealDataDataTypeEnum.Real_Data_type_Int:
                            valuesStr += $"{item.IntValue}, ";
                            testResult &= await JudgeResultBySignAsync(item.IntValue, c.ResultValue, c.ResultSign);
                            break;

                        case RealDataDataTypeEnum.Real_Data_type_Invalid:
                        default:
                            break;
                    }
                }

                valuesStr.TrimEnd().TrimEnd(',');
                valuesStr += "\n";
                callbackOutput?.Invoke(ResultInfoType.ResultInfo_Logs, testResult, valuesStr, 0);

                return testResult;
            }
            catch (Exception ex)
            {
                callbackOutput?.Invoke(ResultInfoType.ResultInfo_Exception, false, $"[{c.Description}]检测异常:{ex.Message}\n", c.Depth);

                return false;
            }
        }

        private static async Task<bool> CheckYXYKAsync(Dictionary<PortUseTypeEnum, CommunicationPort> portDict, CheckItems c, Action<ResultInfoType, bool, string, int> callbackOutput)
        {
            try
            {
                YKOperateParameters param = JsonConvert.DeserializeObject<YKOperateParameters>(c.CmdParam);

                byte[] b = null;
                switch (param.YKOperateType)
                {
                    case YKOperateTypeEnum.YK_Operate_Preset:
                        b = MaintainProtocol.GetYKPresetOn(param.YKNo);
                        break;

                    case YKOperateTypeEnum.YK_Operate_Actual:
                        b = MaintainProtocol.GetYKOnOff(param.YKNo, param.YKOperation);
                        break;

                    case YKOperateTypeEnum.YK_Operate_Cancel_Preset:
                        b = MaintainProtocol.GetYKPresetOff(param.YKNo);
                        break;

                    default:
                        break;
                }

                if (b == null)
                {
                    return false;
                }

                CommunicationPort port = portDict[PortUseTypeEnum.Maintaince];
                MaintainParseRes res = null;
                for (int i = 0; i < 3 && res == null; i++)
                {
                    port.Write(b, 0, b.Length);
                    res = await port.ReadOneFrameAsync(c.TimeOut > 0 ? c.TimeOut : 3000);
                }

                if (res == null)
                {
                    return false;
                }

                callbackOutput?.Invoke(ResultInfoType.ResultInfo_Logs, true, $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}: {MaintainProtocol.ByteArryToString(res.Frame, 0, res.Frame.Length)}\n", c.Depth);

                await Task.Delay(param.DelayTime);

                return MaintainProtocol.ParseYKResult(res.Frame) == Convert.ToByte(c.ResultValue);
            }
            catch (Exception ex)
            {
                callbackOutput?.Invoke(ResultInfoType.ResultInfo_Logs, true, $"异常: {ex.Message}\n", c.Depth);

                return false;
            }
        }

        public static async Task<bool> CheckShellCmdAsync(Dictionary<PortUseTypeEnum, CommunicationPort> port, CheckItems c, Action<ResultInfoType, bool, string, int> callbackOutput)
        {
            try
            {
                if (m_SshClass == null)
                {
                    m_SshClass = new SshClientClass(DataBaseLogical.GetTerminalIP(), DataBaseLogical.GetTerminalSSHPort(), DataBaseLogical.GetTerminalSSHUserName(), DataBaseLogical.GetTerminalSSHPasswd());
                }

                if (!m_SshClass.IsSshConnected)
                {
                    m_SshClass.ConnectToSshServer();
                }

                string res = m_SshClass.ExecCmd(c.CmdParam);
                await Task.Delay(c.TimeOut);
                callbackOutput?.Invoke(ResultInfoType.ResultInfo_Logs, true, $"执行结果: {res}\n", c.Depth);
                bool testres = await JudgeResultBySignAsync(res, c.ResultValue, c.ResultSign);
                m_SshClass.DisConnectSSH();
                return testres;
            }
            catch (Exception ex)
            {
                callbackOutput?.Invoke(ResultInfoType.ResultInfo_Exception, false, $"异常: {ex.Message}", c.Depth);

                return false;
            }
        }

        public static async Task<bool> CheckSftpFileTransferAsync(Dictionary<PortUseTypeEnum, CommunicationPort> port, CheckItems c, Action<ResultInfoType, bool, string, int> callbackOutput)
        {
            try
            {
                if (m_SshClass == null)
                {
                    m_SshClass = new SshClientClass(DataBaseLogical.GetTerminalIP(), DataBaseLogical.GetTerminalSSHPort(), DataBaseLogical.GetTerminalSSHUserName(), DataBaseLogical.GetTerminalSSHPasswd());
                }

                if (!m_SshClass.IsSftpConnected)
                {
                    m_SshClass.ConnectToSftpServer();
                }

                SftpFileTransferParameters param = JsonConvert.DeserializeObject<SftpFileTransferParameters>(c.CmdParam);

                if (param == null)
                {
                    callbackOutput?.Invoke(ResultInfoType.ResultInfo_Exception, false, $"文件传输的配置错误, 请联系开发人员!", c.Depth);
                    return false;
                }

                if (param.IsUploadFileToTerminal)
                {
                    if (string.IsNullOrEmpty(DataBaseLogical.GetUploadDirectory()))
                    {
                        callbackOutput?.Invoke(ResultInfoType.ResultInfo_Exception, false, $"上传目录未配置, 请联系开发人员!", c.Depth);
                        return false;
                    }

                    string dir = DataBaseLogical.GetUploadDirectory();
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                        callbackOutput?.Invoke(ResultInfoType.ResultInfo_Exception, false, $"目录: {dir} 不存在!", c.Depth);

                        return false;
                    }

                    if (!File.Exists(param.FullFileNameComputer))
                    {
                        callbackOutput?.Invoke(ResultInfoType.ResultInfo_Exception, false, $"文件: {param.FullFileNameComputer} 不存在!", c.Depth);

                        return false;
                    }

                    await m_SshClass.UploadFileToTerminalAsync(param.FullFileNameComputer, param.FullFileNameTerminal);
                    string cMd5 = CommonClass.GetComputerFileMd5(param.FullFileNameComputer).ToLower();
                    string tMd5 = m_SshClass.GetSshMd5(param.FullFileNameTerminal).ToLower();

                    callbackOutput?.Invoke(ResultInfoType.ResultInfo_Logs, true, $"computer md5: {cMd5}, terminal md5: {tMd5}\n", c.Depth);

                    return cMd5 == tMd5;
                }
                else
                {
                    if (string.IsNullOrEmpty(DataBaseLogical.GetDownloadDirectory()))
                    {
                        return false;
                    }

                    string dir = DataBaseLogical.GetDownloadDirectory();
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                        callbackOutput?.Invoke(ResultInfoType.ResultInfo_Exception, false, $"目录: {dir} 不存在!", c.Depth);

                        return false;
                    }

                    await m_SshClass.DownLoadFileFromTerminalAsync(param.FullFileNameComputer, param.FullFileNameTerminal);
                    string cMd5 = CommonClass.GetComputerFileMd5(param.FullFileNameComputer).ToLower();
                    string tMd5 = m_SshClass.GetSshMd5(param.FullFileNameTerminal).ToLower();

                    callbackOutput?.Invoke(ResultInfoType.ResultInfo_Logs, true, $"computer md5: {cMd5}, terminal md5: {tMd5}", c.Depth);

                    return cMd5 == tMd5;
                }
            }
            catch (Exception ex)
            {
                callbackOutput?.Invoke(ResultInfoType.ResultInfo_Exception, false, $"异常: {ex.Message}", c.Depth);

                return false;
            }
        }

        public static async Task MqttMessageArrivedAsync(MqttApplicationMessageReceivedEventArgs e)
        {
            string topic = e.ApplicationMessage.Topic;
            string msg = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment.Array);

            m_MsgHandle.AddSRMsg(SRMsgType.报文说明, $"topic: {topic}, message: {msg}");
            await Task.Delay(50);
        }

        public static async Task<bool> CheckMqttCmdAsync(Dictionary<PortUseTypeEnum, CommunicationPort> port, CheckItems c, Action<ResultInfoType, bool, string, int> callbackOutput)
        {
            try
            {
                if (m_MqttHelper == null)
                {
                    m_MqttHelper = new MqttHelper(DataBaseLogical.GetTerminalIP(), DataBaseLogical.GetMqttPort());
                }

                if (!m_MqttHelper.IsConnected)
                {
                    m_MqttHelper.ConnectAndSubscribe(DataBaseLogical.GetMqttTopicList(), MqttMessageArrivedAsync);
                }

                MqttPublishParameters para = JsonConvert.DeserializeObject<MqttPublishParameters>(c.CmdParam);
                m_MqttHelper.PublishMessage(para.Topic, para.Message);

                await Task.Delay(c.TimeOut);

                return true;
            }
            catch (Exception ex)
            {
                callbackOutput?.Invoke(ResultInfoType.ResultInfo_Exception, false, $"异常: {ex.Message}", c.Depth);

                return false;
            }
        }

        public static async Task<bool> CheckWindowsCommandAsync(Dictionary<PortUseTypeEnum, CommunicationPort> port, CheckItems c, Action<ResultInfoType, bool, string, int> callbackOutput)
        {
            try
            {
                string res = await Task.Run(
                async () =>
                {
                    string cmdres = CommonClass.ExecDosCmd(c.CmdParam);
                    await Task.Delay(c.TimeOut);
                    return cmdres;
                });

                callbackOutput?.Invoke(ResultInfoType.ResultInfo_Logs, true, $"command: {c.CmdParam}, result: {res}\n", c.Depth);

                return await JudgeResultBySignAsync(res, c.ResultValue, c.ResultSign);
            }
            catch (Exception ex)
            {
                callbackOutput?.Invoke(ResultInfoType.ResultInfo_Exception, false, $"异常: {ex.Message}", c.Depth);

                return false;
            }
        }

        public static async Task<bool> CheckConsoleAsync(Dictionary<PortUseTypeEnum, CommunicationPort> portDict, CheckItems c, Action<ResultInfoType, bool, string, int> callbackOutput)
        {
            try
            {
                CommunicationPort port = portDict[PortUseTypeEnum.Console];

                callbackOutput?.Invoke(ResultInfoType.ResultInfo_Logs, true, c.CmdParam, c.Depth);
                port.Clear();
                if (!port.Write(c.CmdParam))
                {
                    return false;
                }

                await Task.Delay(c.TimeOut);

                byte[] b = port.Read();
                if (b == null)
                {
                    return false;
                }

                port.Clear();

                callbackOutput?.Invoke(ResultInfoType.ResultInfo_Logs, true, Encoding.UTF8.GetString(b), c.Depth);

                return await JudgeResultBySignAsync(Encoding.UTF8.GetString(b), c.ResultValue, c.ResultSign);
            }
            catch (Exception ex)
            {
                callbackOutput?.Invoke(ResultInfoType.ResultInfo_Exception, false, $"异常: {ex.Message}", c.Depth);

                return false;
            }
        }

        public static async Task<bool> CheckManualAsync(Dictionary<PortUseTypeEnum, CommunicationPort> portDict, CheckItems c, Action<ResultInfoType, bool, string, int> callbackOutput)
        {
            try
            {
                return await Task.Run(async () =>
                {
                    await Task.Delay(10);//消除编译警告
                    return MessageBox.Show(c.CmdParam, "检测确认", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
                });
            }
            catch (Exception ex)
            {
                callbackOutput?.Invoke(ResultInfoType.ResultInfo_Exception, false, $"异常: {ex.Message}", c.Depth);

                return false;
            }
        }

        public static async Task<bool> CheckADESet(Dictionary<PortUseTypeEnum, CommunicationPort> portDict, CheckItems c, Action<ResultInfoType, bool, string, int> callbackOutput)
        {
            try
            {
                bool res = false;
                await Task.Delay(10);//消除编译警告
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ADE9078Set a = new ADE9078Set(portDict[PortUseTypeEnum.Maintaince].IPort, JsonConvert.DeserializeObject<MultiRouteADEError>(c.CmdParam));
                    a.CheckResultEvent += e => { res = e; };
                    a.ShowDialog();
                });

                return res;
            }
            catch (Exception ex)
            {
                callbackOutput?.Invoke(ResultInfoType.ResultInfo_Exception, false, $"异常: {ex.Message}", c.Depth);

                return false;
            }
        }
    }
}