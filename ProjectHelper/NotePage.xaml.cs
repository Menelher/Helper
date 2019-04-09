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
    /// Interakční logika pro NotePage.xaml
    /// </summary>
    public partial class NotePage : Page
    {
        string notePath;
        string name;
        string subfolder;
        string project;
        ProjectPage projectPage;

        public NotePage(string noteName, string path, string projectName, string subfolderName, ProjectPage projectP)
        {
            try
            {
                InitializeComponent();

                notePath = path;
                project = projectName;
                projectPage = projectP;

                //Pokuď otevírá existující poznámku načte ji a nastavuje focus
                if (noteName != null)
                {
                    FileStream file;
                    TextRange range = new TextRange(NoteText.TextEditor.Document.ContentStart, NoteText.TextEditor.Document.ContentEnd);

                    if (subfolderName == null)
                    {
                        file = new FileStream(System.IO.Path.Combine(notePath, "Notes", noteName + ".rtf"), FileMode.Open);
                        SubfolderBox.Items.Add("Hlavní složka projektu");
                        SubfolderBox.SelectedIndex = 0;
                    }
                    else
                    {
                        file = new FileStream(System.IO.Path.Combine(notePath, subfolderName, "Notes", noteName + ".rtf"), FileMode.Open);
                        SubfolderBox.Items.Add(subfolderName);
                        SubfolderBox.SelectedIndex = 0;
                    }

                    name = noteName;
                    subfolder = subfolderName;

                    NoteNameBox.Text = noteName;                    
                    range.Load(file, DataFormats.Rtf);
                    file.Close();

                    //Nastaví focus na textbox s názvem poznámky
                    NoteNameBox.Focus();
                }
                else
                {
                    //Nastaví focus na textbox s názvem poznámky
                    NoteNameBox.Focus();
                    string noteNameLenght = NoteNameBox.Text.ToString();
                    NoteNameBox.SelectionStart = noteNameLenght.Length;

                    if (subfolderName == null)
                        SubfolderBox.Items.Add("Hlavní složka projektu");
                    else
                        SubfolderBox.Items.Add(subfolderName);
                    SubfolderBox.SelectedIndex = 0;
                }
            }
            catch
            {
                Error();
                Back();
            }
        }

        //Metoda pro vyvolání jednoduché chybové hlášky
        private void Error()
        {
            MessageBox.Show("Nastala chyba prosím opakujte", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        //Metoda pro návrat na ProjectPage
        private void Back()
        {
            ProjectPage projectPage = new ProjectPage(project);
            this.NavigationService.Navigate(projectPage);
        }

        //Po kliknutí se vrátí na ProjectPage
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Back();
        }

        //Metoda pro uložení poznámky
        private void SaveNote()
        {
            try
            {
                string[] excludeCharacters = new string[] { @"\", "/", ":", "*", "?", "\"", "<", ">", "|" };
                bool excludedExist = false;

                for (int i = 0; i < 9; i++)
                {
                    if (NoteNameBox.Text.Contains(excludeCharacters[i]))
                        excludedExist = true;
                }

                if (!excludedExist)
                {
                    TextRange range = new TextRange(NoteText.TextEditor.Document.ContentStart, NoteText.TextEditor.Document.ContentEnd);
                    FileStream file;
                    //Nenachází se v subfolder
                    if (subfolder == null)
                    {
                        //Přesouvá se do hlavní složky
                        if (SubfolderBox.SelectedItem.ToString() == "Hlavní složka projektu")
                        {
                            File.Delete(System.IO.Path.Combine(notePath, "Notes", name + ".rtf"));
                            file = new FileStream(System.IO.Path.Combine(notePath, "Notes", NoteNameBox.Text + ".rtf"), FileMode.Create);
                        }
                        //Přesouvá se do subfolder
                        else
                        {
                            File.Delete(System.IO.Path.Combine(notePath, "Notes", name + ".rtf"));
                            file = new FileStream(System.IO.Path.Combine(notePath, SubfolderBox.SelectedItem.ToString(), "Notes", NoteNameBox.Text + ".rtf"), FileMode.Create);
                        }
                    }
                    //Nachází se v subfolder
                    else
                    {
                        //Přesouvá se do hlavní složky
                        if (SubfolderBox.SelectedItem.ToString() == "Hlavní složka projektu")
                        {
                            File.Delete(System.IO.Path.Combine(notePath, subfolder, "Notes", name + ".rtf"));
                            //File.WriteAllText(System.IO.Path.Combine(notePath, "Notes", NoteNameBox.Text + ".rtf"), range.Text);
                            file = new FileStream(System.IO.Path.Combine(notePath, "Notes", NoteNameBox.Text + ".rtf"), FileMode.Create);
                        }
                        //Přesouvá se do subfolder
                        else
                        {
                            File.Delete(System.IO.Path.Combine(notePath, subfolder, "Notes", name + ".rtf"));
                            //File.WriteAllText(System.IO.Path.Combine(notePath, SubfolderBox.SelectedItem.ToString(), "Notes", NoteNameBox.Text + ".rtf"), range.Text);
                            file = new FileStream(System.IO.Path.Combine(notePath, SubfolderBox.SelectedItem.ToString(), "Notes", NoteNameBox.Text + ".rtf"), FileMode.Create);
                        }
                    }

                    range.Save(file, DataFormats.Rtf);
                    file.Close();

                    projectPage.UpdateTabItem(NoteNameBox.Text);
                }
                else
                {
                    MessageBox.Show(@"Název poznámky obsahuje jeden nebo více zakázaných znaků ( \, /, :, *, ?, " + "\", <, >, | )");
                }
            }
            catch
            {
                Error();
            }
        }

        //Uloží poznámku pomocí CTRL+S nebo ji smaže pomocí CTRL+DEL
        private void NotePage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.S && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                SaveNote();
        }

        private void DeleteNoteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DeleteNote();
            }
            catch
            {
                Error();
            }
        }

        private void DeleteNote()
        {
            if (MessageBox.Show("Opravdu chcete smazat poznámku", "Potvrzení smazání", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                if(subfolder == null)
                    File.Delete(System.IO.Path.Combine(notePath, "Notes", NoteNameBox.Text.ToString() + ".rtf"));
                else
                    File.Delete(System.IO.Path.Combine(notePath, subfolder, "Notes", NoteNameBox.Text.ToString() + ".rtf"));


                MessageBox.Show("Poznámka byla úspěšně smazána", "Smazáno");
                Back();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Pokud poznámka obsahuje název uloží ji
                if (string.IsNullOrEmpty(NoteNameBox.Text) == false)
                {
                    SaveNote();
                }
                else
                {
                    MessageBox.Show("Poznámku nelze uložit bez názvu", "Chybějící název", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch
            {
                Error();
            }
        }

        private void NoteNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            projectPage.UpdateTabItem(NoteNameBox.Text);
        }

        //Načte podsložky projektu do comboboxu
        private void SubfolderBox_DropDownOpened(object sender, EventArgs e)
        {
            string[] subfolders = Directory.GetDirectories(notePath);
            SubfolderBox.Items.Clear();

            SubfolderBox.Items.Add("Hlavní složka projektu");
            foreach (string subfolder in subfolders)
            {
                string subfolderName = subfolder.Replace(notePath,"");
                subfolderName = subfolderName.Remove(0, 1);
                if ((subfolderName != "Todo") && (subfolderName != "Notes"))
                    SubfolderBox.Items.Add(subfolderName);
            }
        }

        private void SubfolderBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*SaveButton.Visibility = Visibility.Visible;
            SaveButtonInactive.Visibility = Visibility.Collapsed;*/
        }
    }
}
