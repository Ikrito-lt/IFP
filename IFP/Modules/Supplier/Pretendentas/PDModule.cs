﻿using IFP.Models;
using IFP.Modules.Supplier.Pretendentas.Models;
using IFP.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using static IFP.Models.FullProduct;

namespace IFP.Modules.Supplier.Pretendentas
{
    static class PDModule {

        private const string PDApiKey = Globals.PDApiKey;

        private static readonly Dictionary<string, string> _APIParams = new Dictionary<string, string>()
        {
            { "api_key",  PDApiKey},
        };

        private const string _BaseUrl = Globals.PDApi;
        private const string _CategoriesPath = "categories";
        private const string _ManufacturerPath = "manufacturers";

        private const string _SKUPrefix = "PD-";

        private static readonly Lazy<List<PDCategory>> _LazyCategoriesXML = new(() => GetPDCategories());
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private static List<PDCategory> _CategoriesXML => _LazyCategoriesXML.Value;

        private static readonly Lazy<List<PDManufacturer>> _LazyManufacturerList = new(() => GetPDManufacturerList());
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private static List<PDManufacturer> _ManufList => _LazyManufacturerList.Value;

        private static readonly Lazy<List<FullProduct>> _LazyProductList = new(() => BuildProductList());
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        public static List<FullProduct> ProductList => _LazyProductList.Value;


        //
        // lazy section
        //

        //lazy for getting the categories xml
        private static List<PDCategory> GetPDCategories() {
            RESTClient restClient = new(_BaseUrl);
            string xmlCategoriesStr = restClient.ExecGetParams(_CategoriesPath, _APIParams);

            XmlDocument CategoriesXML = new();
            CategoriesXML.LoadXml(xmlCategoriesStr);
            XmlNodeList catNodes = CategoriesXML.SelectNodes("//items/category");

            List<PDCategory> catList = new();

            foreach (XmlNode cat in catNodes) {
                int catProdCount = int.Parse(cat.SelectSingleNode("products_count").InnerText);
                XmlNodeList SubCategories = cat.SelectNodes("subcategories");

                if (catProdCount > 0) {
                    PDCategory newCat = new();
                    newCat.id = cat.SelectSingleNode("id").InnerText;
                    newCat.title = cat.SelectSingleNode("title").InnerText;
                    newCat.productCount = catProdCount;

                    catList.Add(newCat);
                } 
               
                if ( SubCategories.Count > 0) {
                    foreach (XmlNode subCat in SubCategories) {
                        int subCatProdCount = int.Parse(subCat.SelectSingleNode("products_count").InnerText);
                        if (subCatProdCount > 0) {
                            PDCategory newCat = new();
                            newCat.id = subCat.SelectSingleNode("id").InnerText;
                            newCat.title = subCat.SelectSingleNode("title").InnerText;
                            newCat.productCount = subCatProdCount;

                            catList.Add(newCat);
                        }
                    }
                }

            }
            return catList;
        }

        //lazy for building manufacturer list
        private static List<PDManufacturer> GetPDManufacturerList() {
            List<PDManufacturer> manufList = new();

            RESTClient restClient = new(_BaseUrl);
            string xmlManufStr = restClient.ExecGetParams(_ManufacturerPath, _APIParams);

            XmlDocument ManufXML = new();
            ManufXML.LoadXml(xmlManufStr);
            XmlNodeList ManufNodes = ManufXML.SelectNodes("//items/manufacturer");

            foreach (XmlNode manufNode in ManufNodes) {
                string manufJSON = JsonConvert.SerializeXmlNode(manufNode);
                manufJSON = manufJSON.Substring(16);
                manufJSON = manufJSON.Remove(manufJSON.Length - 1, 1);
                PDManufacturer PDmanuf = JsonConvert.DeserializeObject<PDManufacturer>(manufJSON);

                manufList.Add(PDmanuf);
            }
            return manufList;
        }

