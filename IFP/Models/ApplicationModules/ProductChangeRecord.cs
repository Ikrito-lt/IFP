using System.Collections.Generic;

namespace IFP.Models
{
    /// <summary>
    /// Record that is used to store data on changes made when updating product data from supplier APIs
    /// </summary>
    internal record ProductChangeRecord
    {
        //record for changes applied to products
        public string SKU { get; init; } = "SKU Not Set";
        public string PriceVendor { get; set; } = "PriceVendor Not Set";
        public string Price { get; set; } = "Price Not Set";
        public string VendorStock { get; set; } = "VendorStock Not Set";
        public string Barcode { get; init; } = "Barcode Not Set";
        public string Vendor { get; init; } = "Vendor Not Set";
        public string VendorProductType { get; init; } = "VendorProductType Not Set";
        public string ProductType { get; init; } = "ProductType Not Set";
        public string Status { get; init; } = "Status Not Set";
        public string VariantData { get; set; } = "VariantData Not Set";
        public int VariantCount { get; set; }
        public List<string> ChangesMade = new();

        public string getFormattedSKU => $"{SKU} ({VariantCount}V)";
        public string getChangesMade {
            get {
                string s = "";
                if (ChangesMade.Count > 0)
                {
                    foreach (var change in ChangesMade)
                    {
                        s += $"{change}\n";
                    }
                }
                return s;
            }        
        }
    }
}
