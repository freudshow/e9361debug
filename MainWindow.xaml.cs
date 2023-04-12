using E9361App.Communication;
using E9361App.DBHelper;
using E9361App.Log;
using E9361App.Maintain;
using System.Threading;
using System.Windows;
using System.Threading.Tasks;
using System;

namespace e9361debug
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
                Task<MaintainParseRes> res = m_CommunicationPort.ReadOneFrame(5000);

                var frame = await res;
                if (frame != null)
                {
                    byte[] b = frame.Frame;
                    TextBox_Result.Text += $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}: {MaintainProtocol.ByteArryToString(b, 0, b.Length)}\r\n";
                }
                else
                {
                    MessageBox.Show("超时!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}