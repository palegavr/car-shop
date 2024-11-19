import {Autocomplete, Container, TextField} from "@mui/material";
import AdminAccountEditor, {PerformingAdmin} from "@/components/AdminAccountEditor";
import {useEffect, useState} from "react";
import {Admin} from "@/clients/backendСlient";
import {delay} from "@/utilities/delay";

type Props = {
    admins: Admin[],
    performingAdmin: PerformingAdmin,
    defaultEmail?: string
}

export default function EditAdminPart({admins, performingAdmin, defaultEmail}: Props) {
    const [currentAccount, setCurrentAccount] = useState<Admin>();
    const [loaded, setLoaded] = useState<boolean>(false);

    useEffect(() => {
        if (defaultEmail !== null && defaultEmail !== undefined) {
            const admin = admins.find(admin => admin.email === defaultEmail)
            if (admin !== undefined) {
                setCurrentAccount(admin);
            }
        }
        setLoaded(true);
    }, []);

    async function handleSearch(value: string | null) {
        setCurrentAccount(undefined);
        await delay(100);
        setCurrentAccount(value !== null
            ? admins.find(admin => admin.email === value)
            : undefined)
    }

    if (!loaded) {
        return <div></div>
    }

    return (
        <>
            <Container
                maxWidth={'sm'}>
                <Autocomplete
                    disablePortal
                    defaultValue={currentAccount?.email}
                    options={admins.map(admin => admin.email)}
                    sx={{
                        marginBottom: 2
                    }}
                    onChange={(event, value, reason) => handleSearch(value)}
                    renderInput={(params) =>
                        <TextField {...params}
                                   label="Email администратора"/>}
                />
                {currentAccount !== undefined && (
                    <AdminAccountEditor
                        admin={currentAccount}
                        performingAdmin={performingAdmin}/>
                )}
            </Container>
        </>
    )
}