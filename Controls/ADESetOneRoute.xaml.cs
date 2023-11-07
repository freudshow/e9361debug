using E9361Debug.Communication;
using E9361Debug.Logical;
using E9361Debug.Maintain;
using E9361Debug.MsgBox;
using E9361Debug.SshInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace E9361Debug.Controls
{
    /// <summary>
    /// ADESetOneRoute.xaml 的交互逻辑
    /// </summary>
    public partial class ADESetOneRoute : UserControl
    {
        private enum SetTypeEnum
        {
            Set_Type_Defatult = 0,
            Set_Type_220V5A0Angle,
            Set_Type_220V5A60Angle
        }

        private OneRouteADEError m_OneRouteADEError;
        private readonly ICommunicationPort m_Port;
        private readonly Dictionary<ushort, RealDatabaseCmdParameters> m_Dict = new Dictionary<ushort, RealDatabaseCmdParameters>();
        private bool m_CanReadData = true;
        private readonly SshClientClass m_SshClass;

        public delegate void StopReadDataDelegate();

        public event StopReadDataDelegate StopReadDataEvent;

        public delegate void StartReadDataDelegate();

        public event StartReadDataDelegate StartReadDataEvent;

        public ADESetOneRoute(OneRouteADEError e, ICommunicationPort port)
        {
            InitializeComponent();

            m_OneRouteADEError = e;
            m_Port = port;

            DataGrid_DisplayItems.ItemsSource = m_OneRouteADEError.ItemList;
            if (m_OneRouteADEError != null)
            {
                TextBlock_Route.Text = $"第[{m_OneRouteADEError.RouteNo + 1}]路芯片";
            }

            m_Dict.Clear();

            if (m_OneRouteADEError != null && m_OneRouteADEError.ItemList != null && m_OneRouteADEError.ItemList.Count > 0)
            {
                foreach (var item in m_OneRouteADEError.ItemList)
                {
                    RealDatabaseCmdParameters p = new RealDatabaseCmdParameters
                    {
                        RealDataBaseNo = item.RealDatabaseNo,
                        TeleType = RealDataTeleTypeEnum.Real_Data_TeleType_YC,
                        DataType = RealDataDataTypeEnum.Real_Data_type_Float,
                        DataItemCount = 1
                    };

                    if (!m_Dict.Keys.Contains(item.RealDatabaseNo))
                    {
                        m_Dict.Add(item.RealDatabaseNo, p);
                    }
                }
            }

            StopReadDataEvent += StopReadData;
            StartReadDataEvent += StartReadData;

            if (m_SshClass == null)
            {
                m_SshClass = new SshClientClass(DataBaseLogical.GetTerminalIP(), DataBaseLogical.GetTerminalSSHPort(), DataBaseLogical.GetTerminalSSHUserName(), DataBaseLogical.GetTerminalSSHPasswd());
            }
        }

        public async Task ReadValuesAsync()
        {
            if (m_OneRouteADEError == null || m_OneRouteADEError.ItemList == null || m_OneRouteADEError.ItemList.Count <= 0)
            {
                return;
            }

            foreach (var item in m_OneRouteADEError.ItemList)
            {
                if (m_CanReadData)
                {
                    byte[] b = MaintainProtocol.GetContinueRealDataBaseValue(m_Dict[item.RealDatabaseNo]);

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

                        item.ActualValue = data.RealDataArray[0].FloatValue;
                    }
                }
            }
        }

        public void StopReadData()
        {
            m_CanReadData = false;
        }

        public void StartReadData()
        {
            m_CanReadData = true;
        }

        private async void Button_SetDefault_Click(object sender, RoutedEventArgs e)
        {
            await SetAde9078(SetTypeEnum.Set_Type_Defatult);
        }

        private async void Button_Set220V5A0Angle_Click(object sender, RoutedEventArgs e)
        {
            await SetAde9078(SetTypeEnum.Set_Type_220V5A0Angle);
        }

        private async void Button_Set220V5A60Angle_Click(object sender, RoutedEventArgs e)
        {
            await SetAde9078(SetTypeEnum.Set_Type_220V5A60Angle);
        }

        private async Task SetAde9078(SetTypeEnum t)
        {
            if (m_Port == null || !m_Port.IsOpen())
            {
                MessageBox.Show("端口设置错误!");
            }

            try
            {
                Button_Set220V5A0Angle.IsEnabled = false;
                Button_Set220V5A60Angle.IsEnabled = false;
                Button_SetDefault.IsEnabled = false;

                StopReadDataEvent?.Invoke();
                await Task.Delay(3000);

                byte route = (byte)m_OneRouteADEError.RouteNo;
                byte[] b = null;
                switch (t)
                {
                    case SetTypeEnum.Set_Type_Defatult:
                        b = MaintainProtocol.GetSetADE9078Default(route);
                        break;

                    case SetTypeEnum.Set_Type_220V5A0Angle:
                        b = MaintainProtocol.Get220V5A0AngleSet(route);
                        break;

                    case SetTypeEnum.Set_Type_220V5A60Angle:
                        b = MaintainProtocol.Get220V5A60AngleSet(route);
                        break;

                    default:
                        b = null;
                        break;
                }

                if (b != null)
                {
                    if (!m_Port.IsOpen())
                    {
                        m_Port.Open();
                    }

                    m_Port.Write(b, 0, b.Length);
                    MaintainParseRes res = await m_Port.ReadOneFrameAsync(1000);
                    if (res != null && MaintainProtocol.ParseAdeSetAck(res.Frame))
                    {
                        ShowMsg.ShowMessageBoxTimeout("设置成功", "温馨提示", MessageBoxButton.OK, 1000);

                        if (t == SetTypeEnum.Set_Type_Defatult)
                        {
                            ShowMsg.ShowMessageBoxTimeout("终端重启中, 请等待30秒...", "温馨提示", MessageBoxButton.OK, 1000);

                            if (!m_SshClass.IsSshConnected)
                            {
                                m_SshClass.ConnectToSshServer();
                            }

                            m_SshClass.ExecCmd("/sbin/reboot");
                            await Task.Delay(30 * 1000);
                            m_SshClass.DisConnectSSH();
                        }
                    }
                    else
                    {
                        MessageBox.Show("设置失败!!!");
                    }
                }

                StartReadDataEvent?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Button_Set220V5A0Angle.IsEnabled = true;
                Button_Set220V5A60Angle.IsEnabled = true;
                Button_SetDefault.IsEnabled = true;
            }
        }
    }
}