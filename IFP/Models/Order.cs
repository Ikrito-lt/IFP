using IFP.Utils;
using System;
using System.Collections.Generic;

namespace IFP.Models
{
    public class Order {
        //DB_ID
        public string DBID { set; get; } = "Order DBID Not Set";
        public string contact_email { set; get; } = "Order Email Not Set";
        public bool confirmed { set; get; }
        public string confirmedString {
            get {
                if (confirmed) {
                    return "True";
                } else {
                    return "False";
                }
            }
        }

        //date stuff
        public string created_at { set; get; } = "Order Creation Timestamp Not Set";
        public string Created_date {
            get {
                if (!string.IsNullOrEmpty(created_at)) {
                    DateTime createdDate = created_at.UnixTimeToDateTime();
                    return createdDate.ToString("MM/dd/yyyy HH:mm");
                } else {
                    return "---";
                }
            }
        }
      
        public string cancelled_at { set; get; } = "Order Cancelation Timestamp Not Set";
        public string Canceled_date {
            get {
                if (!string.IsNullOrEmpty(cancelled_at)) {
                    DateTime CanceledDate = cancelled_at.UnixTimeToDateTime();
                    return CanceledDate.ToString("MM/dd/yyyy HH:mm");
                } else {
                    return "Not Canceled";
                }
            }
        }

        public string closed_at { set; get; } = "Order Closed Timestamp Not Set";
        public string Closed_date {
            get {
                if (!string.IsNullOrEmpty(closed_at)) {
                    DateTime ClosedDate = closed_at.UnixTimeToDateTime();
                    return ClosedDate.ToString("MM/dd/yyyy HH:mm");
                } else {
                    return "Not Closed";
                }
            }
        }


        //money stuff
        public string currency { set; get; } = "EUR";
        public double current_subtotal_price { set; get; }
        public string subtotal_price_formated => currency == "EUR" ? $"€ {current_subtotal_price:0.00}" : $"{current_subtotal_price:0.00}";

        public double current_total_discounts { set; get; }
        public string discounts_formated => currency == "EUR" ? $"€ {current_total_discounts:0.00}" : $"{current_total_discounts:0.00}";
        
        public double current_total_price { set; get; }
        public string Full_price_formated => currency == "EUR" ? $"€ {current_total_price:0.00}" : $"{current_total_price:0.00}";
        
        public double current_total_tax { set; get; }
        public string tax_formated => currency == "EUR" ? $"€ {current_total_tax:0.00}" : $"{current_total_tax:0.00}";
        public string financial_status { set; get; } = "Order Finantial Status Not Set";
        public string Item_count => line_items.Count == 1 ? line_items.Count.ToString() + " Item" : line_items.Count.ToString() + " Items";


        public string name { set; get; } = "Order Name Not Set";
        public string note { set; get; } = "Order Note Not Set";
        public string phone { set; get; } = "Order Phone Not Set";
        public Customer customer { set; get; } = new Customer();
        public List<OrderProduct> line_items { set; get; } = new List<OrderProduct>();
        public Address billing_address { set; get; } = new Address();
        public Address shipping_address { set; get; } = new Address();
    }

    public class Address {
        //DB_ID
        public string DBID { set; get; } = "Address DBID Not Set";
        public string first_name { set; get; } = "Address First Name Not Set";
        public string last_name { set; get; } = "Address Last Name Not Set";
        public string full_name {
            get {
                return first_name + " " + last_name;
            }
        }

        public string company { set; get; }

        public string address1 { set; get; } = "Address Not Set";
        public string address2 { set; get; } 

        public string city { set; get; } = "Address City Not Set";
        public string province { set; get; } = "Address Province Not Set";
        public string country { set; get; } = "Address Country Not Set";
        public string cpc => $"{city}, {province}, {country}";

        public string zip { set; get; } = "Address Zip Code Not Set";
        public string country_code { set; get; }
        public string province_code { set; get; }

        public string name { set; get; } = "Address Name Not Set";
        public string phone { set; get; } = "Address Phone Not Set";

        public string latitude { set; get; }
        public string longditude { set; get; }
    }
}
