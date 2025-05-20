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
using Trading.WPFClient.ViewModels;

namespace Trading.WPFClient.Views
{
    /// <summary>
    /// Interaction logic for UIView.xaml
    /// </summary>
    public partial class UIView : UserControl
    {
        public UIView()
        {
            InitializeComponent();
        }
        private void QuantityTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            string fullText = GetFullTextAfterInput(textBox, e.Text);

            e.Handled = !IsValidPositiveInteger(fullText);
        }
        private bool IsValidPositiveInteger(string text)
        {
            return int.TryParse(text, out int result) && result > 0;
        }
        private void QuantityTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string pastedText = (string)e.DataObject.GetData(typeof(string));
                var textBox = sender as TextBox;
                string fullText = GetFullTextAfterInput(textBox, pastedText);

                if (!IsValidPositiveInteger(fullText))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private string GetFullTextAfterInput(TextBox? textBox, string input)
        {
            if (textBox == null) return input;

            var currentText = textBox.Text;
            var selectionStart = textBox.SelectionStart;
            var selectionLength = textBox.SelectionLength;

            return currentText.Remove(selectionStart, selectionLength).Insert(selectionStart, input);
        }




        private void FloatTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            string fullText = GetFullTextAfterInput(textBox, e.Text);

            e.Handled = !IsValidPositiveFloat(fullText);
        }

        private void FloatTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string pastedText = (string)e.DataObject.GetData(typeof(string));
                var textBox = sender as TextBox;
                string fullText = GetFullTextAfterInput(textBox, pastedText);

                if (!IsValidPositiveFloat(fullText))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

       
        private bool IsValidPositiveFloat(string text)
        {
            return float.TryParse(text, out float result) && result >= 0;
        }
    }

}
