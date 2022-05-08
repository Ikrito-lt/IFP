using System;

namespace IFP.Models
{
    public class Customer {
        //DB_ID
        public string DBID { set; get; } = "Customer DBID Not Set";
        public string email { set; get; } = "Customer Email Not Set";
        public string phone { set; get; } = "Customer Phone Not Set";
        public string note { set; get; } = "Customer Note Not Set";
        public string last_order_id { set; get; } = "Customer Last Order ID Not Set";
        public string state { set; get; } = "Customer State Not Set";
        public string first_name { set; get; } = "Customer First Name Not Set";
        public string last_name { set; get; } = "Customer Last Name Not Set";
        public string full_name {
            get {
                return first_name + " " + last_name;
            }
        }

        public bool verified_email { set; get; }
        public int orders_count { set; get; }


        public double total_spent { set; get; }
        public string spent_formated => $"€ {total_spent:0.00}";


        public string created_at { set; get; } = "Customer Creation Timestamp Not Set";
        public string Created_date {
            get {
                if (created_at != null) {
                    DateTime createdDate = DateTime.Parse(created_at);
                    return createdDate.ToString("MM/dd/yyyy HH:mm");
                } else {
                    return "---";
                }
            }
        }


        public string updated_at { set; get; } = "Customer Update Timestamp Not Set";
        public string Updated_date {
            get {
                if (updated_at != null) {
                    DateTime updatedDate = DateTime.Parse(updated_at);
                    return updatedDate.ToString("MM/dd/yyyy HH:mm");
                } else {
                    return "---";
                }
            }
        }
    }
}
