using Ecommerce.Models;
using Ecommerce.Models.Store;
using Ecommerce.Repository.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;


namespace Ecommerce.Controllers
{
    public class StoreController : Controller
    {
        // GET: Store
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Admin()
        {
            return View();
        }

        public ActionResult ProductEntry()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        private bool AdminRegistration(int adminId, int addressId, int credId)
        {
            bool flag = true;
            if (adminId == 0 && addressId == 0 && credId == 0)
            {
                flag = false;
            }
            return flag;
        }

        [HttpPost]
        [Route("/api")]
        public ActionResult Register(FormCollection data)
        {
            AdminRepository adminRepo = new AdminRepository();
            Admin admin = new Admin
            {
                A_FNAME = data["firstname"],
                A_LNAME = data["lastname"],
                A_MNAME = data["middlename"],
                A_PHONE = data["phone"]
            };

            int adminId = 0;

            try
            {
                adminId = adminRepo.AdminDetails(admin);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception catched: " + e);
            }

            Address address = new Address
            {
                AD_STREET = data["street"],
                AD_BRGY = data["baranggay"],
                AD_PROVINCE = data["province"],
                AD_CITY = data["city"],
                AD_ZIPCODE = data["zipcode"],
                A_ID = adminId.ToString()
            };

            // for future references
            int addressId = 0;
            try
            {
                addressId = adminRepo.AdminAddress(address);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception catched: " + e);
            }

            Credential credential = new Credential
            {
                C_EMAIL = data["email"],
                C_PASS = data["password"],
                A_ID = adminId.ToString()
            };

            int credId = 0;
            try
            {
                credId = adminRepo.AdminCredential(credential);
            }
            catch (Exception e)
            {

                System.Diagnostics.Debug.WriteLine("Exception catched: " + e);
            }

            bool registrationResult = AdminRegistration(adminId, addressId, credId);

            return Json(registrationResult);
        }

        [HttpPost]
        public ActionResult EntryApi(FormCollection data, HttpPostedFile prodimg)
        {
            return View();
        }
    }
}