using IFP.Models;
using IFP.Modules;
using IFP.Singletons;
using IFP.Utils;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static IFP.Models.FullProduct;

namespace IFP.Pages
{
    /// <summary>
    /// Interaction logic for ProductInfoPage.xaml
    /// </summary>
    public partial class ProductInfoPage : Page
    {
        protected readonly FullProduct EditableProduct;
        protected readonly Page PreviousPage;
        protected Tuple<int?, string> ProductType = new(null, null);

        //for storing product variants and statuses
        protected Dictionary<string, ProductVariant> EditableVariantsKVP = new();
        protected List<string> ProductStatuses = ProductStatus.GetFields();


        //for handling adding images to the product
        protected List<string> ProductImages;
        protected ObservableCollection<string> ImgListBoxDataSource;
        public ICommand DeleteImageCommand { get; set; }
        public ICommand ShowImageCommand { get; set; }


        //for handling adding tags to the products
        protected List<string> ProductTags;
        protected ObservableCollection<string> TagListBoxDataSource;
        public ICommand DeleteTagCommand { get; set; }


        //Constructor
        public ProductInfoPage(FullProduct product, Page prevPage)
        {
            PreviousPage = prevPage;
            InitializeComponent();

            //changign window title to show the SKU
            MainWindow.Instance.SetWindowTitle($"Product Info Page ({product.SKU})");

            DataContext = this;
            EditableProduct = product;
            ProductType = new(int.Parse(product.ProductTypeID), product.ProductTypeDisplayVal);

            ProductFieldInit();
        }


        //
        // Init method section
        //

        /// <summary>
        /// Method initialiazes UI with product data
        /// </summary>
        private void ProductFieldInit()
        {
            TitleBoxLT.Text = EditableProduct.TitleLT;
            TitleBoxLV.Text = EditableProduct.TitleLV;
            TitleBoxEE.Text = EditableProduct.TitleEE;
            TitleBoxRU.Text = EditableProduct.TitleRU;

            DescBoxLT.Text = EditableProduct.DescLT;
            DescBoxLV.Text = EditableProduct.DescLV;
            DescBoxEE.Text = EditableProduct.DescEE;
            DescBoxRU.Text = EditableProduct.DescRU;

            VendorBox.Text = EditableProduct.Vendor;

            AddedTimeLabel.Content = EditableProduct.GetAddedTime().ToString();
            DeliveryTimeLabel.Content = EditableProduct.DeliveryTime;

            HeightBox.Text = EditableProduct.Height.ToString();
            WeightBox.Text = EditableProduct.Weight.ToString();
            WidthBox.Text = EditableProduct.Width.ToString();
            LenghtBox.Text = EditableProduct.Lenght.ToString();
            VendorProductTypeLabel.Content = EditableProduct.ProductTypeVendor;

            //Product type button init
            SelectCategoryButton.Content = ProductType.Item2;

            //Image listBox init
            ProductImages = new List<string>(EditableProduct.Images);
            ImgListBoxDataSource = new ObservableCollection<string>(EditableProduct.Images);
            ImageListBox.ItemsSource = ImgListBoxDataSource;
            ShowImageCommand = new DelegateCommand<object>(ShowImage);

            //Tag listBox init
            ProductTags = new List<string>(EditableProduct.Tags);
            TagListBoxDataSource = new ObservableCollection<string>(EditableProduct.Tags);
            TagListBox.ItemsSource = TagListBoxDataSource;

            //adding attributtes to data grid
            var productAttributesArray = from row in EditableProduct.ProductAttributtes select new { AttributeName = row.Key, AttributeValue = row.Value };
            productAttributesDG.ItemsSource = productAttributesArray.ToArray();

            //setting attributte and variant labels
            ProductAttributtesLabel.Text = $"Product Attributtes ({EditableProduct.SKU})";
            ProductVariantsLabel.Text = $"Product Variants ({EditableProduct.SKU})";

            //setting up the product status UX
            ProductStatusLabel.Text = $"Product Status ({EditableProduct.SKU})";
            ProductStatusComboBox.ItemsSource = ProductStatuses;
            ProductStatusComboBox.SelectedItem = EditableProduct.Status;
            ProductStatusComboBox.Items.Refresh();

            //setting up the variants
            InitVariants();
        }

