using Microsoft.Win32;
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

namespace ProjectHelper
{
    /// <summary>
    /// Interakční logika pro MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        //Proměné
        string appPath;

        public MainPage()
        {
            try
            {
                InitializeComponent();

                appPath = Environment.CurrentDirectory;

                //Pokud neexistuje složka s projekty vytvoří ji
                if (Directory.Exists(System.IO.Path.Combine(appPath, "Projekty")))
                    Directory.CreateDirectory(System.IO.Path.Combine(appPath, "Projekty"));
            }
            catch
            {
                Error();
                Application.Current.Shutdown();
            }
        }

        //Metoda pro vyvolání jednoduché chybové hlášky
        private void Error()
        {
            MessageBox.Show("Nastala chyba prosím opakujte", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        //Přejde na stránku s projekty
        private void ProjectsButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new ProjectListPage());
        }
    }
}
