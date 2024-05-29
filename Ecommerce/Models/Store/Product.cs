using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security;
using System.Web;

namespace Ecommerce.Models.Store
{
    public class Product
    {
        [Key]
        public int PROD_ID { get; set; }
        public string PROD_MAKE { get; set; }
        public string PROD_MODEL { get; set; }
        public int PROD_WARRANTY { get; set; }
        public string PROD_DESC { get; set; }
        public string PROD_IMG { get; set; }


        public int D_ID { get; set; }
        public int A_ID { get; set; }
    }
}