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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProjectHelper
{
    /// <summary>
    /// Interakční logika pro ProjectSettingsWindow.xaml
    /// </summary>
    public partial class ProjectSettingsWindow : Window
    {
        string settingPath;
        ColorDialog colorDialog;

        public ProjectSettingsWindow(string projectName)
        {
            try
            {
                InitializeComponent();

                settingPath = System.IO.Path.Combine(Environment.CurrentDirectory, "Projekty", projectName, "Settings.txt");

                LoadSettings();

                //Vytváří ColorDialog
                string line;
                StreamReader stream = new StreamReader(System.IO.Path.Combine(Environment.CurrentDirectory, "Settings", "CustomColor.txt"));
                List<int> customColorsList = new List<int>();

                while ((line = stream.ReadLine()) != null)
                    customColorsList.Add(int.Parse(line));

                int[] customColors = customColorsList.ToArray();

                colorDialog = new ColorDialog
                {
                    AllowFullOpen = true,
                    FullOpen = true,
                    CustomColors = customColors
                };

                stream.Close();
            }
            catch
            {
                if ((System.Windows.MessageBox.Show("Nastala chyba v nastavení. Chcete obnovit výchozí nastavení?", "Chybné nastavení", MessageBoxButton.YesNo, MessageBoxImage.Error)) == MessageBoxResult.Yes)
                {
                    File.Delete(settingPath);
                    File.Copy(System.IO.Path.Combine(Environment.CurrentDirectory, "Settings", "DefaultProjectSettings.txt"), settingPath);
                }
                else
                {
                    Error();
                    Close();
                }
            }
        }

        //Metoda pro vyvolání jednoduché chybové hlášky
        private void Error()
        {
            System.Windows.MessageBox.Show("Nastala chyba prosím opakujte", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        //Metoda pro uložení nastavení
        private void SaveSettings()
        {
            File.Delete(settingPath);
            File.AppendAllText(settingPath, "TimerOn=" + TimerOn.IsChecked.ToString() + Environment.NewLine);
            File.AppendAllText(settingPath, "TimerVisibility=" + TimerVisibility.IsChecked.ToString() + Environment.NewLine);
            File.AppendAllText(settingPath, "SubfoldersExpanded=" + SubfoldersExpanded.IsChecked.ToString() + Environment.NewLine);
            File.AppendAllText(settingPath, "ProjectColor=" + ProjectColor.Background.ToString() + Environment.NewLine);
            File.AppendAllText(settingPath, "ToolbarVisibility=" + ToolbarVisibility.IsChecked.ToString() + Environment.NewLine);
        }

        private void TestSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveSettings();
                Close();
            }
            catch
            {
                Error();
            }
        }

        //Metoda pro načtení aktuálně uloženého nastavení
        private void LoadSettings()
        {
            StreamReader stream = new StreamReader(settingPath);
            string line;

            while ((line = stream.ReadLine()) != null)
            {
                string name = "";
                int i = 0;

                while (line[i] != '=')
                {
                    name += line[i];
                    i++;
                }

                string status = "";

                for (int j = i + 1; j < line.Length; j++)
                    status += line[j];

                try
                {
                    var item = this.FindName(name);
                    System.Windows.Controls.CheckBox checkBox = item as System.Windows.Controls.CheckBox;

                    checkBox.IsChecked = bool.Parse(status);
                }
                catch
                { }

                if (name == "ProjectColor")
                    ProjectColor.Background = new BrushConverter().ConvertFromString(status) as SolidColorBrush;
            }

            stream.Close();
        }

        //Pokud uživatel vypne visibilitu timeru vypne i jeho počítání
        private void TimerVisibility_Unchecked(object sender, RoutedEventArgs e)
        {
            TimerOn.IsChecked = false;
        }

        //Pokud uživatel vypne visibilitu timeru vypne i jeho počítání
        private void TimerOn_Click(object sender, RoutedEventArgs e)
        {
            if (TimerVisibility.IsChecked == false)
                TimerOn.IsChecked = false;
        }

        //Otevře ColorPicker pro výběr barvy
        private void ProjectColor_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var results = colorDialog.ShowDialog();

                if (results == System.Windows.Forms.DialogResult.OK)
                {                 
                    SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
                    ProjectColor.Background = brush;
                    SaveCustomColor();
                }
            }
            catch
            {
                Error();
            }
        }

        //Ukládá CustomColors nastavené uživatelem
        void SaveCustomColor()
        {
            int[] customColors = colorDialog.CustomColors;
            File.Delete(System.IO.Path.Combine(Environment.CurrentDirectory, "Settings", "CustomColor.txt"));

            foreach (int customColor in customColors)
                File.AppendAllText(System.IO.Path.Combine(Environment.CurrentDirectory, "Settings", "CustomColor.txt"), customColor.ToString() + Environment.NewLine);
        }

        private void StornoButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            if ((System.Windows.MessageBox.Show("Chcete obnovit výchozí nastavení?", "Chybné nastavení", MessageBoxButton.YesNo, MessageBoxImage.Question)) == MessageBoxResult.Yes)
            {
                File.Delete(settingPath);
                File.Copy(System.IO.Path.Combine(Environment.CurrentDirectory, "Settings", "DefaultProjectSettings.txt"), settingPath);
                LoadSettings();
            }
        }
    }
}
