using System.ComponentModel.DataAnnotations;
using CarShop.AdminService.Database.Entities;
using CarShop.AdminService.Extensions;
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
    public override async Task<CreateAccountReply> CreateAccount(CreateAccountRequest request,
        ServerCallContext context)
    {
        if (await _adminsRepository.GetByEmailAsync(request.Account.Email)
            is not null) // Пользователь с такой почтой уже есть
        {
            return new CreateAccountReply
            {
                Result = CreateAccountReply.Types.CreateAccountResult.UserWithThisEmailAlreadyExists
            };
        }

        Admin admin;

        try
        {
            admin = new Admin
            {
                Email = request.Account.Email,
                Password = request.Account.Password,
                Priority = request.Account.Priority,
                Roles = request.Account.Roles.ToArray(),
                Banned = request.Account.Banned
            };
            await _adminsRepository.CreateAccountAsync(admin);
        }
        catch (ValidationException)
        {
            return new CreateAccountReply
            {
                Result = CreateAccountReply.Types.CreateAccountResult.BadRequest
            };
        }

        return new CreateAccountReply
        {
            Account = admin.ToGrpcMessage(),
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
            .GenerateTokensPair(admin.Id, admin.Email, admin.Roles, admin.Priority, out DateTime refreshTokenExpires);

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
            .GenerateTokensPair(admin.Id, admin.Email, admin.Roles, admin.Priority, out DateTime refreshTokenExpires);

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

    public override async Task<UpdateAccountReply> UpdateAccount(UpdateAccountRequest request,
        ServerCallContext context)
    {
        Admin? admin = await _adminsRepository.GetByIdAsync(request.Id);
        if (admin is null)
        {
            return new UpdateAccountReply
            {
                Result = UpdateAccountReply.Types.UpdateAccountResult.AccountNotFound
            };
        }

        if (request.HasEmail)
        {
            admin.Email = request.Email;
        }

        if (request.HasPassword)
        {
            admin.Password = request.Password;
            try
            {
                Validator.ValidateObject(admin, new(admin), true);
            }
            catch (ValidationException)
            {
                return new UpdateAccountReply
                {
                    Result = UpdateAccountReply.Types.UpdateAccountResult.BadRequest
                };
            }

            admin.Password = Argon2.Hash(admin.Password);
        }

        if (request.HasPriority)
        {
            admin.Priority = request.Priority;
        }

        if (request.HasBanned)
        {
            admin.Banned = request.Banned;
        }

        if (request.Roles.Any() || request is { HasUpdateRoles: true, UpdateRoles: true })
        {
            admin.Roles = request.Roles.ToArray();
        }

        try
        {
            await _adminsRepository.UpdateAccountAsync(admin);
        }
        catch (ValidationException)
        {
            return new UpdateAccountReply
            {
                Result = UpdateAccountReply.Types.UpdateAccountResult.BadRequest
            };
        }
        catch (ArgumentException e)
        {
            if (e.ParamName == nameof(admin.Email))
            {
                return new UpdateAccountReply
                {
                    Result = UpdateAccountReply.Types.UpdateAccountResult.EmailIsBusy
                };
            }

            throw;
        }

        return new UpdateAccountReply
        {
            Account = admin.ToGrpcMessage(),
            Result = UpdateAccountReply.Types.UpdateAccountResult.Success,
        };
    }

    public override async Task<GetAccountReply> GetAccount(GetAccountRequest request, ServerCallContext context)
    {
        Admin? admin = await _adminsRepository.GetByIdAsync(request.AccountId);
        if (admin is null)
        {
            return new GetAccountReply
            {
                Result = GetAccountReply.Types.GetAccountResult.AccountNotFound
            };
        }

        return new GetAccountReply
        {
            Account = admin.ToGrpcMessage(),
            Result = GetAccountReply.Types.GetAccountResult.Success
        };
    }

    public override async Task<GetAccountsReply> GetAccounts(GetAccountsRequest request, ServerCallContext context)
    {
        var admins = await _adminsRepository.GetMany(
            sortType: request.HasSortType ? request.SortType : GetAccountsRequest.Types.SortType.Asc,
            sortBy: request.HasSortBy ? request.SortBy : null,
            minPriority: request.HasMinPriority ? request.MinPriority : null,
            maxPriority: request.HasMaxPriority ? request.MaxPriority : null,
            haveRoles: request.HaveRoles.Count > 0 ? request.HaveRoles.ToArray() : null,
            banned: request.HasBanned ? request.Banned : null);

        return new GetAccountsReply
        {
            Accounts = { admins.Select(admin => admin.ToGrpcMessage()) }
        };
    }
}