using System.Collections.Generic;

namespace Nop.Plugin.Soft2Print.ViewModel
{
    public class ProductList
    {
        public class ProductItem
        {
            public string Sku { get; set; }
            public string Name { get; set; }
            public bool IsCreatedButNotPublished { get; set; }
        }

        public ProductList()
        {

        }

        public IList<ProductItem> Products { get; set; }
    }
}
