using Ecommerce.Models;
using Ecommerce.Repository.User;
using Ecommerce.ViewModel;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ecommerce.Controllers
{
    public class LogInController : Controller
    {
        private bool LogInValidation(string[] credentials)
        {
            try
            {
                Credential cred = new Credential
                {
                    C_EMAIL = credentials[0],
                    C_PASS = credentials[1],
                };

                if(cred == null)
                {
                    return false;
                }

                UserRepository repository = new UserRepository();
                bool isMatched = repository.UserLogin(cred);

                return isMatched;
            } catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception caught while validating user credentials: " + e);
                return false;
            }
        }

        [HttpPost]
        public ActionResult LogIn(FormCollection data)
        {
            string email = data["cus_email"];
            string password = data["cus_pass"];
            string[] credentials = { email, password };

            if (string.IsNullOrEmpty(email))
            {
                TempData["email"] = "Email can not be empty";
                return RedirectToAction("Index");
            }

            if (string.IsNullOrEmpty(password))
            {
                TempData["password"] = "Email can not be empty";
                return RedirectToAction("Index");
            }

            bool isValidated = LogInValidation(credentials);

            if (!isValidated)
            {
                return Json(isValidated);
            }

            return Json(new
            {
                status = isValidated,
                redirectUrl = Url.Action("Index", "Home")
            });
        }

        private bool RegisterUser(string[] formData)
        {
            bool isSuccess = true;
            try
            {
                UserViewData userData = new UserViewData
                {
                    UserModel = new Models.User.UserModel
                    {
                        USER_FNAME = formData[0],
                        USER_LNAME = formData[1],
                        USER_PHONE = formData[3],
                    },
                    Credential = new Models.Credential
                    {
                        C_EMAIL = formData[2],
                        C_PASS = formData[9],
                    },
                    Address = new Models.Address
                    {
                        AD_STREET= formData[4],
                        AD_BRGY= formData[5],
                        AD_CITY = formData[6],
                        AD_PROVINCE = formData[7],
                        AD_ZIPCODE = formData[8],
                    }
                };

                UserRepository userRepository = new UserRepository(userData);
                bool[] result = userRepository.UserRegistrationInsert();

                if (!result[0])
                {
                    isSuccess = false;
                    return isSuccess;
                }

                if (!result[1])
                {
                    isSuccess = false;
                    return isSuccess;
                }

                if (!result[2])
                {
                    isSuccess = false;
                    return isSuccess;
                }

                return isSuccess;

            } catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                isSuccess = false;
                return isSuccess;
            }
        }
        
        public ActionResult Register(FormCollection data)
        {
            string firstName = data["firstname"];
            string lastName = data["lastname"];
            string email = data["email"];
            string phone = data["phone"];
            string street = data["street"];
            string barangay = data["reg_brgy"];
            string city = data["city"];
            string province = data["province"];
            string zipCode = data["zip code"];
            string password = data["password"];

            string serverResponse = "Successfully register information";

            // Store everything to an array
            string[] formData = new string[] { firstName, lastName, email, phone, street, barangay, city, province, zipCode, password };

            bool response = RegisterUser(formData);

            if(!response)
            {
                serverResponse = "Failed to register information";
                return Json(serverResponse);
            }

            return Json(serverResponse);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SignUp()
        {
            return View();
        }
    }
}