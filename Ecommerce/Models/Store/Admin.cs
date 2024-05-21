using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ecommerce.Models.Store
{
    public class Admin
    {
        [Key]
        public string A_ID { get; set; }
        public string A_FNAME { get; set;}
        public string A_LNAME { get; set;}
        public string A_MNAME { get; set;}
        public string A_PHONE { get; set;}
    }
}