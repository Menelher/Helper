using System;
using System.Collections.Generic;
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
using System.IO;

namespace ProjectHelper
{
    /// <summary>
    /// Interakční logika pro ToDoPage.xaml
    /// </summary>
    public partial class ToDoPage : Page
    {
        string todoName;
        string projectName;
        string todoPath;
        string subfolderName;
        ProjectPage projectPage;

        public ToDoPage(string todo, string path, string project, string subfolder, ProjectPage projectP)
        {
            try
            {
                InitializeComponent();

                todoName = todo;
                projectName = project;
                todoPath = path;
                subfolderName = subfolder;
                projectPage = projectP;

                int celkovyPocet = 0;
                int splnenyPocet = 0;

                //Pokud se nejedná o nový list načte ho
                if (todo != null)
                {
                    TodoNameBox.Text = todo;

                    string line;
                    StreamReader todoList;

                    if (subfolder == null)
                    {
                        todoList = new StreamReader(System.IO.Path.Combine(todoPath, "Todo", todo + ".txt"));
                        SubfolderBox.Items.Add("Hlavní složka projektu");
                        SubfolderBox.SelectedIndex = 0;
                    }
                    else
                    {
                        todoList = new StreamReader(System.IO.Path.Combine(todoPath, subfolderName, "Todo", todo + ".txt"));
                        SubfolderBox.Items.Add(subfolder);
                        SubfolderBox.SelectedIndex = 0;
                    }

                    //Načítá jednotlivé řádky
                    while ((line = todoList.ReadLine()) != null)
                    {
                        if (line != "end")
                        {
                            bool check = false;
                            if (line.Substring(0, 2) == "1-")
                            {
                                check = true;
                                splnenyPocet++;
                            }

                            line = line.Remove(0, 2);

                            AddTodo(check, line);

                            celkovyPocet++;
                        }
                    }

                    todoList.Close();
                }
                else
                {
                    if (subfolderName == null)
                        SubfolderBox.Items.Add("Hlavní složka projektu");
                    else
                        SubfolderBox.Items.Add(subfolderName);
                    SubfolderBox.SelectedIndex = 0;

                    AddTodo(false, null);
                }

                TodoNameBox.Focus();
                string todoNameLenght = TodoNameBox.Text.ToString();
                TodoNameBox.SelectionStart = todoNameLenght.Length;
                TodoLabel.Content = "Todo (" + splnenyPocet + "/" + celkovyPocet + ")";

            }
            catch
            {
                Error();
                Back();
            }
        }

        //Metoda pro přidání ToDo Itemu
        private void AddTodo(bool check, string text)
        {
            ToDoItem toDoItem = new ToDoItem(check, text);
            ToDoPanel.Children.Add(toDoItem);
        }

