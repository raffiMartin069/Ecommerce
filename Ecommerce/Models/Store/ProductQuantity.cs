using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ecommerce.Models.Store
{
    public class ProductQuantity
    {
        [Key]
        public int PQ_ID { get; set; }
        public int PQ_QTY { get; set; }

        [ForeignKey("Product")]
        public int PROD_ID { get; set; }
    }
}