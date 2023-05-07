using E9361Debug.Common;
using E9361Debug.Communication;
using E9361Debug.logical;
using E9361Debug.Maintain;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using E9361Debug.SshInterface;
using MQTTnet.Client;
using E9361Debug.Mqtt;
using System.Collections.Generic;

namespace E9361Debug.Logical
{
    public enum CommandType
    {
        Cmd_Type_Invalid = -1,
        Cmd_Type_MaintainFrame = 0,
        Cmd_Type_Shell,
        Cmd_Type_Mqtt,
        Cmd_Type_MaintainReadRealDataBase,
        Cmd_MaintainWriteRealDataBaseYK,
        Cmd_SftpFileTransfer,
        Cmd_DelaySomeTime
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
                if (callbackOutput != null)
                {
                    if (res != null)
                    {
                        callbackOutput(ResultInfoType.ResultInfo_Logs, true, $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}: {MaintainProtocol.ByteArryToString(res.Frame, 0, res.Frame.Length)}, {MaintainProtocol.ParseTerminalTime(res.Frame).ToString("yyyy-MM-dd HH:mm:ss")}\n");
                    }
                    else
                    {
                        callbackOutput(ResultInfoType.ResultInfo_Result, false, "连接超时\n");
                    }
                }
            }
            catch (Exception ex)
            {
                if (callbackOutput != null)
                {
                    callbackOutput(ResultInfoType.ResultInfo_Exception, false, $"异常:{ex.Message}\n");
                }
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
                    Func<S, bool> equalEvaluate = await CommonClass.GetLambdaAsync<S>($"(x)=>x=={target}");
                    testResult = equalEvaluate(result);
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

        public static async Task<bool> CheckOneItemAsync(CommunicationPort port, CheckItems c, Action<ResultInfoType, bool, string, int> callbackOutput)
        {
            bool res = true;
            if (c == null || !c.IsEnable)
            {
                return true;
            }

            if (callbackOutput != null)
            {
                callbackOutput(ResultInfoType.ResultInfo_Logs, res, $"[{c.Description}], 检测开始\n", c.Depth);
            }

            string checkstr = "";
            if (c.Children != null)
            {
                foreach (CheckItems item in c.Children)
                {
                    res &= await CheckOneItemAsync(port, item, callbackOutput);
                }

                if (callbackOutput != null)
                {
                    checkstr = res ? "" : "不";
                    callbackOutput(ResultInfoType.ResultInfo_Result, res, $"[{c.Description}], 检测{checkstr}通过\n", c.Depth);
                }
            }
            else
            {
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

                    case CommandType.Cmd_MaintainWriteRealDataBaseYK:
                        res = await CheckYXYKAsync(port, c, callbackOutput);
                        break;

                    case CommandType.Cmd_SftpFileTransfer:
                        res = await CheckSftpFileTransferAsync(port, c, callbackOutput);
                        break;

                    case CommandType.Cmd_DelaySomeTime:
                        res = true;
                        await Task.Delay(c.TimeOut);
                        break;

                    default:
                        break;
                }

                if (callbackOutput != null)
                {
                    checkstr = res ? "" : "不";
                    callbackOutput(ResultInfoType.ResultInfo_Result, res, $"[{c.Description}], 检测{checkstr}通过\n", c.Depth);
                }
            }

            return res;
        }

