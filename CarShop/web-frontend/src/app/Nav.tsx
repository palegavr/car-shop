'use client'
import {useEffect, useState} from "react";

export default function Nav() {
    const [navHtml, setNavHtml] = useState<string | null>(null);

    useEffect(() => {
        const navElement = (window as any).carShopData.navHtml;
        if (navElement) {
            setNavHtml(navElement.outerHTML);
        }
    }, []);

    return (
        <>
            {navHtml && <div dangerouslySetInnerHTML={{ __html: navHtml }} />}
        </>
    )
}
