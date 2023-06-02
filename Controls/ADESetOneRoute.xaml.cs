using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using E9361Debug.Communication;
using E9361Debug.logical;
using E9361Debug.Maintain;
using Newtonsoft.Json;

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
        private bool m_Result = true;

        private Dictionary<ushort, RealDatabaseCmdParameters> m_Dict = new Dictionary<ushort, RealDatabaseCmdParameters>();

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
        }

        public async Task ReadValuesAsync()
        {
            if (m_OneRouteADEError == null || m_OneRouteADEError.ItemList == null || m_OneRouteADEError.ItemList.Count <= 0)
            {
                return;
            }

            foreach (var item in m_OneRouteADEError.ItemList)
            {
                byte[] b = MaintainProtocol.GetContinueRealDataBaseValue(m_Dict[item.RealDatabaseNo]);
                m_Port.Write(b, 0, b.Length);
                MaintainParseRes res = await m_Port.ReadOneFrameAsync(1000);
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
                    m_Port.Write(b, 0, b.Length);
                    MaintainParseRes res = await m_Port.ReadOneFrameAsync(100);
                    if (res != null && MaintainProtocol.ParseAdeSetAck(res.Frame))
                    {
                        m_Result &= true;
                        MessageBox.Show("设置成功");
                    }
                    else
                    {
                        m_Result &= false;
                        MessageBox.Show("设置失败!!!");
                    }
                }
            }
            catch (Exception ex)
            {
                m_Result &= false;
                MessageBox.Show(ex.Message);
            }
        }
    }
}