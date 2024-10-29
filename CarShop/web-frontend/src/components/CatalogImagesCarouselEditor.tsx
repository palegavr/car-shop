'use client'
import {ProcessData} from "@/types/types";
import styles from './styles/catalog-images-carousel-editor.module.css'
import {ChangeEvent, Dispatch, SetStateAction, useRef} from "react";
import {uploadImageAsync} from "@/clients/backendСlient";
import {backgroundImageStyle} from "@/utilities/backgroundImageStyle";

type Params = {
    imageUrls: string[],
    getSelectedImageIndex(): number | undefined
    onChange?(imageUrls: string[]): void,
    onReset?(): void
}

enum SideButton {
    Left,
    Right
}

export default function CatalogImagesCarouselEditor({imageUrls, onChange, getSelectedImageIndex, onReset}: Params) {
    const hiddenFileInputRef = useRef<HTMLInputElement>(null);

    // Срабатывает, когда пользователь выбрал (или не выбрал) картинки, которые нужно добавить
    async function handleFileInputChange(event: ChangeEvent<HTMLInputElement>) {
        if (!event.target.files) {
            return;
        }

        const files = [];
        for (let file of event.target.files) {
            files.push(file);
        }
        const result = await uploadImageAsync(files);
        if (result.success) {
            if (onChange) {
                onChange([...imageUrls, ...result.publicImageUrls!])
            }
        }
    }

    // При клике на "+"
    function handlePlusClick() {
        hiddenFileInputRef.current!.click();
    }

    // При клике на кнопку удаления
    function handleDeleteClick() {
        const selectedImageIndex = getSelectedImageIndex();
        if (onChange && selectedImageIndex !== undefined) {
            onChange(imageUrls.filter((_, index) => index !== selectedImageIndex));
        }
    }

    function handleSideButtonsClick(sideButton: SideButton) {
        const selectedImageIndex = getSelectedImageIndex()!;
        const newImageUrls = [...imageUrls];

        if (sideButton === SideButton.Left) {
            [newImageUrls[selectedImageIndex - 1], newImageUrls[selectedImageIndex]] =
                [newImageUrls[selectedImageIndex], newImageUrls[selectedImageIndex - 1]];
        } else if (sideButton === SideButton.Right) {
            [newImageUrls[selectedImageIndex], newImageUrls[selectedImageIndex + 1]] =
                [newImageUrls[selectedImageIndex + 1], newImageUrls[selectedImageIndex]];
        }

        if (onChange) {
            onChange(newImageUrls);
        }
    }

    function handleReset() {
        if (onReset) {
            onReset();
        }
    }

    return (
        <>
            <div className="bg-secondary p-1 d-inline-block rounded">
                <input type="button" // <
                       style={{
                           ...backgroundImageStyle('/images/keyboard-right-arrow-button-1_icon-icons.com_72690.svg'),
                           transform: 'rotate(180deg)'
                       }}
                       className={`btn btn-light me-1 ${styles.actionButton} ${getSelectedImageIndex() === undefined || getSelectedImageIndex() === 0 ? 'disabled' : ''}`}
                       onClick={() => handleSideButtonsClick(SideButton.Left)}/>
                <button // DELETE
                    className={`btn btn-danger me-1 ${styles.actionButton} ${styles.deleteActionButton} ${getSelectedImageIndex() === undefined ? 'disabled' : ''}`}
                    onClick={handleDeleteClick}></button>
                <button className={`btn btn-success me-1 ${styles.actionButton} ${styles.addActionButton}`} // PLUS
                        onClick={handlePlusClick}></button>
                <input type="button" // >
                       style={{
                           ...backgroundImageStyle('/images/keyboard-right-arrow-button-1_icon-icons.com_72690.svg'),
                       }}
                       className={`btn btn-light me-1 ${styles.actionButton} ${getSelectedImageIndex() === undefined || getSelectedImageIndex() === (imageUrls.length - 1) ? 'disabled' : ''}`}
                       onClick={() => handleSideButtonsClick(SideButton.Right)}/>
                <button className={`btn btn-danger ${styles.actionButton}`}
                        onClick={handleReset}
                        style={backgroundImageStyle('/images/reset_icon_246246.svg')}>
                </button>

                <input multiple type="file" ref={hiddenFileInputRef} onChange={handleFileInputChange}
                       className="d-none"/>
            </div>
        </>
    )
}
