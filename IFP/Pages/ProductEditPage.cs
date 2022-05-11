using IFP.Models;
using IFP.Singletons;
using IFP.UI;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static IFP.Models.FullProduct;

namespace IFP.Pages
{
    internal class ProductEditPage : ProductInfoPage
    {
        protected bool ProductChanged = false;

        // Constructor
        public ProductEditPage(FullProduct product, Page prevPage) : base(product, prevPage)
        {
            MainWindow.Instance.SetWindowTitle($"Product Edit Page ({product.SKU})");
            MakePageEditable();
            AddProductChangedCheck();
            InitProductAttributeDG();
        }


        // Init Section

        /// <summary>
        /// Method that makes ProductInfoPage UI editable to enable the editing of product information
        /// </summary>
        private void MakePageEditable()
        {
            // Main section
            DescBoxLT.IsReadOnly = false;
            DescBoxLV.IsReadOnly = false;
            DescBoxEE.IsReadOnly = false;
            DescBoxRU.IsReadOnly = false;

            TitleBoxLT.IsReadOnly = false;
            TitleBoxLV.IsReadOnly = false;
            TitleBoxEE.IsReadOnly = false;
            TitleBoxRU.IsReadOnly = false;

            VendorBox.IsReadOnly = false;
            SelectCategoryButton.IsEnabled = true;

            DeliveryTimeBox.IsReadOnly = false;

            WeightBox.IsReadOnly = false;
            LenghtBox.IsReadOnly = false;
            HeightBox.IsReadOnly = false;
            WidthBox.IsReadOnly = false;
            ImageBox.IsReadOnly = false;
            TagBox.IsReadOnly = false;

            AddImageButton.IsEnabled = true;
            AddTagButton.IsEnabled = true;

            //assign commands
            DeleteImageCommand = new DelegateCommand<object>(DeleteImage);
            DeleteTagCommand = new DelegateCommand<object>(DeleteTag);

            // Attribute data grid
            productAttributesDG.IsReadOnly = false;

            // Status
            ProductStatusComboBox.IsEnabled = true;

            // Variant section
            VariantTypeBox.IsReadOnly = false;
            VariantDataBox.IsReadOnly = false;

            OurStockBox.IsReadOnly = false;
            PriceBox.IsReadOnly = false;

            PermPriceCheckBox.IsEnabled = true;
            SaveVariantButton.IsEnabled = true;

            // Selecting first variant
            if (!ProductVariantComboBox.Items.IsEmpty)
            {
                ProductVariantComboBox.SelectedIndex = 0;
            }

            // Changing edit button to save
            SaveProductButton.Visibility = Visibility.Visible;
            EditProductButton.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Method to add product changed check to text boxes 
        /// </summary>
        private void AddProductChangedCheck() {
            // adding on change text flip product saved bool
            TitleBoxLT.TextChanged += SaveFlip_TextChanged;
            TitleBoxLV.TextChanged += SaveFlip_TextChanged;
            TitleBoxEE.TextChanged += SaveFlip_TextChanged;
            TitleBoxRU.TextChanged += SaveFlip_TextChanged;

            DescBoxLT.TextChanged += SaveFlip_TextChanged;
            DescBoxLV.TextChanged += SaveFlip_TextChanged;
            DescBoxEE.TextChanged += SaveFlip_TextChanged;
            DescBoxRU.TextChanged += SaveFlip_TextChanged;

            VendorBox.TextChanged += SaveFlip_TextChanged;
            DeliveryTimeBox.TextChanged += SaveFlip_TextChanged;

            WeightBox.TextChanged += SaveFlip_TextChanged;
            HeightBox.TextChanged += SaveFlip_TextChanged;
            WidthBox.TextChanged += SaveFlip_TextChanged;
            LenghtBox.TextChanged += SaveFlip_TextChanged;

            // adding category KVP to product type combobox
            ProductStatusComboBox.SelectionChanged += SaveFlipComboBox_SelectionChanged;
        }


        // Methods that flip Product changed bit on textbox or combobox change 

        /// <summary>
        /// flips ProductChanged bool to indicate that product was changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SaveFlip_TextChanged(object sender, TextChangedEventArgs e)
        {
            ProductChanged = true;
        }

        /// <summary>
        /// flips ProductChanged bool to indicate that product was changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SaveFlipComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProductChanged = true;
        }


        // Methods that get passed to buttons in tag and image listboxes

        /// <summary>
        /// method deletes image link from list box (passed as a command to the button)
        /// </summary>
        /// <param name="item"></param>
        private void DeleteImage(object item)
        {
            ImgListBoxDataSource.Remove(item as string);
            EditableProduct.Images.Remove(item as string);

            //setting product changed flag to true
            ProductChanged = true;
        }

        /// <summary>
        /// method deletes image link from list box (passed as a command to the button)
        /// </summary>
        /// <param name="item"></param>
        private void DeleteTag(object item)
        {
            TagListBoxDataSource.Remove(item as string);
            EditableProduct.Tags.Remove(item as string);

            //setting product changed flag to true
            ProductChanged = true;
        }