        //lazy for building product list
        private static List<FullProduct> BuildProductList() {
            List<PDCategory> CategoryList = _CategoriesXML;
            List<FullProduct> newProducts = new();

            foreach (PDCategory category in CategoryList) {
                RESTClient restClient = new(_BaseUrl);
                string productsPath = $"{_CategoriesPath}/{category.id}/products/extended";
                string xmlProductString = restClient.ExecGetParams(productsPath, _APIParams);

                XmlDocument ProductsXML = new();
                ProductsXML.LoadXml(xmlProductString);

                if (ProductsXML.SelectSingleNode("//response/success").InnerText == "404") continue;
                XmlNodeList ProductXmlList = ProductsXML.SelectNodes("//items/product");

                foreach (XmlNode productNode in ProductXmlList) {
                    string productJSON = JsonConvert.SerializeXmlNode(productNode);
                    productJSON = productJSON.Substring(11);
                    productJSON = productJSON.Remove(productJSON.Length - 1, 1);
                    PDProduct PDproduct = JsonConvert.DeserializeObject<PDProduct>(productJSON);

                    //getting description
                    XmlNode descNode = productNode.SelectSingleNode("description");
                    PDproduct.descriptionHTML = descNode.InnerText;

                    //getting vendor type
                    string vendorType = category.title;
                    if (vendorType.StartsWith("-")) {
                        vendorType = vendorType.Substring(1);
                    }
                    vendorType.Trim();
                    PDproduct.vandorType = vendorType;

                    //building product from PDProduct
                    FullProduct Product = BuildProduct(PDproduct);
                    newProducts.Add(Product);
                }
            }

            //removing products without images
            newProducts = newProducts.FindAll(x => x.Images.Count > 0);
            
            //remoing product duplicates
            List<FullProduct> noDuplicates = newProducts.GroupBy(x => x.SKU).Select(x => x.First()).ToList();
            return noDuplicates;
        }

        //for building product out of PDProduct
        private static FullProduct BuildProduct(PDProduct PDproduct) {
            FullProduct newProduct = new();

            newProduct.TitleLT = SQLUtil.SQLSafeString(PDproduct.title);
            newProduct.DescLT = SQLUtil.SQLSafeString(PDproduct.descriptionHTML);

            //getting vendor
            PDManufacturer ProdManuf = _ManufList.Find(x => x.id == PDproduct.manufacturer_id);
            newProduct.Vendor = SQLUtil.SQLSafeString(ProdManuf.title);

            newProduct.ProductTypeID = 1.ToString();
            newProduct.SKU = _SKUPrefix + PDproduct.artnum;
            newProduct.ProductTypeVendor = PDproduct.vandorType;

            //building Varinats
            ProductVariant newVariant = new ProductVariant();
            newVariant.Barcode = PDproduct.ean;
            newVariant.Price = PriceGenModule.GenNewPrice(PDproduct.price);
            newVariant.PriceVendor = PDproduct.price;

            //setting product variant stock
            if (PDproduct.stock.ContainsKey("type"))
            {
                if (PDproduct.stock["type"] == "morethan" || PDproduct.stock["type"] == "total")
                {
                    newVariant.VendorStock = int.Parse(PDproduct.stock["amount"]);
                }
                else
                {
                    newVariant.VendorStock = -1;
                }
            }
            else
            {
                newVariant.VendorStock = int.Parse(PDproduct.stock["amount"]);
            }

            //adding images
            dynamic imagesDynamic = PDproduct.pictures;
            imagesDynamic = imagesDynamic.big;
            if (imagesDynamic.Type == JTokenType.Array)
            {
                foreach (dynamic img in imagesDynamic)
                {
                    string imageURL = img.ToString();
                    newProduct.Images.Add(imageURL);
                }
            }
            else if (imagesDynamic.Type == JTokenType.String)
            {
                string imageURL = imagesDynamic.Value.ToString();
                newProduct.Images.Add(imageURL);
            }

            //adding product added timestamp
            newProduct.AddedTimeStamp = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds().ToString();

            //getting the dimensions (they dont give dims)
            newProduct.Weight = PDproduct.weight;
            newProduct.Height = 1;
            newProduct.Lenght = 1;
            newProduct.Width = 1;

            //i can add paramethers but PD doesnt give any
            newProduct.ProductVariants.Add(newVariant);
            return newProduct;
        }
    }
}
