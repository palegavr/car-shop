using CarShop.AdminService.Grpc;
using CarShop.ServiceDefaults;
using CarShop.ServiceDefaults.Services;
using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.ApiGateway.Controllers.Admin;

public class AccountActionHandler(
    long _accountId,
    PasswordGenerator _passwordGenerator,
    AdminService.Grpc.AdminService.AdminServiceClient _adminServiceClient)
{
    public async Task<IActionResult> Create(string email)
    {
        string generatedPassword = _passwordGenerator.GeneratePassword(20);
        var createAccountReply = await _adminServiceClient.CreateAccountAsync(new CreateAccountRequest
        {
            Account = new AdminAccount
            {
                Email = email,
                Password = generatedPassword,
                Priority = Constants.LowestAdminPriority,
                Roles = { Constants.DefaultAdminRoles },
                Banned = false
            }
        });

        if (createAccountReply.Result == CreateAccountReply.Types.CreateAccountResult.BadRequest)
        {
            return new BadRequestResult();
        }

        if (createAccountReply.Result == CreateAccountReply.Types.CreateAccountResult.UserWithThisEmailAlreadyExists)
        {
            return new ConflictResult();
        }

        if (createAccountReply.Result != CreateAccountReply.Types.CreateAccountResult.Success)
        {
            return new ObjectResult(null)
            {
                StatusCode = 500
            };
        }

        return new OkObjectResult(new
        {
            password = generatedPassword
        });
    }

    public async Task<IActionResult> ChangePassword(string newPassword, string oldPassword, long performingAdminId)
    {
        var getAccountReply = await _adminServiceClient.GetAccountAsync(new GetAccountRequest
        {
            AccountId = performingAdminId
        });

        if (getAccountReply.Result == GetAccountReply.Types.GetAccountResult.AccountNotFound)
        {
            return new NotFoundResult();
        }

        if (!Argon2.Verify(getAccountReply.Account.Password, oldPassword))
        {
            return new BadRequestResult();
        }

        var updateAccountReply = await _adminServiceClient.UpdateAccountAsync(new()
        {
            Id = _accountId,
            Password = newPassword
        });

        return UpdateAccountResultToActionResult(updateAccountReply.Result);
    }

    public async Task<IActionResult> Ban()
    {
        var banAccountReply = await _adminServiceClient.BanAccountAsync(new()
        {
            AccountId = _accountId
        });

        return banAccountReply.Result switch
        {
            BanAccountReply.Types.BanAccountResult.AccountAlreadyBanned => new ConflictResult(),
            BanAccountReply.Types.BanAccountResult.AccountNotFound => new NotFoundResult(),
            BanAccountReply.Types.BanAccountResult.Success => new OkResult(),
            _ => new ObjectResult(null)
            {
                StatusCode = 500
            }
        };
    }
    
    public async Task<IActionResult> Unban()
    {
        var banAccountReply = await _adminServiceClient.UnbanAccountAsync(new()
        {
            AccountId = _accountId
        });

        return banAccountReply.Result switch
        {
            UnbanAccountReply.Types.UnbanAccountResult.AccountNotBanned => new ConflictResult(),
            UnbanAccountReply.Types.UnbanAccountResult.AccountNotFound => new NotFoundResult(),
            UnbanAccountReply.Types.UnbanAccountResult.Success => new OkResult(),
            _ => new ObjectResult(null)
            {
                StatusCode = 500
            }
        };
    }

    public async Task<IActionResult> GiveRoles(string[] roles)
    {
        var getAccountReply = await _adminServiceClient.GetAccountAsync(new GetAccountRequest
        {
            AccountId = _accountId
        });
        
        if (getAccountReply.Result == GetAccountReply.Types.GetAccountResult.AccountNotFound)
        {
            return new NotFoundResult();
        }

        if (roles.Any(role => getAccountReply.Account.Roles.Contains(role)))
        {
            return new ConflictResult();
        }
        
        var updateAccountReply = await _adminServiceClient.UpdateAccountAsync(new ()
        {
            Id = _accountId,
            Roles = { getAccountReply.Account.Roles.Union(roles) },
            UpdateRoles = true
        });

        return UpdateAccountResultToActionResult(updateAccountReply.Result);
    }
    
    public async Task<IActionResult> TakeRoles(string[] roles)
    {
        var getAccountReply = await _adminServiceClient.GetAccountAsync(new GetAccountRequest
        {
            AccountId = _accountId
        });
        
        if (getAccountReply.Result == GetAccountReply.Types.GetAccountResult.AccountNotFound)
        {
            return new NotFoundResult();
        }
        
        if (!roles.All(role => getAccountReply.Account.Roles.Contains(role)))
        {
            return new ConflictResult();
        }
        
        var updateAccountReply = await _adminServiceClient.UpdateAccountAsync(new ()
        {
            Id = _accountId,
            Roles =
            {
                getAccountReply.Account.Roles
                    .Where(role => !roles.Contains(role))
            },
            UpdateRoles = true
        });
        
        return UpdateAccountResultToActionResult(updateAccountReply.Result);
    }

    public async Task<IActionResult> SetPriority(int priority)
    {
        var updateAccountReply = await _adminServiceClient.UpdateAccountAsync(new()
        {
            Id = _accountId,
            Priority = priority,
        });

        return UpdateAccountResultToActionResult(updateAccountReply.Result);
    }

    private IActionResult UpdateAccountResultToActionResult(UpdateAccountReply.Types.UpdateAccountResult result)
    {
        return result switch
        {
            UpdateAccountReply.Types.UpdateAccountResult.BadRequest => new BadRequestResult(),
            UpdateAccountReply.Types.UpdateAccountResult.AccountNotFound => new NotFoundResult(),
            UpdateAccountReply.Types.UpdateAccountResult.EmailIsBusy => new ConflictResult(),
            UpdateAccountReply.Types.UpdateAccountResult.Success => new OkResult(),
            _ => new ObjectResult(null)
            {
                StatusCode = 500
            }
        };
    }
}
