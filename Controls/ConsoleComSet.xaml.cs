using E9361Debug.Common;
using E9361Debug.Communication;
using E9361Debug.Logical;
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

        public ConsoleComSet()
        {
            InitializeComponent();

            m_CurrentCom = DataBaseLogical.GetConsoleComName();
            RefreshPortNames();
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
            }

            DataRow[] dr = dtcom.Select($"Name = '{m_CurrentCom}'");

            if (dr != null && dr.Length > 0)
            {
                ComboBox_ComList.SelectedValue = m_CurrentCom;
            }
        }

        private void ComboBox_ComList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void ComboBox_BaudList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}