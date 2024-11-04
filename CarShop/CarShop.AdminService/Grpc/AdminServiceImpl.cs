using System.ComponentModel.DataAnnotations;
using CarShop.AdminService.Repositories;
using CarShop.AdminService.Services;
using CarShop.ServiceDefaults.ServiceInterfaces.AdminService;
using CarShop.ServiceDefaults.Utils;
using Grpc.Core;
using Isopoh.Cryptography.Argon2;

namespace CarShop.AdminService.Grpc;

public class AdminServiceImpl(
    AdminsRepository _adminsRepository,
    RefreshSessionsRepository _refreshSessionsRepository,
    TokensPairGenerator _tokensPairGenerator) : AdminService.AdminServiceBase
{
    public override async Task<CreateAccountReply> CreateAccount(CreateAccountRequest request, ServerCallContext context)
    {
        // Корректные ли данные пришли
        if (string.IsNullOrWhiteSpace(request.Email) ||
            !Utils.IsEmail(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password) ||
            request.Priority < Constants.HighestAdminPriority ||
            request.Priority > Constants.LowestAdminPriority)
        {
            return new CreateAccountReply
            {
                Result = CreateAccountReply.Types.CreateAccountResult.BadRequest
            };
        }
        
        Admin? admin = await _adminsRepository.GetByEmailAsync(request.Email);
        if (admin is not null) // Пользователь с такой почтой уже есть
        {
            return new CreateAccountReply
            {
                Result = CreateAccountReply.Types.CreateAccountResult.UserWithThisEmailAlreadyExists
            };
        }
        
        await _adminsRepository.CreateAccountAsync(
            request.Email,
            Argon2.Hash(request.Password),
            request.Priority);

        return new CreateAccountReply
        {
            Result = CreateAccountReply.Types.CreateAccountResult.Success
        };
    }

    public override async Task<LoginReply> Login(LoginRequest request, ServerCallContext context)
    {
        // Корректные ли данные пришли
        if (string.IsNullOrWhiteSpace(request.Email) ||
            !Utils.IsEmail(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return new LoginReply
            {
                Result = LoginReply.Types.LoginResult.BadRequest
            };
        }
        
        Admin? admin = await _adminsRepository.GetByEmailAsync(request.Email);
        if (admin is null) // Аккаунта с таким email нет
        {
            return new LoginReply
            {
                Result = LoginReply.Types.LoginResult.AccountNotFound
            };
        }

        if (admin.Banned) // Аккаунт заблокирован
        {
            return new LoginReply
            {
                Result = LoginReply.Types.LoginResult.AccountBanned
            };
        }

        if (!Argon2.Verify(admin.Password, request.Password)) // Неверный пароль
        {
            return new LoginReply
            {
                Result = LoginReply.Types.LoginResult.IncorrectPassword
            };
        }
        
        var tokensPair = _tokensPairGenerator
            .GenerateTokensPair(admin.Id, admin.Email, admin.Roles, out DateTime refreshTokenExpires);
        
        await _refreshSessionsRepository.CreateSessionAsync(new RefreshSession
        {
            AdminId = admin.Id,
            RefreshToken = tokensPair.RefreshToken,
            CreatedAt = DateTime.UtcNow,
            ExpiresIn = refreshTokenExpires,
        });

        return new LoginReply
        {
            Result = LoginReply.Types.LoginResult.Success,
            RefreshToken = tokensPair.RefreshToken,
            AccessToken = tokensPair.AccessToken,
        };
    }

    public override async Task<UpdateTokensReply> UpdateTokens(UpdateTokensRequest request, ServerCallContext context)
    {
        // Корректные ли данные пришли
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return new UpdateTokensReply
            {
                Result = UpdateTokensReply.Types.UpdateTokensResult.BadRequest
            };
        }
        
        RefreshSession? refreshSession = await _refreshSessionsRepository
            .GetByRefreshTokenAsync(request.RefreshToken);

        if (refreshSession is null) // Сессия не найдена в бд
        {
            return new UpdateTokensReply
            {
                Result = UpdateTokensReply.Types.UpdateTokensResult.SessionNotFound
            };
        }

        if (refreshSession.IsExpired) // Сессия истекла
        {
            await _refreshSessionsRepository.DeleteSessionAsync(refreshSession.Id);
            return new UpdateTokensReply
            {
                Result = UpdateTokensReply.Types.UpdateTokensResult.SessionNotFound
            };
        }
        
        Admin admin = (await _adminsRepository.GetByIdAsync(refreshSession.AdminId))!;
        var tokensPair = _tokensPairGenerator
            .GenerateTokensPair(admin.Id, admin.Email, admin.Roles, out DateTime refreshTokenExpires);
        
        refreshSession.RefreshToken = tokensPair.RefreshToken;
        refreshSession.ExpiresIn = refreshTokenExpires;
        await _refreshSessionsRepository.UpdateSessionAsync(refreshSession);
        
        return new UpdateTokensReply
        {
            Result = UpdateTokensReply.Types.UpdateTokensResult.Success,
            RefreshToken = tokensPair.RefreshToken,
            AccessToken = tokensPair.AccessToken,
        };
    }

    public override async Task<BanAccountReply> BanAccount(BanAccountRequest request, ServerCallContext context)
    {
        Admin? admin = await _adminsRepository.GetByIdAsync(request.AccountId);
        if (admin is null)
        {
            return new BanAccountReply
            {
                Result = BanAccountReply.Types.BanAccountResult.AccountNotFound
            };
        }

        if (admin.Banned)
        {
            return new BanAccountReply
            {
                Result = BanAccountReply.Types.BanAccountResult.AccountAlreadyBanned
            };
        }

        admin.Banned = true;
        await _adminsRepository.UpdateAccountAsync(admin);
        await _refreshSessionsRepository.RemoveAllSessionsOfAdminAsync(admin.Id);

        return new BanAccountReply
        {
            Result = BanAccountReply.Types.BanAccountResult.Success
        };
    }

    public override async Task<UnbanAccountReply> UnbanAccount(UnbanAccountRequest request, ServerCallContext context)
    {
        Admin? admin = await _adminsRepository.GetByIdAsync(request.AccountId);
        if (admin is null)
        {
            return new UnbanAccountReply
            {
                Result = UnbanAccountReply.Types.UnbanAccountResult.AccountNotFound
            };
        }

        if (!admin.Banned)
        {
            return new UnbanAccountReply
            {
                Result = UnbanAccountReply.Types.UnbanAccountResult.AccountNotBanned
            };
        }

        admin.Banned = false;
        await _adminsRepository.UpdateAccountAsync(admin);

        return new UnbanAccountReply
        {
            Result = UnbanAccountReply.Types.UnbanAccountResult.Success
        };
    }
}