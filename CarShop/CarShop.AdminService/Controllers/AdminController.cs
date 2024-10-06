using CarShop.AdminService.Repositories;
using CarShop.ServiceDefaults.ServiceInterfaces.AdminService;
using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CarShop.AdminService.Controllers
{
    [Route("api/admin")]
    public class AdminController(AdminsRepository _adminsRepository) : ControllerBase
    {

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateAccountAsync(
            [FromBody] CreateAccountRequest createAccountRequest)
        {
            if (!Validator.TryValidateObject(createAccountRequest, new(createAccountRequest), null, true))
            {
                return BadRequest();
            }

            Admin? admin = await _adminsRepository.GetByEmailAsync(createAccountRequest.Email);
            if (admin is not null)
            {
                return Conflict();
            }

            await _adminsRepository.CreateAccount(
                createAccountRequest.Email,
                Argon2.Hash(createAccountRequest.Password));

            return Ok();
        }
    }
}
