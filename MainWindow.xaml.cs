using E9361App.Communication;
using E9361App.DBHelper;
using E9361App.Log;
using E9361App.Maintain;
using System.Threading;
using System.Windows;
using System.Threading.Tasks;

namespace e9361debug
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private UartPort m_UartPort = new UartPort();
        private TcpClientNetPort m_TcpClientNetPort = new TcpClientNetPort();
        private UdpPort m_UdpPort = new UdpPort();

        public MainWindow()
        {
            InitializeComponent();

            NetPara para = new NetPara { ServerIP = "192.168.0.237", ServerPort = 5000, Mode = NetMode.UdpClientMode };
            m_UdpPort.Open(para);
            m_UdpPort.MaintainResHander += new MaintainResEventHander(MaintainResHander);
        }

        public void MaintainResHander(object sender, MaintainResEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new ThreadStart(delegate ()
            {
                TextBox_Result.Text += $"{MaintainProtocol.ByteArryToString(e.m_Res.Frame, 0, e.m_Res.Frame.Length)}\r\n";
            }));
        }

        private async void Button_ReadTime_Click(object sender, RoutedEventArgs e)
        {
            //MaintainProtocol.GetTerminalTime(out byte[] writeFrame);
            //m_UdpPort.Write(writeFrame, 0, writeFrame.Length);

            var t = Task.Run(() =>
            {
                Thread.Sleep(1000);
                return "Hello I am TimeConsumingMethod";
            });

            TextBox_Result.Text += $"{await t}\r\n";
        }
    }
}