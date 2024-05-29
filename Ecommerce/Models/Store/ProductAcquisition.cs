using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ecommerce.Models.Store
{
    public class ProductAcquisition
    {
        [Key]
        public int PA_ID { get; set; }
        public string PA_DATE { get; set; }

        [ForeignKey("Product")]
        public int PROD_ID { get; set; }
    }
}