import {Autocomplete, Container, TextField} from "@mui/material";
import AdminAccountEditor, {PerformingAdmin} from "@/components/AdminAccountEditor";
import {useEffect, useState} from "react";
import {Admin} from "@/clients/backendСlient";
import {delay} from "@/utilities/delay";
import AdminAccountsTable from "@/components/AdminAccountsTable";

type Props = {
    admins: Admin[],
    performingAdmin: PerformingAdmin,
    defaultEmail?: string
}

export default function EditAdminPart({admins, performingAdmin, defaultEmail}: Props) {
    const [currentAccount, setCurrentAccount] = useState<Admin>();
    const [loaded, setLoaded] = useState<boolean>(false);
    const [adminEmailAutocompleteValue, setAdminEmailAutocompleteValue] = useState<string>('');

    useEffect(() => {
        if (defaultEmail !== null && defaultEmail !== undefined) {
            const admin = admins.find(admin => admin.email === defaultEmail)
            if (admin !== undefined) {
                setCurrentAccount(admin);
                setAdminEmailAutocompleteValue(admin.email);
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
        setAdminEmailAutocompleteValue(value ?? '');
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
                    value={adminEmailAutocompleteValue}
                    onInputChange={(event, value) => setAdminEmailAutocompleteValue(value)}
                    options={admins.map(admin => admin.email)}
                    sx={{
                        marginBottom: 2,
                    }}
                    onChange={(event, value, reason) => handleSearch(value)}
                    renderInput={(params) =>
                        <TextField {...params}
                                   value={adminEmailAutocompleteValue}
                                   label="Email администратора"/>}
                />
                {currentAccount !== undefined ? (
                    <AdminAccountEditor
                        admin={currentAccount}
                        performingAdmin={performingAdmin}/>
                ) : (
                    <AdminAccountsTable
                        admins={admins}
                        onAdminSelect={async email => {
                            await handleSearch(email);
                        }}/>
                )}
            </Container>
        </>
    )
}