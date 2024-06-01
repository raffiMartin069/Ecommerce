using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ecommerce.Models.Cart
{
    public class CartItem
    {
        public int CI_ID { get; set; }
        public int CI_QTY { get; set; }
        public int CART_ID { get; set; }
        public int PROD_ID { get; set; }
    }
}