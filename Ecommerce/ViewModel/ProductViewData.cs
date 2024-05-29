using Ecommerce.Models.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ecommerce.ViewModel
{
    public class ProductViewData
    {
        public Product Products { get; set; }
        public ProductPrice ProductPrices { get; set; }
        public ProductQuantity ProductQty {  get; set; } 
        public bool IsSearch {  get; set; }
    }
}