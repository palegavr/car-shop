import {ProcessData} from "@/types/types";
import {encodeProcessDataToJson} from "@/utilities/processDataOperations";

enum AccountAction {
    Create,
    ChangePassword,
    Ban,
    Unban,
    GiveRole,
    TakeRole,
    SetPriority
}

type AccountActionPayload = {
    action: AccountAction,
    data?: {
        email?: string,
        password?: string,
        oldPassword?: string,
        role?: string,
        priority?: number
    }
}

export const ExistingRoles = [
    'admin.car.add',
    'admin.car.edit',
    'admin.car.delete',
    'admin.account.create',
    'admin.account.change-password.own',
    'admin.account.change-password.other',
    'admin.account.ban.own',
    'admin.account.ban.other',
    'admin.account.unban',
    'admin.account.role.give',
    'admin.account.role.take',
    'admin.account.priority.set'
] as const;
export type Role = typeof ExistingRoles[number];

export type Admin = {
    id: number,
    email: string,
    roles: Role[],
    priority: number,
    banned: boolean
}

type LogoutResult = {
    success: boolean,
}
export async function logoutAsync(): Promise<LogoutResult> {
    try {
        const response = await fetch(`/api/admin/logout`, {
            method: 'GET',
            credentials: 'same-origin',
        });

        return {
            success: response.ok
        }
    } catch (error) {
        return {
            success: false
        }
    }
}


type GetAdminsResult = {
    success: boolean,
    admins: Admin[],
}
export async function getAdminsAsync(): Promise<GetAdminsResult> {
    try {
        const response = await fetch(`/api/admin/admins`, {
            method: 'GET',
            credentials: 'same-origin',
            headers: {
                'Content-Type': 'application/json'
            },
        });

        return {
            success: response.ok,
            admins: await response.json()
        }
    } catch (error) {
        return {
            success: false,
            admins: [],
        }
    }
}

type AddCarResult = {
    success: boolean,
    badRequest: boolean,
    carId?: number,
}
export async function addCarAsync(carData: ProcessData): Promise<AddCarResult> {
    try {
        const response = await fetch(`/api/admin/car`, {
            method: 'POST',
            credentials: 'same-origin',
            headers: {
                'Content-Type': 'application/json'
            },
            body: encodeProcessDataToJson(carData)
        });

        return {
            success: response.ok,
            badRequest: response.status === 400,
            carId: (await response.json()).id
        }
    } catch (error) {
        return {
            success: false,
            badRequest: false,
        }
    }
}

type UploadImageResult = {
    success: boolean,
    publicImageUrls?: string[]
}
export async function uploadImageAsync(files: File[]): Promise<UploadImageResult> {
    const formData = new FormData();
    files.forEach(file => formData.append('images', file));
    try {
        const response = await fetch('/api/admin/uploadimage', {
            method: 'POST',
            body: formData,
            credentials: 'same-origin',
        })

        if (response.ok) {
            return {success: true, publicImageUrls: (await response.json()) as string[]}
        } else {
            return {success: false}
        }
    } catch (error) {
        return {success: false}
    }
}

type ApplyChangesResult = {
    success: boolean,
}
export async function applyChangesAsync(carId: number): Promise<ApplyChangesResult> {
    try {
        const response = await fetch(`/api/admin/editcar/${carId}/applychanges`, {
            method: 'POST',
            credentials: 'same-origin',
        });

        return {success: response.ok}
    } catch (error) {
        return {success: false}
    }
}

export type PushProcessDataResult = {
    success: boolean,
}
export async function pushProcessData(carId: number, processData: ProcessData): Promise<PushProcessDataResult> {
    try {
        const response = await fetch(`/api/admin/editcar/${carId}/process`, {
            method: 'POST',
            credentials: 'same-origin',
            headers: {
                'Content-Type': 'application/json'
            },
            body: encodeProcessDataToJson(processData)
        });

        return {success: response.ok}
    } catch (error) {
        return {success: false}
    }
}

