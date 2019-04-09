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

namespace ProjectHelper
{
    /// <summary>
    /// Interakční logika pro ToDoItem.xaml
    /// </summary>
    public partial class ToDoItem : UserControl
    {
        //Hlavní metoda
        public ToDoItem(bool check, string text)
        {
            InitializeComponent();

            DeleteToDoItemButton.Visibility = Visibility.Hidden;
            TodoCheckbox.IsChecked = check;
            TodoText.Text = text;
        }      

        //EVENTY

        //Zobrazí button pro smazání položky
        private void ToDoItem_MouseEnter(object sender, MouseEventArgs e)
        {
            DeleteToDoItemButton.Visibility = Visibility.Visible;
        }

        //Skryje button pro smazání položky
        private void ToDoItem_MouseLeave(object sender, MouseEventArgs e)
        {
            DeleteToDoItemButton.Visibility = Visibility.Hidden;
        }
        
        //Maže ToDo Item
        private void DeleteToDoItemButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteToDoItem();        
        }

        //METODY

        //Metoda, která vrací pokud je checkbox zaškrtnutý
        public string ReturnCheck()
        {
            if (TodoCheckbox.IsChecked == true)
                return "1-";
            else
                return "0-";
        }

        //Metoda, která vrací text ToDo Listu
        public string ReturnText()
        {
            return TodoText.Text;
        }

        //Metoda pro mazání ToDo Itemu
        public void DeleteToDoItem()
        {
            StackPanel parent = this.Parent as StackPanel;
            parent.Children.Remove(this);
            parent.Focus();
        }

        private void TodoCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            TodoText.Foreground = new SolidColorBrush(Color.FromRgb(154, 154, 154));
        }

        private void TodoCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            TodoText.Foreground = new SolidColorBrush(Color.FromRgb(33, 33, 33));
        }
    }
}
