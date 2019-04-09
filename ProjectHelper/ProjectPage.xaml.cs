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
using System.Windows.Threading;
using System.Diagnostics;
using ProjectHelper.Classes;
using System.Reflection;

namespace ProjectHelper
{
    /// <summary>
    /// Interakční logika pro ProjectPage.xaml
    /// </summary>
    public partial class ProjectPage : Page
    {
        //PROMĚNÉ
        string appPath;
        string notePath;
        string projectName;
        string setttingsPath;
        ProjectTab projectTab;

        public ProjectPage(string projectName)
        {
            InitializeComponent();

            //Načte název projektu do hlavního labelu
            ProjectNameLabel.Text = projectName;

            //Vytvoří cesty
            appPath = Environment.CurrentDirectory;
            notePath = System.IO.Path.Combine(appPath, "Projekty", projectName, "Poznámky");
            setttingsPath = System.IO.Path.Combine(appPath, "Projekty", projectName, "Settings.txt");

            //Pokuď neexistuje složka s poznámkami vytvoří ji
            if (Directory.Exists(notePath) == false)
                Directory.CreateDirectory(notePath);

            //Pokuď neexistuje složka pro poznámky vytvoří ji
            if (Directory.Exists(System.IO.Path.Combine(notePath, "Notes")))
                Directory.CreateDirectory(System.IO.Path.Combine(notePath, "Notes"));

            //Pokuď neexistuje složka pro todo vytvoří ji
            if (Directory.Exists(System.IO.Path.Combine(notePath, "Todo")))
                Directory.CreateDirectory(System.IO.Path.Combine(notePath, "Todo"));

            //Pokud neexistuje .txt file s popisem vytvoří ji
            if (File.Exists(System.IO.Path.Combine(appPath, "Projekty", projectName, "Description.txt")) == false)
                File.Create(System.IO.Path.Combine(appPath, "Projekty", projectName, "Description.txt"));

            //Pokud neexistuje .txt file s infem vytvoří ji
            if (File.Exists(System.IO.Path.Combine(appPath, "Projekty", projectName, "Info.txt")) == false)
                File.Create(System.IO.Path.Combine(appPath, "Projekty", projectName, "Info.txt"));

            //Pokud neexituje .txt file s časem vytvoří ho
            if (File.Exists(System.IO.Path.Combine(appPath, "Projekty", projectName, "Time.txt")) == false)
                File.AppendAllText(System.IO.Path.Combine(appPath, "Projekty", projectName, "Time.txt"), "00:00:00:00");

            //Pokud neexistuje .txt se settingem zkopíruje ho
            if (File.Exists(setttingsPath) == false)
                File.Copy(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Settings\DefaultProjectSettings.txt"), setttingsPath);

            try
            {
                Header.Background = new BrushConverter().ConvertFromString(GetSettings("ProjectColor")) as SolidColorBrush;
            }
            catch
            {
                if ((MessageBox.Show("Nastala chyba v nastavení. Chcete obnovit výchozí nastavení?", "Chybné nastavení", MessageBoxButton.YesNo, MessageBoxImage.Error)) == MessageBoxResult.Yes)
                {
                    File.Delete(setttingsPath);
                    File.Copy(System.IO.Path.Combine(Environment.CurrentDirectory, "Settings", "DefaultProjectSettings.txt"), setttingsPath);

                    Header.Background = new BrushConverter().ConvertFromString(GetSettings("ProjectColor")) as SolidColorBrush;
                }
            }

            LoadProjectTab();
        }

        ///METODY
        //Metoda pro zjistění nastavení zadané položky
        private string GetSettings(string settings)
        {
            StreamReader stream = new StreamReader(setttingsPath);
            string line;
            string value = "False";

            while ((line = stream.ReadLine()) != null)
            {
                if (line.Contains(settings + "="))
                    value = line.Substring(settings.Length + 1);
            }

            stream.Close();

            return value;
        }

        //Metoda pro vyvolání základní chybová hláška
        public void Error()
        {
            MessageBox.Show("Nastala chyba prosím opakujte", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        //Metoda pro návrat na MainPage
        private void Back()
        {
            projectTab.StopWatchStop();
            NavigationService.Navigate(new ProjectListPage());
        }

        //Metoda pro načtení ProjectTab
        public void LoadProjectTab()
        {
            projectTab = new ProjectTab(ProjectNameLabel.Text, this);
            ProjectFrame.NavigationService.Navigate(projectTab);
        }

        ///EVENTY
        //Návrat na hlavní stránku
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Back();
        }

        //Po dvojitém klinutí na název projektu dovolí změnu názvu
        private void ProjectNameLabel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            projectName = ProjectNameLabel.Text.ToString();

            ProjectNameLabel.IsReadOnly = false;

            ProjectNameLabel.SelectionStart = projectName.Length;
        }

        //Uloží nový název projektu pomocí enteru
        private void ProjectNameLabel_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                
                if (e.Key == Key.Enter)
                {
                    string[] excludeCharacters = new string[] { @"\", "/", ":", "*", "?", "\"", "<", ">", "|" };
                    bool excludedExist = false;

                    for (int i = 0; i < 9; i++)
                    {
                        if (ProjectNameLabel.Text.Contains(excludeCharacters[i]))
                            excludedExist = true;
                    }

                    if (!excludedExist)
                    {
                        projectTab.StopWatchStop();
                        Directory.Move(System.IO.Path.Combine(appPath, "Projekty", projectName), System.IO.Path.Combine(appPath, "Projekty", ProjectNameLabel.Text.ToString()));
                        Keyboard.ClearFocus();
                        ProjectNameLabel.IsReadOnly = true;
                        LoadProjectTab();
                        projectTab.StartWatchStop();
                    }
                    else
                    {
                        MessageBox.Show(@"Název todoListu obsahuje jeden nebo více zakázaných znaků ( \, /, :, *, ?, " + "\", <, >, | )");
                    }

                }
            }
            catch
            {
                Error();
            }

        }