export type SetPriorityResult = {
    success: boolean,
}
export async function setPriorityAsync(accountId: number, priority: number): Promise<SetPriorityResult> {
    try {
        const payload: AccountActionPayload = {
            action: AccountAction.SetPriority,
            data: {
                priority: priority
            }
        };

        const response = await fetch(`/api/admin/account/${accountId}`, {
            method: 'POST',
            credentials: 'same-origin',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(payload)
        });

        return {success: response.ok}
    } catch (error) {
        return {success: false}
    }
}

export type BanResult = {
    success: boolean,
}
export async function banAsync(accountId: number): Promise<BanResult> {
    try {
        const payload: AccountActionPayload = {
            action: AccountAction.Ban,
        };
        const response = await fetch(`/api/admin/account/${accountId}`, {
            method: 'POST',
            credentials: 'same-origin',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(payload)
        });

        return {success: response.ok}
    } catch (error) {
        return {success: false}
    }
}
export type UnbanResult = {
    success: boolean,
}
export async function unbanAsync(accountId: number): Promise<UnbanResult> {
    try {
        const payload: AccountActionPayload = {
            action: AccountAction.Unban,
        };
        const response = await fetch(`/api/admin/account/${accountId}`, {
            method: 'POST',
            credentials: 'same-origin',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(payload)
        });

        return {success: response.ok}
    } catch (error) {
        return {success: false}
    }
}

export type GiveRoleResult = {
    success: boolean,
}
export async function giveRoleAsync(accountId: number, role: Role): Promise<GiveRoleResult> {
    try {
        const payload: AccountActionPayload = {
            action: AccountAction.GiveRole,
            data: {
                role: role
            }
        };
        const response = await fetch(`/api/admin/account/${accountId}`, {
            method: 'POST',
            credentials: 'same-origin',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(payload)
        });

        return {success: response.ok}
    } catch (error) {
        return {success: false}
    }
}

export type TakeRoleResult = {
    success: boolean,
}
export async function takeRoleAsync(accountId: number, role: Role): Promise<TakeRoleResult> {
    try {
        const payload: AccountActionPayload = {
            action: AccountAction.TakeRole,
            data: {
                role: role
            }
        };
        const response = await fetch(`/api/admin/account/${accountId}`, {
            method: 'POST',
            credentials: 'same-origin',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(payload)
        });

        return {success: response.ok}
    } catch (error) {
        return {success: false}
    }
}

export type ChangePasswordResult = {
    success: boolean,
    oldPasswordIncorrect?: boolean
}
export async function changePasswordAsync(accountId: number, newPassword: string, oldPassword: string): Promise<ChangePasswordResult> {
    try {
        const payload: AccountActionPayload = {
            action: AccountAction.ChangePassword,
            data: {
                password: newPassword,
                oldPassword: oldPassword
            }
        };
        const response = await fetch(`/api/admin/account/${accountId}`, {
            method: 'POST',
            credentials: 'same-origin',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(payload)
        });

        return {
            success: response.ok,
            oldPasswordIncorrect: response.ok ? undefined : response.status === 400
        }
    } catch (error) {
        return {
            success: false
        }
    }
}

export type CreateAccountResult = {
    success: boolean,
    password?: string,
    emailAlreadyExists: boolean
}
export async function createAccountAsync(email: string): Promise<CreateAccountResult> {
    try {
        const payload: AccountActionPayload = {
            action: AccountAction.Create,
            data: {
                email: email
            }
        };
        const response = await fetch(`/api/admin/account`, {
            method: 'POST',
            credentials: 'same-origin',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(payload)
        });

        return {
            success: response.ok,
            password: response.ok ? (await response.json()).password : undefined,
            emailAlreadyExists: response.status === 409
        }
    } catch (error) {
        return {
            success: false,
            emailAlreadyExists: false
        }
    }
}