using System;
using System.Collections.Generic;
using System.IO;
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
using Path = System.IO.Path;

namespace ProjectHelper
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double positionX;
        double positionY;

        public MainWindow()
        {
            InitializeComponent();

            MainFrame.NavigationService.Navigate(new MainPage());

            //Aplikovaní velikosti
            Height = Properties.Settings.Default.height;
            Width = Properties.Settings.Default.width;

            //Aplikování pozice
            Application.Current.MainWindow.Left = Properties.Settings.Default.positionX;
            Application.Current.MainWindow.Top = Properties.Settings.Default.positionY;

            CheckStructure();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //Ukládání velikosti
            Properties.Settings.Default.height = Height;
            Properties.Settings.Default.width = Width;

            //Ukládání pozice
            Properties.Settings.Default.positionX = positionX;
            Properties.Settings.Default.positionY = positionY;

            Properties.Settings.Default.Save();
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            positionX = Application.Current.MainWindow.Left;
            positionY = Application.Current.MainWindow.Top; ;
        }

        public void CheckStructure()
        {
            //Kontrola nastavení
            if (Directory.Exists(@"Settings") == false)
                Directory.CreateDirectory(@"Settings");

                StreamWriter writer = new StreamWriter(@"Settings\DefaultProjectSettings.txt");
                writer.WriteLine("TimerOn=True");
                writer.WriteLine("TimerVisibility=True");
                writer.WriteLine("SubfoldersExpanded=True");
                writer.WriteLine("ProjectColor=#FF2F6DAC");
                writer.WriteLine("ToolbarVisibility=True");
                writer.Close();

            //CustomColor.txt
            if (File.Exists(@"Settings\CustomColor.txt") == false)
            {
                StreamWriter writer2 = new StreamWriter(@"Settings\CustomColor.txt");
                writer2.WriteLine("2171169");
                writer2.WriteLine("15790320");
                writer2.WriteLine("11300143");
                for (int i = 0; i < 13; i++)
                    writer2.WriteLineAsync("16777215");
                writer2.Close();
            }

            //Kontrola projektu
            if (Directory.Exists(@"Projekty") == false)
                Directory.CreateDirectory(@"Projekty");
        }
    }
}
