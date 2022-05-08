namespace IFP.Models {
    /// <summary>
    /// This class is used to filter products by status in productPage datagrid
    /// </summary>
    internal class CheckBoxListItem
    {
        public string Name { get; set; }
        public bool IsSelected { get; set; }
        public CheckBoxListItem(string Name) {
            this.Name = Name;
            this.IsSelected = true;
        }
    }
}
