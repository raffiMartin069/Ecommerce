using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ecommerce.Models.Store
{
    public class Distributor
    {
        [Key]
        public int D_ID { get; set; }
        public string D_NAME { get; set; }
        public string D_DATE_ENTERED { get; set; }
        public string D_TIME_ENTERED { get; set; }
    }
}