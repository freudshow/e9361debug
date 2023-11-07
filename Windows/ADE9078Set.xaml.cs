using E9361Debug.Communication;
using E9361Debug.Controls;
using E9361Debug.Logical;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static E9361Debug.Controls.ADESetOneRoute;

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
        private List<ADESetOneRoute> m_ADESetOneRouteList;

        public event GetCheckResult CheckResultEvent;

        public event StopReadDataDelegate StopReadDataEvent;

        public event StartReadDataDelegate StartReadDataEvent;

        public ADE9078Set(ICommunicationPort port, MultiRouteADEError routes)
        {
            InitializeComponent();

            m_MultiRouteADEError = routes;
            m_Port = port;
            InitControls();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StartReadDataEvent?.Invoke();
            RefreshDataAsync();
        }

        private void InitControls()
        {
            if (m_MultiRouteADEError.RouteList.Count <= 0)
            {
                return;
            }

            int cols = (int)Math.Sqrt(m_MultiRouteADEError.RouteList.Count);
            int rows = m_MultiRouteADEError.RouteList.Count / cols + (m_MultiRouteADEError.RouteList.Count % cols > 0 ? 1 : 0);

            Grid_Routes.RowDefinitions.Clear();
            for (int i = 0; i < rows; i++)
            {
                Grid_Routes.RowDefinitions.Add(new RowDefinition());
            }

            Grid_Routes.ColumnDefinitions.Clear();
            for (int i = 0; i < cols; i++)
            {
                Grid_Routes.ColumnDefinitions.Add(new ColumnDefinition());
            }

            m_ADESetOneRouteList = new List<ADESetOneRoute>();
            foreach (var item in m_MultiRouteADEError.RouteList)
            {
                var r = new ADESetOneRoute(item, m_Port);
                Grid_Routes.Children.Add(r);
                m_ADESetOneRouteList.Add(r);
                int row = item.RouteNo / cols;
                int col = item.RouteNo % cols;
                Grid.SetRow(r, row);
                Grid.SetColumn(r, col);
            }

            CoConnect();
        }

        private void CoConnect()
        {
            foreach (var a in m_ADESetOneRouteList)
            {
                StopReadDataEvent += a.StopReadData;
                StartReadDataEvent += a.StartReadData;

                foreach (var b in m_ADESetOneRouteList)
                {
                    if (a != b)
                    {
                        a.StartReadDataEvent += b.StartReadData;
                        a.StopReadDataEvent += b.StopReadData;
                    }
                }
            }
        }

        private async void RefreshDataAsync()
        {
            while (true)
            {
                foreach (var item in m_ADESetOneRouteList)
                {
                    await item.ReadValuesAsync();
                    await Task.Delay(50);
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            m_Result = true;
            foreach (var item in m_MultiRouteADEError.RouteList)
            {
                if (item != null && !item.IsResultPass)
                {
                    m_Result = false;
                    break;
                }
            }

            StopReadDataEvent?.Invoke();

            CheckResultEvent(m_Result);
        }
    }
}