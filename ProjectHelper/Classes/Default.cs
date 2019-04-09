using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProjectHelper
{
    class Default
    {
        public void Error(string type, string projectPath)
        {
            switch (type)
            {
                case "LoadTimer":
                    if ((MessageBox.Show("Nastala chyba při načítání času projektu. Checete vymazat čas projektu?", "Chyba při načítání času", MessageBoxButton.YesNo, MessageBoxImage.Error)) == MessageBoxResult.Yes)
                        File.WriteAllText(Path.Combine(projectPath, "Time.txt"), "00:00:00:00");
                    break;

                case "LoadSettings":
                    if ((MessageBox.Show("Nastala chyba při načítání nastavení projektu. Chcete nahrát defaultní nastavení pro projekty?", "Chyba při načítání nastavení", MessageBoxButton.YesNo, MessageBoxImage.Error)) == MessageBoxResult.Yes)
                        File.Copy(Path.Combine(Environment.CurrentDirectory, "Settings", "DefaultProjectSettings.txt"), Path.Combine(projectPath, "Settings.txt"));
                    break;

                case "LoadNotes":
                    MessageBox.Show("Nastala chyba při načítání souborů. Zkuste opakovat akci.", "Chyba při načítání souboru", MessageBoxButton.OK,MessageBoxImage.Error);
                    break;

                case "OpenNote":
                    MessageBox.Show("Nastala chyba při otevírání souboru. Zkuste opakovat akci.", "Chyba při otevírání souboru", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;

                case "DeleteNote":
                    MessageBox.Show("Nastala chyba při mazání souboru. Zkuste opakovat akci.", "Chyba při mazání souboru", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;

                case "CopyNote":
                    MessageBox.Show("Nastala chyba při kopírování souboru. Zkuste opakovat akci.", "Chyba při kopírování souboru", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;

                case "RenameNote":
                    MessageBox.Show("Nastala chyba při přejmenovávaní souboru. Zkuste opakovat akci.", "Chyba při přejmenovávání souboru", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;

                case "NewNote":
                    MessageBox.Show("Nastala chyba při vytváření nového souboru. Zkuste opakovat akci.", "Chyba při vytváření souboru", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;

                default:
                    MessageBox.Show("Nastala chyba. Zkuste opakovat akci.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
        }
    }
}
