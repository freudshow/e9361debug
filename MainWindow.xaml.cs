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
using System.Windows.Documents;

namespace E9361DEBUG
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private CommunicationPort m_CommunicationPort;
        private CheckItems m_CheckItems;
        private Paragraph m_ParagraphResult = new Paragraph();
        private Paragraph m_ParagraphException = new Paragraph();

        public MainWindow()
        {
            InitializeComponent();
            InitAll();
        }

        private void InitAll()
        {
            InitCheckTree();
            InitTextBox();
            OpenTerminalMaintain();
        }

        private void InitTextBox()
        {
            RichTextBox_Result.Document = new FlowDocument();
            RichTextBox_Result.Document.Blocks.Clear();
            RichTextBox_Result.Document.Blocks.Add(m_ParagraphResult);

            RichTextBox_Exception.Document = new FlowDocument();
            RichTextBox_Exception.Document.Blocks.Clear();
            RichTextBox_Exception.Document.Blocks.Add(m_ParagraphException);
        }

        /// <summary>
        /// 打开维护规约通道
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
            m_CheckItems = new CheckItems
            {
                IsEnable = true,
                Description = "总体检测",
                ChildTableName = DataBaseLogical.GetBaseCheckTableName()
            };

            Controls_CheckTree.SetDataSource(m_CheckItems);
        }

        private void DisplayCheckInfo(ResultInfoType resultType, bool isResultTrue, string info, int depth)
        {
            string tabs = "";
            while (depth-- > 0)
            {
                tabs += "\t";
            }

            switch (resultType)
            {
                case ResultInfoType.ResultInfo_Logs:
                    m_ParagraphResult.Inlines.Add(new Run(tabs + info));
                    break;

                case ResultInfoType.ResultInfo_Result:
                    m_ParagraphResult.Inlines.Add(new Run { Text = tabs + info, Foreground = isResultTrue ? Brushes.Green : Brushes.Red });

                    if (!isResultTrue)
                    {
                        m_ParagraphException.Inlines.Add(new Run { Text = tabs + info, Foreground = Brushes.Red });
                    }
                    break;

                case ResultInfoType.ResultInfo_Exception:
                    m_ParagraphException.Inlines.Add(new Run { Text = tabs + info, Foreground = Brushes.Red });
                    break;

                default:
                    break;
            }
        }

        private void Menu_About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"E9361-C检测软件\n软件版本:{Common.Version}");
        }

        private async void Button_StartDebug_Click(object sender, RoutedEventArgs e)
        {
            _ = await CheckProcess.CheckOneItemAsync(m_CommunicationPort, m_CheckItems, DisplayCheckInfo);
        }

        private void RichTextBox_Result_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            RichTextBox_Result.ScrollToEnd();
        }

        private void RichTextBox_Exception_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            RichTextBox_Exception.ScrollToEnd();
        }
    }
}