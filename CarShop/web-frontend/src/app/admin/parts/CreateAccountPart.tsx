import {Button, Container, IconButton, Paper, Stack, TextField, Typography} from "@mui/material";
import {useRef, useState} from "react";
import {createAccountAsync} from "@/clients/backendСlient";
import {LoadingButton} from "@mui/lab";
import ContentCopyIcon from '@mui/icons-material/ContentCopy';
import {toast} from "react-toastify";
import {CopyResult, copyToClipboard} from "@/utilities/copyToClipboard";

const defaultAutoCloseTime = 2000;
const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

export default function CreateAccountPart() {
    const [createdAccountPassword, setCreatedAccountPassword] = useState<string>();
    const [creatingAccount, setCreatingAccount] = useState<boolean>(false);
    const [emailInInputIsCorrect, setEmailInInputIsCorrect] = useState<boolean>(false);
    const [accountCreated, setAccountCreated] = useState<boolean>(false);

    const emailInputRef = useRef<HTMLInputElement>(null);

    async function handleCreateAccount(email: string) {
        if (email.trim() !== '') {
            const response = await createAccountAsync(email);
            if (response.success) {
                setCreatedAccountPassword(response.password);
                setAccountCreated(true);
            } else if (response.emailAlreadyExists) {
                toast.error('Эта почта уже занята.', {
                    position: 'bottom-right',
                    autoClose: defaultAutoCloseTime
                })
            } else {
                toast.error('Ошибка.', {
                    position: 'bottom-right',
                    autoClose: defaultAutoCloseTime
                })
            }
        }
    }

    return (
        <>
            <Container
                maxWidth={'sm'}>
                <Stack spacing={2}>
                    <TextField
                        inputRef={emailInputRef}
                        disabled={accountCreated || creatingAccount}
                        variant={'standard'}
                        label={'Email'}
                        onChange={event =>
                            setEmailInInputIsCorrect(emailRegex.test(event.currentTarget.value))}/>
                    {creatingAccount ? (
                        <LoadingButton loading variant={'contained'}>Loading...</LoadingButton>
                    ) : (
                        <>
                            <Button
                                variant={'contained'}
                                disabled={!emailInInputIsCorrect || accountCreated}
                                onClick={async () => {
                                    setCreatingAccount(true);
                                    await handleCreateAccount(emailInputRef.current!.value);
                                    setCreatingAccount(false);
                                }}>
                                Создать аккаунт
                            </Button>
                            {createdAccountPassword && (
                                <Paper
                                    sx={{
                                        padding: 2,
                                        backgroundColor: '#eee',
                                        textAlign: 'center'
                                    }}
                                    variant={'outlined'}>
                                    <Typography>
                                        Аккаунт успешно создан, пароль:&nbsp;
                                        <Typography
                                            component={'span'}
                                            sx={{
                                                background: '#afa',
                                                border: '2px green solid',
                                                borderRadius: 1
                                            }}>{createdAccountPassword}</Typography>
                                        <IconButton size="small"
                                                    onClick={async event => {
                                                        const result = await copyToClipboard(createdAccountPassword);
                                                        if (result == CopyResult.Success) {
                                                            toast.success('Пароль скопирован в буфер обмена.', {
                                                                position: 'bottom-right',
                                                                autoClose: defaultAutoCloseTime
                                                            });
                                                        } else {
                                                            toast.error('При копировании произошла ошибка, скопируйте вручную.', {
                                                                position: 'bottom-right',
                                                                autoClose: defaultAutoCloseTime
                                                            });
                                                        }
                                                    }}>
                                            <ContentCopyIcon fontSize="inherit"/>
                                        </IconButton>
                                    </Typography>
                                    <Typography>
                                        Сохраните данный пароль, он
                                        <Typography color={'error'} component={'span'}> НЕ </Typography>
                                        будет отображаться в админ-панели.
                                    </Typography>
                                    <Button
                                        variant={'outlined'}
                                        fullWidth
                                        sx={{
                                            marginTop: 1
                                        }}
                                        onClick={event => {
                                            const url = new URL(window.location.href);
                                            url.searchParams.set('email', emailInputRef.current!.value);
                                            window.location.href = url.toString();
                                        }}>
                                        Настроить аккаунт
                                    </Button>
                                </Paper>
                            )}
                        </>
                    )}
                </Stack>
            </Container>
        </>
    )
}