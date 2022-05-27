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
using static IFP.Models.FullProduct;

namespace IFP.Pages
{
    internal class NewProductPage : ProductEditPage
    {
        // Constructor
        public NewProductPage(Page prevPage) : base(new FullProduct(creatingNew: true), prevPage)
        {
            MainWindow.Instance.SetWindowTitle($"New Product Page");
            NewProductPageInit();
        }

        // Init Page Section

        /// <summary>
        /// This method initializes
        /// </summary>
        private void NewProductPageInit()
        {
            // disabling delebe product button
            DeleteProductButton.IsEnabled = false;

            // Adding Preview text input to SKU box
            SKUBox.IsReadOnly = false;
            SKUBox.PreviewKeyDown += SKUBox_PreviewKeyDown;
            SKUBox.PreviewTextInput += SKUPreviewTextInput;
            //setting flip on change to SKU
            SKUBox.TextChanged += SaveFlip_TextChanged;

            // enabling variant barcode textbox
            VariantBarcodeBox.IsReadOnly = false;
        }

        // Section for input validation of SKU

        /// <summary>
        /// Method to check if SKUBox input starts with "IKR-"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void SKUBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // if key == backspace and string is IKR- dont accept back space
            var textBox = sender as TextBox;
            string currentText = textBox.Text;
            if (e.Key == Key.Back)
            {
                if (currentText == "IKR-") e.Handled = true;
            }
        }

        /// <summary>
        /// allows only double to be entered (numbers and .)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SKUPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //matching if value can be parsed as double
            var textBox = sender as TextBox;
            string updatedText = textBox.Text.Insert(textBox.CaretIndex, e.Text);

