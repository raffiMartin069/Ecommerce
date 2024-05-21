using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ecommerce.Models
{
    public class Address
    {
        [Key]
        public string AD_ID { get; set; }
        public string AD_STREET { get; set; }
        public string AD_BRGY { get; set; }
        public string AD_PROVINCE { get; set; }
        public string AD_CITY { get; set; }
        public string AD_ZIPCODE { get; set;}
        public string A_ID { get; set;}
    }
}