using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ecommerce.Models.Store
{
    public class ProductPrice
    {
        [Key]
        public int PP_ID { get; set; }
        public string PP_PRICE { get; set; }

        [ForeignKey("Product")]
        public int PROD_ID { get; set; }
    }
}