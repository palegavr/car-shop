'use client'
import '@fontsource/roboto/300.css';
import '@fontsource/roboto/400.css';
import '@fontsource/roboto/500.css';
import '@fontsource/roboto/700.css';

import React, {useEffect, useState} from "react";
import {Container, Tab} from "@mui/material";
import EditAdminPart from "@/app/admin/parts/EditAdminPart";

import 'react-toastify/dist/ReactToastify.css';
import {Admin} from "@/clients/backendСlient";
import AddCarPart from "@/app/admin/parts/AddCarPart";
import CreateAccountPart from "@/app/admin/parts/CreateAccountPart";
import {ToastContainer} from "react-toastify";
import {PerformingAdmin} from "@/components/AdminAccountEditor";
import {TabContext, TabList, TabPanel} from "@mui/lab";
import ProfilePart from "@/app/admin/parts/ProfilePart";

type PageInMainPart = 'create account' | 'edit admin' | 'add car' | 'profile';
/*(window as any).carShopData = {};
(window as any).carShopData.admins = [{
    id: 5,
    email: 'abc@ggg.com',
    priority: 20000,
    banned: false,
    roles: ['admin.account.create', 'admin.car.add', 'admin.account.ban.own', 'admin.account.change-password.own'],
}];
(window as any).carShopData.performingAdmin = {
    id: 1,
    email: 'ggg@123.com',
    roles: ['admin.account.create', 'admin.car.add', 'admin.account.ban.own', 'admin.account.change-password.own'],
    priority: 1000,
};*/

export default function Page() {
    const [adminsList, setAdminsList] = useState<Admin[]>([]);
    const [performingAdmin, setPerformingAdmin] = useState<PerformingAdmin>()
    const [adminEmail, setAdminEmail] = useState<string | null>(null);
    const [loaded, setLoaded] = useState<boolean>(false);
    const [tabsValue, setTabsValue] = useState<PageInMainPart>('profile');

    useEffect(() => {
        setAdminsList((window as any).carShopData.admins);
        setPerformingAdmin((window as any).carShopData.performingAdmin);
        setAdminEmail((window as any).carShopData.adminEmail);
        if ((window as any).carShopData.adminEmail) {
            setTabsValue('edit admin');
        }
        setLoaded(true);
    }, []);

    if (!loaded) {
        return <div>Loading...</div>
    }

    function handleChangePageInMainPart(page: PageInMainPart) {
        if (adminEmail) {
            setAdminEmail(null);
        }
        setTabsValue(page);
    }

    return (
        <>
            <TabContext value={tabsValue}>
                <Container
                    sx={{
                        width: 'fit-content'
                    }}>
                    <TabList
                        onChange={(event, value) => {
                            handleChangePageInMainPart(value);
                        }}
                        variant={'scrollable'}
                        scrollButtons={'auto'}>
                        <Tab label="Добавить товар" value="add car"
                             disabled={!performingAdmin!.roles.includes('admin.car.add')}/>
                        <Tab label="Управление аккаунтом администратора" value="edit admin"/>
                        <Tab label="Создать аккаунт" value="create account"
                             disabled={!performingAdmin!.roles.includes('admin.account.create')}/>
                        <Tab label={'Профиль'} value={'profile'}/>
                    </TabList>
                </Container>
                <TabPanel value="add car">
                    <AddCarPart/>
                </TabPanel>
                <TabPanel value="edit admin">
                    <>
                        <EditAdminPart
                            defaultEmail={adminEmail ?? undefined}
                            admins={adminsList}
                            performingAdmin={performingAdmin ?? {
                                id: 0,
                                email: '',
                                roles: [],
                                priority: 2000000000,
                            }}/>
                    </>
                </TabPanel>
                <TabPanel value="create account">
                    <CreateAccountPart/>
                </TabPanel>
                <TabPanel value="profile">
                    <ProfilePart
                        id={performingAdmin!.id}
                        roles={performingAdmin!.roles}
                        priority={performingAdmin!.priority}
                        email={performingAdmin!.email}/>
                </TabPanel>
            </TabContext>
            <ToastContainer/>
        </>
    )
}