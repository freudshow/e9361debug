using E9361App.Communication;
using E9361App.DBHelper;
using E9361App.Log;
using E9361App.Maintain;
using System.Data;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

            byte[] writeFrame = new byte[] { 0xAA, 0xFF, 0xFF, 0x03, 0x00, 0x0A, 0x01, 0x00, 0x08 };

            NetPara para = new NetPara { ServerIP = "192.168.0.231", ServerPort = 5000, Mode = NetMode.UdpClientMode };
            m_UdpPort.Open(para);
            m_UdpPort.MaintainResHander += new MaintainResEventHander(MaintainResHander);
            int count = 0;
            while (count < 3)
            {
                m_UdpPort.Write(writeFrame, 0, writeFrame.Length);
                count++;
                Thread.Sleep(3000);
            }

            m_UdpPort.Close();
        }

        public void MaintainResHander(object sender, MaintainResEventArgs e)
        {
            MessageBox.Show(MaintainProtocol.ByteArryToString(e.m_Res.Frame, 0, e.m_Res.Frame.Length));
        }
    }
}