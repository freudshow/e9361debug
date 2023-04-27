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

            NetPara udpclientpara = new NetPara { ServerIP = "192.168.0.237", ServerPort = 5000, Mode = PortTypeEnum.PortType_Net_UDP_Client };
            NetPara tcpclientpara = new NetPara { ServerIP = "192.168.1.232", ServerPort = 5001, Mode = PortTypeEnum.PortType_Net_TCP_Client };
            UartPortPara uartPortPara = new UartPortPara { PortName = "COM3" };

            m_CommunicationPort = new CommunicationPort(PortTypeEnum.PortType_Net_UDP_Client);
            m_CommunicationPort.Open(udpclientpara);
            InitCheckTree();
        }

        private void InitCheckTree()
        {
            m_CheckItems = new CheckItems { ChildTableName = DataBaseLogical.GetBaseCheckTableName() };
            Controls_CheckTree.SetDataSource(m_CheckItems);
        }

        private async void ReadTime()
        {
            try
            {
                MaintainProtocol.GetTerminalTime(out byte[] writeFrame);
                m_CommunicationPort.Write(writeFrame, 0, writeFrame.Length);
                MaintainParseRes res = await m_CommunicationPort.ReadOneFrameAsync(5000);

                if (res != null)
                {
                    byte[] b = res.Frame;
                    TextBox_Result.Text += $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}: {MaintainProtocol.ByteArryToString(b, 0, b.Length)}, {MaintainProtocol.ParseTerminalTime(res.Frame).ToString("yyyy-MM-dd HH:mm:ss")}\n";
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

        private async Task ReadRealDatabase()
        {
            try
            {
                MaintainProtocol.GetContinueRealDataBaseValue(317, RealDataTeleTypeEnum.Real_Data_TeleType_YC, RealDataDataTypeEnum.Real_Data_type_Float, 5, out byte[] b);
                m_CommunicationPort.Write(b, 0, b.Length);
                MaintainParseRes res = await m_CommunicationPort.ReadOneFrameAsync(5000);

                if (res != null)
                {
                    byte[] f = res.Frame;
                    TextBox_Result.Text += $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}: {MaintainProtocol.ByteArryToString(f, 0, f.Length)}\n";
                    ContinueRealData data = MaintainProtocol.ParseContinueRealDataValue(f);

                    if (data.IsValid && data.RealDataArray != null && data.RealDataArray.Length > 0)
                    {
                        TextBox_Result.Text += $"Values[{data.RealDataArray.Length}]: ";
                        foreach (var item in data.RealDataArray)
                        {
                            switch (data.DataType)
                            {
                                case RealDataDataTypeEnum.Real_Data_type_Float:
                                    TextBox_Result.Text += $"{item.FloatValue}";
                                    break;

                                case RealDataDataTypeEnum.Real_Data_type_Char:
                                    TextBox_Result.Text += $"{item.CharValue}";
                                    break;

                                case RealDataDataTypeEnum.Real_Data_type_Int:
                                    TextBox_Result.Text += $"{item.IntValue}";
                                    break;

                                case RealDataDataTypeEnum.Real_Data_type_Invalid:
                                default:
                                    break;
                            }

                            TextBox_Result.Text += ", ";
                        }

                        TextBox_Result.Text.TrimEnd().TrimEnd(',');
                        TextBox_Result.Text += "\n";
                    }
                }
                else
                {
                    TextBox_Exception.Foreground = new SolidColorBrush(Colors.Red);
                    TextBox_Exception.Text += "连接超时\n";
                }
            }
            catch (Exception ex)
            {
                TextBox_Exception.Foreground = new SolidColorBrush(Colors.Red);
                TextBox_Exception.Text += $"异常:{ex.Message}\n";
            }

            TextBox_Result.ScrollToEnd();
            TextBox_Exception.ScrollToEnd();
        }

        private void Menu_About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"E9361-C检测软件\n软件版本:{Common.Version}");
        }

        private async void Button_StartDebug_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 100; i++)
            {
                await ReadRealDatabase();
            }
        }

        private async void CheckAllItems()
        {
            bool childres = await checkOneItem(m_CheckItems);
        }

        private async Task<bool> checkOneItem(CheckItems c)
        {
            bool res = false;
            if (c == null || !c.IsEnable)
            {
                return res;
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
                    break;

                default:
                    break;
            }

            if (!res)
            {
                await Console.Out.WriteLineAsync($"[{c.Description}], 检测不通过");
            }

            if (c.Children != null)
            {
                foreach (var item in c.Children)
                {
                    bool childres = await checkOneItem(item);
                    if (!childres)
                    {
                        await Console.Out.WriteLineAsync($"[{item.Description}], 检测不通过");
                    }
                }
            }

            return res;
        }
    }
}