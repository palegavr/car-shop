import type {Metadata} from "next";
import Nav from "@/app/Nav";

export const metadata: Metadata = {
    title: "Редактирование",
};

export default function RootLayout({
                                       children,
                                   }: Readonly<{
    children: React.ReactNode;
}>) {
    return (
        <html lang="en">
        <head>
            <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"/>
        </head>
        <body>
        <Nav/>
        {children}
        <script type={'text/javascript'} src={'https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.min.js'}/>
        <script type={'text/javascript'}
                src={'https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js'}/>
        </body>
        </html>
    );
}
