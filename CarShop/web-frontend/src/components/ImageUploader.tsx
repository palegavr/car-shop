'use client'
import {useEffect, useRef, useState} from "react";
import {IconButton, Paper, Stack, Tooltip, Typography} from "@mui/material";
import FileUploadIcon from '@mui/icons-material/FileUpload';
import ClearIcon from '@mui/icons-material/Clear';
import {uploadImageAsync} from "@/clients/backendСlient";
import {Swiper, SwiperRef, SwiperSlide} from 'swiper/react';

import 'swiper/css';
import 'swiper/css/navigation';
import 'swiper/css/pagination';
import {Keyboard, Mousewheel, Navigation, Pagination} from "swiper/modules";
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import ArrowForwardIcon from '@mui/icons-material/ArrowForward';

type ImageUploaderVariant = 'single' | 'multiple';
type Arrow = 'left' | 'right';

type Props = {
    variant?: ImageUploaderVariant,
    defaultImageUrls?: string[],
    imageMaxHeight?: string,
    allowedImageExtensions?: string[],
    onUploadStart?(file: File): void,
    onUploadEnd?(file: File, success: boolean, imageUrl?: string): void,
    onChange?(imageUrls: string[]): void,
}

export default function ImageUploader({
                                          variant = 'single',
                                          defaultImageUrls = [],
                                          imageMaxHeight = '300px',
                                          allowedImageExtensions = [],
                                          onUploadStart = () => {
                                          },
                                          onUploadEnd = () => {
                                          },
                                          onChange = () => {
                                          },
                                      }: Props) {
    const [imageUrls, setImageUrls] = useState<string[]>(defaultImageUrls);
    const [uploading, setUploading] = useState<boolean>(false);
    const [selectedIndex, setSelectedIndex] = useState<number | null>(defaultImageUrls.length > 0 ? 0 : null);

    const uploadInputRef = useRef<HTMLInputElement>(null);
    const swiperRef = useRef<SwiperRef>(null);
    const hiddenFormRef = useRef<HTMLFormElement>(null);

    if (variant === 'single' && imageUrls.length > 1) {
        throw new Error('В режиме single не может быть несколько картинок.');
    }

    useEffect(() => {
        if (selectedIndex !== null && variant === 'multiple') {
            swiperRef.current!.swiper.slideTo(selectedIndex);
        }
    }, [selectedIndex]);

    function handleChangeSelectedIndex(newSelectedIndex: number | null) {
        setSelectedIndex(newSelectedIndex);
    }

    async function handleUpload(files: File[]) {
        if (files.length === 0) {
            return;
        }
        const newImageUrls = [...imageUrls];
        setUploading(true);
        for (const file of files) {
            onUploadStart(file);
            const result = await uploadImageAsync([file]);
            if (result.success &&
                (result.publicImageUrls?.length ?? 0) > 0) {
                if (variant === 'single') {
                    newImageUrls.pop();
                }
                newImageUrls.push(result.publicImageUrls![0]);
                setImageUrls(newImageUrls);
                const newSelectedIndex = newImageUrls.length - 1;
                handleChangeSelectedIndex(newSelectedIndex);
                onUploadEnd(file, true, newImageUrls[newSelectedIndex]);
            } else {
                onUploadEnd(file, false);
            }
        }
        setUploading(false);
        onChange(newImageUrls);
    }

    function handleClear() {
        if (selectedIndex !== null) {
            const newImageUrls = [...imageUrls]
                .filter((_, index) => index !== selectedIndex);
            setImageUrls(newImageUrls);
            if (newImageUrls.length === 0) {
                handleChangeSelectedIndex(null);
            } else if (selectedIndex > newImageUrls.length - 1) {
                handleChangeSelectedIndex(newImageUrls.length - 1);
            } else if (selectedIndex < 0) {
                handleChangeSelectedIndex(0);
            }
            onChange(newImageUrls);
        } else {
            throw new Error();
        }
    }

    function handleArrowClick(arrow: Arrow) {
        const newImageUrls = [...imageUrls];
        if (arrow === 'left') {
            [newImageUrls[selectedIndex!], newImageUrls[selectedIndex! - 1]]
                = [newImageUrls[selectedIndex! - 1], newImageUrls[selectedIndex!]];
        } else if (arrow === 'right') {
            [newImageUrls[selectedIndex!], newImageUrls[selectedIndex! + 1]]
                = [newImageUrls[selectedIndex! + 1], newImageUrls[selectedIndex!]];
        }
        setImageUrls(newImageUrls);
        handleChangeSelectedIndex(selectedIndex! + (arrow === 'left' ? -1 : 1));
        onChange(newImageUrls);
    }

    return (
        <>
            <Stack>
                <Paper
                    variant={'outlined'}
                    sx={{
                        maxHeight: imageMaxHeight,
                        textAlign: 'center',
                        pointerEvents: uploading ? 'none' : undefined,
                        opacity: uploading ? '0.7' : undefined,
                    }}>
                    {variant === 'single' && (
                        <>
                            {imageUrls.length > 0 ? (
                                <img
                                    src={imageUrls[0]}
                                    style={{
                                        maxHeight: imageMaxHeight,
                                        maxWidth: '100%',
                                        objectFit: 'contain'
                                    }}
                                    alt="Image"/>
                            ) : (
                                <Typography align={'center'}>Картинка отсутствует.</Typography>
                            )}
                        </>
                    )}
                    {variant === 'multiple' && (
                        <>
                            {imageUrls.length > 0 ? (
                                <Swiper
                                    ref={swiperRef}
                                    allowTouchMove={true}
                                    style={{
                                        height: imageMaxHeight
                                    }}
                                    navigation={true}
                                    pagination={{
                                        clickable: true
                                    }}
                                    onSlideChange={swiper => handleChangeSelectedIndex(swiper.activeIndex)}
                                    speed={200}
                                    mousewheel={true}
                                    keyboard={true}
                                    modules={[Navigation, Pagination, Mousewheel, Keyboard]}>
                                    {imageUrls.map((imageUrl, index) => (
                                        <SwiperSlide
                                            key={index}
                                            style={{
                                                textAlign: 'center',
                                                alignContent: 'center',
                                            }}>

                                            <img
                                                src={imageUrl}
                                                style={{
                                                    maxHeight: '100%',
                                                    maxWidth: '100%',
                                                    objectFit: 'contain'
                                                }}
                                                alt={`Image ${index}`}/>
                                        </SwiperSlide>
                                    ))}
                                </Swiper>
                            ) : (
                                <Typography align={'center'}>Картинки отсутствуют.</Typography>
                            )}
                        </>
                    )}
                </Paper>
                <Stack
                    direction={'row'}
                    spacing={2}
                    boxSizing={'border-box'}
                    justifyContent="center"
                    alignItems="center">
                    {variant === 'multiple' && (
                        <>
                            <Tooltip title={'Переместить на одну позицию влево'}>
                                <span>
                                    <IconButton
                                        disabled={selectedIndex === 0 || selectedIndex === null || uploading}
                                        onClick={() => handleArrowClick('left')}>
                                        <ArrowBackIcon/>
                                    </IconButton>
                                </span>
                            </Tooltip>
                            <Tooltip title={'Переместить на одну позицию вправо'}>
                                <span>
                                    <IconButton
                                        disabled={selectedIndex === imageUrls.length - 1 || selectedIndex === null || uploading}
                                        onClick={() => handleArrowClick('right')}>
                                        <ArrowForwardIcon/>
                                    </IconButton>
                                </span>
                            </Tooltip>
                        </>
                    )}
                    <Tooltip title={'Загрузить картинку'}>
                        <span>
                            <IconButton
                                color={'success'}
                                disabled={uploading}
                                onClick={event => uploadInputRef.current!.click()}>
                                <FileUploadIcon/>
                                <form
                                    ref={hiddenFormRef}
                                    style={{
                                        display: 'none'
                                    }}>

                                    <input
                                        type={'file'}
                                        ref={uploadInputRef}
                                        multiple={variant === 'multiple'}
                                        accept={allowedImageExtensions.length > 0
                                            ? allowedImageExtensions.map(ext => `.${ext}`).join(', ')
                                            : undefined
                                        }
                                        onChange={async event => {
                                            await handleUpload([...event.currentTarget.files!]);
                                            hiddenFormRef.current!.reset();
                                        }}/>
                                </form>
                            </IconButton>
                        </span>
                    </Tooltip>
                    <Tooltip title={'Удалить картинку'}>
                        <span>
                            <IconButton
                                color={'error'}
                                disabled={selectedIndex === null || uploading}
                                onClick={handleClear}>
                                <ClearIcon/>
                            </IconButton>
                        </span>
                    </Tooltip>
                </Stack>
            </Stack>
        </>
    )
}