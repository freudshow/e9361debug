using E9361Debug.Communication;
using E9361Debug.logical;
using System;
using System.Collections.Generic;
using System.Windows;
using E9361Debug.Controls;
using System.Windows.Controls;

namespace E9361Debug.Windows
{
    /// <summary>
    /// ADE9078Set.xaml 的交互逻辑
    /// </summary>
    public partial class ADE9078Set : Window
    {
        public delegate void GetCheckResult(bool res);

        private readonly ICommunicationPort m_Port;
        private bool m_Result = true;
        private MultiRouteADEError m_MultiRouteADEError;

        public event GetCheckResult CheckResultEvent;

        public ADE9078Set(ICommunicationPort port, MultiRouteADEError routes)
        {
            InitializeComponent();

            m_MultiRouteADEError = routes;
            m_Port = port;

            InitControls();
            RefreshDataAsync();
        }

        private void InitControls()
        {
            if (m_MultiRouteADEError.RouteList.Count <= 0)
            {
                return;
            }

            if (m_MultiRouteADEError.RouteList.Count > 6)
            {
                MessageBox.Show("超过最大采集路数");
            }

            int cols = 2;
            int rows = m_MultiRouteADEError.RouteList.Count / cols + m_MultiRouteADEError.RouteList.Count % cols > 0 ? 1 : 0;

            Grid_Routes.RowDefinitions.Clear();
            for (int i = 0; i < rows; i++)
            {
                Grid_Routes.RowDefinitions.Add(new System.Windows.Controls.RowDefinition());
            }

            Grid_Routes.ColumnDefinitions.Clear();
            for (int i = 0; i < cols; i++)
            {
                Grid_Routes.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition());
            }

            for (int i = 0; i < m_MultiRouteADEError.RouteList.Count; i++)
            {
                var c = new ADESetOneRoute(m_MultiRouteADEError.RouteList[i], m_Port);
                Grid_Routes.Children.Add(c);
                Grid.SetRow(c, i / m_MultiRouteADEError.RouteList.Count);
                Grid.SetColumn(c, i % cols);
            }
        }

        private async void RefreshDataAsync()
        {
            for (int i = 0; i < Grid_Routes.Children.Count; i++)
            {
                ADESetOneRoute a = (ADESetOneRoute)Grid_Routes.Children[i];
                await a.ReadValuesAsync();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            CheckResultEvent(m_Result);
        }
    }
}