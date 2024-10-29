'use client'
import {MouseEventHandler, useEffect, useState} from "react";
import {AdditionalCarOption, AdditionalCarOptionType, FuelType, ProcessData} from "@/types/types";
import CatalogImagesCarousel from "@/components/CatalogImagesCarousel";
import CatalogImagesCarouselEditor from "@/components/CatalogImagesCarouselEditor";
import {parseJsonEncodedProcessData} from "@/utilities/processDataOperations";
import Editable, {EditableSupportedTypes} from "@/components/Editable";
import {applyChangesAsync, pushProcessData} from "@/clients/backendСlient";
import AdditionalCarOptionsContainer from "@/components/AdditionalCarOptionsContainer";
import {EDITED_CLASS} from "@/constants";
import {deepEqual} from "assert";
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import ImageChanger from "@/components/ImageChanger";


type DataFromBackend = {
    processDataInDbJsonEncoded: string,
    currentProcessDataJsonEncoded: string,
    carId: number
}

const defaultToastTime = 2000;

export default function Page() {

    const [dataFromBackendLoaded, setDataFromBackendLoaded] = useState<boolean>();
    const [processDataInDb, setProcessDataInDb] = useState<ProcessData>(null!);
    const [currentProcessData, setCurrentProcessData] = useState<ProcessData>(null!);
    const [processData, setProcessData] = useState<ProcessData>(null!);
    const [carId, setCarId] = useState(null!);
    const [carouselSelectedImageIndex, setCarouselSelectedImageIndex] = useState<number>();
    const [alertMessage, setAlertMessage] = useState<string>();

    useEffect(() => {
        // @ts-ignore
        window.carShopData = {
            processDataInDbJsonEncoded: `{
                "brand": "ccscs",
                "model": "ggg",
                "price": 222,
                "color": "string",
                "engine_capacity": 1,
                "corpus_type": 0,
                "fuel_type": 3,
                "count": 123,
                "image_url": "https://img2.akspic.ru/previews/2/8/1/8/7/178182/178182-legkovyye_avtomobili-sportkar-lambordzhini-superkar-lamborgini_aventador-x750.jpg",
                "big_image_urls": ["hello", "world"],
                "additional_car_options": "[{\\"type\\":0,\\"price\\":888,\\"isRequired\\":true},{\\"type\\":2,\\"price\\":1999,\\"isRequired\\":false}]"
            }`,
            currentProcessDataJsonEncoded: `{
                "brand": "ccscs",
                "model": "ggg",
                "price": 222,
                "color": "string",
                "engine_capacity": 1,
                "corpus_type": 0,
                "fuel_type": 3,
                "count": 124,
                "image_url": "https://img2.akspic.ru/previews/2/8/1/8/7/178182/178182-legkovyye_avtomobili-sportkar-lambordzhini-superkar-lamborgini_aventador-x750.jpg",
                "big_image_urls": 
                ["https://img3.akspic.ru/previews/1/7/8/7/7/177871/177871-zelenyj_lamborgini-lamborghini_gallardo-lambordzhini-lamborghini_urakan-legkovyye_avtomobili-x750.jpg", 
                "https://img2.akspic.ru/previews/2/8/1/8/7/178182/178182-legkovyye_avtomobili-sportkar-lambordzhini-superkar-lamborgini_aventador-x750.jpg",
                 "https://img1.akspic.ru/previews/3/8/0/8/7/178083/178083-gonochnyj_avtomobil-legkovyye_avtomobili-avtomobilnoe_osveshhenie-koleso-purpurnyj_cvet-x750.jpg"],
                "additional_car_options": "[{\\"type\\":0,\\"price\\":888,\\"isRequired\\":true},{\\"type\\":2,\\"price\\":1999,\\"isRequired\\":false}]"
            }`,
            carId: 67
        };

        try {
            // @ts-ignore
            let processDataInDb = parseJsonEncodedProcessData(window['carShopData'].processDataInDbJsonEncoded);
            // @ts-ignore
            let currentProcessData = parseJsonEncodedProcessData(window['carShopData'].currentProcessDataJsonEncoded);
            // @ts-ignore
            let carId = JSON.parse(window['carShopData'].carId);

            setProcessData(currentProcessData);
            setProcessDataInDb(processDataInDb);
            setCurrentProcessData(currentProcessData);
            setCarId(carId);
            setDataFromBackendLoaded(true);

            if (currentProcessData.bigImageUrls.length > 0)
                setCarouselSelectedImageIndex(0);
        } catch (e) {
            setDataFromBackendLoaded(false);
        }
    }, [])

    async function handleChangeSomething(progressDataPropName: keyof ProcessData, newValue: any) {
        const newProcessData = {...processData, [progressDataPropName]: newValue} as ProcessData;
        if (progressDataPropName === 'bigImageUrls') {
            if (newProcessData.bigImageUrls.length === 0)
                setCarouselSelectedImageIndex(undefined);

            if (processData.bigImageUrls.length === 0 && newProcessData.bigImageUrls.length > 0)
                setCarouselSelectedImageIndex(0);
        }

        const result = await pushProcessData(carId,
            newProcessData.fuelType === FuelType.Electric ? {...newProcessData, engineCapacity: 0} : newProcessData);
        if (result.success) {
            setCurrentProcessData(newProcessData);
        } else {
            toast.error('Не удалось синхронизировать изменения.', {
                position: 'bottom-right',
                autoClose: defaultToastTime
            });
        }
        setProcessData(newProcessData);
        return result;
    }

    async function handleSaveChangesButton() {
        const pushResult = await handleChangeSomething('brand', processData.brand);
        if (!pushResult.success) {
            return;
        }

        const applyResult = await applyChangesAsync(carId);
        if (applyResult.success) {
            setProcessDataInDb(processData);
            toast.success('Изменения сохранены!', {
                position: 'bottom-right',
                autoClose: defaultToastTime
            });
        } else {
            toast.error('Не удалось сохранить изменения.', {
                position: 'bottom-right',
                autoClose: defaultToastTime
            })
        }
    }

    function handleReset({progressDataPropName, optionType} : {progressDataPropName: keyof ProcessData, optionType?: AdditionalCarOptionType})
    : EditableSupportedTypes | AdditionalCarOption[] {
        if (progressDataPropName === 'additionalCarOptions') {
            let newAdditionalCarOptions = processData.additionalCarOptions;
            newAdditionalCarOptions = newAdditionalCarOptions
                .map(option => option.type === optionType ?
                    processDataInDb.additionalCarOptions.find(option => option.type === optionType) : option)
                .filter(v => v !== undefined);


            handleChangeSomething(progressDataPropName, newAdditionalCarOptions);
            return newAdditionalCarOptions;
        } else {
            handleChangeSomething(progressDataPropName, processDataInDb[progressDataPropName as keyof ProcessData]);
            return processDataInDb[progressDataPropName as keyof ProcessData] as EditableSupportedTypes;
        }
    }

    if (dataFromBackendLoaded === undefined) {
        return <div
            className={'alert alert-warning position-absolute top-50 start-50 translate-middle'}>Загрузка...</div>
    }

    if (!dataFromBackendLoaded) {
        return <div className={'alert alert-danger position-absolute top-50 start-50 translate-middle'}>Не удалось
            загрузить данные.</div>
    }

    // @ts-ignore
    return (
        <>
            <div className={'text-center mb-1'}>
                <h5>Картинка для /catalog</h5>
                <ImageChanger
                    imageUrl={processData.imageUrl}
                    onReset={() => handleReset({progressDataPropName: 'imageUrl'})}
                    onChange={imageUrl => handleChangeSomething('imageUrl', imageUrl !== undefined ? imageUrl : '')}/>
            </div>

            {processData.bigImageUrls.length > 0 ? (
                <CatalogImagesCarousel
                    imageUrls={processData.bigImageUrls}
                    onSelectedImageChange={setCarouselSelectedImageIndex}/>
            ) : (
                <div className="text-center mt-1">
                    <div className="alert alert-info d-inline-block">Картинок пока нет.</div>
                </div>
            )}
            <div className="text-center mt-2">
                <CatalogImagesCarouselEditor
                    imageUrls={processData.bigImageUrls}
                    getSelectedImageIndex={() => carouselSelectedImageIndex}
                    onReset={() => handleReset({progressDataPropName: 'bigImageUrls'})}
                    onChange={(imageUrls) => handleChangeSomething('bigImageUrls', imageUrls)}/>
            </div>

            <table className="table mt-3" style={{fontSize: '6mm'}}>
                <colgroup>
                    <col className="w-50"/>
                    <col className="w-50"/>
                </colgroup>
                <tbody>
                <tr>
                    <td>Марка</td>
                    <td className={processData.brand !== processDataInDb.brand ? EDITED_CLASS : ''}>
                        <Editable initialValue={processData.brand} type={'string'}
                                  edited={processData.brand !== processDataInDb.brand}
                                  onReset={() => handleReset({progressDataPropName: 'brand'}) as EditableSupportedTypes}
                                  onChange={(newValue) => handleChangeSomething('brand', newValue)}/></td>
                </tr>
                <tr>
                    <td>Модель</td>
                    <td className={processData.model !== processDataInDb.model ? EDITED_CLASS : ''}>
                        <Editable initialValue={processData.model} type={'string'}
                                  edited={processData.model !== processDataInDb.model}
                                  onReset={() => handleReset({progressDataPropName: 'model'}) as EditableSupportedTypes}
                                  onChange={(newValue) => handleChangeSomething('model', newValue)}/></td>
                </tr>
                <tr>
                    <td>Цвет</td>
                    <td className={processData.color !== processDataInDb.color ? EDITED_CLASS : ''}>
                        <Editable initialValue={processData.color} type={'string'}
                                  edited={processData.color !== processDataInDb.color}
                                  onReset={() => handleReset({progressDataPropName: 'color'}) as EditableSupportedTypes}
                                  onChange={(newValue) => handleChangeSomething('color', newValue)}/></td>
                </tr>
                {processData.fuelType !== FuelType.Electric && (
                    <tr>
                        <td>Объём двигателя</td>
                        <td className={processData.engineCapacity !== processDataInDb.engineCapacity ? EDITED_CLASS : ''}>
                            <Editable
                                initialValue={processData.engineCapacity}
                                numberValidationOptions={{
                                    allowFloat: true,
                                    minNumberValue: 0,
                                    minNumberValueExclusive: true
                                }}
                                displayIfNotEditFormat={'{0} л'}
                                type={'number'}
                                edited={processData.engineCapacity !== processDataInDb.engineCapacity}
                                onReset={() => handleReset({progressDataPropName: 'engineCapacity'}) as EditableSupportedTypes}
                                onChange={(newValue) => handleChangeSomething('engineCapacity', newValue)}/></td>
                    </tr>
                )}
                <tr>
                    <td>Вид корпуса</td>
                    <td className={processData.corpusType !== processDataInDb.corpusType ? EDITED_CLASS : ''}>
                        <Editable initialValue={processData.corpusType} type={'CorpusType'}
                                  edited={processData.corpusType !== processDataInDb.corpusType}
                                  onReset={() => handleReset({progressDataPropName: 'corpusType'}) as EditableSupportedTypes}
                                  onChange={(newValue) => handleChangeSomething('corpusType', newValue)}/></td>
                </tr>
                <tr>
                    <td>Вид топлива</td>
                    <td className={processData.fuelType !== processDataInDb.fuelType ? EDITED_CLASS : ''}>
                        <Editable initialValue={processData.fuelType} type={'FuelTypes'}
                                  edited={processData.fuelType !== processDataInDb.fuelType}
                                  onReset={() => handleReset({progressDataPropName: 'fuelType'}) as EditableSupportedTypes}
                                  onChange={(newValue) => handleChangeSomething('fuelType', newValue)}/></td>
                </tr>
                <tr>
                    <td>Количество единиц товара на складе</td>
                    <td className={processData.count !== processDataInDb.count ? EDITED_CLASS : ''}>
                        <Editable
                            initialValue={processData.count}
                            type={'number'}
                            numberValidationOptions={{
                                minNumberValue: 0
                            }}
                            edited={processData.count !== processDataInDb.count}
                            onReset={() => handleReset({progressDataPropName: 'count'}) as EditableSupportedTypes}
                            onChange={(newValue) => handleChangeSomething('count', newValue)}/></td>
                </tr>
                <tr>
                    <td style={{backgroundColor: 'rgb(141, 248, 141)'}}>Цена за стандартную комплектацию</td>
                    <td className={processData.price !== processDataInDb.price ? EDITED_CLASS : ''}>
                        <Editable
                            initialValue={processData.price}
                            type={'number'}
                            numberValidationOptions={{
                                allowFloat: true,
                                minNumberValue: 0,
                                minNumberValueExclusive: true
                            }}
                            displayIfNotEditFormat={'{0} грн'}
                            edited={processData.price !== processDataInDb.price}
                            onReset={() => handleReset({progressDataPropName: 'price'}) as EditableSupportedTypes}
                            onChange={(newValue) => handleChangeSomething('price', newValue)}/></td>
                </tr>
                </tbody>
            </table>
            <div className="mb-2">
                <AdditionalCarOptionsContainer
                    additionalCarOptions={processData.additionalCarOptions}
                    markAsEditedTypes={getMarkAsEditedTypes()}
                    // @ts-ignore
                    onReset={(optionType) => handleReset({progressDataPropName: 'additionalCarOptions', optionType})}
                    onChange={optionsWithEnabled =>
                        handleChangeSomething('additionalCarOptions',
                            optionsWithEnabled.filter(e => e.enabled)
                                .map(optionWithEnabled => optionWithEnabled.additionalCarOption))}/>
            </div>
            <div className={'mb-2'}>
                <SaveChangesButton onClick={handleSaveChangesButton}/>
            </div>
            <ToastContainer/>
        </>
    )

    function getMarkAsEditedTypes() {
        return Object.values(AdditionalCarOptionType)
            .filter(e => typeof e === 'number')
            .filter(optionType => {
                const option = processData.additionalCarOptions
                    .find(option => option.type === optionType);
                const optionInDb = processDataInDb.additionalCarOptions
                        .find(option => option.type === optionType);

                    if (option !== undefined && optionInDb !== undefined) {
                        try {
                            deepEqual(option, optionInDb);
                        } catch (e) {
                            return true;
                        }
                        return false;
                    } else {
                        return !(option === undefined && optionInDb === undefined)
                    }
                }
            )
    }
}

function SaveChangesButton({onClick} : {onClick?: MouseEventHandler<HTMLButtonElement>}) {
    return (<button className={'btn btn-lg btn-outline-success w-100'} onClick={onClick}>Сохранить изменения</button>)
}
