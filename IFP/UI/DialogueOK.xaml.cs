using System.Windows;

namespace IFP.UI
{
    /// <summary>
    /// Interaction logic for DialogueOK.xaml
    /// </summary>
    public partial class DialogueOK : Window
    {
        private readonly string labelStr = "Test message";

        public DialogueOK(string text)
        {
            labelStr = text;
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            LabelText.Text = labelStr;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
