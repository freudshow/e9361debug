using E9361Debug.Communication;
using E9361Debug.Maintain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// ADE9078Set.xaml 的交互逻辑
    /// </summary>
    public partial class ADE9078Set : Window
    {
        public delegate void GetCheckResult(bool res);

        private enum SetTypeEnum
        {
            Set_Type_Defatult = 0,
            Set_Type_220V5A0Angle,
            Set_Type_220V5A60Angle
        }

        private readonly ICommunicationPort m_Port;
        private long m_TimeOut = 3000;
        private bool m_Result = true;

        public event GetCheckResult CheckResultEvent;

        public ADE9078Set(ICommunicationPort port, long timeout)
        {
            InitializeComponent();
            m_Port = port;
            m_TimeOut = timeout;
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
                byte route = Convert.ToByte(ComboBox_RouteIdx.SelectedIndex);
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
                    MaintainParseRes res = await m_Port.ReadOneFrameAsync(m_TimeOut);
                    if (MaintainProtocol.ParseAdeSetAck(res.Frame))
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
                base.Close();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            CheckResultEvent(m_Result);
        }
    }
}