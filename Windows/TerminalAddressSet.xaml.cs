using E9361Debug.Common;
using E9361Debug.Communication;
using E9361Debug.Logical;
using E9361Debug.Maintain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace E9361Debug.Windows
{
    /// <summary>
    /// TerminalAddressSet.xaml 的交互逻辑
    /// </summary>
    public partial class TerminalAddressSet : Window
    {
        private readonly CommunicationPort m_Port;

        public TerminalAddressSet()
        {
            InitializeComponent();

            NetPara udpclientpara = new NetPara { ServerIP = DataBaseLogical.GetTerminalIP(), ServerPort = DataBaseLogical.GetTerminalUDPPort(), Mode = PortTypeEnum.PortType_Net_UDP_Client };
            m_Port = new CommunicationPort(PortTypeEnum.PortType_Net_UDP_Client, udpclientpara);
            m_Port.Open();
        }

        private async void Button_SetDlt645Adress_Click(object sender, RoutedEventArgs e)
        {
            if (!CommonClass.PrepareInputString(TextBox_DLT645Address.Text, out _, out byte[] outaddress))
            {
                MessageBox.Show("DLT645地址的格式错误!");
                return;
            }

            byte[] frame = MaintainProtocol.GetSetDlt645Address(outaddress);
            m_Port.Write(frame, 0, frame.Length);

            MaintainParseRes res = await m_Port.ReadOneFrameAsync(3000);
            if (res != null && MaintainProtocol.ParseSetDlt645Address(res.Frame))
            {
                MessageBox.Show("设置成功");
            }
            else
            {
                MessageBox.Show("设置失败");
            }

            TextBox_ScanResult.Focus();
            TextBox_ScanResult.Text = string.Empty;
        }

        private void TextBox_ScanResult_TextChanged(object sender, TextChangedEventArgs e)
        {
            Regex re = new Regex("ID：[a-zA-Z0-9]{24}");
            MatchCollection group = re.Matches(TextBox_ScanResult.Text);
            if (group != null && group.Count > 0)
            {
                TextBox_TerminalID.Text = group[0].ToString().Substring(3, 24);
                TextBox_DLT645Address.Text = TextBox_TerminalID.Text.Substring(12, 12);
            }
        }
    }
}