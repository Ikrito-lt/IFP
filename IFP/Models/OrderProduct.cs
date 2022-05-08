using IFP.Utils;

namespace IFP.Models
{
    public class OrderProduct {
        //DB_ID
        public string DBID { set; get; } = "Order Product DBID Not Set";
        public string OrderDBID { set; get; } = "Order DBID Not Set";
        public string sku { set; get; } = "Order Product SKU Not Set";
        public string vendor { set; get; } = "Order Product Vendor Not Set";
        public string name { set; get; } = "Order Product Name Not Set";
        public string name_trimmed => name.Trim().Truncate(65);

        public int quantity { set; get; }
        public string vnt => $"{quantity} vnt.";
        public double price { set; get; }
        public string priceStr => $"€ {price}";
        public string full_price => $"€ {quantity * price}";
        public double total_discount { set; get; }
        public string discountStr => $"€ {total_discount}";

        public int grams { set; get; }

        public bool product_exists { set; get; }
        public bool taxable { set; get; }

        public string Barcode { set; get; } = "Order Product Barcode Not Set";
        public string variantName { set; get; } = "Order Product Variant Name Not Set";
    }
}
