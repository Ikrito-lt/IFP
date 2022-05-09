﻿using IFP.Models;
using IFP.Singletons;
using IFP.Utils;
using System.Collections.Generic;
using System.ComponentModel;

namespace IFP.Modules
{
    static class ProductUpdateModule
    {

        //
        // General method section
        //

        //method to get list of sync product from database
        public static List<ProductState> GetSyncProducts(string status = null)
        {
            List<ProductState> p = new();
            DataBaseInterface db = new();
            Dictionary<string, Dictionary<string, string>> whereCond;

            if (status == null)
            {
                whereCond = new Dictionary<string, Dictionary<string, string>>
                {
                    ["Status"] = new Dictionary<string, string>
                    {
                        ["!="] = ProductStatus.Ok
                    }
                };
            }
            else
            {
                whereCond = new Dictionary<string, Dictionary<string, string>>
                {
                    ["Status"] = new Dictionary<string, string>
                    {
                        ["="] = status
                    }
                };
            }

            var result = db.Table("Products").Where(whereCond).Get();
            foreach (var row in result.Values)
            {
                ProductState syncProduct = new();

                syncProduct.sku = row["SKU"];
                syncProduct.productType_ID = row["ProductType_ID"];
                syncProduct.status = row["Status"];
                syncProduct.lastUpdateTime = row["LastUpdateTime"].UnixTimeToSrt();

                if (row["LastSyncTime"] == null || row["LastSyncTime"] == "")
                {
                    syncProduct.lastSyncTime = "Not In Shop";
                }
                else
                {
                    syncProduct.lastSyncTime = row["LastSyncTime"].UnixTimeToSrt();
                }

                p.Add(syncProduct);
            }

            p.RemoveAll(x => x.status == ProductStatus.Archived);
            return p;
        }

        //checking if products are in stock if not, chnaging their status
        public static void ChangeStatusByStock(object sender, DoWorkEventArgs e)
        {
            //setting up progress reporting
            var worker = sender as BackgroundWorker;
            int promiles = 0;
            worker.ReportProgress(promiles, (true, $"Downloading Products From DataBase"));

            //downloading list of all products
            ProductStore.Instance.GetProductKVP();
            Dictionary<string, FullProduct> fullProducts = ProductStore.Instance.ProductKVP;

            //looping and changing statuses if necessery 
            int productCount = fullProducts.Count;
            foreach (((string sku, FullProduct product), int index) in fullProducts.LoopIndex())
            {
                //reporting
                promiles = (1000 * index) / productCount;
                worker.ReportProgress(promiles, (false, $"Changing Product Status If Stock > 0 ({index}/{productCount})"));

                //cause new status only changes by button
                if (product.Status != ProductStatus.New)
                {
                    //checking if product isn out of stock
                    if (!ProductStore.CheckIfProductOutOfStock(product))
                    {
                        ProductStore.ChangeProductStatus(sku, ProductStatus.OutOfStock, false);
                    }
                }
            }
        }

        //
        // Export sync product section
        //

        ////method sets out of stock products as NeedsArchiving
        //public void MarkOutOfStockNeedsArchival(object sender, DoWorkEventArgs e)
        //{
        //    List<ProductState> syncProducts = GetSyncProducts();
        //    syncProducts.AddRange(GetSyncProducts(ProductStatus.Ok));
        //    int count = syncProducts.Count;
        //    for (int i = 0; i < count; i++)
        //    {
        //        FullProduct p = ProductModule.GetProduct(syncProducts[i].sku);
        //        if (p.productVariants.All(x => x.stock < 0))
        //        {
        //            ProductModule.ChangeProductStatus(syncProducts[i].sku, ProductStatus.NeedsArchiving);
        //        }

        //        int progress = i * 1000 / count;
        //        (sender as BackgroundWorker).ReportProgress(progress);
        //    }
        //}


        //
        // Export sync product section
        //

        ////method that is used by the Export sync product worker to send each product to ExportSyncProduct and track exporting progress
        //public void ExportShopifyProducts(object sender, DoWorkEventArgs e)
        //{
        //    List<ProductState> syncProducts = GetSyncProducts();
        //    int count = syncProducts.Count;
        //    for (int i = 0; i < count; i++)
        //    {
        //        ExportShopifyProduct(syncProducts[i]);

        //        int progress = i * 1000 / count;
        //        (sender as BackgroundWorker).ReportProgress(progress);
        //    }
        //}

        ////method decides what to do with syncProduct
        //private void ExportShopifyProduct(ProductState sync) {
        //    FullProduct p = ProductModule.GetProduct(sync.sku);

