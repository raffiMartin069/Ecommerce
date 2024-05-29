using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ecommerce.Models.Store
{
    public class ProductLog
    {
        [Key]
        public int PL_ID { get; set; }
        public string PL_DATE { get; set; }
        public string PL_TIME { get; set; }
        
        [ForeignKey("Product")]
        public int PROD_ID { get; set; }
    }
}