using CarShop.AdminService.Repositories;
using CarShop.ServiceDefaults.ServiceInterfaces.AdminService;
using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

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
            if (!ModelState.IsValid)
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
                Argon2.Hash(createAccountRequest.Password),
                createAccountRequest.Priority);

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

            TokensPair tokensPair = GenerateTokensPair(admin.Id, admin.Email, admin.Roles, out DateTime refreshTokenExpires);

            await _refreshSessionsRepository.CreateSessionAsync(new RefreshSession
            {
                AdminId = admin.Id,
                RefreshToken = tokensPair.RefreshToken,
                CreatedAt = DateTime.UtcNow,
                ExpiresIn = refreshTokenExpires,
            });

            return Ok(new TokensPairResponce
            {
                AccessToken = tokensPair.AccessToken,
                RefreshToken = tokensPair.RefreshToken,
            });
        }

        [HttpPost]
        [Route("updatetokens")]
        public async Task<IActionResult> UpdateTokensAsync([FromBody] UpdateTokensRequest updateTokensRequest)
        {
            RefreshSession? refreshSession = await _refreshSessionsRepository.GetByRefreshTokenAsync(updateTokensRequest.RefreshToken);

            if (refreshSession is null ||
                refreshSession.IsExpired)
            {
                if (refreshSession?.IsExpired ?? false)
                {
                    await _refreshSessionsRepository.DeleteSessionAsync(refreshSession.Id);
                }
                return Unauthorized();
            }

            Admin admin = (await _adminsRepository.GetByIdAsync(refreshSession.AdminId))!;
            TokensPair tokensPair = GenerateTokensPair(admin.Id, admin.Email, admin.Roles, out DateTime refreshTokenExpires);

            refreshSession.RefreshToken = tokensPair.RefreshToken;
            refreshSession.ExpiresIn = refreshTokenExpires;
            await _refreshSessionsRepository.UpdateSessionAsync(refreshSession);

            return Ok(new TokensPairResponce
            {
                AccessToken = tokensPair.AccessToken,
                RefreshToken = tokensPair.RefreshToken,
            });
        }

        [HttpPost]
        [Route("ban/{id:long}")]
        public async Task<IActionResult> BanAsync([FromRoute(Name = "id")] long id)
        {
            Admin? admin = await _adminsRepository.GetByIdAsync(id);
            if (admin is null)
            {
                return NotFound();
            }

            if (admin.Banned)
            {
                return Conflict();
            }

            admin.Banned = true;
            await _adminsRepository.UpdateAccountAsync(admin);
            await _refreshSessionsRepository.RemoveAllSessionsOfAdminAsync(admin.Id);

            return Ok();
        }

        [HttpPost]
        [Route("unban/{id:long}")]
        public async Task<IActionResult> UnbanAsync([FromRoute(Name = "id")] long id)
        {
            Admin? admin = await _adminsRepository.GetByIdAsync(id);
            if (admin is null)
            {
                return NotFound();
            }

            if (!admin.Banned)
            {
                return Conflict();
            }

            admin.Banned = false;
            await _adminsRepository.UpdateAccountAsync(admin);

            return Ok();
        }

        [NonAction]
        private TokensPair GenerateTokensPair(long id, string email, string[] roles, out DateTime refreshTokenExpires)
        {
            refreshTokenExpires = DateTime.UtcNow.Add(AuthOptions.REFRESH_TOKEN_LIFETIME);
            JwtSecurityToken refreshJwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                expires: refreshTokenExpires,
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            string refreshToken = new JwtSecurityTokenHandler().WriteToken(refreshJwt);
            
            var roleClaims = from role in roles select new Claim(ClaimTypes.Role, role);
            var claims = new List<Claim>(roleClaims)
            {
                new("id", id.ToString()),
                new("email", email)
            };

            JwtSecurityToken accessJwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claims,
                expires: DateTime.UtcNow.Add(AuthOptions.ACCESS_TOKEN_LIFETIME),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            string accessToken = new JwtSecurityTokenHandler().WriteToken(accessJwt);

            return new TokensPair(refreshToken, accessToken);
        }

        private record TokensPair(string RefreshToken, string AccessToken);
    }
}