        //    if (sync.status == ProductStatus.New) {
        //        NewShopifyProduct(p, sync);
        //    } else if (sync.status == ProductStatus.NeedsArchiving) {
        //        ArchiveShopifyProduct(sync);
        //    } else if (sync.status == ProductStatus.NeedsUnArchiving) {
        //        UpdateShopifyProduct(p, sync);
        //    } else if (sync.status == ProductStatus.WaitingShopSync) {
        //        UpdateShopifyProduct(p, sync);
        //    } else {
        //        throw new Exception($"No Product Status - {sync.status}");
        //    }
        //}


        //
        // section with diffrent export options
        //

        ////method that Archives product in shopify web-store
        //private void UnArchiveShopifyProduct(ProductState sync) {
        //    //archiving product in shopify
        //    string unArchiveRequestBody = $@"{{""product"": {{""id"": {sync.shopifyID},""status"": ""active""}}}}";

        //    IRestResponse unArchiveRes = ProductClient.ExecPutProd($"products/{sync.shopifyID}.json", mainHeaders, unArchiveRequestBody);
        //    if (!unArchiveRes.IsSuccessful) {
        //        if (unArchiveRes.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable) {
        //            Thread.Sleep(5000);

        //            UnArchiveShopifyProduct(sync);
        //            return;
        //        } else {
        //            throw unArchiveRes.ErrorException;
        //        }
        //    }

        //    //updating product status
        //    try {
        //        ProductModule.ChangeProductStatus(sync.sku, ProductStatus.WaitingShopSync);
        //    }
        //    catch (Exception ex) {
        //        MessageBox.Show("An exception just occurred:\n" + ex.Message + "\n\nSend screenshot you know where.", "Change Product Status Exception", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        //method that Archives product in shopify web-store
        //private void ArchiveShopifyProduct(ProductState sync) {
        //    //archiving product in shopify
        //    string archiveRequestBody = $@"{{""product"": {{""id"": {sync.shopifyID},""status"": ""archived""}}}}";

        //    IRestResponse archiveRes = ProductClient.ExecPutProd($"products/{sync.shopifyID}.json", mainHeaders, archiveRequestBody);
        //    if (!archiveRes.IsSuccessful) {
        //        if (archiveRes.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable) {
        //            Thread.Sleep(5000);

        //            ArchiveShopifyProduct(sync);
        //            return;
        //        } else {
        //            throw archiveRes.ErrorException;
        //        }
        //    }

        //    //updating product status
        //    try {
        //        ProductModule.ChangeProductStatus(sync.sku, ProductStatus.Archived);
        //    }
        //    catch (Exception ex) {
        //        MessageBox.Show("An exception just occurred:\n" + ex.Message + "\n\nSend screenshot you know where.", 
        //            "Change Product Status Exception", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        //method that updates product in shopify web-store
        //private void UpdateShopifyProduct(FullProduct p, ProductState sync) {
        //    //updating product in shopify
        //    IRestResponse updateRes = ProductClient.ExecPutProd($"products/{sync.shopifyID}.json", mainHeaders, p.GetImportJsonString());
        //    if (!updateRes.IsSuccessful) {
        //        if (updateRes.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable) {
        //            Thread.Sleep(5000);

        //            UpdateShopifyProduct(p, sync);
        //            return;
        //        } else {
        //            throw updateRes.ErrorException;
        //        }
        //    }

        //    //getting various shopify ids required for the update
        //    dynamic data = JsonConvert.DeserializeObject<dynamic>(updateRes.Content);
        //    string shopifyVariantID = data["product"]["variants"].First["id"].Value.ToString();
        //    string inventoryItemID = data["product"]["variants"].First["inventory_item_id"].Value.ToString();

        //    sync.shopifyVariantID = shopifyVariantID;
        //    sync.inventoryItemID = inventoryItemID;

        //    //to update metafields i need to get metafield IDs
        //    //its was ExecGet();
        //    string getMetaRes = ProductClient.ExecGetHeader($"products/{sync.shopifyID}/metafields.json", mainHeaders);
        //    var ids = ExtractMetafieldIDs(getMetaRes);

        //    //updating height metafield
        //    if (ids.ContainsKey("dimensions")) {
        //        string heightVal = $"\"{{\\\"unit\\\": \\\"mm\\\",\\\"value\\\": {p.height}}}\"";
        //        string heightBody = $@"{{""metafield"": {{""value"": {heightVal} }}}}";
        //        IRestResponse heightRes = ProductClient.ExecPutProd($"products/{sync.shopifyID}/metafields/{ids["dimensions"]}.json", mainHeaders, heightBody);
        //        if (!heightRes.IsSuccessful) {
        //            if (heightRes.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable) {
        //                Thread.Sleep(5000);

