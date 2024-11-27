'use client'
import {DataGrid, GridColDef} from '@mui/x-data-grid';
import {Admin} from "@/clients/backendСlient";
import {GridEventListener} from "@mui/x-data-grid/models/events";
import {useEffect, useRef, useState} from "react";
import {Button} from "@mui/material";


type Props = {
    admins: Admin[],
    onColumnResize?: GridEventListener<'columnResize'>,
    onAdminSelect?(email: string): void
}

const paginationModel = {page: 0, pageSize: 100};

let onIdButtonClick: (email: string) => void = () => {};
const columns: GridColDef[] = [
    {field: 'id', headerName: 'ID', flex: 1,
    renderCell: params => (
        <Button
            variant={'outlined'}
            onClick={event => onIdButtonClick(params.row.email)}>
            {params.row.id}
        </Button>
    )},
    {field: 'email', headerName: 'Email', flex: 1},
    {field: 'priority', headerName: 'Приоритет', flex: 1},
    {field: 'banned', headerName: 'Заблокирован', flex: 1},
];

export default function AdminAccountsTable({admins, onColumnResize, onAdminSelect = () => {}}: Props) {
    const [currentPageInTable, setCurrentPageInTable] = useState<number>(0)
    const tableDivRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        onIdButtonClick = email => {
            onAdminSelect(email);
        }
        const interval = setInterval(() => {
            const labelRowsPerPage = tableDivRef.current!.querySelector('.MuiTablePagination-selectLabel');
            const labelDisplayedRows = tableDivRef.current!.querySelector('.MuiTablePagination-displayedRows');
            if (labelRowsPerPage && labelDisplayedRows) {
                labelRowsPerPage.textContent = 'Строк на странице';
                labelDisplayedRows.textContent = labelDisplayedRows.textContent!.replace('of', 'из')
                clearInterval(interval);
            }
        }, 50)
    }, [currentPageInTable]);

    return (
        <div ref={tableDivRef}>
            <DataGrid
                onColumnResize={onColumnResize}
                rows={admins}
                columns={columns}
                initialState={{pagination: {paginationModel}}}
                pageSizeOptions={[5, 10, 20, 50, 100]}
                rowSelection={false}
                onPaginationModelChange={model => {
                    if (currentPageInTable != model.page) {
                        setCurrentPageInTable(model.page);
                    }
                }}
                sx={{
                    border: 1,
                }}
            />
        </div>
    )
}