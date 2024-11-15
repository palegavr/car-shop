'use client'

import '@fontsource/roboto/300.css';
import '@fontsource/roboto/400.css';
import '@fontsource/roboto/500.css';
import '@fontsource/roboto/700.css';
import {
    Box,
    Button,
    Checkbox,
    CircularProgress,
    Container,
    FormControlLabel, IconButton, InputAdornment, Paper,
    Stack,
    TextField,
    Typography
} from "@mui/material";
import React, {useEffect, useRef, useState} from "react";
import {
    banAsync,
    changePasswordAsync,
    ChangePasswordResult, ExistingRoles,
    giveRoleAsync,
    Role,
    setPriorityAsync, SetPriorityResult,
    takeRoleAsync,
    unbanAsync
} from "@/clients/backendСlient";
import {LoadingButton} from "@mui/lab";
import {delay} from "@/utilities/delay";
import {toast, ToastContainer} from "react-toastify";
import 'react-toastify/dist/ReactToastify.css';

const defaultToastTime = 2000;

type Admin = {
    id: number,
    priority: number,
    banned: boolean,
    roles: Role[],
}

type PerformingAdmin = {
    id: number,
    priority: number,
    roles: Role[],
}

type Props = {
    admin: Admin,
    performingAdmin: PerformingAdmin,
}

let performingAdmin: PerformingAdmin = {
    id: 0,
    roles: [],
    priority: 0
}

let admin: Admin = {
    id: 0,
    roles: [],
    banned: false,
    priority: 0
}

