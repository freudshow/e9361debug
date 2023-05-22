using E9361Debug.Communication;
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
        private enum SetTypeEnum
        {
            Set_Type_Defatult = 0,
            Set_Type_220V5A0Angle,
            Set_Type_220V5A60Angle
        }

        private readonly ICommunicationPort m_Port;

        public ADE9078Set(ICommunicationPort port)
        {
            InitializeComponent();
            m_Port = port;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
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
                switch (t)
                {
                    case SetTypeEnum.Set_Type_Defatult:
                        break;

                    case SetTypeEnum.Set_Type_220V5A0Angle:
                        break;

                    case SetTypeEnum.Set_Type_220V5A60Angle:
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}