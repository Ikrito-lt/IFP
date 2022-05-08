using System.Collections.Generic;

namespace IFP.Models
{
    /// <summary>
    /// This static class describes all possible product statuses
    /// </summary>
    static class ProductStatus {
        // IMPORTANT: When adding new statuses update ProductModule.ChangeProductStatus method

        public static string New = "New";
        public static string Ok = "Ok";
        public static string OutOfStock = "Out Of Stock";
        public static string Disabled = "Disabled";
        public static string Archived = "Archived";

        // Method for getting list of values for all declared fields
        public static List<string> GetFields() {
            List<string> fieldValues = new();
            foreach (var fieldInfo in typeof(ProductStatus).GetFields()) {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                string fieldVal = fieldInfo.GetValue(null).ToString() ?? string.Empty;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                fieldValues.Add(fieldVal);
            }
            return fieldValues;
        }
    }
}
