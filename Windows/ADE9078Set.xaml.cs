﻿using E9361Debug.Communication;
using E9361Debug.logical;
using System;
using System.Collections.Generic;
using System.Windows;
using E9361Debug.Controls;
using System.Windows.Controls;
using System.Threading.Tasks;

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
        private bool CanReadData = true;
        private MultiRouteADEError m_MultiRouteADEError;

        public event GetCheckResult CheckResultEvent;

        public ADE9078Set(ICommunicationPort port, MultiRouteADEError routes)
        {
            InitializeComponent();

            m_MultiRouteADEError = routes;
            m_Port = port;
            InitControls();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StartReadData();
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

            foreach (var item in m_MultiRouteADEError.RouteList)
            {
                var r = new ADESetOneRoute(item, m_Port);
                Grid_Routes.Children.Add(r);
                int row = item.RouteNo / cols;
                int col = item.RouteNo % cols;
                Grid.SetRow(r, row);
                Grid.SetColumn(r, col);

                r.StartReadDataEvent += StartReadData;
                r.StopReadDataEvent += StopReadData;
            }
        }

        private async void RefreshDataAsync()
        {
            while (true)
            {
                foreach (var item in Grid_Routes.Children)
                {
                    if (CanReadData)
                    {
                        ADESetOneRoute a = item as ADESetOneRoute;
                        if (a != null)
                        {
                            await a.ReadValuesAsync();
                        }
                    }

                    await Task.Delay(50);
                }
            }
        }

        private void StopReadData()
        {
            CanReadData = false;
            foreach (var item in Grid_Routes.Children)
            {
                ADESetOneRoute a = item as ADESetOneRoute;
                if (a != null)
                {
                    a.StopReadData();
                }
            }
        }

        private void StartReadData()
        {
            CanReadData = true;
            foreach (var item in Grid_Routes.Children)
            {
                ADESetOneRoute a = item as ADESetOneRoute;
                if (a != null)
                {
                    a.StartReadData();
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

            StopReadData();

            CheckResultEvent(m_Result);
        }
    }
}