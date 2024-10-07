using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarShop.ServiceDefaults.ServiceInterfaces.AdminService
{
    public class UpdateTokensRequest
    {
        [Required]
        [ModelBinder(Name = "refresh_token")]
        public string RefreshToken { get; set; }
    }
}
