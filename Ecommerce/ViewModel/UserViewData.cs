using Ecommerce.Models;
using Ecommerce.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ecommerce.ViewModel
{
    public class UserViewData
    {
        public UserModel UserModel { get; set; }
        public Address Address { get; set; }
        public Credential Credential { get; set; }
    }
}