using IFP.Models;
using IFP.Singletons;
using IFP.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace IFP.Pages
{
    /// <summary>
    /// Interaction logic for ProductBulkEditPage.xaml
    /// </summary>
    public partial class ProductBulkEditPage : Page
    {
        private List<string> PossibleVendorTypes;
        //for saving selected current product type
        Tuple<int?, string> currentProductType = new(null, null);
        //for saving selected new product type
        Tuple<int?, string> newProductType = new(null, null);

        private List<TypeListBoxItem> ListBoxSource;
        private bool AllTypeChangeProductsSelected = false;


        public ProductBulkEditPage()
        {
            InitializeComponent();

            //sorting possible vendor product types
            PossibleVendorTypes = GetVendorTypes();
            PossibleVendorTypes.Sort();
            ListBoxSource = new();

            DataContext = this;

            InitTypes();
        }

        /// <summary>
        /// Class that defines data displayed in listbox item
        /// </summary>
        class TypeListBoxItem
        {
            public string SKU { get; set; }
            public string Title { get; set; }
            public string ProductType { get; set; }
            public string VendorProductType { get; set; }
            public bool Selected { get; set; }
        }

        // Init section

        /// <summary>
        /// this method initialises possible vendor product types combobox
        /// </summary>
        private void InitTypes()
        {
            VendorTypeFilterCBox.ItemsSource = PossibleVendorTypes;
            ChangeTypeListBox.ItemsSource = ListBoxSource;
        }

        /// <summary>
        /// method compiles a list of vendor types
        /// </summary>
        /// <returns>List of Possible vendor types</returns>
        private List<string> GetVendorTypes()
        {
            List<string> possibleVendorTypes = new();
            foreach ((_, FullProduct p) in ProductStore.Instance.ProductKVP)
            {
                if (!possibleVendorTypes.Contains(p.ProductTypeVendor.Trim()))
                {
                    possibleVendorTypes.Add(p.ProductTypeVendor.Trim());
                }
            }
            return possibleVendorTypes;
        }


        // selecting new and current product types

        /// <summary>
        /// method that saves state of current product type for list box filtration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectCurrentCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var categoryTreeSelector = new CategoryTreeSelectorWindow("Select Current Category From CategoryTree");
            if (categoryTreeSelector.ShowDialog() == true)
            {
                var selectedCategoryTuple = categoryTreeSelector.selectionResult;
                button.Content = selectedCategoryTuple.Item2;
                currentProductType = selectedCategoryTuple;
            }
            else
            {
                button.Content = "Select Current Category";
                currentProductType = new(null, null);
            }
        }

        /// <summary>
        /// method that saves state of new product type for list box filtration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectNewCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var categoryTreeSelector = new CategoryTreeSelectorWindow("Select New Category From CategoryTree");
            if (categoryTreeSelector.ShowDialog() == true)
            {
                var selectedCategory = categoryTreeSelector.selectionResult;
                button.Content = selectedCategory.Item2;
                newProductType = selectedCategory;
            }
            else
            {
                button.Content = "Select New Category";
                newProductType = new(null, null);
            }
        }


        // refreshing product list box section

        /// <summary>
        /// method that is responsible for loading products in product list box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshProductListBoxButton_Click(object sender, RoutedEventArgs e)
        {
            //if vendor type combo box is empty or current product type isnt selected, return
            if (currentProductType.Item1 == null || currentProductType.Item2 == null)
            {
                return;
            }

            //clearing ListBox Source
            ListBoxSource.Clear();

            //cheking if i need to check for Vendor Product Type and assigning types to check against
            bool checkVendorType;
            string vendorProductType = string.Empty;
            string productType = currentProductType.Item2;
            if (VendorTypeFilterCBox.SelectedItem == null) {
                checkVendorType = false;
            }else{
                checkVendorType = true;
                vendorProductType = VendorTypeFilterCBox.SelectedItem.ToString();
            }

            //compiling new ListBoxSource with selected product types
            foreach ((_, FullProduct p) in ProductStore.Instance.ProductKVP)
            {
                var TPlistboxitem = new TypeListBoxItem();

                if (checkVendorType)
                {
                    //check if product has required product type, if check fails loop continues
                    if (p.ProductTypeDisplayVal != productType) continue;
                    //check if product has required vendor product type, if check fails loop continues
                    if (p.ProductTypeVendor.Trim() != vendorProductType) continue;
                }
                else {
                    //check if product has required product type, if check fails loop continues
                    if (p.ProductTypeDisplayVal != productType) continue;
                }

                //if checks passed, give TPlistboxitem - sku - title and both product types;
                TPlistboxitem.VendorProductType = p.ProductTypeVendor;
                TPlistboxitem.ProductType = productType;
                TPlistboxitem.SKU = p.SKU;
                TPlistboxitem.Title = p.TitleLT;
                TPlistboxitem.Selected = false;

                //adding it to list box source
                ListBoxSource.Add(TPlistboxitem);
            }
            ChangeTypeListBox.Items.Refresh();
        }


        // Buttons section

        /// <summary>
        /// Button to go back
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ProductBrowsePage.Instance.RefreshDataGrid();
            MainWindow.Instance.setFrame(ProductBrowsePage.Instance);
        }

        /// <summary>
        /// select and unselect all typechange products in a listbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectAllProductsButton_Click(object sender, RoutedEventArgs e)
        {
            AllTypeChangeProductsSelected = !AllTypeChangeProductsSelected;
            if (AllTypeChangeProductsSelected)
            {
                SelectAllProductsButton.Content = "Unselect all";
                foreach (var item in ListBoxSource)
                {
                    item.Selected = true;
                }
                ChangeTypeListBox.Items.Refresh();
            }
            else
            {
                SelectAllProductsButton.Content = "SelectAll";
                foreach (var item in ListBoxSource)
                {
                    item.Selected = false;
                }
                ChangeTypeListBox.Items.Refresh();
            }
        }

        /// <summary>
        /// button that unselects vendortypeCboox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteVendorTypeFilterButton_Click(object sender, RoutedEventArgs e)
        {
            VendorTypeFilterCBox.SelectedItem = null;
        }


        // List box item interaction section

        /// <summary>
        /// Refreshes Product attributes section
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeTypeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            TypeListBoxItem selectedItem = listBox.SelectedItem as TypeListBoxItem;
            if (selectedItem != null)
            {
                //loading attributtes
                string selectedSKU = selectedItem.SKU;
                FullProduct selectedProduct = ProductStore.Instance.ProductKVP[selectedSKU];

                var productAttributesArray = from row in selectedProduct.ProductAttributtes select new { AttributeName = row.Name, AttributeValue = row.Attribute };
                productAttributesDG.ItemsSource = productAttributesArray.ToArray();

                //loading images
                ProductImagesLabel.Text = $"Product Images ({selectedProduct.Images.Count})";
                ProductImagesListBox.ItemsSource = selectedProduct.Images;
            }
        }

        /// <summary>
        /// for checking listbox checkboxes by pressing enter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var item = sender as ListBoxItem;
            var selectedItemIndex = ChangeTypeListBox.SelectedIndex;
            var selectedData = item.DataContext as TypeListBoxItem;

            if (selectedData != null && e.Key == Key.Enter)
            {
                ListBoxSource.FirstOrDefault(x => x.SKU == selectedData.SKU).Selected ^= true;
                ChangeTypeListBox.Items.Refresh();

                ChangeTypeListBox.UpdateLayout(); // Pre-generates item containers 
                var newFocusTarget = ChangeTypeListBox.ItemContainerGenerator.ContainerFromIndex(selectedItemIndex) as ListBoxItem;
                if (newFocusTarget != null)
                {
                    newFocusTarget.Focus();
                }
            }
        }


        // change products types logic section

        /// <summary>
        /// /button that changes types 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeTypesButton_Click(object sender, RoutedEventArgs e)
        {
            if (newProductType.Item1 != null && newProductType.Item2 != null)
            {
                ChangeTypes(newProductType);
            }
        }

        /// <summary>
        /// method for  loading products to datagrid
        /// </summary>
        /// <param name="newType"></param>
        private void ChangeTypes(Tuple<int?, string> newType)
        {
            string newTypeID = newType.Item1.ToString();
            string newTypeName = newType.Item2;

            BackgroundWorker worker = new();
            worker.WorkerReportsProgress = true;
            worker.DoWork += (object sender, DoWorkEventArgs e) =>
            {
                Dictionary<string, TypeListBoxItem> changeList = ListBoxSource.Where(x => x.Selected == true).ToDictionary(v => v.SKU, v => v);
                var i = 1;
                var changesCount = changeList.Count;
                foreach ((var sku, var t) in changeList)
                {
                    ProductCategoryStore.ChangeProductCategory(sku, newTypeID);

                    //changing category in products
                    ProductStore.Instance.ProductKVP[sku].ProductTypeID = newTypeID;
                    ProductStore.Instance.ProductKVP[sku].ProductTypeDisplayVal = newTypeName;

                    //changing category in listbox
                    ListBoxSource.Remove(t);
                    (sender as BackgroundWorker).ReportProgress(i, changesCount);
                    i++;
                }

            };

            worker.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) =>
            {
                BackButton.IsEnabled = true;
                loadingbarLabel.Text = "";
                loadingBar.Value = 0;
                ChangeTypeListBox.Items.Refresh();
            };

            worker.ProgressChanged += (object sender, ProgressChangedEventArgs e) =>
            {
                int progress = e.ProgressPercentage;
                int max = (int)e.UserState;
                loadingBar.Value = progress;
                loadingBar.Maximum = max;
                loadingbarLabel.Text = $"Changing Product Types ({progress}/{max})";
            };

            //blocking pre do work
            BackButton.IsEnabled = false;
            loadingbarLabel.Text = "Changing Product Types";

            worker.RunWorkerAsync();
        }
    }
}
