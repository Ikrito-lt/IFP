using System;
using System.Collections.Generic;
using System.Linq;

namespace IFP.Models
{
    /// <summary>
    /// Class for storing full product model
    /// </summary>
    public class FullProduct
    {
        public string SKU { set; get; } = "Product SKU Not Set";
        public string ProductTypeID { set; get; }
        public string Vendor { set; get; } = "Product Vendor Not Set";
        public string TitleLT { set; get; } = "Product LT Title Not Set";
        public string TitleLV { set; get; }
        public string TitleEE { set; get; }
        public string TitleRU { set; get; }
        public string DescLT { set; get; } = "Product LT Description Not Set";
        public string DescLV { set; get; }
        public string DescEE { set; get; }
        public string DescRU { set; get; }
        public double Weight { set; get; } = .0;                //in kg
        public int Height { set; get; } = 0;                    //in mm
        public int Lenght { set; get; } = 0;                    //in mm
        public int Width { set; get; } = 0;                     //in mm
        public string ProductTypeVendor { set; get; }  //for saving vendor product type to database.
        public string DeliveryTime { set; get; }       //delivery time string
        public string AddedTimeStamp { set; get; }     //timestamp of when product was created
        public DateTime GetAddedTime()
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            double timestamp = double.Parse(AddedTimeStamp ?? "0");
            dateTime = dateTime.AddSeconds(timestamp).ToLocalTime();
            return dateTime;
        }

        public List<string> Tags = new();
        public List<string> Images = new();
        public List<ProductVariant> ProductVariants = new();
        public List<ProductAttribute> ProductAttributtes = new();

        //for util
        public string Status { set; get; }              //for product status as in sync Status
        public string ProductTypeDisplayVal { set; get; }   // for saving category text val

        //for showing data to screen
        public string VariantCount => ProductVariants?.Count > 0 ? ProductVariants.Count.ToString() : "0";
        public string FirstVariantOurStock => ProductVariants?.Count > 0 ? ProductVariants.First().OurStock.ToString() : "NaN";
        public string FirstVariantVendorStock => ProductVariants?.Count > 0 ? ProductVariants.First().VendorStock.ToString() : "NaN";
        public string FirstVariantPrice => ProductVariants?.Count > 0 ? ProductVariants.First().Price.ToString() : "NaN";
        public string FirstVariantVendorPrice => ProductVariants?.Count > 0 ? ProductVariants.First().PriceVendor.ToString() : "NaN";


        /// <summary>
        /// For describing product attributtes
        /// </summary>
        public class ProductAttribute
        {
            public string Name { set; get; }
            public string Attribute { set; get; }

            public ProductAttribute(string name, string attr) { 
                this.Name = name;
                this.Attribute = attr;
            }
        }


        /// <summary>
        /// class for describing product variants
        /// </summary>
        public class ProductVariant
        {
            public int VariantDBID { set; get; }
            public double Price { set; get; }
            public int VendorStock { set; get; }
            public int OurStock { set; get; }
            public string Barcode { set; get; }
            public double PriceVendor { set; get; }
            public string VariantType { set; get; }
            public string VariantData { set; get; }
            public bool PermPrice { set; get; }

            public bool isSame(ProductVariant other)
            {
                if (Barcode != other.Barcode) return false;
                if (PriceVendor != other.PriceVendor) return false;
                if (VendorStock != other.VendorStock) return false;
                return true;
            }
        }


        // For creating new product inside the app
        public FullProduct(bool creatingNew = false) {
            if (creatingNew) {
                SKU = "IKR-";
                ProductTypeID = "1"; 
                Vendor = "Product Vendor Not Set";
                TitleLT = "Product LT Title Not Set";
                TitleLV = "";
                TitleEE = "";
                TitleRU = ""; 
                DescLT = "Product LT Description Not Set";
                DescLV = "";
                DescEE = "";
                DescRU = ""; 
                Weight = .0;                //in kg
                Height = 0;                    //in mm
                Lenght = 0;                    //in mm
                Width = 0;                     //in mm
                ProductTypeVendor = "";
                DeliveryTime = "3";
                AddedTimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
                Status = "New";
                ProductTypeDisplayVal = "Not-Assigned";
            }        
        }
    }
}
