using System;
using System.Text.RegularExpressions;
using System.Windows;
using E9361Debug.Common;
using E9361Debug.Communication;
using E9361Debug.Config;
using E9361Debug.Json;
using E9361Debug.Logical;
using E9361Debug.Maintain;
using Newtonsoft.Json;

namespace E9361Debug.Windows
{
    /// <summary>
    /// Dlt645AddressSet.xaml 的交互逻辑
    /// </summary>
    public partial class Dlt645AddressSet : Window
    {
        private readonly CommunicationPort m_Port;
        private string m_IDPattern = "[A-Z][A-Z0-9]{11}[0-9]{12}";
        private string m_Dlt645Pattern = "[0-9]{12}";

        public Dlt645AddressSet()
        {
            InitializeComponent();

            NetPara udpclientpara = new NetPara { ServerIP = DataBaseLogical.GetTerminalIP(), ServerPort = DataBaseLogical.GetTerminalUDPPort(), Mode = PortTypeEnum.PortType_Net_UDP_Client };
            m_Port = new CommunicationPort(PortTypeEnum.PortType_Net_UDP_Client, udpclientpara);
            m_Port.Open();

            LoadConfig();
        }

        private void LoadConfig()
        {
            try
            {
                Configuration config = new Configuration();
                if (JsonProcess.ReadJsonFile("config/config.json", ref config))
                {
                    if (!string.IsNullOrEmpty(config.Dlt645Address.Pattern))
                    {
                        m_Dlt645Pattern = config.Dlt645Address.Pattern;
                    }

                    if (!string.IsNullOrEmpty(config.TerminalId.Pattern))
                    {
                        m_IDPattern = config.TerminalId.Pattern;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void Button_SetID_Click(object sender, RoutedEventArgs e)
        {
            string id = TextBox_Prefix.Text.Trim() + TextBox_Surfix.Text.Trim();
            if (!Regex.IsMatch(id, m_IDPattern))
            {
                MessageBox.Show("终端ID的格式错误!");
                TextBox_Surfix.Focus();
                return;
            }

            byte[] frame = MaintainProtocol.GetSetTerminalID(id);
            m_Port.Write(frame, 0, frame.Length);

            MaintainParseRes res = await m_Port.ReadOneFrameAsync(3000);
            if (res != null && MaintainProtocol.ParseSetTerminalID(res.Frame))
            {
                MessageBox.Show("设置成功");
            }
            else
            {
                MessageBox.Show("设置失败");
            }

            TextBox_Surfix.Focus();
        }

        private void Button_ReadAddress_Click(object sender, RoutedEventArgs e)
        {
        }

        private async void Button_SetDlt645_Click(object sender, RoutedEventArgs e)
        {
            string address = TextBox_Dlt645.Text.Trim();
            if (!Regex.IsMatch(address, m_Dlt645Pattern))
            {
                MessageBox.Show("DLT645地址的格式错误!");
                TextBox_Surfix.Focus();
                return;
            }

            if (!CommonClass.PrepareInputString(address, out _, out byte[] outaddress))
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
        }

        private void TextBox_Surfix_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string id = TextBox_Prefix.Text.Trim() + TextBox_Surfix.Text.Trim();
            if (Regex.IsMatch(id, m_IDPattern))
            {
                TextBox_Dlt645.Text = id.Substring(12, 12);
            }
        }
    }
}