        // Section for onClick methods that are overridden in child classes 

        /// <summary>
        /// (Overridden) button onClick for changing product category
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void SelectCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var categoryTreeSelector = new CategoryTreeSelectorWindow("Select Category From CategoryTree");
            if (categoryTreeSelector.ShowDialog() == true)
            {
                var selectedCategoryTuple = categoryTreeSelector.selectionResult;
                button.Content = selectedCategoryTuple.Item2;
                ProductType = selectedCategoryTuple;
            }
            else
            {
                button.Content = "Not-Assigned";
                ProductType = new(1, "Not-Assigned");
            }
        }

        /// <summary>
        /// (Overridden) button onClick method that adds link to product.images
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void AddImageButton_Click(object sender, RoutedEventArgs e)
        {
            string newImageLink = ImageBox.Text;
            ImgListBoxDataSource.Add(newImageLink);
            EditableProduct.Images.Add(newImageLink);
            ImageBox.Text = null;

            //setting product changed flag to true
            ProductChanged = true;
        }

        /// <summary>
        /// (Overridden) button onClick method that adds tag to product.tags
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void AddTagButton_Click(object sender, RoutedEventArgs e)
        {
            string newTag = TagBox.Text;
            TagListBoxDataSource.Add(newTag);
            EditableProduct.Tags.Add(newTag);
            TagBox.Text = null;

            //setting product changed flag to true
            ProductChanged = true;
        }

        /// <summary>
        /// (Overridden) button onClick method that saves product variant to database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void SaveVariantButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedVariantKVP = ProductVariantComboBox.SelectedItem;
            //returning if not variant is selected
            if (selectedVariantKVP == null) return;

            ProductVariant selectedVariant = ((KeyValuePair<string, ProductVariant>)selectedVariantKVP).Value;
            string key = ((KeyValuePair<string, ProductVariant>)selectedVariantKVP).Key;

            //saving product variant data
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

        /// <summary>
        /// (Overridden) button onClick method that saves product to database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void SaveProductButton_Click(object sender, RoutedEventArgs e)
        {
            SaveProduct();
            ProductChanged = false;
        }

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
                DialogueYN dialog = new("Save product?");
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


        // Saving Product Section

        /// <summary>
        /// Collects data from UI and creates new product object, saves it to database and ProductStore  
        /// </summary>
        /// <returns></returns>
        private void SaveProduct()
        {
            FullProduct newProduct = new();
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
            newProduct.SKU = EditableProduct.SKU;

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

            //saving variants
            newProduct.ProductVariants = EditableVariantsKVP.Values.ToList();

            //saving product attributes
            AttributeDataGridDataSource.RemoveAll(x => string.IsNullOrWhiteSpace(x.Name) || string.IsNullOrWhiteSpace(x.Attribute));
            newProduct.ProductAttributtes = AttributeDataGridDataSource;

            //saving product util fields
            newProduct.Status = EditableProduct.Status;
            newProduct.ProductTypeDisplayVal = ProductType.Item2;
            newProduct.ProductTypeID = ProductType.Item1.ToString();

            //saving Product Status
            newProduct.Status = ProductStatusComboBox.SelectedItem as string;

            //todo: change this and add manual product status change
            ProductStore.UpdateProductToDB(newProduct, newProduct.Status);
            ProductStore.Instance.ProductKVP[newProduct.SKU] = newProduct;
        }


        // Product Attiributtes section

        /// <summary>
        /// Init method for Product attribute datagrid
        /// </summary>
        protected void InitProductAttributeDG() {
            productAttributesDG.IsReadOnly = false;
            SaveAttributeButton.IsEnabled = true;
            AddAttributeButton.IsEnabled = true;
        }

        /// <summary>
        /// (Overridden) Button to save Product Attributes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void SaveAttributeButton_Click(object sender, RoutedEventArgs e) {
            ProductChanged = true;
            AttributeDataGridDataSource.RemoveAll(x => string.IsNullOrWhiteSpace(x.Name) || string.IsNullOrWhiteSpace(x.Attribute));
            productAttributesDG.ItemsSource = AttributeDataGridDataSource;
            productAttributesDG.Items.Refresh();

            //adding animation
            SaveAttributeLabel.Visibility = Visibility.Visible;
            BackgroundWorker worker = new();
            worker.DoWork += (sender, e) =>
            {
                //wait for 3s
                System.Threading.Thread.Sleep(3000);
            };
            worker.RunWorkerCompleted += (sender, e) =>
            {
                SaveAttributeLabel.Visibility = Visibility.Collapsed;
            };
            worker.RunWorkerAsync();
        }

        /// <summary>
        /// (Overridden) Button to add new product attribute
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void AddAttributeButton_Click(object sender, RoutedEventArgs e) {
            ProductChanged = true;
            AttributeDataGridDataSource.Add(new ProductAttribute("", ""));
            productAttributesDG.ItemsSource = AttributeDataGridDataSource;
            productAttributesDG.Items.Refresh();
        }






        //todo: add editing to product attributtes
    }
}
