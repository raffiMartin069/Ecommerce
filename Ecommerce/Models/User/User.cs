using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;

namespace Ecommerce.Models.User
{
    public class UserModel
    {
        [Key]
        public int USER_ID { get; set; }
        
        public string USER_FNAME {  get; set; }
        
        public string USER_LNAME {  get; set; }

        public string USER_PHONE {  get; set; }
        
    }
}