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
    /// Interaction logic for ProjectTab.xaml
    /// </summary>
    public partial class ProjectTab : Page
    {
        //PROMĚNÉ
        string appPath;
        string notePath;
        string projectPath;
        string projectName;
        string setttingsPath;
        string errorType;
        List<string> tabName = new List<string>();
        ProjectPage projectPage;
        Default @default = new Default();
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        ProjectStopWatch stopWatch;
        public string newThingType;

        //Hlavní metoda
        public ProjectTab(string project, ProjectPage mainPage)
        {
            try
            {
                InitializeComponent();

                projectName = project;
                projectPage = mainPage;

                //Vytvoří cesty
                appPath = Environment.CurrentDirectory;
                notePath = System.IO.Path.Combine(appPath, "Projekty", projectName, "Poznámky");
                setttingsPath = System.IO.Path.Combine(appPath, "Projekty", projectName, "Settings.txt");
                projectPath = System.IO.Path.Combine(Environment.CurrentDirectory, "Projekty", project);

                //Načte info a popis projektu a čas strávený prací 
                DescriptionBox.Text = File.ReadAllText(System.IO.Path.Combine(appPath, "Projekty", projectName, "Description.txt"), Encoding.GetEncoding("Windows-1250"));
                DateLabel.Content = File.ReadAllText(System.IO.Path.Combine(appPath, "Projekty", projectName, "Info.txt"));

                NoteList.Focus();

                //Timer
                errorType = "LoadTimer";
                string elapsedTime = File.ReadAllText(System.IO.Path.Combine(appPath, "Projekty", projectName, "Time.txt"));
                ProjectTime.Content = elapsedTime;
                stopWatch = new ProjectStopWatch(elapsedTime);

                dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 1);
                dispatcherTimer.Start();

                errorType = "LoadSettings";
                LoadAllSettings();

                errorType = "LoadNotes";
                LoadNotes();
            }
            catch
            {
                @default.Error(errorType, projectPath);
            }

            errorType = "";
        }

        ///METODY - OBECNÉ///

        //Metoda pro návrat na MainPage
        private void Back()
        {
            StopWatchStop();
            NavigationService.Navigate(new ProjectListPage());
        }

        //Metoda pro vrací Parent TreeViewItemu
        public TreeViewItem ReturnParent(TreeViewItem child)
        {
            try
            {
                var parent = VisualTreeHelper.GetParent(child as DependencyObject);

                while ((parent as TreeViewItem) == null)
                    parent = VisualTreeHelper.GetParent(parent);

                    return parent as TreeViewItem;
            }
            catch
            {
                return null;
            }
        }

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

        //Načte nastavení pro timer
        private void LoadTimerSettings()
        {
            if (File.Exists(setttingsPath))
            {
                if (bool.Parse(GetSettings("TimerOn")))
                {
                    stopWatch.StartStopWatch();
                    StopWatchButton.IsChecked = true;
                }
                else
                {
                    stopWatch.StopStopWatch();
                    StopWatchButton.IsChecked = false;
                }

                if (!bool.Parse(GetSettings("TimerVisibility")))
                {
                    ProjectTime.Visibility = Visibility.Hidden;
                    ProjectTimeLable.Visibility = Visibility.Hidden;
                    StopWatchButton.Visibility = Visibility.Hidden;
                }
                else
                {
                    ProjectTime.Visibility = Visibility.Visible;
                    ProjectTimeLable.Visibility = Visibility.Visible;
                    StopWatchButton.Visibility = Visibility.Visible;
                }
            }
        }

        //Načte všechny nastavení
        private void LoadAllSettings()
        {
            LoadTimerSettings();

            LoadNotes();

            if (bool.Parse((GetSettings("ToolbarVisibility"))))
                Toolbar.Visibility = Visibility.Visible;
            else
                Toolbar.Visibility = Visibility.Hidden;

        }

        public void StopWatchStop()
        {
            dispatcherTimer.Stop();
            stopWatch.StopStopWatch();
        }

        public void StartWatchStop()
        {
            dispatcherTimer.Start();
            stopWatch.StartStopWatch();
        }

        ///EVENTY - OBECNÉ///

        //Když se změní popis uloží se
        private void DescriptionBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            File.Delete(System.IO.Path.Combine(appPath, "Projekty", projectName, "Description.txt"));
            File.AppendAllText(System.IO.Path.Combine(appPath, "Projekty", projectName, "Description.txt"), DescriptionBox.Text, Encoding.GetEncoding("Windows-1250"));
        }

        //Event Handler pro zavření okno s novou podsložkou
        private void NewSubfolderClosed(object sender, EventArgs e)
        {
            try
            {
                errorType = "LoadNotes";
                LoadNotes();
                Application.Current.MainWindow.Activate();
            }
            catch
            {
                @default.Error(errorType, projectPath);
            }

            errorType = "";
        }

        //Každou sekundu volá ProjectStopWatch kvůli aktualizaci času
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            ProjectTime.Content = stopWatch.GetCurrentTime();
            File.WriteAllText(System.IO.Path.Combine(appPath, "Projekty", projectName, "Time.txt"), ProjectTime.Content.ToString());
        }

        //Zastavuje a spouští stopky
        private void StopWatchButton_Click(object sender, RoutedEventArgs e)
        {
            if (StopWatchButton.IsChecked == true)
                stopWatch.StartStopWatch();
            else
                stopWatch.StopStopWatch();
        }

        //Otevře nastavení projektu
        private void ProjectSettings_Click(object sender, RoutedEventArgs e)
        {
            ProjectSettingsWindow projectSettings = new ProjectSettingsWindow(projectName);
            projectSettings.Closed += new EventHandler(ProjectSettingsClosed);
            projectSettings.Owner = Application.Current.MainWindow;
            projectSettings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            projectSettings.Show();
        }

        //Event Handler pro zaveření okna s nastavením
        private void ProjectSettingsClosed(object sender, EventArgs e)
        {
            try
            {
                errorType = "LoadSettings";
                LoadAllSettings();
                Application.Current.MainWindow.Activate();
            }
            catch
            {
                @default.Error(errorType, projectPath);
            }

            errorType = "";
        }

        //Volá metodu na ProjectPage která zavře zvolené okno
        public void CloseButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            projectPage.CloseTabItem(button.Tag.ToString());
        }

        //Nová věc pomocí CTRL+N
        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.N && (Keyboard.Modifiers == ModifierKeys.Control))
            {
                NewThingWindow newThingWindow = new NewThingWindow(this);
                newThingWindow.Owner = Application.Current.MainWindow;
                newThingWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                newThingWindow.Closed += new EventHandler(NewThingWindowClosed);
                newThingWindow.Show();
            }
        }

        //Event handler pro zavření okna NewThingWindow
        private void NewThingWindowClosed(object sender, EventArgs e)
        {
            switch(newThingType)
            {
                case "Note":
                    NewNote();
                    break;

                case "Todo":
                    NewTodo();
                    break;

                case "Subfolder":
                    NewSubfolder();
                    break;

                default:
                    break;

            }
        }

        ///METODY - NOTELIST///

        //Metoda pro načtení poznámek
        public void LoadNotes()
        {
            NoteList.Items.Clear();

            //Načte podsložky projektu
            string[] subfolders = Directory.GetDirectories(notePath);

            //Vloží název podsložky do NoteListu
            foreach (string subfolder in subfolders)
            {
                string subfolderName = subfolder.Replace(notePath, "");
                subfolderName = subfolderName.Remove(0, 1);

                if ((subfolderName != "Notes") && (subfolderName != "Todo"))
                {
                    TreeViewItem subfolderItem = NewTreeViewItem("subfolder", subfolderName);
                    NoteList.Items.Add(subfolderItem);

                    //Načte všechny poznámky do pole
                    string[] notesSubfolder = Directory.GetFiles(System.IO.Path.Combine(notePath, subfolderName, "Notes"));

                    //Vloží upravený název poznámky do NoteListu
                    foreach (string note in notesSubfolder)
                    {
                        string noteName = note.Replace(System.IO.Path.Combine(notePath, subfolderName, "Notes"), "");
                        noteName = noteName.Remove(0, 1);
                        noteName = noteName.Replace(".rtf", "");
                        subfolderItem.Items.Add(NewTreeViewItem("note", noteName));
                    }

                    //Načte všechny todo do pole
                    string[] todoesSubfolder = Directory.GetFiles(System.IO.Path.Combine(notePath, subfolderName, "Todo"));

                    //Vloží upravený název poznámky do NoteListu
                    foreach (string todo in todoesSubfolder)
                    {
                        string todoName = todo.Replace(System.IO.Path.Combine(notePath, subfolderName, "Todo"), "");
                        todoName = todoName.Remove(0, 1);
                        todoName = todoName.Replace(".txt", "");
                        subfolderItem.Items.Add(NewTreeViewItem("todo", todoName));
                    }
                }
            }

            //Načte všechny poznámky do pole
            string[] notes = Directory.GetFiles(System.IO.Path.Combine(notePath, "Notes"));

            //Vloží upravený název poznámky do NoteListu
            foreach (string note in notes)
            {
                string noteName = note.Replace(System.IO.Path.Combine(notePath, "Notes"), "");
                noteName = noteName.Remove(0, 1);
                noteName = noteName.Replace(".rtf", "");
                NoteList.Items.Add(NewTreeViewItem("note", noteName));
            }

            //Načte všechny poznámky do pole
            string[] todoes = Directory.GetFiles(System.IO.Path.Combine(notePath, "Todo"));

            //Vloží upravený název poznámky do NoteListu
            foreach (string todo in todoes)
            {
                string todoName = todo.Replace(System.IO.Path.Combine(notePath, "Todo"), "");
                todoName = todoName.Remove(0, 1);
                todoName = todoName.Replace(".txt", "");
                NoteList.Items.Add(NewTreeViewItem("todo", todoName));
            }
        }

        //Metoda pro vytvoření TreeviewItemu
        private TreeViewItem NewTreeViewItem(string type, string name)
        {
            Tags tags = new Tags();

            //Image
            Image icon = new Image();
            icon.Width = 16;
            icon.Height = 16;

            //Texblock
            TextBlock header = new TextBlock();
            header.Text = name;
            header.Margin = new Thickness(5, 0, 0, 0);

            //Treeviewitem
            TreeViewItem item = new TreeViewItem();
            item.Cursor = Cursors.Hand;
            tags.name = name;

            switch (type)
            {
                case "subfolder":
                    icon.Source = new BitmapImage(new Uri(@"UI\Buttons\Context Menu (20x20)\NewButtons (20x20)\newSubfolder.png", UriKind.RelativeOrAbsolute));
                    header.FontWeight = FontWeights.Bold;
                    tags.type = "Subfolder";
                    item.IsExpanded = bool.Parse(GetSettings("SubfoldersExpanded"));
                    break;

                case "note":
                    icon.Source = new BitmapImage(new Uri(@"UI\Buttons\Context Menu (20x20)\NewButtons (20x20)\newNote.png", UriKind.RelativeOrAbsolute));
                    tags.type = "Note";
                    break;

                case "todo":
                    icon.Source = new BitmapImage(new Uri(@"UI\Buttons\Context Menu (20x20)\NewButtons (20x20)\newTodo.png", UriKind.RelativeOrAbsolute));
                    tags.type = "Todo";
                    break;
            }

            //Stackpanel
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;
            stackPanel.Children.Add(icon);
            stackPanel.Children.Add(header);

            item.Header = stackPanel;
            item.Tag = tags;

            return item;
        }

        //Metoda pro otevření vybrané poznámky
        public void Open()
        {
            TreeViewItem selected = NoteList.SelectedItem as TreeViewItem;
            if (selected != null)
            {
                //Vybraná položka
                Tags tags = new Tags();
                tags = selected.Tag as Tags;
                string noteName = tags.name;

                //Subfolder
                TreeViewItem parent = ReturnParent(selected);
                Tags subfolderTags = new Tags();
                if (parent != null)
                    subfolderTags = parent.Tag as Tags;

                switch (tags.type)
                {
                    //Poznámka a todo
                    case "Note":
                    case "Todo":

                        //Parent existuje
                        if (parent != null)
                        {
                            string subfolderName = subfolderTags.name;
                            NewTab(noteName, subfolderName, tags.type);
                        }
                        //Parent neexistuje
                        else
                        {                           
                            NewTab(noteName, null, tags.type);
                        }
                        break;

                    //Subfolder
                    case "Subfolder":
                        if (selected.IsExpanded == true)
                            selected.IsExpanded = false;
                        else
                            selected.IsExpanded = true;
                            break;
                }
            }
        }

        //Metoda pro vytvoření TabItemu
        public void NewTab(string name, string subfolder, string type)
        {
            Random random = new Random();
            int randomID = 0;
            do
            {
                randomID = random.Next(0,1001);
            } while (tabName.Contains(randomID.ToString()));

            //Základní nastavení tabitemu
            TabItem tabItem = new TabItem
            {
                Height = 25,
                Name = "tab" + randomID.ToString(),
            };

            projectPage.ProjectTabControl.RegisterName(tabItem.Name, tabItem);

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
                Tag = tabItem.Name.ToString(),
                Content = new Image
                {
                    Source = new BitmapImage(new Uri(@"UI\Buttons\TabItem (15x15)\closeButton.png", UriKind.RelativeOrAbsolute))
                }
            };
            button.Click += new RoutedEventHandler(CloseButton_Click);

            stackPanel.Children.Add(label);
            stackPanel.Children.Add(button);

            tabItem.Header = stackPanel;

            Frame frame = new Frame();
            tabItem.Content = frame;

            switch (type)
            {
                case "Note":
                    NotePage notePage = new NotePage(name, notePath, projectName, subfolder, projectPage);
                    frame.NavigationService.Navigate(notePage);
                    break;

                case "Todo":
                    ToDoPage toDoPage = new ToDoPage(name, notePath, projectName, subfolder, projectPage);
                    frame.NavigationService.Navigate(toDoPage);
                    break;
            }

            projectPage.ProjectTabControl.Items.Add(tabItem);
            tabItem.IsSelected = true;
        }

        //Metoda pro vytvoření nové poznámky
        private void NewNote()
        {
            TreeViewItem treeViewItem = NoteList.SelectedItem as TreeViewItem;

            Tags tags = new Tags();
            if (treeViewItem != null)
                tags = treeViewItem.Tag as Tags;

            if ((treeViewItem != null) && ((tags.type == "Subfolder") || (ReturnParent(treeViewItem) != null)))
            {
                TreeViewItem parent = ReturnParent(treeViewItem);
                Tags subfolderTags = new Tags();

                if (parent != null)
                    subfolderTags = parent.Tag as Tags;
            else
                subfolderTags = treeViewItem.Tag as Tags;

                NewTab(null, subfolderTags.name, "Note");
            }
            else
            {
                NewTab(null, null, "Note");
            }
        }

        //Metoda pro vytvoření nového todo listu
        private void NewTodo()
        {
            TreeViewItem treeViewItem = NoteList.SelectedItem as TreeViewItem;
            Tags tags = new Tags();
            if (treeViewItem != null)
                tags = treeViewItem.Tag as Tags;

            if ((treeViewItem != null) && ((tags.type == "Subfolder") || (ReturnParent(treeViewItem) != null)))
            {
                TreeViewItem parent = ReturnParent(treeViewItem);
                Tags subfolderTags = new Tags();

                if (parent != null)
                    subfolderTags = parent.Tag as Tags;
                else
                    subfolderTags = treeViewItem.Tag as Tags;

                NewTab(null, subfolderTags.name, "Todo");
            }
            else
                NewTab(null, null, "Todo");
        }

        //Metoda pro vytvoření nového subfolderu
        private void NewSubfolder()
        {
            NewSubfolderWindow newSubfolder = new NewSubfolderWindow(projectName);
            newSubfolder.Closed += new EventHandler(NewSubfolderClosed);
            newSubfolder.Owner = Application.Current.MainWindow;
            newSubfolder.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            newSubfolder.Show();
        }

        //Metoda pro smazání vybrané věci
        private void DeleteThing()
        {
            TreeViewItem child = NoteList.SelectedItem as TreeViewItem;
            if (child != null)
            {
                //Vybraná složka
                Tags tags = new Tags();
                tags = child.Tag as Tags;
                string noteName = tags.name;

                //Podsložka
                TreeViewItem parent = ReturnParent(child);
                Tags subfolderTags = new Tags();
                if (parent != null)
                    subfolderTags = parent.Tag as Tags;

                //Note
                if (tags.type == "Note")
                {
                    if (parent == null)
                        File.Delete(System.IO.Path.Combine(appPath, "Projekty", projectName, "Poznámky", "Notes", noteName + ".rtf"));
                    else
                    {
                        string subfolderName = subfolderTags.name;
                            File.Delete(System.IO.Path.Combine(appPath, "Projekty", projectName, "Poznámky", subfolderName, "Notes", noteName + ".rtf"));
                    }
                }
                //Todo list
                else if (tags.type == "Todo")
                {
                    if (parent == null)
                        File.Delete(System.IO.Path.Combine(appPath, "Projekty", projectName, "Poznámky", "Todo", noteName + ".txt"));
                    else
                    {
                        string subfolderName = subfolderTags.name;
                        File.Delete(System.IO.Path.Combine(appPath, "Projekty", projectName, "Poznámky", subfolderName, "Todo", noteName + ".txt"));
                    }
                }
                //Subfolder
                else
                     Directory.Delete(System.IO.Path.Combine(appPath, "Projekty", projectName, "Poznámky", noteName), true);

                LoadNotes();
            }
        }

        ///EVENTY - NOTELIST///          

        //Otevře vybranou poznáku
        public void NoteList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                errorType = "OpenNote";
                Open();
            }
            catch
            {
                @default.Error(errorType, projectPath);
            }

            errorType = "";
        }

        //Otevře označenou poznámku pomocí enteru nebo ji smaže pomocí delete
        private void NoteList_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                //Otevírá vybranou poznámku pomocí enteru
                if (e.Key == Key.Enter)
                {
                    errorType = "OpenNote";
                    Open();
                }

                //Maže vybranou poznámku pomocí delete
                if (e.Key == Key.Delete)
                {
                    errorType = "DeleteNote";
                    DeleteThing();
                }
            }
            catch
            {
                @default.Error(errorType, projectPath);
            }

            errorType = "";
        }

        //Označí položku na kterou uživatel kliknul pravým tlačítkem
        private void TreeViewItem_MouseRightButtonDown(object sender, MouseEventArgs e)
        {
            TreeViewItem item = VisualUpwardSearch(e.OriginalSource as DependencyObject);
            if (item != null)
            {
                item.Focus();
                e.Handled = true;
            }
        }

        //Označí položku na kterou uživatel kliknul pravým tlačítkem
        static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }

        //Smaže vybranou poznámku/podsložku přes kontextové menu
        private void DeleteNote_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                errorType = "DeleteNote";
                DeleteThing();
            }
            catch
            {
                @default.Error(errorType, projectPath);
            }

            errorType = "";
        }

        //Zkopíruje vybranou poznámku/podsložku přes kontextové menu
        private void CopyNote_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TreeViewItem child = NoteList.SelectedItem as TreeViewItem;
                if (child != null)
                {
                    errorType = "CopyNote";
                    //Vybraná položka
                    Tags tags = new Tags();
                    tags = child.Tag as Tags;
                    string noteName = tags.name;

                    //Podsložka
                    TreeViewItem parent = ReturnParent(child);
                    Tags subfolderTags = new Tags();
                    if (parent != null)
                        subfolderTags = parent.Tag as Tags;


                    //Note
                    if (tags.type == "Note")
                    {
                        if (parent == null)
                            File.Copy(System.IO.Path.Combine(appPath, "Projekty", projectName, "Poznámky", "Notes", noteName + ".rtf"), System.IO.Path.Combine(appPath, "Projekty", projectName, "Poznámky", "Notes", noteName + "-kopie.rtf"));
                        else
                        {
                            string subfolderName = subfolderTags.name;
                            File.Copy(System.IO.Path.Combine(appPath, "Projekty", projectName, "Poznámky", subfolderName, "Notes", noteName + ".rtf"), System.IO.Path.Combine(appPath, "Projekty", projectName, "Poznámky", subfolderName, "Notes", noteName + "-kopie.rtf"));
                        }
                    }
                    //Todo List
                    else if (tags.type == "Todo")
                    {
                        if (parent == null)
                            File.Copy(System.IO.Path.Combine(appPath, "Projekty", projectName, "Poznámky", "Todo", noteName + ".txt"), System.IO.Path.Combine(appPath, "Projekty", projectName, "Poznámky", "Todo", noteName + "-kopie.txt"));
                        else
                        {
                            string subfolderName = subfolderTags.name;
                            File.Copy(System.IO.Path.Combine(appPath, "Projekty", projectName, "Poznámky", subfolderName, "Todo", noteName + ".txt"), System.IO.Path.Combine(appPath, "Projekty", projectName, "Poznámky", subfolderName, "Todo", noteName + "-kopie.txt"));
                        }
                    }

                    errorType = "LoadNotes";
                    LoadNotes();
                }
            }
            catch
            {
                @default.Error(errorType, projectPath);
            }

            errorType = "";
        }

        //Přejmenuje poznámky/podsložku přes kontextové menu
        private void RenameNote_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                errorType = "RenameNote";

                TreeViewItem child = NoteList.SelectedItem as TreeViewItem;
                if (child != null)
                {
                    //Vybraná položka
                    Tags tags = new Tags();
                    tags = child.Tag as Tags;
                    string noteName = tags.name;

                    //Podsložka
                    TreeViewItem parent = ReturnParent(child);
                    Tags subfolderTags = new Tags();
                    if (parent != null)
                        subfolderTags = parent.Tag as Tags;

                    //Ostatní
                    string project = projectName;
                    RenameProjectWindow renameProject = null;

                    //Note
                    if (tags.type == "Note")
                    {
                        if (parent == null)
                            renameProject = new RenameProjectWindow(noteName, "note", project, null);
                        else
                        {
                            string subfolderName = subfolderTags.name;
                            renameProject = new RenameProjectWindow(noteName, "note", project, subfolderName);
                        }
                    }
                    //Todo List
                    else if (tags.type == "Todo")
                    {
                        if (parent == null)
                            renameProject = new RenameProjectWindow(noteName, "todo", projectName, null);
                        else
                        {
                            string subfolderName = subfolderTags.name;
                            renameProject = new RenameProjectWindow(noteName, "todo", projectName, subfolderName);
                        }
                    }
                    //Subfolder
                    else
                        renameProject = new RenameProjectWindow(noteName, "subfolder", project, null);


                    renameProject.Closed += new EventHandler(RenameProjectClosed);
                    renameProject.Owner = Application.Current.MainWindow;
                    renameProject.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    renameProject.Show();
                }
            }
            catch
            {
                @default.Error(errorType, projectPath);
            }

            errorType = "";
        }

        //Event Handler pro zavření okna pro přejmenování poznámky
        private void RenameProjectClosed(object sender, System.EventArgs e)
        {
            try
            {
                errorType = "LoadNotes";
                LoadNotes();
                Application.Current.MainWindow.Activate();
            }
            catch
            {
                @default.Error(errorType, projectPath);
            }

            errorType = "";
        }

        //Vytvoří novou poznámku přes kontextové menu
        private void NewNote_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                errorType = "NewNote";
                NewNote();
            }
            catch
            {
                @default.Error(errorType, projectPath);
            }

            errorType = "";
        }

        //Vytvoří nový todo list přes kontextové menu
        private void NewTodo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                errorType = "NewNote";
                NewTodo();
            }
            catch
            {
                @default.Error(errorType, projectPath);
            }

            errorType = "";
        }

        //Vytvoří novou podsložku přes kontextové menu
        private void NewSubfolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                errorType = "NewNote";
                NewSubfolder();
            }
            catch
            {
                @default.Error(errorType, projectPath);
            }

            errorType = "";
        }

        //Nová poznámka přes dockpanel
        private void NewNoteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                errorType = "NewNote";
                NewNote();                
            }
            catch
            {
                @default.Error(errorType, projectPath);
            }

            errorType = "";
        }

        //Nový todo list přes dockpanel
        private void NewToDoButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                errorType = "NewNote";
                NewTodo();
            }
            catch
            {
                @default.Error(errorType, projectPath);
            }

            errorType = "";
        }

        //Nový subfolder přes dockpanel
        private void NewSubfolderButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                errorType = "NewNote";
                NewSubfolder();
            }
            catch
            {
                @default.Error(errorType, projectPath);
            }

            errorType = "";
        }

        //Smazat přes dockpanel
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                errorType = "NewNote";
                DeleteThing();
            }
            catch
            {
                @default.Error(errorType, projectPath);
            }

            errorType = "";
        }
    }
}