using E9361App.Communication;
using E9361App.Maintain;
using System.Windows;
using System.Threading.Tasks;
using System;
using E9361App.MsgBox;
using E9361App.Common;
using E9361Debug.Logical;
using System.Web.Hosting;
using System.Windows.Media;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

namespace E9361DEBUG
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private CommunicationPort m_CommunicationPort;
        private CheckItems m_CheckItems;

        public MainWindow()
        {
            InitializeComponent();

            InitCheckTree();
            OpenTerminalMaintain();
        }

        /// <summary>
        /// 通过网络打开维护规约通道
        /// </summary>
        private void OpenTerminalMaintain()
        {
            PortTypeEnum e = DataBaseLogical.GetMaintainPortType();
            switch (e)
            {
                case PortTypeEnum.PortType_Error:
                    break;

                case PortTypeEnum.PortType_Serial:
                    break;

                case PortTypeEnum.PortType_Net_UDP_Client:
                    m_CommunicationPort = new CommunicationPort(PortTypeEnum.PortType_Net_UDP_Client);

                    NetPara udpclientpara = new NetPara { ServerIP = DataBaseLogical.GetTerminalIP(), ServerPort = DataBaseLogical.GetTerminalUDPPort(), Mode = PortTypeEnum.PortType_Net_UDP_Client };
                    m_CommunicationPort.Open(udpclientpara);
                    break;

                case PortTypeEnum.PortType_Net_TCP_Client:
                    m_CommunicationPort = new CommunicationPort(PortTypeEnum.PortType_Net_TCP_Client);
                    NetPara tcpclientpara = new NetPara { ServerIP = DataBaseLogical.GetTerminalIP(), ServerPort = DataBaseLogical.GetTerminalTCPClientPort(), Mode = PortTypeEnum.PortType_Net_TCP_Client };
                    m_CommunicationPort.Open(tcpclientpara);
                    break;

                case PortTypeEnum.PortType_Net_TCP_Server:
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// 打开终端的Console口
        /// </summary>
        private void OpenConsole()
        {
            UartPortPara uartPortPara = new UartPortPara { PortName = "COM3" };
        }

        private void InitCheckTree()
        {
            m_CheckItems = new CheckItems { ChildTableName = DataBaseLogical.GetBaseCheckTableName() };
            m_CheckItems.IsEnable = true;
            m_CheckItems.Description = "总体检测";
            Controls_CheckTree.SetDataSource(m_CheckItems);
        }

        private async Task ReadTime()
        {
            try
            {
                byte[] b = MaintainProtocol.GetTerminalTime();
                m_CommunicationPort.Write(b, 0, b.Length);
                MaintainParseRes res = await m_CommunicationPort.ReadOneFrameAsync(5000);

                if (res != null)
                {
                    TextBox_Result.Text += $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}: {MaintainProtocol.ByteArryToString(res.Frame, 0, res.Frame.Length)}, {MaintainProtocol.ParseTerminalTime(res.Frame).ToString("yyyy-MM-dd HH:mm:ss")}\n";
                }
                else
                {
                    TextBox_Exception.Foreground = new SolidColorBrush(Colors.Red);
                    TextBox_Exception.Text += "连接超时\n";
                }
            }
            catch (Exception ex)
            {
                TextBox_Exception.Text += $"异常:{ex.Message}\n";
            }
        }

        private async Task<bool> ReadRealDatabase(CheckItems c)
        {
            bool testResult = true;
            try
            {
                //317,1,0,1: 317 - 实时库号,
                string[] args = c.CmdParam.Split(',');

                ushort startIdx = Convert.ToUInt16(args[0]);
                RealDataTeleTypeEnum teleTypeEnum = (RealDataTeleTypeEnum)Convert.ToUInt32(args[1]);
                RealDataDataTypeEnum dataType = (RealDataDataTypeEnum)Convert.ToUInt32(args[2]);
                byte regCount = (byte)Convert.ToInt32(args[3]);

                byte[] b = MaintainProtocol.GetContinueRealDataBaseValue(startIdx, teleTypeEnum, dataType, regCount);
                m_CommunicationPort.Write(b, 0, b.Length);
                MaintainParseRes res = await m_CommunicationPort.ReadOneFrameAsync(c.TimeOut > 0 ? c.TimeOut : 3000);

                if (res == null)
                {
                    return false;
                }

                byte[] f = res.Frame;
                TextBox_Result.Text += $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}: {MaintainProtocol.ByteArryToString(f, 0, f.Length)}\n";
                ContinueRealData data = MaintainProtocol.ParseContinueRealDataValue(f);

                if (!data.IsValid || data.RealDataArray == null || data.RealDataArray.Length <= 0)
                {
                    return false;
                }

                TextBox_Result.Text += $"Values[{data.RealDataArray.Length}]: ";

                foreach (var item in data.RealDataArray)
                {
                    switch (data.DataType)
                    {
                        case RealDataDataTypeEnum.Real_Data_type_Float:
                            TextBox_Result.Text += $"{item.FloatValue}";
                            switch (c.ResultSign)
                            {
                                case ResultSignEnum.Result_Sign_Equal://相等
                                    testResult &= (item.FloatValue == Convert.ToSingle(c.ResultValue));
                                    break;

                                case ResultSignEnum.Result_Sign_Greater_Than://大于等于
                                    testResult &= (item.FloatValue >= Convert.ToSingle(c.ResultValue));
                                    break;

                                case ResultSignEnum.Result_Sign_Less_Than://小于等于
                                    testResult &= (item.FloatValue <= Convert.ToSingle(c.ResultValue));
                                    break;

                                case ResultSignEnum.Result_Sign_Interval://区间
                                    string[] bounds = c.ResultValue.Split(',');
                                    float lower = Convert.ToSingle(bounds[0]);
                                    float upper = Convert.ToSingle(bounds[1]);
                                    testResult &= (item.FloatValue >= lower && item.FloatValue <= upper);
                                    break;

                                case ResultSignEnum.Result_Sign_Regex://正则表达式
                                    Regex re = new Regex(c.ResultValue, RegexOptions.Compiled);
                                    break;

                                case ResultSignEnum.Result_Sign_Lambda://lambda表达式
                                    Func<float, bool> resultEvaluate = await Common.GetLambdaAsync<float>(c.ResultValue);
                                    testResult &= resultEvaluate(item.FloatValue);
                                    break;

                                case ResultSignEnum.Result_Sign_Invalid://无效
                                default:
                                    testResult &= false;
                                    break;
                            }
                            break;

                        case RealDataDataTypeEnum.Real_Data_type_Char:
                            TextBox_Result.Text += $"{item.CharValue}";
                            switch (c.ResultSign)
                            {
                                case ResultSignEnum.Result_Sign_Equal://相等
                                    testResult &= (item.CharValue == Convert.ToSByte(c.ResultValue));
                                    break;

                                case ResultSignEnum.Result_Sign_Greater_Than://大于等于
                                    testResult &= (item.CharValue >= Convert.ToSByte(c.ResultValue));
                                    break;

                                case ResultSignEnum.Result_Sign_Less_Than://小于等于
                                    testResult &= (item.CharValue <= Convert.ToSByte(c.ResultValue));
                                    break;

                                case ResultSignEnum.Result_Sign_Interval://区间
                                    string[] bounds = c.ResultValue.Split(',');
                                    sbyte lower = Convert.ToSByte(bounds[0]);
                                    sbyte upper = Convert.ToSByte(bounds[1]);
                                    testResult &= (item.CharValue >= lower && item.IntValue <= upper);
                                    break;

                                case ResultSignEnum.Result_Sign_Regex://正则表达式
                                    Regex re = new Regex(c.ResultValue, RegexOptions.Compiled);
                                    break;

                                case ResultSignEnum.Result_Sign_Lambda://lambda表达式
                                    Func<sbyte, bool> resultEvaluate = await Common.GetLambdaAsync<sbyte>(c.ResultValue);
                                    testResult &= resultEvaluate(item.CharValue);
                                    break;

                                case ResultSignEnum.Result_Sign_Invalid://无效
                                default:
                                    testResult &= false;
                                    break;
                            }
                            break;

                        case RealDataDataTypeEnum.Real_Data_type_Int:
                            TextBox_Result.Text += $"{item.IntValue}";
                            switch (c.ResultSign)
                            {
                                case ResultSignEnum.Result_Sign_Equal://相等
                                    testResult &= (item.IntValue == Convert.ToInt32(c.ResultValue));
                                    break;

                                case ResultSignEnum.Result_Sign_Greater_Than://大于等于
                                    testResult &= (item.IntValue >= Convert.ToInt32(c.ResultValue));
                                    break;

                                case ResultSignEnum.Result_Sign_Less_Than://小于等于
                                    testResult &= (item.IntValue <= Convert.ToInt32(c.ResultValue));
                                    break;

                                case ResultSignEnum.Result_Sign_Interval://区间
                                    string[] bounds = c.ResultValue.Split(',');
                                    int lower = Convert.ToInt32(bounds[0]);
                                    int upper = Convert.ToInt32(bounds[1]);
                                    testResult &= (item.IntValue >= lower && item.IntValue <= upper);
                                    break;

                                case ResultSignEnum.Result_Sign_Regex://正则表达式
                                    Regex re = new Regex(c.ResultValue, RegexOptions.Compiled);
                                    break;

                                case ResultSignEnum.Result_Sign_Lambda://lambda表达式
                                    Func<int, bool> resultEvaluate = await Common.GetLambdaAsync<int>(c.ResultValue);
                                    testResult &= resultEvaluate(item.IntValue);
                                    break;

                                case ResultSignEnum.Result_Sign_Invalid://无效
                                default:
                                    testResult &= false;
                                    break;
                            }
                            break;

                        case RealDataDataTypeEnum.Real_Data_type_Invalid:
                        default:
                            break;
                    }

                    TextBox_Result.Text += ", ";
                }

                TextBox_Result.Text.TrimEnd().TrimEnd(',');
                TextBox_Result.Text += "\n";

                return testResult;
            }
            catch (Exception ex)
            {
                TextBox_Exception.Foreground = new SolidColorBrush(Colors.Red);
                TextBox_Exception.Text += $"异常:{ex.Message}\n";
                return false;
            }
        }

        private void Menu_About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"E9361-C检测软件\n软件版本:{Common.Version}");
        }

        private async void Button_StartDebug_Click(object sender, RoutedEventArgs e)
        {
            await CheckAllItems();
        }

        private async Task CheckAllItems()
        {
            _ = await CheckOneItem(m_CheckItems);
        }

        private async Task<bool> CheckOneItem(CheckItems c)
        {
            bool res = true;

            if (c.Children != null)
            {
                foreach (CheckItems item in c.Children)
                {
                    res &= await CheckOneItem(item);
                }

                if (res)
                {
                    TextBox_Result.Text += $"[{c.Description}], 检测通过\n";
                }
                else
                {
                    TextBox_Exception.Text += $"[{c.Description}], 检测不通过\n";
                }
            }
            else
            {
                if (c == null || !c.IsEnable)
                {
                    return true;
                }

                switch (c.CmdType)
                {
                    case CommandType.Cmd_Type_Invalid:
                        break;

                    case CommandType.Cmd_Type_Mqtt:
                        break;

                    case CommandType.Cmd_Type_MaintainFrame:
                        break;

                    case CommandType.Cmd_Type_Shell:
                        break;

                    case CommandType.Cmd_Type_MaintainReadRealDataBase:
                        res = await ReadRealDatabase(c);
                        break;

                    default:
                        break;
                }

                if (res)
                {
                    TextBox_Result.Text += $"[{c.Description}], 检测通过\n";
                }
                else
                {
                    TextBox_Exception.Text += $"[{c.Description}], 检测不通过\n";
                }
            }

            return res;
        }

        private void TextBox_Result_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TextBox_Result.ScrollToEnd();
        }

        private void TextBox_Exception_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TextBox_Exception.ScrollToEnd();
        }
    }
}