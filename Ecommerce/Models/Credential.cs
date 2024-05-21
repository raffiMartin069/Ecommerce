using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ecommerce.Models
{
    public class Credential
    {
        [Key]
        public string C_ID { get; set; }
        public string C_EMAIL { get; set; }
        public string C_PASS { get; set; }
        public string A_ID { get; set; }
    }
}