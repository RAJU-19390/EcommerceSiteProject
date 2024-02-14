using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace EcommerceSite.Models
{
    public class Product
    {
        [Key]
        [Required(ErrorMessage = "Product Id Required")]
        [Display(Name = "Enter Id")]
        public string ProductId { get; set; }
        [Required(ErrorMessage ="Product Name Required")]
        [Display(Name = "Enter Name")]
        public string ProductName { get; set; }
        [Required(ErrorMessage = "Product Price Required")]
        [Display(Name = "Enter Price")]
        public double ProductPrice { get; set; }
        [Display(Name = "Enter Available Stock")]
        public int ProductQty { get; set; }
    }
}