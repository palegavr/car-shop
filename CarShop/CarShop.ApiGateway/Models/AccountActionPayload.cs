using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using CarShop.ServiceDefaults;
using CarShop.ServiceDefaults.ServiceInterfaces.AdminService;

namespace CarShop.ApiGateway.Models;

[AccountActionPayloadValidation]
public class AccountActionPayload
{
    [JsonPropertyName("action")] public AccountAction ActionType { get; set; }
    [JsonPropertyName("data")] public AccountActionDataPayload? Data { get; set; }

    public bool ActionIsAllowed(string[] roles,
        int performingAdminPriority,
        int adminPriority,
        long performingAdminId,
        long adminId)
    {
        switch (ActionType)
        {
            case AccountAction.Create:
                if (!roles.Contains(Role.Admin.Account.Create) ||
                    performingAdminPriority == Constants.LowestAdminPriority)
                {
                    return false;
                }

                break;
            case AccountAction.ChangePassword:
                if ((performingAdminId == adminId &&
                     !roles.Contains(Role.Admin.Account.ChangePassword.Own)) ||
                    (performingAdminId != adminId &&
                     !roles.Contains(Role.Admin.Account.ChangePassword.Other)) ||
                    (adminPriority <= performingAdminPriority && performingAdminId != adminId))
                {
                    return false;
                }

                break;
            case AccountAction.Ban:
                if ((performingAdminId == adminId &&
                     !roles.Contains(Role.Admin.Account.Ban.Own)) ||
                    (performingAdminId != adminId &&
                     !roles.Contains(Role.Admin.Account.Ban.Other)) ||
                    (adminPriority <= performingAdminPriority && performingAdminId != adminId))
                {
                    return false;
                }

                break;
            case AccountAction.Unban:
                if (!roles.Contains(Role.Admin.Account.Unban) ||
                    adminPriority <= performingAdminPriority)
                {
                    return false;
                }

                break;
            case AccountAction.GiveRole:
                if (!roles.Contains(Role.Admin.Account.Role.Give) ||
                    adminPriority <= performingAdminPriority ||
                    !roles.Contains(Data!.Role))
                {
                    return false;
                }

                break;
            case AccountAction.TakeRole:
                if (!roles.Contains(Role.Admin.Account.Role.Take) ||
                    adminPriority <= performingAdminPriority ||
                    !roles.Contains(Data!.Role))
                {
                    return false;
                }

                break;
            case AccountAction.SetPriority:
                if (!roles.Contains(Role.Admin.Account.Priority.Set) ||
                    adminPriority <= performingAdminPriority ||
                    performingAdminPriority >= Data!.Priority)
                {
                    return false;
                }

                break;
        }

        return true;
    }

    public class AccountActionPayloadValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            try
            {
                if (validationContext.ObjectInstance is AccountActionPayload payload)
                {
                    if (!Enum.IsDefined(payload.ActionType))
                    {
                        throw new ArgumentException("Action dont exists.");
                    }

                    switch (payload.ActionType)
                    {
                        case AccountAction.Create:
                            ArgumentException.ThrowIfNullOrWhiteSpace(payload.Data?.Email);
                            break;
                        case AccountAction.ChangePassword:
                            ArgumentException.ThrowIfNullOrWhiteSpace(payload.Data?.Password);
                            ArgumentException.ThrowIfNullOrWhiteSpace(payload.Data?.OldPassword);
                            break;
                        case AccountAction.GiveRole:
                            ArgumentException.ThrowIfNullOrWhiteSpace(payload.Data?.Role);
                            if (!Role.AllExistingRoles.Contains(payload.Data.Role))
                            {
                                throw new ArgumentException("Role not exists.", nameof(payload.Data.Role));
                            }
                            break;
                        case AccountAction.TakeRole:
                            ArgumentException.ThrowIfNullOrWhiteSpace(payload.Data?.Role);
                            if (!Role.AllExistingRoles.Contains(payload.Data.Role))
                            {
                                throw new ArgumentException("Role not exists.", nameof(payload.Data.Role));
                            }
                            break;
                        case AccountAction.SetPriority:
                            ArgumentNullException.ThrowIfNull(payload.Data?.Priority);
                            break;
                    }
                }
            }
            catch (ArgumentException e)
            {
                return new ValidationResult(e.Message);
            }

            return ValidationResult.Success;
        }
    }
}

public enum AccountAction
{
    Create,
    ChangePassword,
    Ban,
    Unban,
    GiveRole,
    TakeRole,
    SetPriority
}