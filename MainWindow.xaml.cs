using E9361App.Communication;
using E9361App.DBHelper;
using E9361App.Maintain;
using System.Data;
using System.Threading;
using System.Windows;

namespace e9361debug
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private UartPort m_UartPort = new UartPort();

        public MainWindow()
        {
            InitializeComponent();

            byte[] writeFrame = new byte[] { 0xAA, 0xFF, 0xFF, 0x03, 0x00, 0x0A, 0x01, 0x00, 0x08 };

            m_UartPort.Open("COM3", 9600);
            m_UartPort.MaintainResHander += new MaintainResEventHander(MaintainResHander);
            int count = 0;
            while (count < 3)
            {
                m_UartPort.Write(writeFrame, 0, writeFrame.Length);
                count++;
                Thread.Sleep(3000);
            }

            m_UartPort.Close();
        }

        public void MaintainResHander(object sender, MaintainResEventArgs e)
        {
            MessageBox.Show(MaintainProtocol.ByteArryToString(e.m_Res.Frame, 0, e.m_Res.Frame.Length));
        }
    }
}