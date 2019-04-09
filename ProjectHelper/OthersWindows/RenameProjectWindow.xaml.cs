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
    /// Interakční logika pro RenameProjectWindow.xaml
    /// </summary>
    public partial class RenameProjectWindow : Window
    {
        string appPath = Environment.CurrentDirectory;
        string name;
        string type;
        string projectName;
        string subfolderName;

        public RenameProjectWindow(string originalName, string objectType, string project, string subfolder)
        {
            try
            {
                InitializeComponent();

                type = objectType;
                name = originalName;
                projectName = project;
                subfolderName = subfolder;

                //Nastavení focusu a selectu na jméno projektu/poznámky
                NameBox.Text = name;
                NameBox.Focus();
                NameBox.SelectAll();
            }
            catch
            {
                Error();
                this.Close();
            }
        }

        //Po kliknutí přejmenuje projekt/poznámku
        private void RenameButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] excludeCharacters = new string[] { @"\", "/", ":", "*", "?", "\"", "<", ">", "|" };
                bool excludedExist = false;

                for (int i = 0; i < 9; i++)
                {
                    if (NameBox.Text.Contains(excludeCharacters[i]))
                        excludedExist = true;
                }

                if (!excludedExist)
                {
                    //Přejmenování projetu
                    if (type == "project")
                {
                    Directory.Move(System.IO.Path.Combine(appPath, "Projekty", name), System.IO.Path.Combine(appPath, "Projekty", NameBox.Text));
                    MessageBox.Show("Projekt byl úspěšně přejmenován", "Úspěch");
                    this.Close();
                }
                //Přejmenování poznámky
                else if(type == "note")
                {
                    if(subfolderName == null)
                        File.Move(System.IO.Path.Combine(appPath, "Projekty", projectName,"Poznámky","Notes",name + ".txt"), System.IO.Path.Combine(appPath, "Projekty",projectName,"Poznámky","Notes", NameBox.Text + ".txt"));
                    else
                        File.Move(System.IO.Path.Combine(appPath, "Projekty", projectName, "Poznámky", subfolderName, "Notes",name + ".txt"), System.IO.Path.Combine(appPath, "Projekty", projectName, "Poznámky",subfolderName,"Notes", NameBox.Text + ".txt"));

                    MessageBox.Show("Poznámky byla úspěšně přejmenována", "Úspěch");
                    this.Close();
                }
                //Přejmenování todo listu
                else if(type == "todo")
                {
                    if (subfolderName == null)
                        File.Move(System.IO.Path.Combine(appPath, "Projekty", projectName, "Poznámky", "Todo", name + ".txt"), System.IO.Path.Combine(appPath, "Projekty", projectName, "Poznámky", "Todo", NameBox.Text + ".txt"));
                    else
                        File.Move(System.IO.Path.Combine(appPath, "Projekty", projectName, "Poznámky", subfolderName, "Todo",name + ".txt"), System.IO.Path.Combine(appPath, "Projekty", projectName, "Poznámky", subfolderName, "Todo", NameBox.Text + ".txt"));

                    MessageBox.Show("Podsložka byla úspěšně přejmenována", "Úspěch");
                    this.Close();
                }
                //Přejmenování subfolderu
                else if (type == "subfolder")
                {
                    Directory.Move(System.IO.Path.Combine(appPath, "Projekty", projectName,"Poznámky",name), System.IO.Path.Combine(appPath, "Projekty", projectName, "Poznámky", NameBox.Text));
                    MessageBox.Show("Podsložka byla úspěšně přejmenována", "Úspěch");
                    this.Close();
                }
                }
                else
                {
                    MessageBox.Show(@"Název todoListu obsahuje jeden nebo více zakázaných znaků ( \, /, :, *, ?, " + "\", <, >, | )");
                }
            }
            catch
            {
                Error();
            }
        }

        //Metoda pro vyvolání jednoduché chybové hlášky
        private void Error()
        {
            MessageBox.Show("Nastala chyba prosím opakujte", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
