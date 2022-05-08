﻿using System.Windows;

namespace IFP.UI
{
    /// <summary>
    /// Interaction logic for DIalogueYN.xaml
    /// </summary>
    public partial class DialogueYN : Window
    {

        private readonly string labelStr = "Test Question yes/no?";

        public DialogueYN(string text)
        {
            labelStr = text;
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            LabelText.Text = labelStr;
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
