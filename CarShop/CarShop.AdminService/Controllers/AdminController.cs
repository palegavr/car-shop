using CarShop.AdminService.Repositories;
using CarShop.ServiceDefaults.ServiceInterfaces.AdminService;
using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CarShop.AdminService.Controllers
{
    [Route("api/admin")]
    public class AdminController(
        AdminsRepository _adminsRepository,
        RefreshSessionsRepository _refreshSessionsRepository) : ControllerBase
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

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest loginRequest)
        {
            if (!Validator.TryValidateObject(loginRequest, new(loginRequest), null, true))
            {
                return BadRequest();
            }

            Admin? admin = await _adminsRepository.GetByEmailAsync(loginRequest.Email);
            if (admin is null || 
                admin.Banned ||
                !Argon2.Verify(admin.Password, loginRequest.Password))
            {
                return Unauthorized();
            }

            var refreshTokenExpires = DateTime.UtcNow.Add(AuthOptions.REFRESH_TOKEN_LIFETIME);
            JwtSecurityToken refreshJwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                expires: refreshTokenExpires,
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            string refreshToken = new JwtSecurityTokenHandler().WriteToken(refreshJwt);

            JwtSecurityToken accessJwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: [new ("email", admin.Email)],
                expires: DateTime.UtcNow.Add(AuthOptions.ACCESS_TOKEN_LIFETIME),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            string accessToken = new JwtSecurityTokenHandler().WriteToken(accessJwt);

            await _refreshSessionsRepository.CreateSessionAsync(new RefreshSession
            {
                AdminId = admin.Id,
                RefreshToken = refreshToken,
                CreatedAt = DateTime.UtcNow,
                ExpiresIn = refreshTokenExpires,
            });

            return Ok(new
            {
                refresh_token = refreshToken,
                access_token = accessToken,
            });
        }
    }
}
