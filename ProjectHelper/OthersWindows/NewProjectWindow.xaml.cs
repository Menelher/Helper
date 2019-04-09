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
using System.Windows.Shapes;

namespace ProjectHelper
{
    /// <summary>
    /// Interakční logika pro NewProjectWindow.xaml
    /// </summary>
    public partial class NewProjectWindow : Window
    {
        string appPath = Environment.CurrentDirectory;

        public NewProjectWindow()
        {
            InitializeComponent();

            ProjectName.Focus();
        }

        //Vytvoří nový projekt
        public void CreateProjectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Pokud projekt obsahuje název uloží ho
                if (string.IsNullOrEmpty(ProjectName.Text) == false)
                {
                    if (string.IsNullOrEmpty(ProjectDescription.Text) == false)
                    {
                        SaveProject();
                    }
                    else
                    {
                        if (MessageBox.Show("Nebyl zadán popis, chcete pokračovat", "Chybějící popis", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        {
                            SaveProject();   
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Projekt nelze vytvořit bez názvu", "Chybějící název", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch
            {
                MessageBox.Show("Při vytváření projektu nastala chyba, prosím opakujte", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //Zavře okno
        private void StornoProjectButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //Uloží projekt
        private void SaveProject()
        {
            string[] excludeCharacters = new string[] { @"\", "/", ":", "*", "?", "\"", "<", ">", "|" };
            bool excludedExist = false;

            for (int i = 0; i < 9; i++)
            {
                if (ProjectName.Text.Contains(excludeCharacters[i]))
                    excludedExist = true;
            }

            if (!excludedExist)
            {
                Directory.CreateDirectory(System.IO.Path.Combine(appPath, "Projekty", ProjectName.Text));
            Directory.CreateDirectory(System.IO.Path.Combine(appPath, "Projekty", ProjectName.Text, "Poznámky"));
            Directory.CreateDirectory(System.IO.Path.Combine(appPath, "Projekty", ProjectName.Text, "Poznámky","Notes"));
            Directory.CreateDirectory(System.IO.Path.Combine(appPath, "Projekty", ProjectName.Text, "Poznámky","Todo"));
            File.AppendAllText(System.IO.Path.Combine(appPath, "Projekty", ProjectName.Text, "Description.txt"), ProjectDescription.Text, Encoding.GetEncoding("Windows-1250"));

            //Uložení informací o projektu
            var date = DateTime.Now;
            string info = date.ToShortDateString();
            File.AppendAllText(System.IO.Path.Combine(appPath,"Projekty",ProjectName.Text,"Info.txt"),info,Encoding.GetEncoding("Windows-1250"));

            MessageBox.Show("Projekt byl úspěšně vytvořen", "Projekt vytvořen", MessageBoxButton.OK);
            this.Close();
            }
            else
            {
                MessageBox.Show(@"Název todoListu obsahuje jeden nebo více zakázaných znaků ( \, /, :, *, ?, " + "\", <, >, | )");
            }
        }
    }
}
