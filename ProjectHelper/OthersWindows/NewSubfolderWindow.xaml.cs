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
    /// Interakční logika pro NewSubfolderWindow.xaml
    /// </summary>
    public partial class NewSubfolderWindow : Window
    {
        string project;

        public NewSubfolderWindow(string projectName)
        {
            InitializeComponent();

            project = projectName;
            SubfolderName.Focus();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            string[] excludeCharacters = new string[] { @"\", "/", ":", "*", "?", "\"", "<", ">", "|" };
            bool excludedExist = false;

            for (int i = 0; i < 9; i++)
            {
                if (SubfolderName.Text.Contains(excludeCharacters[i]))
                    excludedExist = true;
            }

            if (!excludedExist)
            {
                Directory.CreateDirectory(System.IO.Path.Combine(Environment.CurrentDirectory, "Projekty", project, "Poznámky", SubfolderName.Text));
            Directory.CreateDirectory(System.IO.Path.Combine(Environment.CurrentDirectory, "Projekty", project, "Poznámky", SubfolderName.Text, "Notes"));
            Directory.CreateDirectory(System.IO.Path.Combine(Environment.CurrentDirectory, "Projekty", project, "Poznámky", SubfolderName.Text, "Todo"));
            MessageBox.Show("Podsložka úspěšně vytvořena", "Podsložka vytvořena");
            this.Close();
            }
            else
            {
                MessageBox.Show(@"Název todoListu obsahuje jeden nebo více zakázaných znaků ( \, /, :, *, ?, " + "\", <, >, | )");
            }
        }

        private void StronoButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
