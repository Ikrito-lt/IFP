namespace IFP.Models
{
    class ProductState {
        public string sku { set; get; } = "Product State SKU Not Set";
        public string productType_ID { set; get; } = "Product State Category ID Not Set";
        public string productTypeVal { set; get; } = "Product State Category Not Set";
        public string status { set; get; } = "Product Status Not Set";
        public string lastSyncTime { set; get; } = "Product State Last Sync Timestamp Not Set";
        public string lastUpdateTime { set; get; } = "Product State Last Update Timestamp Not Set";
    }
}