        //                UpdateShopifyProduct(p, sync);
        //                return;
        //            } else {
        //                throw heightRes.ErrorException;
        //            }
        //        }
        //    } else {
        //        throw new Exception($"SKU: {sync.sku} >> UpdateFail no heightMeta");
        //    }

        //    //updating lenght metafield
        //    if (ids.ContainsKey("lenght")) {
        //        string lenghtVal = $"\"{{\\\"unit\\\": \\\"mm\\\",\\\"value\\\": {p.lenght}}}\"";
        //        string lenghtBody = $@"{{""metafield"": {{""value"": {lenghtVal} }}}}";
        //        IRestResponse lenghtRes = ProductClient.ExecPutProd($"products/{sync.shopifyID}/metafields/{ids["lenght"]}.json", mainHeaders, lenghtBody);
        //        if (!lenghtRes.IsSuccessful) {
        //            if (lenghtRes.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable) {
        //                Thread.Sleep(5000);

        //                UpdateShopifyProduct(p, sync);
        //                return;
        //            } else {
        //                throw lenghtRes.ErrorException;
        //            }
        //        }
        //    } else {
        //        throw new Exception($"SKU: {sync.sku} >> UpdateFail no lenghtMeta");
        //    }

        //    //updating width metafield
        //    if (ids.ContainsKey("width")) {
        //        string widthVal = $"\"{{\\\"unit\\\": \\\"mm\\\",\\\"value\\\": {p.width}}}\"";
        //        string widthBody = $@"{{""metafield"": {{""value"": {widthVal} }}}}";
        //        IRestResponse widthRes = ProductClient.ExecPutProd($"products/{sync.shopifyID}/metafields/{ids["width"]}.json", mainHeaders, widthBody);
        //        if (!widthRes.IsSuccessful) {
        //            if (widthRes.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable) {
        //                Thread.Sleep(5000);

        //                UpdateShopifyProduct(p, sync);
        //                return;
        //            } else {
        //                throw widthRes.ErrorException;
        //            }
        //        }
        //    } else {
        //        throw new Exception($"SKU: {sync.sku} >> UpdateFail no widthMeta");
        //    }

        //    //updating vendor price
        //    IRestResponse vendorPriceRes = ProductClient.ExecPutProd($"inventory_items/{sync.inventoryItemID}.json", mainHeaders, p.GetVendorPriceBody());
        //    if (!vendorPriceRes.IsSuccessful) {
        //        if (vendorPriceRes.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable) {
        //            Thread.Sleep(5000);

        //            UpdateShopifyProduct(p, sync);
        //            return;
        //        } else {
        //            throw vendorPriceRes.ErrorException;
        //        }
        //    }

        //    //updating product status
        //    try {
        //        ProductModule.ChangeProductStatus(sync.sku, ProductStatus.Ok);
        //    }
        //    catch (Exception ex) {
        //        MessageBox.Show("An exception just occurred:\n" + ex.Message + "\n\nSend screenshot you know where.",
        //                        "Change Product Status Exception", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        //method that adds new product to shopify web-store
        //    private void NewShopifyProduct(FullProduct p, ProductState sync) {

        //        //adding product to shopify
        //        IRestResponse creationRes = ProductClient.ExecAddProd(ProductPath, mainHeaders, p.GetImportJsonString());
        //        if (!creationRes.IsSuccessful) {
        //            if (creationRes.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable) {
        //                Thread.Sleep(5000);

        //                bool deleted = ProductClient.ExecDeleteProd(mainHeaders, sync.shopifyID);
        //                if (deleted == false) {
        //                    throw new Exception($"SKU: {sync.sku} >> 503 and DeleteFail (NewShopifyProduct createProd)");
        //                } else {
        //                    NewShopifyProduct(p, sync);
        //                    return;
        //                }
        //            } else {
        //                throw creationRes.ErrorException;
        //            }
        //        }

        //        dynamic data = JsonConvert.DeserializeObject<dynamic>(creationRes.Content);
        //        string shopifyID = data["product"]["variants"].First["product_id"].Value.ToString();
        //        string shopifyVariantID = data["product"]["variants"].First["id"].Value.ToString();
        //        string inventoryItemID = data["product"]["variants"].First["inventory_item_id"].Value.ToString();

        //        sync.shopifyID = shopifyID;
        //        sync.shopifyVariantID = shopifyVariantID;
        //        sync.inventoryItemID = inventoryItemID;

