using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarShop.ServiceDefaults.ServiceInterfaces.AdminService
{
    public class CreateAccountRequest
    {
        [Required]
        [EmailAddress]
        [ModelBinder(Name = "email")]
        public string Email { get; set; }

        [Required]
        [ModelBinder(Name = "password")]
        public string Password { get; set; }
        
        [Range(1, 2000000000)]
        public int Priority { get; set; }
    }
}