        /// <summary>
        /// Method to initialize product variant UX
        /// </summary>
        private void InitVariants()
        {
            EditableVariantsKVP.Clear();
            foreach (var pv in EditableProduct.ProductVariants)
            {
                EditableVariantsKVP.Add(GetVariantComboBoxItemName(pv), pv);
            }

            ProductVariantComboBox.ItemsSource = EditableVariantsKVP;
            ProductVariantComboBox.SelectionChanged += (object sender, SelectionChangedEventArgs e) =>
            {
                ComboBox comboBox = e.Source as ComboBox;
                if (comboBox.SelectedItem != null)
                {
                    KeyValuePair<string, ProductVariant> variantKVP = (KeyValuePair<string, ProductVariant>)comboBox.SelectedItem;
                    ProductVariant pv = variantKVP.Value;

                    VariantTypeBox.Text = pv.VariantType;
                    VariantDataBox.Text = pv.VariantData;
                    VendorStockBox.Text = pv.VendorStock.ToString();
                    OurStockBox.Text = pv.OurStock.ToString();
                    PriceBox.Text = pv.Price.ToString();
                    VendorPriceBox.Text = pv.PriceVendor.ToString();
                    PermPriceCheckBox.IsChecked = pv.PermPrice;
                }
                else
                {
                    VariantTypeBox.Text = null;
                    VariantDataBox.Text = null;
                    VendorStockBox.Text = null;
                    OurStockBox.Text = null;
                    PriceBox.Text = null;
                    VendorPriceBox.Text = null;
                    PermPriceCheckBox.IsChecked = false;
                }
            };
            //selectiong first variant
            ProductVariantComboBox.SelectedIndex = 0;
        }


        //
        // Helper method section
        //

        /// <summary>
        /// Method used to build a name for ProductVariantCombobox item KVP
        /// </summary>
        /// <param name="pv"></param>
        /// <returns></returns>
        protected string GetVariantComboBoxItemName(ProductVariant pv)
        {
            string variantBarcode;
            if (string.IsNullOrEmpty(pv.Barcode))
            {
                variantBarcode = "No Barcode";
            }
            else
            {
                variantBarcode = pv.Barcode;
            }

            string variantType;
            if (string.IsNullOrEmpty(pv.VariantType))
            {
                variantType = "No Variant Type";
            }
            else
            {
                variantType = pv.VariantType;
            }

            string variantData;
            if (string.IsNullOrEmpty(pv.VariantData))
            {
                variantData = "No Variant Data";
            }
            else
            {
                variantData = pv.VariantData;
            }

            return $"{variantBarcode} ({variantType}: {variantData})";
        }

        /// <summary>
        /// method opens image in default browser (passed as a command to the button)
        /// </summary>
        /// <param name="item"></param>
        private void ShowImage(object item)
        {
            string imgLink = item as string;
            //todo: doesnt open BF image links
            SiteNav.GoToSite(imgLink);
        }

        /// <summary>
        /// Method that navigates to previous page
        /// </summary>
        protected void NavigateToPreviousPage() {
            MainWindow.Instance.SetWindowTitle();
            ProductBrowsePage.Instance.RefreshDataGrid();
            MainWindow.Instance.mainFrame.Content = PreviousPage;
        }


        //
        // Buttons onClick method section 
        //

        /// <summary>
        /// Button onClick method to navigate to ProductEditPage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditProductButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.setFrame(new ProductEditPage(EditableProduct, PreviousPage));
        }

        /// <summary>
        /// Button onClick method to delete editable product from database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            ProductStore.DeleteProduct(EditableProduct.SKU);
            ProductStore.Instance.ProductKVP.Remove(EditableProduct.SKU);
            NavigateToPreviousPage();
        }

        /// <summary>
        /// (Overridden) Button onClick method to navigate to previous page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPreviousPage();
        }


        //
        // Section for input validation of double and int inputs
        //

        /// <summary>
        /// allows only double to be entered (numbers and .)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoublePreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //matching if value can be parsed as double
            var textBox = sender as TextBox;
            string updatedText = textBox.Text + e.Text;
            bool doubleParsable = !double.TryParse(updatedText, out _);

            e.Handled = doubleParsable;
        }

        /// <summary>
        /// allows only int to be entered (numbers)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IntPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //matching if value can be parsed as int
            var textBox = sender as TextBox;
            string updatedText = textBox.Text + e.Text;
            bool intParsable = !int.TryParse(updatedText, out _);

            e.Handled = intParsable;
        }


        //
        // Section for onClick methods that are overridden in child classes 
        //

        /// <summary>
        /// (Overridden) button onClick for changing product category
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void SelectCategoryButton_Click(object sender, RoutedEventArgs e) { }

        /// <summary>
        /// (Overridden) button onClick method that adds link to product.images
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void AddImageButton_Click(object sender, RoutedEventArgs e) { }

        /// <summary>
        /// (Overridden) button onClick method that adds tag to product.tags
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void AddTagButton_Click(object sender, RoutedEventArgs e) { }

        /// <summary>
        /// (Overridden) button onClick method that saves product to database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void SaveProductButton_Click(object sender, RoutedEventArgs e) { }

        /// <summary>
        /// (Overridden) button onClick method that saves product variant to database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void SaveVariantButton_Click(object sender, RoutedEventArgs e) { }
    }
}
