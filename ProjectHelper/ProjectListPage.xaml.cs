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
    /// Interakční logika pro ProjectListPage.xaml
    /// </summary>
    public partial class ProjectListPage : Page
    {
        //Proměné a okna
        string appPath;
        NewProjectWindow newProjectWindow;
        RenameProjectWindow renameProjectWindow;

        //Hlavní metoda
        public ProjectListPage()
        {
            try
            {
                InitializeComponent();

                ProjectList.Focus();

                appPath = Environment.CurrentDirectory;
             
                LoadProjects();
                ColumnWidth();
            }
            catch
            {
                Error();
                Back();
            }
        }

        //Nastavuje šířku sloupců
        public void ColumnWidth()
        {
            double width = Properties.Settings.Default.width - 47;

            DateColumn.Width = (width / 12) * 5;
            NameColumn.Width = (width / 12) * 7;
        }

        ///EVENTY - OBECNÉ///

        //Po kliknutí na button se navrátí na předchozí stránku
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Back();
        }

        ///METODY A EVENT HANDLERY///

        //Metoda pro návrat na předchozí stránku
        private void Back()
        {
            this.NavigationService.Navigate(new MainPage());
        }

        //Metoda pro vyvolání jednoduché chybové hlášky
        private void Error()
        {
            MessageBox.Show("Nastala chyba prosím opakujte", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        //Metoda pro otevření projektu
        private void OpenProject()
        {
            if (ProjectList.SelectedItem != null)
            {
                string projectName = GetName();
                this.NavigationService.Navigate(new ProjectPage(projectName));
            }
        }

        //Metoda pro načtení projektů do listboxu
        private void LoadProjects()
        {
            //Smaže aktuální obsah ProjectNameList
            ProjectList.ItemsSource = null;

            List<ProjectInfo> projectInfos = new List<ProjectInfo>();

            //Cyklus pro načtení názvů složek
            string[] folders = Directory.GetDirectories(System.IO.Path.Combine(appPath, "Projekty"));
            foreach (string folder in folders)
            {
                //Přidá projekt do přehledu               
                string projectName = folder.Replace(System.IO.Path.Combine(appPath, @"Projekty\"), "");
                string date = File.ReadAllText(System.IO.Path.Combine(appPath, "Projekty", projectName, "Info.txt"));

                projectInfos.Add(new ProjectInfo() { Name = projectName, Date = date });
            }

            ProjectList.ItemsSource = projectInfos;
        }

        //Metoda pro vytvoření nového projektu
        private void NewProject()
        {
            newProjectWindow = new NewProjectWindow();
            newProjectWindow.Closed += new EventHandler(NewProjectWidnow_Closed);
            newProjectWindow.Owner = Application.Current.MainWindow;
            newProjectWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            newProjectWindow.Show();
        }

        //Event Handler pro zavření okna pro nový projekt
        private void NewProjectWidnow_Closed(object sender, System.EventArgs e)
        {
            LoadProjects();
            Application.Current.MainWindow.Activate();
        }

        //Metoda pro přejmenování projektu
        private void RenameProject()
        {
            if (ProjectList.SelectedItem != null)
            {
                string projectName = GetName();

                renameProjectWindow = new RenameProjectWindow(projectName, "project", projectName, null);
                renameProjectWindow.Closed += new EventHandler(RenameProjectWindow_Closed);
                renameProjectWindow.Owner = Application.Current.MainWindow;
                renameProjectWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                renameProjectWindow.Show();
            }

        }

        //Event Handler pro zavření okna pro přejmenování projektu
        private void RenameProjectWindow_Closed(object sender, System.EventArgs e)
        {
            LoadProjects();
            Application.Current.MainWindow.Activate();
        }

        //Metoda pro smazání projektu
        private void DeleteProject()
        {
            if (ProjectList.SelectedItem != null)
            {
                string projectName = GetName();

                if (MessageBox.Show("Opravdu chcete smazat vybraný projekt", "Potvrzení", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Directory.Delete(System.IO.Path.Combine(appPath, "Projekty", projectName), true);
                    MessageBox.Show("Projekt byl úspěšně smazán", "Úspěch");
                    LoadProjects();
                }
            }
        }

        private string GetName()
        {
            ProjectInfo projectInfo = ProjectList.SelectedItem as ProjectInfo;

            return projectInfo.Name;
        }

        ///EVENTY - LISTBOX///

        //Otevření projektu pomocí double clicku
        private void ProjectList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenProject();
        }

        //Otevření projektu pomocí enteru
        private void ProjectList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    OpenProject();
                }
                catch
                {
                    Error();
                }
            }
        }

        ///EVENTY - LISTBOX - CONTEXT MENU///

        //Nový projekt přes kontextové menu
        private void NewProject_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NewProject();
            }
            catch
            {
                Error();
            }
        }

        //Přejmenuje projekt přes kontextové menu
        private void RenameProjectItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RenameProject();
            }
            catch
            {
                Error();
            }
        }

        //Smaže projekt přes kontextové menu
        private void DeleteProjectItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DeleteProject();
            }
            catch
            {
                Error();
            }
        }

        
    }
}
