using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using EcommerceSite.Models;

namespace EcommerceSite.Controllers
{
    public class CustomersController : Controller
    {
        public string customersfile = ConfigurationManager.AppSettings["Customers_File"];
        public static string CustomersFile;
        List<Customer> customerlist;

        public CustomersController()
        {
            CustomersFile = System.Web.Hosting.HostingEnvironment.MapPath(customersfile);
            customerlist = new List<Customer>();
        }

        public ActionResult CustomerRegister()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CustomerRegister(Customer cust)
        {
            if (ModelState.IsValid)
            {
                if (IsCustomerExists(cust.CustomerMail))
                {
                    ViewBag.ErrorMessage = "This email is already registered. Please use a different email.";
                    return View(cust);
                }

                SaveCustomerToFile(cust);
                return RedirectToAction("GetProducts", "Ecommerce",new { custaddress = cust.CustomerAddress });
            }

            return View(cust);
        }

        private void SaveCustomerToFile(Customer customer)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(CustomersFile, true))
                {
                    writer.WriteLine($"{customer.CustomerName},{customer.CustomerMail},{customer.CustomerPasswd},{customer.CustomerAddress}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving customer to file: {ex.Message}");
            }
        }

        private bool IsCustomerExists(string email)
        {
            List<Customer> existingCustomers = ReadCustomersFromFile();
            return existingCustomers.Any(c => c.CustomerMail == email);
        }

        private List<Customer> ReadCustomersFromFile()
        {
            List<Customer> customers = new List<Customer>();

            try
            {
                if (!System.IO.File.Exists(CustomersFile))
                {
                    return customers;
                }

                using (StreamReader reader = new StreamReader(CustomersFile))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] data = line.Split(',');

                        Customer customer = new Customer
                        {
                            CustomerName = data[0],
                            CustomerMail = data[1],
                            CustomerPasswd = data[2],
                            CustomerAddress = data[3]
                        };

                        customers.Add(customer);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading customers from file: {ex.Message}");
            }

            return customers;
        }

        public ActionResult CustomerLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CustomerLogin(Customer loginModel)
        {
                if (IsCustomerExists(loginModel.CustomerMail))
                {
                    if (IsValidCredentials(loginModel.CustomerMail, loginModel.CustomerPasswd))
                    {
                        Customer customer = GetCustomerByEmail(loginModel.CustomerMail);
                        
                        return RedirectToAction("GetProducts", "Ecommerce",new { custaddress=customer.CustomerAddress});
                    }
                    ModelState.AddModelError("Password", "Invalid password");
                }
                else
                {
                    ModelState.AddModelError("Email", "Customer not found");
                }
            return View(loginModel);
        }

        private bool IsValidCredentials(string email, string password)
        {
            Customer customer = GetCustomerByEmail(email);
            return customer != null && customer.CustomerPasswd == password;
        }

        private Customer GetCustomerByEmail(string email)
        {
            List<Customer> existingCustomers = ReadCustomersFromFile();
            return existingCustomers.Find(c => c.CustomerMail == email);
        }

        public ActionResult PaymentMode(string custaddress)
        {
            ViewBag.TotalAmount = TempData["Total_Price"];
            ViewBag.Deliver = custaddress;
            return View();
        }
        public ActionResult OrderSuccess()
        {
            return View();
        }
        public List<Customer> ReadFromFile()
        {
            List<Customer> customerlist = new List<Customer>();

            try
            {
                using (StreamReader reader = new StreamReader(CustomersFile))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] words = line.Split(',');

                        string mail = words[0];
                        string name = words[1];
                        string passwd = words[2];
                        string address= words[3];

                        Customer customerdata = new Customer { CustomerMail = mail, CustomerName = name, CustomerPasswd=passwd, CustomerAddress = address };
                        customerlist.Add(customerdata);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading from file: {ex.Message}");
            }

            return customerlist;
        }
        public ActionResult GetCustomerData()
        {
            List<Customer> customers = new List<Customer>();
            try
            {
                if (!System.IO.File.Exists(CustomersFile))
                {
                    return Content("File not found");
                }

                customers = ReadFromFile();
                return View(customers);
            }
            catch (Exception ex)
            {
                return Content($"Error reading from file: {ex.Message}");
            }
        }
    }
}
