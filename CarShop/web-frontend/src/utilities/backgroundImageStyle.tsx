import {CSSProperties} from "react";


export function backgroundImageStyle(imageUrl: string): CSSProperties {
    return {
        backgroundImage: `url('${imageUrl}')`,
        backgroundRepeat: 'no-repeat',
        backgroundPosition: 'center',
        backgroundSize: 'contain'
    }
}