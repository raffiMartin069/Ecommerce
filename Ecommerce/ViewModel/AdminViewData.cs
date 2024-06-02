using Ecommerce.Models.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ecommerce.ViewModel
{
    public class AdminViewData
    {
        public ProductViewData productViewData {  get; set; }
        public Distributor distributor { get; set; }
    }
}