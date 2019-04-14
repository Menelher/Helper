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
using System.Windows.Shapes;

namespace ProjectHelper
{
    /// <summary>
    /// Interaction logic for NewThingWindow.xaml
    /// </summary>
    public partial class NewThingWindow : Window
    {
        ProjectTab projectTab;
        string type;

        public NewThingWindow(ProjectTab project)
        {
            InitializeComponent();

            projectTab = project;
        }

        private void NewNoteButton_Click(object sender, RoutedEventArgs e)
        {
            type = "Note";
            Close();
        }

        private void NewTodoButton_Click(object sender, RoutedEventArgs e)
        {
            type = "Todo";
            Close();
        }

        private void NewSubfolder_Click(object sender, RoutedEventArgs e)
        {
            type = "Subfolder";
            Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            switch (type)
            {
                case "Note":
                    projectTab.newThingType = "Note";
                    break;

                case "Todo":
                    projectTab.newThingType = "Todo";
                    break;

                case "Subfolder":
                    projectTab.newThingType = "Subfolder";
                    break;

                default:
                    projectTab.newThingType = "";
                    break;
            }
        }
    }
}
