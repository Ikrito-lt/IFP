using IFP.Models;
using IFP.Modules;
using System;
using System.Windows;
using System.Windows.Controls;

namespace IFP.UI
{
    /// <summary>
    /// Interaction logic for CategoryTreeSelectorWindow.xaml
    /// </summary>
    public partial class CategoryTreeSelectorWindow : Window
    {
        public Tuple<int?, string> selectionResult;

        public CategoryTreeSelectorWindow(string windowTitle)
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            Title = windowTitle;
            windowInit();
        }

        private void windowInit()
        {
            trVwCategory.ItemsSource = CategoryTreeModule.Instance.categoryTree.children;
        }

        private void trVwCategory_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var s = sender as TreeView;
            var selectedItem = s.SelectedItem as CategoryTree;
            if (selectedItem.CatID != null)
            {

                selectionResult = new(selectedItem.CatID, selectedItem.CatName);
                DialogResult = true;
                Close();
            }
        }
    }
}