            e.Handled = !updatedText.StartsWith("IKR-");
        }


        // variants section

        /// <summary>
        /// Method to initialize product variant UX
        /// </summary>
        protected override void InitVariants()
        {
            // adding on selection changed to combobox
            ProductVariantComboBox.SelectionChanged += (object sender, SelectionChangedEventArgs e) =>
            {
                ComboBox comboBox = e.Source as ComboBox;
                if (comboBox.SelectedItem != null)
                {
                    KeyValuePair<string, ProductVariant> variantKVP = (KeyValuePair<string, ProductVariant>)comboBox.SelectedItem;
                    ProductVariant pv = variantKVP.Value;

                    VariantBarcodeBox.Text = pv.Barcode;
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
                    VariantBarcodeBox.Text = null;
                    VariantTypeBox.Text = null;
                    VariantDataBox.Text = null;
                    VendorStockBox.Text = null;
                    OurStockBox.Text = null;
                    PriceBox.Text = null;
                    VendorPriceBox.Text = null;
                    PermPriceCheckBox.IsChecked = false;
                }
            };

            //adding new variant option to combobox
            EditableVariantsKVP.Add("Add New Variant", new ProductVariant());
            ProductVariantComboBox.ItemsSource = EditableVariantsKVP;
            ProductVariantComboBox.Items.Refresh();

            //selecting first variant
            ProductVariantComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// (Overridden) save variant behaviour
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void SaveVariantButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedCBItem = ProductVariantComboBox.SelectedItem;
            KeyValuePair<string, ProductVariant> selectedVariantKVP;
            if (selectedCBItem == null)
            {
                selectedVariantKVP = new KeyValuePair<string, ProductVariant>("Add New Variant", new ProductVariant());
            }
            else {
                selectedVariantKVP = (KeyValuePair<string, ProductVariant>)selectedCBItem;
            }

            //returning if not variant is selected
            if (selectedVariantKVP.Key == "Add New Variant")
            {
                //add new variant
                ProductVariant newV = new();
                newV.Price = double.Parse(PriceBox.Text);
                newV.OurStock = int.Parse(OurStockBox.Text);
                newV.Barcode = VariantBarcodeBox.Text;
                newV.VariantType = VariantTypeBox.Text;
                newV.VariantData = VariantDataBox.Text;
                newV.PermPrice = PermPriceCheckBox.IsChecked ?? false;

                //add to combobox dictionary
                string newKey = GetVariantComboBoxItemName(newV);
                EditableVariantsKVP.Add(newKey, newV);
                ProductVariantComboBox.ItemsSource = EditableVariantsKVP;
                ProductVariantComboBox.Items.Refresh();

                //clearing all varaint textboxes
                PriceBox.Clear();
                OurStockBox.Clear();
                VariantBarcodeBox.Clear();
                VariantDataBox.Clear();
                VariantTypeBox.Clear();
                PermPriceCheckBox.IsChecked = false;
            }
            else
            {
                ProductVariant selectedVariant = ((KeyValuePair<string, ProductVariant>)selectedVariantKVP).Value;
                string key = ((KeyValuePair<string, ProductVariant>)selectedVariantKVP).Key;

                //saving product variant data
                selectedVariant.Barcode = VariantBarcodeBox.Text;
                selectedVariant.VariantType = VariantTypeBox.Text;
                selectedVariant.VariantData = VariantDataBox.Text;
                selectedVariant.VendorStock = int.Parse(VendorStockBox.Text);
                selectedVariant.OurStock = int.Parse(OurStockBox.Text);
                selectedVariant.Price = double.Parse(PriceBox.Text);
                selectedVariant.PriceVendor = double.Parse(VendorPriceBox.Text);
                selectedVariant.PermPrice = PermPriceCheckBox.IsChecked == true;

                //replace in combobox dictionary
                string newKey = GetVariantComboBoxItemName(selectedVariant);
                EditableVariantsKVP.Remove(key);
                EditableVariantsKVP.Add(newKey, selectedVariant);
                ProductVariantComboBox.ItemsSource = EditableVariantsKVP;
                ProductVariantComboBox.Items.Refresh();
            }

            //adding animation
            SaveVariantLabel.Visibility = Visibility.Visible;
            BackgroundWorker worker = new();
            worker.DoWork += (sender, e) =>
            {
                //wait for 3s
                System.Threading.Thread.Sleep(3000);
            };
            worker.RunWorkerCompleted += (sender, e) =>
            {
                SaveVariantLabel.Visibility = Visibility.Collapsed;
            };
            worker.RunWorkerAsync();

            //setting product changed flag to true
            ProductChanged = true;
        }


        // Saving new Product Section

        /// <summary>
        /// (Overridden) button onClick method that saves product to database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void SaveProductButton_Click(object sender, RoutedEventArgs e)
        {
            SaveProduct();
            ProductChanged = false;
            NavigateToPreviousPage();
        }

        /// <summary>
        /// Collects data from UI and creates new product object, saves it to database and ProductStore  
        /// </summary>
        /// <returns></returns>
        private void SaveProduct()
        {
            FullProduct newProduct = new();

            //adding sku
            newProduct.SKU = SKUBox.Text;

            //checking if sku was changed
            if (newProduct.SKU == "IKR-") {
                var DialogOKBox = new DialogueOK("Select new SKU");
                DialogOKBox.ShowDialog();

                return;
            }

            //adding string values
            newProduct.TitleLT = TitleBoxLT.Text;
            newProduct.TitleLV = TitleBoxLV.Text;
            newProduct.TitleEE = TitleBoxEE.Text;
            newProduct.TitleRU = TitleBoxRU.Text;

            newProduct.DescLT = DescBoxLT.Text;
            newProduct.DescLV = DescBoxLV.Text;
            newProduct.DescEE = DescBoxEE.Text;
            newProduct.DescRU = DescBoxRU.Text;

            newProduct.Vendor = VendorBox.Text;

            //saving product type selection category

            //saving product dimensions
            newProduct.Height = int.Parse(HeightBox.Text);
            newProduct.Width = int.Parse(WidthBox.Text);
            newProduct.Lenght = int.Parse(LenghtBox.Text);
            newProduct.Weight = double.Parse(WeightBox.Text);

            //adding tags and images;
            newProduct.Images = ImgListBoxDataSource.ToList();
            newProduct.Tags = TagListBoxDataSource.ToList();

            //adding vendor type and added date
            newProduct.AddedTimeStamp = EditableProduct.AddedTimeStamp;
            newProduct.ProductTypeVendor = EditableProduct.ProductTypeVendor;
            newProduct.DeliveryTime = DeliveryTimeBox.Text;

            //saving variants and attributes

            newProduct.ProductVariants = EditableVariantsKVP.Where(v => v.Key != "Add New Variant").ToDictionary(i => i.Key, i => i.Value).Values.ToList();
            newProduct.ProductAttributtes = EditableProduct.ProductAttributtes;

            //saving product util fields
            newProduct.Status = EditableProduct.Status;
            newProduct.ProductTypeDisplayVal = ProductType.Item2;
            newProduct.ProductTypeID = ProductType.Item1.ToString();

            //saving Product Status
            newProduct.Status = ProductStatusComboBox.SelectedItem as string;

            var a = newProduct;
            ProductStore.AddProductToDB(newProduct);
            ProductStore.Instance.ProductKVP.Add(newProduct.SKU, newProduct);
        }


        // Back button behaviour Section

        /// <summary>
        /// (Overridden) button onClick method that to navigate to previous page 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ProductChanged)
            {
                NavigateToPreviousPage();
            }
            else
            {
                DialogueYN dialog = new("Save New product?");
                bool answer = dialog.ShowDialog() ?? false;

                if (answer)
                {
                    SaveProduct();
                    NavigateToPreviousPage();
                }
                else
                {
                    //restoring tags and images
                    EditableProduct.Images = ProductImages;
                    EditableProduct.Tags = ProductTags;
                    NavigateToPreviousPage();
                }
            }
        }

        //todo: 
        // do attributtes
        // do multitehreading in downloading 
    }
}
