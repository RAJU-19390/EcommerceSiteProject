using EcommerceSite.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Mvc;
namespace EcommerceSite.Controllers
{
    public class EcommerceController : Controller
    {
        private readonly string productsfile = ConfigurationManager.AppSettings["Products_File"];
        private static string ProductsFile;
        private List<Product> productlist;

        public EcommerceController()
        {
            ProductsFile = System.Web.Hosting.HostingEnvironment.MapPath(productsfile);
            productlist = ReadFromFile();
        }

        public ActionResult Index()
        {
            return RedirectToAction("CustomerLogin", "Customers");
        }

        public ActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddProduct(Product newproduct)
        {
            if (ModelState.IsValid)
            {
                if (IsProductExists(newproduct.ProductId))
                {
                    ViewBag.ErrorMessage = "Product with the same Id already exists.";
                    return View(newproduct);
                }

                productlist.Add(newproduct);
                SaveProductsToFile();

                return RedirectToAction("GetProducts");
            }
            return RedirectToAction("AddProduct");
        }

        private bool IsProductExists(string productId)
        {
            return productlist.Any(p => p.ProductId == productId);
        }

        private List<Product> ReadFromFile()
        {
            List<Product> products = new List<Product>();

            try
            {
                if (System.IO.File.Exists(ProductsFile))
                {
                    using (StreamReader reader = new StreamReader(ProductsFile))
                    {
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            string[] data = line.Split(',');

                            Product product = new Product
                            {
                                ProductId = data[0],
                                ProductName = data[1],
                                ProductPrice = double.Parse(data[2]),
                                ProductQty = int.Parse(data[3])
                            };

                            products.Add(product);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading from file: {ex.Message}");
            }

            return products;
        }

        private void SaveProductsToFile()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(ProductsFile))
                {
                    foreach (var product in productlist)
                    {
                        writer.WriteLine($"{product.ProductId},{product.ProductName},{product.ProductPrice},{product.ProductQty}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving products to file: {ex.Message}");
            }
        }

        public ActionResult GetProducts(string custaddress)
        {
            List<Product> productlist = new List<Product>();
            TempData["Cust_addr"] = custaddress;
            try
            {
                if (!System.IO.File.Exists(ProductsFile))
                {
                    return Content("File not found");
                }

                productlist = ReadFromFile();
                var productsinorder = productlist.OrderBy(p => p.ProductId).ToList(); 
                return View(productsinorder);
            }
            catch (Exception ex)
            {
                return Content($"Error reading from file: {ex.Message}");
            }
        }

        public ActionResult AddToCart(string pid)
        {
            TempData["td_pid"] = pid;
            TempData["Customer_Address"] = TempData["Cust_addr"];
            TempData.Keep();
            ViewBag.id = pid;
            return View();
        }

        [HttpPost]
        public ActionResult AddToCart(int productqty)
        {
            string selectedid = TempData["td_pid"].ToString();
            string address = TempData["Customer_Address"].ToString();
            List<Product> productlist = new List<Product>();
            productlist = ReadFromFile();
            Product productid = productlist.Find(p => p.ProductId == selectedid);

            try
            {
                if (productid != null)
                {
                    double amt = productqty * productid.ProductPrice;
                    TempData["Total_Price"] = amt;
                    TempData.Keep();
                    return RedirectToAction("PaymentMode", "Customers", new { custaddress = address });
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                return Content($"Error processing request: {ex.Message}");
            }
        }

        public ActionResult UpdateProducts()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UpdateProducts(Product product)
        {
            List<Product> products = ReadFromFile();
            Product updatedata = products.Find(p => p.ProductId == product.ProductId);
            if (products != null)
            {
                updatedata.ProductId = product.ProductId;
                updatedata.ProductName = product.ProductName;
                updatedata.ProductPrice = product.ProductPrice;
                updatedata.ProductQty = product.ProductQty;
                WriteToFile(products);
            }
            return RedirectToAction("GetProducts");
        }

        public void WriteToFile(List<Product> productdata)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(ProductsFile))
                {
                    foreach (var product in productdata)
                    {
                        writer.WriteLine($"{product.ProductId},{product.ProductName},{product.ProductPrice},{product.ProductQty}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to file: {ex.Message}");
            }
        }
    }
}
