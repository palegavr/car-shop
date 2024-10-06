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
            if (admin is not null) // Пользователь с такой почтой уже есть
            {
                return Conflict();
            }

            await _adminsRepository.CreateAccountAsync(
                createAccountRequest.Email,
                Argon2.Hash(createAccountRequest.Password));

            return Ok();
        }

        [HttpDelete]
        [Route("{id:long}")]
        public async Task<IActionResult> DeleteAccountAsync(long id)
        {
            if ((await _adminsRepository.GetByIdAsync(id)) is null)
            {
                return NotFound();
            }

            await _adminsRepository.DeleteAccountAsync(id);
            return Ok();
        }
    }
}
