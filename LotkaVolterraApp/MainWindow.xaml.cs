using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using LotkaVolterra.ViewModels;
using LotkaVolterra.Python;
using System.IO;

namespace LotkaVolterraApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();

        }

        private void Card_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void BtnMinimalize_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private int CreateFlag ()
        {
            int flag=0;
            if (checkbox.IsChecked == true)
                flag = 1;
            return flag;
        }

        private void RunPython()
        {
            PythonPath pythonPath = new PythonPath();

            string fileName = $@"D:\PYCHARM_PROJECTS\MSIPBtest\main.py {s.Text} {a.Text} {ab.Text} {dt.Text} {r.Text} {N.Text} {v0.Text} {p0.Text} {pk.Text} {CreateFlag()}";
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo(pythonPath.GetPythonPath(), fileName)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
            };
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            StreamWriter st = new StreamWriter("log.txt", true);
            st.WriteLine("{0} Error: {1}", DateTime.Now, p.StandardError.ReadToEnd());
            st.Flush();
            st.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RunPython();
            }
            catch { }
        }
    }
}
