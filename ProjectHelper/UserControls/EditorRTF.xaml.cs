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
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using System.IO;

namespace ProjectHelper
{
    /// <summary>
    /// Interakční logika pro EditorRTF.xaml
    /// </summary>
    public partial class EditorRTF : System.Windows.Controls.UserControl
    {
        ColorDialog colorDialog;

        public EditorRTF()
        {
            InitializeComponent();
            
            //naplnění comboboxů
            FontFamilyComboBox.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            FontSizeComboBox.ItemsSource = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };

            //nastavování ColorDialogu
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
        }

        //změna fontu  
        private void FontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontFamilyComboBox.SelectedItem != null)
                TextEditor.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, FontFamilyComboBox.SelectedItem);
        }

        //změna velikosti písma
        private void FontSizeComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                TextEditor.Selection.ApplyPropertyValue(Inline.FontSizeProperty, FontSizeComboBox.Text);
            }
            catch
            { }
        }

        //nastavuje stav buttonu
        private void TextEditor_SelectionChanged(object sender, RoutedEventArgs e)
        {
            //BoldButton
            object temp = TextEditor.Selection.GetPropertyValue(Inline.FontWeightProperty);
            if ((temp != DependencyProperty.UnsetValue) && (temp.Equals(FontWeights.Bold)))
                BoldButton.IsChecked = true;
            else
                BoldButton.IsChecked = false;

            //ItalicButton
            temp = TextEditor.Selection.GetPropertyValue(Inline.FontSizeProperty);
            if ((temp != DependencyProperty.UnsetValue) && (temp.Equals(FontStyles.Italic)))
                ItalicButton.IsChecked = true;
            else
                ItalicButton.IsChecked = false;

            //UnderlineButton
            temp = TextEditor.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            if ((temp != DependencyProperty.UnsetValue) && (temp.Equals(TextDecorations.Underline)))
                UnderlineButton.IsChecked = true;
            else
                UnderlineButton.IsChecked = false;

            //FontFamilyComboBox
            temp = TextEditor.Selection.GetPropertyValue(Inline.FontFamilyProperty);
            FontFamilyComboBox.SelectedItem = temp;

            //FontSizeComboBox
            temp = TextEditor.Selection.GetPropertyValue(Inline.FontSizeProperty);
            FontSizeComboBox.SelectedItem = temp;

            //Barva písma
            ForegroundColorButton.Foreground = TextEditor.Selection.GetPropertyValue(ForegroundProperty) as Brush;

            //Pozadí písma
            if (TextEditor.Selection.GetPropertyValue(TextElement.BackgroundProperty) != null)
                BackgroundColorButton.Background = TextEditor.Selection.GetPropertyValue(TextElement.BackgroundProperty) as Brush;
            else
                BackgroundColorButton.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));

            //Zarovnání písma
            switch (TextEditor.Selection.GetPropertyValue(FlowDocument.TextAlignmentProperty))
            {
                case TextAlignment.Left:
                    AlignLeftButton.IsChecked = true;
                    AlignCenterButton.IsChecked = false;
                    AlignRightButton.IsChecked = false;
                    AlignJustifyButton.IsChecked = false;
                    break;

                case TextAlignment.Center:
                    AlignCenterButton.IsChecked = true;
                    AlignLeftButton.IsChecked = false;
                    AlignRightButton.IsChecked = false;
                    AlignJustifyButton.IsChecked = false;
                    break;

                case TextAlignment.Right:
                    AlignRightButton.IsChecked = true;
                    AlignLeftButton.IsChecked = false;
                    AlignCenterButton.IsChecked = false;
                    AlignJustifyButton.IsChecked = false;
                    break;

                case TextAlignment.Justify:
                    AlignJustifyButton.IsChecked = true;
                    AlignLeftButton.IsChecked = false;
                    AlignCenterButton.IsChecked = false;
                    AlignRightButton.IsChecked = false;
                    break;

                default:
                    AlignLeftButton.IsChecked = false;
                    AlignCenterButton.IsChecked = false;
                    AlignRightButton.IsChecked = false;
                    AlignJustifyButton.IsChecked = false;
                    break;
            }
        }

        //Otevírá ColorDialog pro změnu barvy písma
        private void ForegroundColorButton_Click(object sender, RoutedEventArgs e)
        {
            var results = colorDialog.ShowDialog();

            if (results == DialogResult.OK)
            {
                SaveCustomColor();
                TextEditor.Selection.ApplyPropertyValue(ForegroundProperty, new SolidColorBrush(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B)));
            }
        }

        //Otevírá ColorDialog pro změnu pozadí písma
        private void BackgroundColorButton_Click(object sender, RoutedEventArgs e)
        {
            var results = colorDialog.ShowDialog();

            if (results == DialogResult.OK)
            {
                SaveCustomColor();
                TextEditor.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B)));
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

        private void BoldButton_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.ToggleBold.Execute(null, TextEditor);
        }

        private void ItalicButton_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.ToggleItalic.Execute(null, TextEditor);
        }

        private void UnderlineButton_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.ToggleUnderline.Execute(null, TextEditor);
        }

        private void BulletButton_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.ToggleBullets.Execute(null, TextEditor);
        }

        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.ToggleNumbering.Execute(null, TextEditor);
        }

        private void AlignLeftButton_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.AlignLeft.Execute(null, TextEditor);
        }

        private void AlignCenterButton_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.AlignCenter.Execute(null, TextEditor);
        }

        private void AlignRightButton_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.AlignRight.Execute(null, TextEditor);
        }

        private void AlignJustifyButton_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.AlignJustify.Execute(null, TextEditor);
        }

        private void TextEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }
    }
}
