import React, {useState} from "react";
import {logoutAsync} from "@/clients/backendСlient";
import {Button} from "@mui/material";


export default function LogoutButton() {
    const [loggingOut, setLoggingOut] = useState<boolean>(false);

    async function handleClick() {
        setLoggingOut(true);
        await logoutAsync();
        window.location.href = '/';
    }

    return (
        <>
            <Button
                variant={'outlined'}
                disabled={loggingOut}
                onClick={handleClick}
                sx={{
                    backgroundColor: '#eee',
                    color: 'black',
                    borderColor: '#ccc'
                }}>
                Выход
            </Button>
        </>
    )
}