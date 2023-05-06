using E9361Debug.Common;
using E9361Debug.Communication;
using E9361Debug.Logical;
using E9361Debug.MsgBox;
using System;
using System.Data;
using System.Management;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace E9361Debug.Controls
{
    /// <summary>
    /// ConsoleComSet.xaml 的交互逻辑
    /// </summary>
    public partial class ConsoleComSet : UserControl
    {
        private string m_CurrentCom;
        private int m_CurrentBaudrate;

        public ConsoleComSet()
        {
            InitializeComponent();

            m_CurrentCom = DataBaseLogical.GetConsoleComName();
            m_CurrentBaudrate = DataBaseLogical.GetConsoleComBaudRate();
            RefreshPortNames();
            RefreshPortBaudrate();
            UsbDection.AddRemoveUSBHandler(USBChanged);
            UsbDection.AddInsetUSBHandler(USBChanged);
        }

        public void USBChanged(object sender, EventArrivedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new ThreadStart(delegate ()
            {
                RefreshPortNames();
            }));
        }

        private void RefreshPortNames()
        {
            DataTable dtcom = CommonClass.GetPortNames();
            if (dtcom == null || dtcom.Rows == null || dtcom.Rows.Count <= 0)
            {
                ComboBox_ComList.ItemsSource = null;
                return;
            }

            ComboBox_ComList.DisplayMemberPath = "Description";
            ComboBox_ComList.SelectedValuePath = "Name";
            ComboBox_ComList.ItemsSource = dtcom.DefaultView;

            if (ComboBox_ComList.Items != null && ComboBox_ComList.Items.Count > 0)
            {
                ComboBox_ComList.SelectedIndex = 0;

                DataRow[] dr = dtcom.Select($"Name = '{m_CurrentCom}'");

                if (dr != null && dr.Length > 0)
                {
                    ComboBox_ComList.SelectedValue = m_CurrentCom;
                }
            }
        }

        private void RefreshPortBaudrate()
        {
            DataTable dt = DataBaseLogical.GetBaudrateList();

            ComboBox_BaudList.DisplayMemberPath = "enumName";
            ComboBox_BaudList.SelectedValuePath = "enum";
            ComboBox_BaudList.ItemsSource = dt.DefaultView;

            if (ComboBox_BaudList.Items != null && ComboBox_BaudList.Items.Count > 0)
            {
                ComboBox_BaudList.SelectedIndex = 0;
                DataRow[] dr = dt.Select($"enum = '{m_CurrentBaudrate}'");
                if (dr != null && dr.Length > 0)
                {
                    ComboBox_BaudList.SelectedValue = m_CurrentBaudrate;
                }
            }
        }

        private void ComboBox_ComList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_CurrentCom = ComboBox_ComList.SelectedValue.ToString();
        }

        private void ComboBox_BaudList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_CurrentBaudrate = Convert.ToInt32(ComboBox_BaudList.SelectedValue);
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if (DataBaseLogical.SaveComName(m_CurrentCom) && DataBaseLogical.SaveBaudRate(m_CurrentBaudrate))
            {
                ShowMsg.ShowMessageBoxTimeout("保存成功", "温馨提示", MessageBoxButton.OK, 1000);
            }
            else
            {
                ShowMsg.ShowMessageBoxTimeout("保存失败!", "警告", MessageBoxButton.OK, 3000);
            }
        }
    }
}