export default function Page() {
    const [priority, setPriority] = useState<number>(admin.priority);
    const [banned, setBanned] = useState<boolean>(admin.banned);
    const [roles, setRoles] = useState<string[]>(admin.roles);
    const [pageLoaded, setPageLoaded] = useState<boolean>();

    useEffect(() => {
        try {
            admin = (window as any).carShopData.admin;
            performingAdmin = (window as any).carShopData.performingAdmin;
            setPriority(admin.priority);
            setRoles(admin.roles);
            setBanned(admin.banned)
            setPageLoaded(true);
        } catch {
            setPageLoaded(false);
        }
    }, []);

    async function handleChangePriority(newPriority: number): Promise<SetPriorityResult> {
        const result = await setPriorityAsync(admin.id, newPriority);
        if (result.success) {
            setPriority(newPriority);
        }
        return result;
    }

    async function handleChangeBanned(newBanned: boolean) {
        if (newBanned && !banned) {
            const result = await banAsync(admin.id);
            if (result.success) {
                setBanned(true);
            }
        } else if (!newBanned && banned) {
            const result = await unbanAsync(admin.id);
            if (result.success) {
                setBanned(false);
            }
        }
    }

    async function handleChangeRole(role: Role, enabled: boolean) {
        console.log(`enabled: ${enabled} includes: ${roles.includes(role)}`)
        if (enabled && !roles.includes(role)) {
            const result = await giveRoleAsync(admin.id, role);
            if (result.success) {
                setRoles([...roles, role]);
            }
        } else if (!enabled && roles.includes(role)) {
            const result = await takeRoleAsync(admin.id, role);
            if (result.success) {
                setRoles(roles.filter(_role => _role !== role));
            }
        }
    }

    async function handleChangePassword(newPassword: string, oldPassword: string): Promise<ChangePasswordResult> {
        return await changePasswordAsync(admin.id, newPassword, oldPassword);
    }

    if (pageLoaded === undefined) {
        return (
            <Paper sx={{
                backgroundColor: 'lightyellow',
                padding: 1,
                textAlign: 'center',
                marginTop: '50vh',
                marginLeft: '50%',
                transform: 'translate(-50%)',
                display: 'inline-block'
            }}>
                Загрузка...
            </Paper>
        )
    }

    if (!pageLoaded) {
        return (
            <Paper sx={{
                backgroundColor: '#faa',
                padding: 1,
                textAlign: 'center',
                marginTop: '50vh',
                marginLeft: '50%',
                transform: 'translate(-50%)',
                display: 'inline-block'
            }}>
                Ошибка
            </Paper>
        )
    }

    return (
        <>
            <Container maxWidth={'sm'} sx={{
                backgroundColor: '#fff',
                marginTop: 2,
                marginBottom: 2,
                paddingTop: 2,
                paddingBottom: 2,
                borderRadius: 2
            }}>
                <Stack spacing={2}>
                    <PriorityChanger/>
                    <BanChanger/>
                    <RolesManager/>
                    <PasswordChanger/>
                </Stack>
            </Container>
            <ToastContainer/>
        </>
    )

    function BanChanger() {
        type BannedState = 'loading' | 'default';
        const [bannedState, setBannedState] = useState<BannedState>('default')
        const [preparingBanned, setPreparingBanned] = useState<boolean>()

        const canBan: boolean =
            performingAdmin.priority < admin.priority &&
            performingAdmin.roles.includes('admin.account.ban.other');
        const canUnban: boolean =
            performingAdmin.priority < admin.priority &&
            performingAdmin.roles.includes('admin.account.unban');

        async function handleClick(newBanned: boolean) {
            setPreparingBanned(newBanned);
            await handleChangeBanned(newBanned);
            setPreparingBanned(undefined);
        }

        return (
            <>
                {preparingBanned === undefined || preparingBanned === false ? (
                    <Button variant={'contained'} color={'error'}
                            disabled={banned || !canBan}
                            onClick={event => handleClick(true)}>
                        {!canBan ? 'Недостаточно прав для блокировки' : 'Забанить'}</Button>
                ) : (
                    <LoadingButton loading variant="outlined">Loading...</LoadingButton>
                )}
                {preparingBanned === undefined || preparingBanned === true ? (
                    <Button variant={'contained'} color={'success'}
                            disabled={!banned || !canUnban}
                            onClick={event => handleClick(false)}>
                        {!canUnban ? 'Недостаточно прав для разблокировки' : 'Разбанить'}</Button>
                ) : (
                    <LoadingButton loading variant="outlined">Loading...</LoadingButton>
                )}
            </>
        )
    }

    function PriorityChanger() {
        type PriorityInputState = 'loading' | 'error' | 'default';

        const [inputState, setInputState]
            = useState<PriorityInputState>('default');

        const inputRef = useRef<HTMLInputElement>(null);

        const canChangePriority: boolean = performingAdmin.priority < admin.priority &&
            performingAdmin.roles.includes('admin.account.priority.set');

        useEffect(() => {
            inputRef.current!.value = String(priority);
        }, []);

        async function handleConfirm() {

            const inputValue = inputRef.current!.value.trim();
            if (/^\d*$/.test(inputValue) && inputValue.length <= 10) {
                const numberValue = parseInt(inputValue, 10);

                if (!isNaN(numberValue) && numberValue <= 2000000000 && numberValue >= performingAdmin.priority + 1) {
                    setInputState('loading');
                    const result = await handleChangePriority(numberValue);
                    setInputState('default');
                    if (result.success) {
                        toast.success('Приоритет обновлен.', {
                            position: 'bottom-right',
                            autoClose: defaultToastTime
                        });
                        return;
                    } else {
                        toast.error('Ошибка.', {
                            position: 'bottom-right',
                            autoClose: defaultToastTime
                        });
                        inputRef.current!.value = String(priority);
                    }
                }
            }

            setInputState('error');
        }

        return (
            <>
                <TextField
                    variant={'standard'}
                    label={'Приоритет'}
                    helperText={`Введите число от ${performingAdmin.priority + 1} до 2.000.000.000`}
                    error={inputState === 'error'}
                    disabled={inputState === 'loading' || !canChangePriority}
                    inputRef={inputRef}/>
                {inputState === 'loading' ? (
                    <LoadingButton loading variant="outlined">Loading...</LoadingButton>
                ) : (
                    <Button
                        variant={'contained'}
                        color={'primary'}
                        onClick={handleConfirm}
                        disabled={!canChangePriority}>
                        {!canChangePriority ? 'Недостаточно прав для смены приоритета' : 'Сменить приоритет'}
                    </Button>
                )}

            </>
        )
    }

    function RolesManager() {
        const [changingRole, setChangingRole] = useState<Role>();
        const givableRoles: Role[] = performingAdmin.priority < admin.priority &&
        performingAdmin.roles.includes('admin.account.role.give') ?
            ExistingRoles.filter(role => performingAdmin.roles.includes(role)) : [];
        const takebleRoles: Role[] = performingAdmin.priority < admin.priority &&
        performingAdmin.roles.includes('admin.account.role.take') ?
            ExistingRoles.filter(role => performingAdmin.roles.includes(role)) : [];

        async function handleRoleChange(role: Role, enabled: boolean) {
            setChangingRole(role);
            await handleChangeRole(role, enabled);
            setChangingRole(undefined);
        }

        return (
            <>
                <Typography
                    align={'center'}
                    variant={'h6'}>
                    Роли
                </Typography>
                <Stack spacing={1} border={'1px solid grey'} padding={2} borderRadius={'5px'}>
                    {ExistingRoles.map(role => (
                        <RoleCheck key={role} role={role}/>
                    ))}
                </Stack>
            </>
        )

        function RoleCheck({role}: { role: Role }) {
            return (
                <FormControlLabel
                    disabled={changingRole !== undefined}
                    control={
                        <Box>
                            {role === changingRole &&
                                <CircularProgress size={42} sx={{position: 'absolute'}}/>}
                            <Checkbox
                                disabled={changingRole !== undefined ||
                                    (roles.includes(role) && !takebleRoles.includes(role)) ||
                                    (!roles.includes(role) && !givableRoles.includes(role))}
                                checked={roles.includes(role)}
                                onChange={async (event, checked) => await handleRoleChange(role, checked)}/>
                        </Box>
                    }
                    label={role}
                    labelPlacement={'start'}
                    sx={{
                        '& .MuiFormControlLabel-label': {
                            marginRight: 'auto',
                        },
                    }}/>
            )
        }
    }

    function PasswordChanger() {
        type PasswordInputState = 'error' | 'default';

        const canChangePassword: boolean =
            performingAdmin.roles.includes('admin.account.change-password.other') &&
            performingAdmin.priority < admin.priority;


        const oldPasswordInputRef = useRef<HTMLInputElement>(null);
        const newPasswordInputRef = useRef<HTMLInputElement>(null);

        const [newPasswordInputState, setNewPasswordInputState] = useState<PasswordInputState>('default')
        const [oldPasswordInputState, setOldPasswordInputState] = useState<PasswordInputState>('default')

        const [changingPassword, setChangingPassword] = useState<boolean>(false);

        async function handleChangePasswordClick() {
            const newPassword = newPasswordInputRef.current!.value;
            const oldPassword = oldPasswordInputRef.current!.value;

            setNewPasswordInputState('default');
            setOldPasswordInputState('default');

            let dontSendRequest: boolean = false;
            if (newPassword === '') {
                setNewPasswordInputState('error');
                dontSendRequest = true;
            }

            if (oldPassword === '') {
                setOldPasswordInputState('error')
                dontSendRequest = true;
            }

            if (dontSendRequest) {
                return;
            }

            setChangingPassword(true);
            const result = await handleChangePassword(newPassword, oldPassword);
            if (result.success) {
                toast.success('Пароль изменен.', {
                    position: 'bottom-right',
                    autoClose: defaultToastTime
                });
            } else if (!result.success && result.oldPasswordIncorrect === true) {
                toast.error('Не верный свой пароль.', {
                    position: 'bottom-right',
                    autoClose: defaultToastTime
                });
                setOldPasswordInputState('error');
            } else {
                toast.error('Ошибка.', {
                    position: 'bottom-right',
                    autoClose: defaultToastTime
                });
            }
            console.log(result);
            setChangingPassword(false);
        }

        return (
            <>
                <Typography
                    align={'center'}
                    variant={'h6'}>
                    Смена пароля
                </Typography>
                <Stack spacing={1} border={'1px solid grey'} padding={2} borderRadius={'5px'}>
                    <TextField variant={'standard'} label={'Свой пароль'} inputRef={oldPasswordInputRef}
                               type={'password'}
                               error={oldPasswordInputState === 'error'}
                               disabled={changingPassword || !canChangePassword}
                               onInput={event => setOldPasswordInputState('default')}/>
                    <TextField variant={'standard'} label={'Новый пароль администратора'} inputRef={newPasswordInputRef}
                               type={'password'}
                               error={newPasswordInputState === 'error'}
                               disabled={changingPassword || !canChangePassword}
                               onInput={event => setNewPasswordInputState('default')}/>
                    {changingPassword ? (
                        <LoadingButton loading variant={'contained'}>Loading...</LoadingButton>
                    ) : (
                        <Button variant={'contained'}
                                disabled={!canChangePassword}
                                onClick={handleChangePasswordClick}>
                            {canChangePassword ? 'Сменить пароль' : 'Недостаточно прав для смены пароля'}</Button>
                    )}
                </Stack>
            </>
        )
    }
}

