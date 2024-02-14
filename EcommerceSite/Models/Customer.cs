using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Web;
namespace EcommerceSite.Models
{
    public class Customer
    {
        [RegularExpression("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$", ErrorMessage = "Invalid email format")]
        [Display(Name = "Email")]
        public string CustomerMail { get; set; }

        [Required(ErrorMessage = "Required Field")]
        [Display(Name = "Full Name")]
        public string CustomerName { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Required Field")]
        [Display(Name = "Password")]
        public string CustomerPasswd { get; set; }

        [Compare("CustomerPasswd", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string CustomerCfmPwd { get; set; }

        [Required(ErrorMessage = "Required Field To Delivery")]
        [Display(Name = "Delivery Address")]
        public string CustomerAddress { get; set; }
    }
}