        //        //adding height metafield
        //        IRestResponse heightRes = ProductClient.ExecAddProd($"products/{shopifyID}/metafields.json", mainHeaders, p.GetHeightMetaBody());
        //        if (!heightRes.IsSuccessful) {
        //            if (heightRes.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable) {
        //                Thread.Sleep(5000);

        //                bool deleted = ProductClient.ExecDeleteProd(mainHeaders, sync.shopifyID);
        //                if (deleted == false) {
        //                    throw new Exception($"SKU: {sync.sku} >> 503 and DeleteFail (NewShopifyProduct height)");
        //                } else {
        //                    NewShopifyProduct(p, sync);
        //                    return;
        //                }
        //            } else {
        //                throw heightRes.ErrorException;
        //            }
        //        }

        //        //adding lenght metafield
        //        IRestResponse lenghtRes = ProductClient.ExecAddProd($"products/{shopifyID}/metafields.json", mainHeaders, p.GetLenghtMetaBody());
        //        if (!lenghtRes.IsSuccessful) {
        //            if (lenghtRes.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable) {
        //                Thread.Sleep(5000);

        //                bool deleted = ProductClient.ExecDeleteProd(mainHeaders, sync.shopifyID);
        //                if (deleted == false) {
        //                    throw new Exception($"SKU: {sync.sku} >> 503 and DeleteFail (NewShopifyProduct lenght)");
        //                } else {
        //                    NewShopifyProduct(p, sync);
        //                    return;
        //                }
        //            } else {
        //                throw lenghtRes.ErrorException;
        //            }
        //        }

        //        //adding width metafield
        //        IRestResponse widthRes = ProductClient.ExecAddProd($"products/{shopifyID}/metafields.json", mainHeaders, p.GetWidthMetaBody());
        //        if (!widthRes.IsSuccessful) {
        //            if (widthRes.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable) {
        //                Thread.Sleep(5000);

        //                bool deleted = ProductClient.ExecDeleteProd(mainHeaders, sync.shopifyID);
        //                if (deleted == false) {
        //                    throw new Exception($"SKU: {sync.sku} >> 503 and DeleteFail (NewShopifyProduct width)");
        //                } else {
        //                    NewShopifyProduct(p, sync);
        //                    return;
        //                }
        //            } else {
        //                throw widthRes.ErrorException;
        //            }
        //        }

        //        //adding vendor price
        //        IRestResponse vendorPriceRes = ProductClient.ExecPutProd($"inventory_items/{inventoryItemID}.json", mainHeaders, p.GetVendorPriceBody());
        //        if (!vendorPriceRes.IsSuccessful) {
        //            if (vendorPriceRes.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable) {
        //                Thread.Sleep(5000);

        //                bool deleted = ProductClient.ExecDeleteProd(mainHeaders, sync.shopifyID);
        //                if (deleted == false) {
        //                    throw new Exception($"SKU: {sync.sku} >> 503 and DeleteFail (NewShopifyProduct vendorPrice)");
        //                } else {
        //                    NewShopifyProduct(p, sync);
        //                    return;
        //                }
        //            } else {
        //                throw vendorPriceRes.ErrorException;
        //            }
        //        }

        //        //adding new shopify id to
        //        DataBaseInterface db = new();
        //        var updateData = new Dictionary<string, string> {
        //            ["ShopifyID"] = sync.shopifyID,
        //            ["ShopifyVariantID"] = sync.shopifyVariantID,
        //            ["ShopifyInventoryItemID"] = sync.inventoryItemID,
        //            ["Status"] = ProductStatus.Ok,
        //            ["LastSyncTime"] = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds().ToString()
        //        };
        //        var whereUpdate = new Dictionary<string, Dictionary<string, string>> {
        //            ["SKU"] = new Dictionary<string, string> {
        //                ["="] = sync.sku
        //            }
        //        };
        //        db.Table("Products").Where(whereUpdate).Update(updateData);

        //    }

        //    //method that extrack metafield IDs from json response (needed to update metafield values in update method)
        //    public static Dictionary<string, string> ExtractMetafieldIDs(string json) {
        //        Dictionary<string, string> ids = new();

        //        dynamic dJson = JsonConvert.DeserializeObject(json);
        //        var metaFields = dJson["metafields"];

        //        foreach (var field in metaFields) {
        //            string key = Convert.ToString(field["key"]);
        //            string id = Convert.ToString(field["id"]);
        //            string nameSpace = Convert.ToString(field["namespace"]);

        //            if (nameSpace == "my_fields") {
        //                if (key == "lenght" || key == "dimensions" || key == "width") {
        //                    ids.Add(key, id);
        //                }
        //            }
        //        }
        //        return ids;
        //    }
        //}
    }
}
