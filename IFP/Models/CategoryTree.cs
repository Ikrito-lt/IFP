using System.Collections.ObjectModel;

namespace IFP.Models
{
    /// <summary>
    /// This class is used to recursively represent the category tree 
    /// </summary>
    internal class CategoryTree
    {
        public string CatName { get; set; } = "Category Name Not Set";
        public int? CatID { get; set; }
        public ObservableCollection<CategoryTree> children { get; set; }
        public CategoryTree()
        {
            children = new ObservableCollection<CategoryTree>();
        }
    }
}
