import {ChangeEvent, useRef} from "react";
import {uploadImageAsync} from "@/clients/backendСlient";
import {backgroundImageStyle} from "@/utilities/backgroundImageStyle";

type Props = {
    imageUrl?: string,
    onChange?: (imageUrl?: string) => void,
    onReset?: () => void,
    onUploadFailed?: () => void,
}

export default function ImageChanger({imageUrl, onChange, onUploadFailed, onReset} : Props) {
    const hiddenFileInputRef = useRef<HTMLInputElement>(null);

    async function handleFileInputChange(event: ChangeEvent<HTMLInputElement>) {
        const fileSelected = event.currentTarget.files!.length > 0;
        if (fileSelected) {
            const file = event.currentTarget.files![0];
            const result = await uploadImageAsync([file]);
            if (result.success) {
                if (onChange) {
                    onChange(result.publicImageUrls![0])
                }
            } else {
                if (onUploadFailed) {
                    onUploadFailed();
                }
            }
        }
    }

    function handleClearImage() {
        if (onChange) {
            onChange(undefined);
        }
    }

    function handlePlusClick() {
        hiddenFileInputRef.current!.click();
    }

    function handleReset() {
        if (onReset) {
            onReset();
        }
    }

    return (
        <div className={'p-1 border rounded'}>
            <div className={'bg-secondary-subtle rounded'}>
                {imageUrl !== undefined ? (
                    <img src={imageUrl} className={'img-fluid rounded'} style={{maxHeight: '200px'}} alt="Картинка для /catalog"/>
                ) : (
                    <div className={'alert alert-info'}>Картинка не выбрана.</div>
                )}
            </div>

            <div className={'mt-2 bg-secondary d-inline-block p-1 rounded'}>
                <ToolBar/>
            </div>
        </div>
    )

    function ToolBar() {
        let buttonStyle = {width: '50px', height: '50px'}

        return (
            <div>
                <button className={'btn btn-danger me-1'} onClick={handleClearImage} // CLEAR
                    style={{...backgroundImageStyle('/images/dustbin_120823.svg'), ...buttonStyle}}></button>
                <button className={'btn btn-success me-1'} onClick={handlePlusClick} // PLUS
                        style={{...backgroundImageStyle('/images/Plus_icon-icons.com_71848.svg'), ...buttonStyle}}></button>
                <button className={'btn btn-danger'} onClick={handleReset} // RESET
                    style={{...backgroundImageStyle('/images/reset_icon_246246.svg'), ...buttonStyle}}></button>
                <input type="file" ref={hiddenFileInputRef} onChange={handleFileInputChange}
                       className="d-none"/>
            </div>
        )
    }
}