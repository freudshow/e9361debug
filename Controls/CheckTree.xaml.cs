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
using System.Windows.Navigation;
using E9361Debug.Logical;

namespace E9361DEBUG.Controls
{
    /// <summary>
    /// CheckTree.xaml 的交互逻辑
    /// </summary>
    public partial class CheckTree : UserControl
    {
        private CheckItems m_CheckItems;

        public CheckTree()
        {
            InitializeComponent();
        }

        internal void SetDataSource(CheckItems items)
        {
            if (items == null)
            {
                return;
            }

            m_CheckItems = items;
            TreeView_CheckTree.ItemsSource = m_CheckItems.Children;
        }

        private void TreeView_CheckTree_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
        }

        private void TreeView_CheckTree_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void Menu_PreviewItem(object sender, RoutedEventArgs e)
        {
        }
    }
}