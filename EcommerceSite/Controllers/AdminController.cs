using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.IO;
using EcommerceSite.Models;
namespace EcommerceSite.Controllers
{
    public class AdminController : Controller
    {
        public string adminfile = ConfigurationManager.AppSettings["Admin_File"];
        public static string AdminFile;
        List<Admin> adminlist;
        public AdminController()
        {
            AdminFile = System.Web.Hosting.HostingEnvironment.MapPath(adminfile);
            adminlist = new List<Admin>();
        }
        public ActionResult AdminRegister()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AdminRegister(Admin admin)
        {
            if (ModelState.IsValid)
            {
                if (IsAdminExists(admin.AdminMail))
                {
                    ViewBag.ErrorMessage = "This email is already registered. Please use a different email.";
                    return View(admin);
                }

                SaveAdminToFile(admin);
                return RedirectToAction("AddProduct", "Ecommerce");
            }

            return View(admin);
        }

        private void SaveAdminToFile(Admin admin)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(AdminFile, true))
                {
                    writer.WriteLine($"{admin.AdminName},{admin.AdminMail},{admin.AdminPasswd}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving Admin to file: {ex.Message}");
            }
        }

        private bool IsAdminExists(string email)
        {
            List<Admin> existingadmin = ReadAdminFromFile();
            return existingadmin.Any(c => c.AdminMail == email);
        }

        private List<Admin> ReadAdminFromFile()
        {
            List<Admin> admin = new List<Admin>();

            try
            {
                if (!System.IO.File.Exists(AdminFile))
                {
                    return admin;
                }

                using (StreamReader reader = new StreamReader(AdminFile))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] data = line.Split(',');

                        Admin adminlist = new Admin
                        {
                            AdminName = data[0],
                            AdminMail = data[1],
                            AdminPasswd = data[2],
                        };

                        admin.Add(adminlist);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading customers from file: {ex.Message}");
            }

            return admin;
        }

        public ActionResult AdminLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AdminLogin(Admin loginModel)
        {
            if (IsAdminExists(loginModel.AdminMail))
            {
                if (IsValidCredentials(loginModel.AdminMail, loginModel.AdminPasswd))
                {
                  Admin admin= GetAdminByEmail(loginModel.AdminMail);

                    return RedirectToAction("AddProduct", "Ecommerce");
                }
                ModelState.AddModelError("Password", "Invalid password");
            }
            else
            {
                ModelState.AddModelError("Email", "Admin not found");
            }
            return View(loginModel);
        }

        private bool IsValidCredentials(string email, string password)
        {
          Admin admin = GetAdminByEmail(email);
            return admin != null && admin.AdminPasswd == password;
        }

        private Admin GetAdminByEmail(string email)
        {
            List<Admin> existingadmin = ReadAdminFromFile();
            return existingadmin.Find(c => c.AdminMail == email);
        }


    }
}