        //Aktualizuje ProjectList
        private void ProjectTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (projectTab != null)
                projectTab.LoadNotes();     
        }

        //Aktualizuje název upravovaného TabItemu
        public void UpdateTabItem(string name)
        {
            TabItem tabItem = ProjectTabControl.SelectedItem as TabItem;

            StackPanel stackPanel = new StackPanel
            {
                Height = 25,
                Name = "Stackpanel",
                Orientation = Orientation.Horizontal
            };

            Label label = new Label
            {
                Content = name,
                VerticalAlignment = VerticalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                FontSize = 12,
            };

            Button button = new Button
            {
                Width = 15,
                Height = 15,
                VerticalAlignment = VerticalAlignment.Center,
                Tag = tabItem.Name,

                Content = new Image
                {
                    Source = new BitmapImage(new Uri(@"UI\Buttons\TabItem (15x15)\closeButton.png", UriKind.RelativeOrAbsolute)),
                    
                }
            };
            button.Click += new RoutedEventHandler(projectTab.CloseButton_Click);

            stackPanel.Children.Add(label);
            stackPanel.Children.Add(button);

            tabItem.Header = stackPanel;
            HomePage.Header = "HomePage";
        }

        public void CloseTabItem(string name)
        {
            TabItem tabItem = ProjectTabControl.FindName(name) as TabItem;
            ProjectTabControl.Items.Remove(tabItem);
        }

        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            //Přehazovaní pomocí CTRL+TAB
            if (e.Key == Key.Tab && (Keyboard.Modifiers == ModifierKeys.Control))
                ProjectTabControl.SelectedIndex = ProjectTabControl.SelectedIndex + 1;

            //Zavírání pomocí CTRL+W
            if ((e.Key == Key.W && (Keyboard.Modifiers == ModifierKeys.Control)) && (ProjectTabControl.SelectedItem as TabItem).Name.ToString() != "HomePage")
                ProjectTabControl.Items.RemoveAt(ProjectTabControl.SelectedIndex);
        }
    }
}