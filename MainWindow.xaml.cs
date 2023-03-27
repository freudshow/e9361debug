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
using System.Windows.Shapes;
using System.Data;
using E9361App.DBHelper;
using E9361App.Maintain;

namespace e9361debug
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataSet ds = SQLiteHelper.Query("select * from t_chkPara");

            byte[] frame;
            MaintainProtocol.GetResetFrame(out frame);

            MessageBox.Show(MaintainProtocol.ByteArryToString(frame, 0, frame.Length));
        }
    }
}