using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ecommerce.Models.Cart
{
    public class Cart
    {
        public int CART_ID { get; set; }
        public DateTime CART_DATE_CREATED { get; set; }
        public int USER_ID { get; set; }
    }
}