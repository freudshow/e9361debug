﻿using E9361Debug.Communication;
using E9361Debug.Logical;
using E9361Debug.Maintain;
using E9361Debug.MsgBox;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using static E9361Debug.Windows.ADE9078Set;

namespace E9361Debug.Windows
{
    /// <summary>
    /// PT100Set.xaml 的交互逻辑
    /// </summary>
    public partial class PT100Set : Window
    {
        private bool m_CanReadData = true;
        private bool m_WindowIsShow = false;
        private readonly ICommunicationPort m_Port;
        private readonly ADEErrorParameter m_Para;
        private readonly ObservableCollection<ADEErrorParameter> m_DataList = new ObservableCollection<ADEErrorParameter>();
        private readonly RealDatabaseCmdParameters m_Realdata;

        public event GetCheckResult CheckResultEvent;

        public PT100Set(ICommunicationPort port, ADEErrorParameter para)
        {
            InitializeComponent();

            m_Port = port;
            m_Para = para;
            m_DataList.Add(m_Para);
            DataGrid_DisplayItems.ItemsSource = m_DataList;

            m_Realdata = new RealDatabaseCmdParameters
            {
                RealDataBaseNo = m_Para.RealDatabaseNo,
                TeleType = RealDataTeleTypeEnum.Real_Data_TeleType_YC,
                DataType = RealDataDataTypeEnum.Real_Data_type_Float,
                DataItemCount = 1
            };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            m_WindowIsShow = true;
            ReadValueAsync();
        }

        public async void ReadValueAsync()
        {
            while (m_WindowIsShow)
            {
                if (m_CanReadData)
                {
                    byte[] b = MaintainProtocol.GetContinueRealDataBaseValue(m_Realdata);

                    if (!m_Port.IsOpen())
                    {
                        m_Port.Open();
                    }

                    m_Port.Write(b, 0, b.Length);
                    MaintainParseRes res = await m_Port.ReadOneFrameAsync(500);
                    if (res != null)
                    {
                        ContinueRealData data = MaintainProtocol.ParseContinueRealDataValue(res.Frame);
                        if (data == null || !data.IsValid || data.RealDataArray == null || data.RealDataArray.Length <= 0)
                        {
                            continue;
                        }

                        m_Para.ActualValue = data.RealDataArray[0].FloatValue;
                    }
                }

                await Task.Delay(50);
            }
        }

        private async void Button_SetNegtive49_Click(object sender, RoutedEventArgs e)
        {
            Button_SetPositive155.IsEnabled = false;
            await SetPT100(-49, false);
            Button_SetPositive155.IsEnabled = true;
        }

        private async void Button_SetPositive155_Click(object sender, RoutedEventArgs e)
        {
            await SetPT100(155, true);
        }

        private async Task SetPT100(int value, bool isWriteFile)
        {
            byte[] b = MaintainProtocol.GetPt100Setting(value, isWriteFile);
            if (b != null)
            {
                m_CanReadData = false;
                await Task.Delay(2000);//等待读取数据函数暂停

                if (!m_Port.IsOpen())
                {
                    m_Port.Open();
                }

                m_Port.Write(b, 0, b.Length);
                MaintainParseRes res = await m_Port.ReadOneFrameAsync(1000);
                if (res != null && MaintainProtocol.ParseSetPt100Ack(res.Frame))
                {
                    ShowMsg.ShowMessageBoxTimeout("设置成功", "温馨提示", MessageBoxButton.OK, 1000);
                }
                else
                {
                    MessageBox.Show("设置失败!!!");
                }

                await Task.Delay(2000);//等待终端的温度值稳定后再读取

                m_CanReadData = true;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            m_WindowIsShow = false;
            CheckResultEvent(m_Para.Result);
        }
    }
}