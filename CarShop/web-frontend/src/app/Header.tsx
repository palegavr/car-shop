'use client'

import {Box, Button} from "@mui/material";
import LogoutButton from "@/components/LogoutButton";

export default function Header() {
    return (
        <Box
            sx={{
                padding: 1,
                display: 'flex',
                justifyContent: 'center',
                alignItems: 'center',
                gap: 1
            }}>
            <Button
                variant={'contained'}
                color={'success'}
                onClick={() => window.location.href = '/'}>
                На главную
            </Button>
            <LogoutButton/>
        </Box>
    )
}