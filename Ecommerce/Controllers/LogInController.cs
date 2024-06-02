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

        public ActionResult LogOut(string logout)
        {
            string action = logout;

            if (string.IsNullOrEmpty(action))
            {
                return RedirectToAction("Home", "Index");
            }

            // Abandon the session
            Session.Abandon();

            // Expire the session cookie
            if (Response.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddYears(-1);
            }

            // Redirect the user to the login page (or wherever you want them to go after logging out)
            return RedirectToAction("Index", "LogIn");
        }

        public ActionResult AdminLogOut(string logout)
        {
            string action = logout;

            if (string.IsNullOrEmpty(action))
            {
                return RedirectToAction("Home", "Index");
            }

            // Abandon the session
            Session.Abandon();

            // Expire the session cookie
            if (Response.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddYears(-1);
            }

            // Redirect the user to the login page (or wherever you want them to go after logging out)
            return RedirectToAction("Index", "Store");
        }

        private object[] LogInValidation(string[] credentials)
        {
            object[] response = new object[6];

            try
            {
                Credential cred = new Credential
                {
                    C_EMAIL = credentials[0],
                    C_PASS = credentials[1],
                };

                if (cred == null)
                {
                    response.Append(false);
                    return response;
                }

                UserRepository repository = new UserRepository();
                object[] loginResponse = repository.UserLogin(cred);

                response[0] = loginResponse[0];
                response[1] = loginResponse[1];
                response[2] = loginResponse[2];
                response[3] = loginResponse[3];
                response[4] = loginResponse[4];
                response[5] = loginResponse[5];

                if (response[0]  == null || response[1] == null || response[2] == null || response[3] == null || response[4] == null || response[5] == null)
                {
                    response[0] = false;
                    return response;
                }

                return response;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception caught while validating user credentials: " + e);
                response[0] = false;
                return response;
            }
        }

        private bool InputValidation(string[] arr)
        {
            bool isNull = false;
            for (int i = 0; i < arr.Length; i++)
            {
                if (string.IsNullOrEmpty(arr[i]))
                {
                    isNull = true;
                    break;
                }
            }
            return isNull;
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

            object[] loginDetails = LogInValidation(credentials);

            try
            {
                bool isValidated = (bool)loginDetails[0];

                if (!isValidated)
                {
                    return Json(new
                    {
                        response = false,
                        content = "User not found."
                    });
                }

                string userEmail = (string)loginDetails[1];
                string userPass = (string)loginDetails[2];
                string userId = (string)loginDetails[3];
                string userFname = (string)loginDetails[4];
                string userLname = (string)loginDetails[5];

                Session["userEmail"] = userEmail;
                Session["userPass"] = userPass;
                Session["userId"] = userId;
                Session["userFname"] = userFname;
                Session["userLname"] = userLname;
                Session["Role"] = "Customer";

                return Json(new
                {
                    response = true,
                    redirectUrl = Url.Action("Index", "Home")
                });
            } catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception caught in log in: " + e);
                return Json(false);
            }

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
            string zipCode = data["zipcode"];
            string password = data["password"];

            // Store everything to an array
            string[] formData = { firstName, lastName, email, phone, street, barangay, city, province, zipCode, password };

            bool isFilled = InputValidation(formData);

            if(isFilled)
            {
                return Json(new
                {
                    response = false,
                    content = "Please check and fill up the fields."
                });
            }

            bool response = RegisterUser(formData);

            if(!response)
            {
                return Json(new
                {
                    response = false,
                    content = "Failed to register information."
                });
            }

            return Json(new
            {
                response = true,
                content = "Registered Successfuly!."
            });
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