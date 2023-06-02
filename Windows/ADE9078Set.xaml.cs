using E9361Debug.Communication;
using System;
using System.Windows;

namespace E9361Debug.Windows
{
    /// <summary>
    /// ADE9078Set.xaml 的交互逻辑
    /// </summary>
    public partial class ADE9078Set : Window
    {
        public delegate void GetCheckResult(bool res);

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

        private void Window_Closed(object sender, EventArgs e)
        {
            CheckResultEvent(m_Result);
        }
    }
}