        //Metoda pro vyvolání jednoduché chybové hlášky
        private void Error()
        {
            MessageBox.Show("Nastala chyba prosím opakujte", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        //Metoda pro návrat
        public void Back()
        {
            ProjectPage projectPage = new ProjectPage(projectName);
            this.NavigationService.Navigate(projectPage);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Back();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveTodo();
            }
            catch
            {
                Error();
            }
        }

        //Metoda pro uložení ToDoListu
        private void SaveTodo()
        {
            string[] excludeCharacters = new string[] { @"\", "/", ":", "*", "?", "\"", "<", ">", "|" };
            bool excludedExist = false;

            for (int i = 0; i < 9; i++)
            {
                if (TodoNameBox.Text.Contains(excludeCharacters[i]))
                    excludedExist = true;
            }

            if (!excludedExist)
            {
                //Todo není v subfolderu
                if (subfolderName == null)
            {
                if (File.Exists(System.IO.Path.Combine(todoPath, "Todo", todoName + ".txt")))
                    File.Delete(System.IO.Path.Combine(todoPath, "Todo", todoName + ".txt"));

                //Todo zůstává v hlavní složce
                if (SubfolderBox.SelectedItem.ToString() == "Hlavní složka projektu")
                {                  
                    for (int i = 0; i < ToDoPanel.Children.Count; i++)
                    {
                        ToDoItem todoItem = ToDoPanel.Children[i] as ToDoItem;
                        string check = todoItem.ReturnCheck();
                        string text = todoItem.ReturnText();

                        File.AppendAllText(System.IO.Path.Combine(todoPath, "Todo", TodoNameBox.Text + ".txt"), check + text + Environment.NewLine);
                    }

                    File.AppendAllText(System.IO.Path.Combine(todoPath, "Todo", TodoNameBox.Text + ".txt"), "end");
                }
                //Todo se přesouvá do subfolderu
                else
                {
                    for (int i = 0; i < ToDoPanel.Children.Count; i++)
                    {
                        ToDoItem todoItem = ToDoPanel.Children[i] as ToDoItem;
                        string check = todoItem.ReturnCheck();
                        string text = todoItem.ReturnText();

                        File.AppendAllText(System.IO.Path.Combine(todoPath,SubfolderBox.SelectedItem.ToString(), "Todo", TodoNameBox.Text + ".txt"), check + text + Environment.NewLine);
                    }

                    File.AppendAllText(System.IO.Path.Combine(todoPath, SubfolderBox.SelectedItem.ToString(), "Todo", TodoNameBox.Text + ".txt"), "end");
                }              
            }
            //Todo je v subfolderu
            else
            {
                if (File.Exists(System.IO.Path.Combine(todoPath, subfolderName, "Todo", todoName + ".txt")))
                    File.Delete(System.IO.Path.Combine(todoPath,subfolderName, "Todo", todoName + ".txt"));

                //Todo se přesouvá do hlavní složky
                if (SubfolderBox.SelectedItem.ToString() == "Hlavní složka projektu")
                {
                    for (int i = 0; i < ToDoPanel.Children.Count; i++)
                    {
                        ToDoItem todoItem = ToDoPanel.Children[i] as ToDoItem;
                        string check = todoItem.ReturnCheck();
                        string text = todoItem.ReturnText();

                        File.AppendAllText(System.IO.Path.Combine(todoPath, "Todo", TodoNameBox.Text + ".txt"), check + text + Environment.NewLine);
                    }

                    File.AppendAllText(System.IO.Path.Combine(todoPath, "Todo", TodoNameBox.Text + ".txt"), "end");
                }
                //Todo se přesouvá do subfolderu
                else
                {
                    for (int i = 0; i < ToDoPanel.Children.Count; i++)
                    {
                        ToDoItem todoItem = ToDoPanel.Children[i] as ToDoItem;
                        string check = todoItem.ReturnCheck();
                        string text = todoItem.ReturnText();

                        File.AppendAllText(System.IO.Path.Combine(todoPath, SubfolderBox.SelectedItem.ToString(), "Todo", TodoNameBox.Text + ".txt"), check + text + Environment.NewLine);
                    }

                    File.AppendAllText(System.IO.Path.Combine(todoPath, SubfolderBox.SelectedItem.ToString(), "Todo", TodoNameBox.Text + ".txt"), "end");
                }
            }

            projectPage.UpdateTabItem(TodoNameBox.Text);
            }
            else
            {
                MessageBox.Show(@"Název todoListu obsahuje jeden nebo více zakázaných znaků ( \, /, :, *, ?, " + "\", <, >, | )");
            }
        }

        //Klávesové zkratky
        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                //Uložení pomocí CTRL+S
                if (e.Key == Key.S && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                    SaveTodo();

                //Nový todo item pomocí CTRL+N nebo ENTERU
                if ((e.Key == Key.N && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) || e.Key == Key.Enter)
                    AddTodo(false, null);
            }
            catch
            {
                Error();
            }
        }

        //Přidá prázdny todo item
        private void AddCheckbox_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddTodo(false, null);
            }
            catch
            {
                Error();
            }
        }

        //Načte podsložky projektu do comboboxu
        private void SubfolderBox_DropDownOpened(object sender, EventArgs e)
        {
            string[] subfolders = Directory.GetDirectories(todoPath);
            SubfolderBox.Items.Clear();

            SubfolderBox.Items.Add("Hlavní složka projektu");
            foreach (string subfolder in subfolders)
            {
                string subfolderName = subfolder.Replace(todoPath, "");
                subfolderName = subfolderName.Remove(0, 1);
                if ((subfolderName != "Todo") && (subfolderName != "Notes"))
                    SubfolderBox.Items.Add(subfolderName);
            }
        }

        //Smaže todo list
        private void DeleteToDoButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DeleteToDo();
            }
            catch
            {
                Error();
            }
        }

        //Metoda pro mazání todo listu
        private void DeleteToDo()
        {
            //hlavní složka
            if (subfolderName == null)
                File.Delete(System.IO.Path.Combine(todoPath, "Todo", todoName + ".txt"));
            //subfolder
            else
                File.Delete(System.IO.Path.Combine(todoPath, subfolderName, "Todo", todoName + ".txt"));

            MessageBox.Show("Poznámka byla úspěšně smazána", "Smazáno");
            Back();
        }

        private void TodoNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            projectPage.UpdateTabItem(TodoNameBox.Text);
        }
    }
}