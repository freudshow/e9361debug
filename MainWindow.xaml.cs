using E9361App.Communication;
using E9361App.Maintain;
using System.Windows;
using System.Threading.Tasks;
using System;
using E9361App.MsgBox;
using E9361App.Common;

namespace E9361DEBUG
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private CommunicationPort m_CommunicationPort;

        public MainWindow()
        {
            InitializeComponent();

            NetPara udpclientpara = new NetPara { ServerIP = "192.168.0.237", ServerPort = 5000, Mode = PortTypeEnum.PortType_Net_UDP_Client };
            NetPara tcpclientpara = new NetPara { ServerIP = "192.168.1.232", ServerPort = 5001, Mode = PortTypeEnum.PortType_Net_TCP_Client };
            UartPortPara uartPortPara = new UartPortPara { PortName = "COM3" };

            m_CommunicationPort = new CommunicationPort(PortTypeEnum.PortType_Serial);
            m_CommunicationPort.Open(uartPortPara);
        }

        private async void Button_ReadTime_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MaintainProtocol.GetTerminalTime(out byte[] writeFrame);
                m_CommunicationPort.Write(writeFrame, 0, writeFrame.Length);
                Task<MaintainParseRes> res = m_CommunicationPort.ReadOneFrameAsync(5000);

                var frame = await res;
                if (frame != null)
                {
                    byte[] b = frame.Frame;
                    TextBox_Result.Text += $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}: {MaintainProtocol.ByteArryToString(b, 0, b.Length)}\r\n";
                }
                else
                {
                    ShowMsg.ShowMessageBoxTimeout("超时!", "温馨提示", MessageBoxButton.OK, 1000);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Menu_About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"E9361-C检测软件\n软件版本:{Common.Version}");
        }

        private void RestartButton_OnClick(object sender, RoutedEventArgs e)
        {
        }
    }
}