        private static async Task<bool> ReadRealDatabaseAsync(CommunicationPort port, CheckItems c, Action<ResultInfoType, bool, string, int> callbackOutput)
        {
            bool testResult = true;
            try
            {
                byte[] b = MaintainProtocol.GetContinueRealDataBaseValue(JsonConvert.DeserializeObject<RealDatabaseCmdParameters>(c.CmdParam));
                port.Write(b, 0, b.Length);
                MaintainParseRes res = await port.ReadOneFrameAsync(c.TimeOut > 0 ? c.TimeOut : 3000);

                if (res == null)
                {
                    return false;
                }

                if (callbackOutput != null)
                {
                    callbackOutput(ResultInfoType.ResultInfo_Logs, true, $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}: {MaintainProtocol.ByteArryToString(res.Frame, 0, res.Frame.Length)}\n", c.Depth);
                }

                ContinueRealData data = MaintainProtocol.ParseContinueRealDataValue(res.Frame);

                if (!data.IsValid || data.RealDataArray == null || data.RealDataArray.Length <= 0)
                {
                    return false;
                }

                if (callbackOutput != null)
                {
                    callbackOutput(ResultInfoType.ResultInfo_Logs, true, $"Values[{data.RealDataArray.Length}]: ", c.Depth);
                }

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
                if (callbackOutput != null)
                {
                    callbackOutput(ResultInfoType.ResultInfo_Logs, testResult, valuesStr, 0);
                }

                return testResult;
            }
            catch (Exception ex)
            {
                if (callbackOutput != null)
                {
                    callbackOutput(ResultInfoType.ResultInfo_Exception, false, $"[{c.Description}]检测异常:{ex.Message}\n", c.Depth);
                }

                return false;
            }
        }

        private static async Task<bool> CheckYXYKAsync(CommunicationPort port, CheckItems c, Action<ResultInfoType, bool, string, int> callbackOutput)
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

            port.Write(b, 0, b.Length);
            MaintainParseRes res = await port.ReadOneFrameAsync(c.TimeOut > 0 ? c.TimeOut : 3000);

            if (res == null)
            {
                return false;
            }

            if (callbackOutput != null)
            {
                callbackOutput(ResultInfoType.ResultInfo_Logs, true, $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}: {MaintainProtocol.ByteArryToString(res.Frame, 0, res.Frame.Length)}\n", c.Depth);
            }

            await Task.Delay(param.DelayTime);

            return MaintainProtocol.ParseYKResult(res.Frame) == Convert.ToByte(c.ResultValue);
        }

        public static async Task<bool> CheckShellCmdAsync(CommunicationPort port, CheckItems c, Action<ResultInfoType, bool, string, int> callbackOutput)
        {
            if (m_SshClass == null)
            {
                m_SshClass = new SshClientClass(DataBaseLogical.GetTerminalIP(), DataBaseLogical.GetTerminalSSHPort(), DataBaseLogical.GetTerminalSSHUserName(), DataBaseLogical.GetTerminalSSHPasswd());
            }

            if (!m_SshClass.SSHConnected)
            {
                m_SshClass.ConnectToSshServer();
            }

            string res = m_SshClass.ExecCmd(c.CmdParam);
            bool testres = await JudgeResultBySignAsync(res, c.ResultValue, c.ResultSign);

            return testres;
        }

        public static async Task<bool> CheckSftpFileTransferAsync(CommunicationPort port, CheckItems c, Action<ResultInfoType, bool, string, int> callbackOutput)
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

            if (param.IsUploadFileToTerminal)
            {
                await m_SshClass.UploadFileToTerminalAsync(param.FullFileNameComputer, param.FullFileNameTerminal);
            }
            else
            {
                await m_SshClass.DownLoadFileFromTerminalAsync(param.FullFileNameComputer, param.FullFileNameTerminal);
            }

            return true;
        }

        public static async Task MqttMessageArrivedAsync(MqttApplicationMessageReceivedEventArgs e)
        {
        }

        public static async Task<bool> CheckMqttCmdAsync(CommunicationPort port, CheckItems c, Action<ResultInfoType, bool, string, int> callbackOutput)
        {
            if (m_MqttHelper == null)
            {
                m_MqttHelper = new MqttHelper(DataBaseLogical.GetTerminalIP(), DataBaseLogical.GetMqttPort());
            }

            if (!m_MqttHelper.IsConnected)
            {
                await m_MqttHelper.ConnectAndSubscribeAsync(DataBaseLogical.GetMqttTopicList(), MqttMessageArrivedAsync);
            }

            MqttPublishParameters para = JsonConvert.DeserializeObject<MqttPublishParameters>(c.CmdParam);
            m_MqttHelper.PublishMessage(para.Topic, para.Message);

            await Task.Delay(c.TimeOut * 1000);

            return true;
        }
    }
}