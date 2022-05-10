using IFP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace IFP.Pages
{
    internal class NewProductPage : ProductEditPage
    {
        // Constructor
        public NewProductPage(Page prevPage) : base(new FullProduct(creatingNew: true), prevPage) {
            MainWindow.Instance.SetWindowTitle($"New Product Page");
            NewProductPageInit();
        }

        // Init Page Section
        
        /// <summary>
        /// This method initializes
        /// </summary>
        private void NewProductPageInit() { 
            // disabling delebe product button
            DeleteProductButton.IsEnabled = false;
            // enabling variant barcode textbox
            VariantBarcodeBox.IsReadOnly = false;
            // Adding Preview text input to SKU box
            SKUBox.IsReadOnly = false;
            SKUBox.PreviewKeyDown += SKUBox_PreviewKeyDown;
            SKUBox.PreviewTextInput += SKUPreviewTextInput;
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
            string updatedText = textBox.Text + e.Text;

            e.Handled = !updatedText.StartsWith("IKR-");
        }



        //todo: 
        // add on flip save but sku added
        // diffrent save
        // do barcode
        // do attributtes
        // do multitehreading in downloading 
    }
}
