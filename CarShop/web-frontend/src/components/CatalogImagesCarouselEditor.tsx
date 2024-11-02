'use client'
import styles from './styles/catalog-images-carousel-editor.module.css'
import {ChangeEvent, useEffect, useRef, useState} from "react";
import {uploadImageAsync} from "@/clients/backendСlient";
import {backgroundImageStyle} from "@/utilities/backgroundImageStyle";
import CatalogImagesCarousel from "@/components/CatalogImagesCarousel";

type Params = {
    imageUrls: string[],
    edited: boolean
    onChange?(imageUrls: string[]): Promise<void>,
    onReset?(): Promise<void>,
    onUploadFailed?: () => void,
}

enum SideButton {
    Left,
    Right
}

export default function CatalogImagesCarouselEditor({imageUrls, onChange, onReset, edited, onUploadFailed}: Params) {
    const hiddenFileInputRef = useRef<HTMLInputElement>(null);
    const [selectedImageIndex, setSelectedImageIndex] = useState<number>();
    const [waitingAcceptChange, setWaitingAcceptChange] = useState<boolean>(false);


    useEffect(() => {
        if (imageUrls.length === 0) {
            handleSelectedImageChange(undefined);
        } else if (selectedImageIndex !== undefined) {
            if (selectedImageIndex >= imageUrls.length) {
                handleSelectedImageChange(imageUrls.length - 1);
            } else if (selectedImageIndex < 0) {
                handleSelectedImageChange(0);
            }
        } else if (imageUrls.length > 0) {
            handleSelectedImageChange(0);
        }
    }, [imageUrls]);

    // Срабатывает, когда пользователь выбрал (или не выбрал) картинки, которые нужно добавить
    async function handleFileInputChange(event: ChangeEvent<HTMLInputElement>) {
        if (!event.target.files) {
            return;
        }

        const files = [];
        for (let file of event.target.files) {
            files.push(file);
        }
        setWaitingAcceptChange(true);
        const result = await uploadImageAsync(files);
        if (result.success) {
            if (onChange) {
                await onChange([...imageUrls, ...result.publicImageUrls!]);
            }
        } else {
            if (onUploadFailed) {
                onUploadFailed();
            }
        }
        setWaitingAcceptChange(false);
    }

    // При клике на "+"
    function handlePlusClick() {
        hiddenFileInputRef.current!.click();
    }

    // При клике на кнопку удаления
    async function handleDeleteClick() {
        if (onChange && selectedImageIndex !== undefined) {
            setWaitingAcceptChange(true);
            await onChange(imageUrls.filter((_, index) => index !== selectedImageIndex));
            setWaitingAcceptChange(false);
        }
    }

    async function handleSideButtonsClick(sideButton: SideButton) {
        const newImageUrls = [...imageUrls];

        let newSelectedImageIndex;
        if (sideButton === SideButton.Left) {
            [newImageUrls[selectedImageIndex! - 1], newImageUrls[selectedImageIndex!]] =
                [newImageUrls[selectedImageIndex!], newImageUrls[selectedImageIndex! - 1]];
            newSelectedImageIndex = selectedImageIndex! - 1;
        } else if (sideButton === SideButton.Right) {
            [newImageUrls[selectedImageIndex!], newImageUrls[selectedImageIndex! + 1]] =
                [newImageUrls[selectedImageIndex! + 1], newImageUrls[selectedImageIndex!]];
            newSelectedImageIndex = selectedImageIndex! + 1;
        }

        if (onChange) {
            setWaitingAcceptChange(true);
            await onChange(newImageUrls);
            setWaitingAcceptChange(false);
            setSelectedImageIndex(newSelectedImageIndex);
        }
    }

    async function handleReset() {
        if (onReset) {
            setWaitingAcceptChange(true);
            await onReset();
            setWaitingAcceptChange(false);
        }
    }

    function handleSelectedImageChange(index: number | undefined) {
        setSelectedImageIndex(index);
    }

    return (
        <>
            {imageUrls.length > 0 ? (
                <div className={'mb-1'}>
                    <CatalogImagesCarousel
                        imageUrls={imageUrls}
                        selectedImageIndex={selectedImageIndex!}
                        onSelectedImageChange={handleSelectedImageChange}/>
                </div>
            ) : (
                <div className="alert alert-info">Картинок пока нет.</div>
            )}
            <div className={edited ? 'd-inline-block rounded bg-warning p-1' : ''}>
                <div className={`bg-secondary p-1 d-inline-block rounded`}>
                    <input type="button" // <
                           style={{
                               ...backgroundImageStyle('/images/keyboard-right-arrow-button-1_icon-icons.com_72690.svg'),
                               transform: 'rotate(180deg)'
                           }}
                           className={`btn btn-light me-1 ${styles.actionButton}`}
                           disabled={selectedImageIndex === undefined || selectedImageIndex === 0 || waitingAcceptChange}
                           onClick={() => handleSideButtonsClick(SideButton.Left)}/>
                    <button className={`btn btn-success me-1 ${styles.actionButton} ${styles.addActionButton}`} // PLUS
                            disabled={waitingAcceptChange}
                            onClick={handlePlusClick}></button>
                    <input type="button" // >
                           style={{
                               ...backgroundImageStyle('/images/keyboard-right-arrow-button-1_icon-icons.com_72690.svg'),
                           }}
                           className={`btn btn-light me-1 ${styles.actionButton}`}
                           disabled={selectedImageIndex === undefined || selectedImageIndex === (imageUrls.length - 1) || waitingAcceptChange}
                           onClick={() => handleSideButtonsClick(SideButton.Right)}/>
                    <button className={`btn btn-danger ${styles.actionButton}`} // RESET
                            onClick={handleReset}
                            disabled={waitingAcceptChange}
                            style={backgroundImageStyle('/images/reset_icon_246246.svg')}>
                    </button>
                    <button // DELETE
                        className={`btn btn-danger ms-1 ${styles.actionButton} ${styles.deleteActionButton}`}
                        disabled={selectedImageIndex === undefined || waitingAcceptChange}
                        onClick={handleDeleteClick}></button>

                    <input multiple type="file" ref={hiddenFileInputRef} onChange={handleFileInputChange}
                           className="d-none"/>
                </div>
            </div>
        </>
    )
}
