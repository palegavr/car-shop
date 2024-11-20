import {
    Button,
    Container,
    Dialog, DialogActions,
    DialogContent, DialogContentText,
    DialogTitle,
    Paper,
    Stack,
    TextField,
    Tooltip,
    Typography
} from "@mui/material";
import {banAsync, changePasswordAsync, ChangePasswordResult, logoutAsync, Role} from "@/clients/backendСlient";
import React, {useRef, useState} from "react";
import {toast} from "react-toastify";
import {LoadingButton} from "@mui/lab";

type Props = {
    id: number,
    email: string,
    priority: number,
    roles: Role[],
}

type CloseReason = 'click outside dialog' | 'click close button' | 'click confirm button';


const defaultToastTime = 2000;

export default function ProfilePart({id, email, priority, roles}: Props) {

    async function handleChangePassword(newPassword: string, oldPassword: string): Promise<ChangePasswordResult> {
        return await changePasswordAsync(id, newPassword, oldPassword);
    }

    return (
        <>
            <Container
                maxWidth={'sm'}>
                <Stack
                    spacing={2}>
                    <Paper
                        variant={'outlined'}
                        sx={{
                            padding: 1,
                            textAlign: 'center',
                        }}>
                        Id аккаунта: <b>{id}</b>
                    </Paper>
                    <Paper
                        variant={'outlined'}
                        sx={{
                            padding: 1,
                            textAlign: 'center',
                        }}>
                        Email: <b>{email}</b>
                    </Paper>
                    <Paper
                        variant={'outlined'}
                        sx={{
                            padding: 1,
                            textAlign: 'center',
                        }}>
                        Приоритет: <b>{priority}</b>
                    </Paper>
                    <Typography
                        align={'center'}
                        variant={'h6'}>
                        Роли
                    </Typography>
                    <Stack
                        spacing={1}
                        border={'1px solid grey'}
                        padding={2}
                        borderRadius={1}>
                        {roles.map(role => (
                            <>
                                <Typography
                                    textAlign={'center'}>
                                    {role}
                                </Typography>
                            </>
                        ))}
                    </Stack>
                    <PasswordChanger/>
                    <BanButton/>
                </Stack>
            </Container>
        </>
    )



    function BanButton() {
        const [banning, setBanning] = useState<boolean>(false);
        const [banned, setBanned] = useState<boolean>(false)
        const [banConfirmDialogOpened, setBanConfirmDialogOpened] = useState<boolean>(false);

        const canBan = roles.includes('admin.account.ban.own');

        async function handleCloseBanConfirmDialog(reason: CloseReason) {
            setBanConfirmDialogOpened(false);
            if (reason === 'click confirm button') {
                setBanning(true);
                const banResult = await banAsync(id);
                setBanning(false);
                if (banResult.success) {
                    setBanned(true);
                    await logoutAsync();
                    window.location.href = '/';
                } else {
                    toast.error('Ошибка.', {
                        position: 'bottom-right',
                        autoClose: defaultToastTime
                    });
                }
            }
        }

        function handleClick() {
            setBanConfirmDialogOpened(true);
        }

        return (
            <>
                <Tooltip title={canBan ? '' : 'Недостаточно прав для блокировки аккаунта'}
                         sx={{
                             width: '100%'
                         }}>
                    <div>
                        <Button
                            variant={'contained'}
                            color={'error'}
                            fullWidth
                            disabled={banning || banned || !canBan}
                            onClick={handleClick}>
                            Заблокировать аккаунт
                        </Button>
                    </div>
                </Tooltip>
                <BanConfirmDialog/>
            </>
        )

        function BanConfirmDialog() {
            return (
                <Dialog
                    open={banConfirmDialogOpened}
                    onClose={handleCloseBanConfirmDialog}
                    aria-labelledby="alert-dialog-title"
                    aria-describedby="alert-dialog-description"
                >
                    <DialogTitle id="alert-dialog-title">
                        Блокировка аккаунта
                    </DialogTitle>
                    <DialogContent>
                        <DialogContentText id="alert-dialog-description">
                            Если вы заблокируете свой аккаунт, разблокировка будет возможна только через администратора с более высоким приоритетом, чем у вас.
                        </DialogContentText>
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={() => handleCloseBanConfirmDialog('click close button')}>Отмена</Button>
                        <Button onClick={() => handleCloseBanConfirmDialog('click confirm button')} autoFocus>
                            Подтвердить
                        </Button>
                    </DialogActions>
                </Dialog>
            )
        }
    }

    function PasswordChanger() {
        type PasswordInputState = 'error' | 'default';

        const canChangePassword: boolean = roles.includes('admin.account.change-password.own');

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
                toast.error('Не верный текущий пароль.', {
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
                    <TextField variant={'standard'} label={'Текущий пароль'} inputRef={oldPasswordInputRef}
                               type={'password'}
                               error={oldPasswordInputState === 'error'}
                               disabled={changingPassword || !canChangePassword}
                               onInput={event => setOldPasswordInputState('default')}/>
                    <TextField variant={'standard'} label={'Новый пароль'} inputRef={newPasswordInputRef}
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