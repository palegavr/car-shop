import {ChangeEvent, useRef, useState} from "react";
import {uploadImageAsync} from "@/clients/backendСlient";
import {backgroundImageStyle} from "@/utilities/backgroundImageStyle";

type Props = {
    imageUrl?: string,
    edited: boolean,
    onChange?: (imageUrl?: string) => Promise<void>,
    onReset?: () => Promise<void>,
    onUploadFailed?: () => void,
}

export default function ImageChanger({imageUrl, onChange, onUploadFailed, onReset, edited}: Props) {
    const hiddenFileInputRef = useRef<HTMLInputElement>(null);
    const [waitingAcceptChange, setWaitingAcceptChange] = useState<boolean>(false);

    async function handleFileInputChange(event: ChangeEvent<HTMLInputElement>) {
        const fileSelected = event.currentTarget.files!.length > 0;
        if (fileSelected) {
            const file = event.currentTarget.files![0];
            setWaitingAcceptChange(true);
            const result = await uploadImageAsync([file]);
            if (result.success) {
                if (onChange) {
                    await onChange(result.publicImageUrls![0])
                }
            } else {
                if (onUploadFailed) {
                    onUploadFailed();
                }
            }
            setWaitingAcceptChange(false);
        }
    }

    async function handleClearImage() {
        if (onChange) {
            setWaitingAcceptChange(true);
            await onChange(undefined);
            setWaitingAcceptChange(false);
        }
    }

    function handlePlusClick() {
        hiddenFileInputRef.current!.click();
    }

    async function handleReset() {
        if (onReset) {
            setWaitingAcceptChange(true);
            await onReset();
            setWaitingAcceptChange(false);
        }
    }

    return (
        <div className={'p-1 border rounded'}>
            <div className={'bg-secondary-subtle rounded'}>
                {imageUrl !== undefined ? (
                    <img src={imageUrl} className={'img-fluid rounded'} style={{maxHeight: '200px'}}
                         alt="Картинка для /catalog"/>
                ) : (
                    <div className={'alert alert-info'}>Картинка не выбрана.</div>
                )}
            </div>

            <div className={`${edited ? 'd-inline-block rounded bg-warning p-1' : ''} mt-1`}>
                <div className={'bg-secondary d-inline-block p-1 rounded'}>
                    <ToolBar/>
                </div>
            </div>
        </div>
    )

    function ToolBar() {
        let buttonStyle = {width: '50px', height: '50px'}

        return (
            <div>
                <button className={'btn btn-danger me-1'} onClick={handleClearImage} // CLEAR
                        style={{...backgroundImageStyle('/images/dustbin_120823.svg'), ...buttonStyle}}
                        disabled={waitingAcceptChange || imageUrl === undefined}></button>
                <button className={'btn btn-success me-1'} onClick={handlePlusClick} // PLUS
                        style={{...backgroundImageStyle('/images/Plus_icon-icons.com_71848.svg'), ...buttonStyle}}
                        disabled={waitingAcceptChange}></button>
                <button className={'btn btn-danger'} onClick={handleReset} // RESET
                        style={{...backgroundImageStyle('/images/reset_icon_246246.svg'), ...buttonStyle}}
                        disabled={waitingAcceptChange}></button>
                <input type="file" ref={hiddenFileInputRef} onChange={handleFileInputChange}
                       className="d-none"/>
            </div>
        )
    }
}