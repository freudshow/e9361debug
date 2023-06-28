using E9361Debug.Common;
using E9361Debug.Communication;
using E9361Debug.Logical;
using E9361Debug.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace E9361Debug
{
    public enum PortUseTypeEnum
    {
        Maintaince = 0,
        Console
    }

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<PortUseTypeEnum, CommunicationPort> m_PortDict = new Dictionary<PortUseTypeEnum, CommunicationPort>();
        private CommunicationPort m_CommunicationPort;
        private CommunicationPort m_ConsolePort;
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
            try
            {
                PortTypeEnum e = DataBaseLogical.GetMaintainPortType();

                switch (e)
                {
                    case PortTypeEnum.PortType_Error:
                        break;

                    case PortTypeEnum.PortType_Serial:
                        break;

                    case PortTypeEnum.PortType_Net_UDP_Client:
                        NetPara udpclientpara = new NetPara { ServerIP = DataBaseLogical.GetTerminalIP(), ServerPort = DataBaseLogical.GetTerminalUDPPort(), Mode = PortTypeEnum.PortType_Net_UDP_Client };
                        m_CommunicationPort = new CommunicationPort(PortTypeEnum.PortType_Net_UDP_Client, udpclientpara);
                        break;

                    case PortTypeEnum.PortType_Net_TCP_Client:
                        NetPara tcpclientpara = new NetPara { ServerIP = DataBaseLogical.GetTerminalIP(), ServerPort = DataBaseLogical.GetTerminalTCPClientPort(), Mode = PortTypeEnum.PortType_Net_TCP_Client };
                        m_CommunicationPort = new CommunicationPort(PortTypeEnum.PortType_Net_TCP_Client, tcpclientpara);
                        break;

                    case PortTypeEnum.PortType_Net_TCP_Server:
                        break;

                    default:
                        break;
                }

                if (m_CommunicationPort != null && !m_PortDict.Keys.Contains(PortUseTypeEnum.Maintaince))
                {
                    m_CommunicationPort.Open();
                    m_PortDict[PortUseTypeEnum.Maintaince] = m_CommunicationPort;
                }
            }
            catch (Exception ex)
            {
                m_ParagraphException.Inlines.Add(new Run { Text = ex.Message, Foreground = Brushes.Red });
            }
        }

        /// <summary>
        /// 打开终端的Console口
        /// </summary>
        private void OpenConsole()
        {
            if (m_ConsolePort != null && m_ConsolePort.IsOpen())
            {
                return;
            }

            try
            {
                UartPortPara uartPortPara = new UartPortPara { PortName = ConsoleComSet_ComSet.CurrentCom, BaudRate = ConsoleComSet_ComSet.CurrentBaudrate };
                m_ConsolePort = new CommunicationPort(PortTypeEnum.PortType_Serial, uartPortPara);
                m_ConsolePort.Open();

                if (m_ConsolePort != null && !m_PortDict.Keys.Contains(PortUseTypeEnum.Console))
                {
                    m_PortDict[PortUseTypeEnum.Console] = m_ConsolePort;
                }
            }
            catch (Exception ex)
            {
                m_ParagraphException.Inlines.Add(new Run { Text = ex.Message, Foreground = Brushes.Red });
            }
        }

        private void InitCheckTree()
        {
            m_CheckItems = new CheckItems
            {
                IsEnable = true,
                Children = new ObservableCollection<CheckItems>
                {
                    new CheckItems
                    {
                        IsEnable = true,
                        Description = "总体检测",
                        ChildTableName = DataBaseLogical.GetBaseCheckTableName()
                    }
                }
            };

            Controls_CheckTree.SetDataSource(m_CheckItems);
        }

        private void DisplayCheckInfo(ResultInfoType resultType, bool isResultTrue, string info, int depth)
        {
            this.Dispatcher.BeginInvoke(new ThreadStart(delegate ()
            {
                string tabs = "";
                while (depth-- > 0)
                {
                    tabs += "|____";
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
                            m_ParagraphException.Inlines.Add(new Run { Text = info, Foreground = Brushes.Red });
                        }
                        break;

                    case ResultInfoType.ResultInfo_Exception:
                        m_ParagraphException.Inlines.Add(new Run { Text = info, Foreground = Brushes.Red });
                        break;

                    default:
                        break;
                }
            }));
        }

        private void Menu_About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"E9361-C0工装调试软件\n软件版本:{CommonClass.Version}");
        }

        private async void Button_StartDebug_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenTerminalMaintain();
                OpenConsole();
                Button_StartDebug.IsEnabled = false;
                m_ParagraphResult.Inlines.Clear();
                m_ParagraphException.Inlines.Clear();

                await Task.Run(
                    async () =>
                    {
                        SetCheckInit(m_CheckItems);
                        _ = await CheckProcess.CheckOneItemAsync(m_PortDict, m_CheckItems.Children[0], DisplayCheckInfo);
                    });
                Button_StartDebug.IsEnabled = true;
            }
            catch (Exception ex)
            {
                m_ParagraphException.Inlines.Add(new Run { Text = ex.Message, Foreground = Brushes.Red });
            }
        }

        private void SetCheckInit(CheckItems checkItems)
        {
            if (checkItems != null)
            {
                checkItems.CheckIsPassed = CheckIsPassed.Check_Init;
                if (checkItems.Children != null)
                {
                    foreach (var c in checkItems.Children)
                    {
                        SetCheckInit(c);
                    }
                }
            }
        }

        private void RichTextBox_Result_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            RichTextBox_Result.ScrollToEnd();
        }

        private void RichTextBox_Exception_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            RichTextBox_Exception.ScrollToEnd();
        }

        private void Button_SetTerminalAddress_Click(object sender, RoutedEventArgs e)
        {
            Dlt645AddressSet dlt645set = new Dlt645AddressSet();
            dlt645set.ShowDialog();
        }

        private void Menu_ConfigBaseParas_Click(object sender, RoutedEventArgs e)
        {
            ConfigBaseParas baseParas = new ConfigBaseParas();
            baseParas.ShowDialog();
        }

        private void Menu_ConfigCheckItems_Click(object sender, RoutedEventArgs e)
        {
            ConfigCheckItems checkItems = new ConfigCheckItems();
            checkItems.ShowDialog();
        }
    }
}