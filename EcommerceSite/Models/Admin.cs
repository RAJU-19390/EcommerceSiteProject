using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace EcommerceSite.Models
{
    public class Admin
    {
        [Key]
        [RegularExpression("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$")]
        [Display(Name = "Enter Mail")]
        public string AdminMail { get; set; }

        [Required(ErrorMessage = "Required Field")]
        [Display(Name = "Enter Full Name")]
        public string AdminName { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Enter Password")]
        public string AdminPasswd { get; set; }

        [DataType(DataType.Password)]
        [Compare("AdminPasswd", ErrorMessage = "Passwords do not match")]
        [Display(Name = "Confirm Password")]
        public string AdminCfmPwd { get; set; }
    }
}