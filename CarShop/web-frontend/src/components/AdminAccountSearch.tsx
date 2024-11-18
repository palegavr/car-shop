import {Autocomplete, Box, TextField} from "@mui/material";

type Props = {
    adminEmails: string[],
    onSearch?(email: string): void,
}

export default function AdminAccountSearch({adminEmails, onSearch = () => {}}: Props) {

    function handleSearch(value: string) {
        onSearch(value);
    }

    return (
        <>
            <Box sx={{
                padding: 1,
                borderRadius: 2,
                backgroundColor: 'white'
            }}>
                <SearchInput/>
            </Box>
        </>
    )

    function SearchInput() {
        return (
            <Autocomplete
                disablePortal
                options={adminEmails}
                onInputChange={(event, value, reason) => {
                    if (reason !== 'input') {
                        handleSearch(value)
                    }
                }}
                renderInput={(params) => <TextField {...params} label="Email администратора"/>}
            />
        )
    }
}