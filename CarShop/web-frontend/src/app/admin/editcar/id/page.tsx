'use client'
import {MouseEventHandler, useEffect, useRef, useState} from "react";
import {AdditionalCarOptionType, FuelType, ProcessData} from "@/types/types";
import CatalogImagesCarouselEditor from "@/components/CatalogImagesCarouselEditor";
import {parseJsonEncodedProcessData} from "@/utilities/processDataOperations";
import Editable, {EditableSupportedTypes} from "@/components/Editable";
import {applyChangesAsync, pushProcessData, PushProcessDataResult} from "@/clients/backendСlient";
import AdditionalCarOptionsContainer from "@/components/AdditionalCarOptionsContainer";
import {EDITED_CLASS} from "@/constants";
import {deepEqual} from "assert";
import {toast, ToastContainer} from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import ImageChanger from "@/components/ImageChanger";
import {delay} from "@/utilities/delay";


type DataFromBackend = {
    processDataInDbJsonEncoded: string,
    currentProcessDataJsonEncoded: string,
    carId: number
}

type HandleChangeSomethingResult = {
    pushResult: PushProcessDataResult,
    newProcessData?: ProcessData
}

const defaultToastTime = 2000;

export default function Page() {

    const [dataFromBackendLoaded, setDataFromBackendLoaded] = useState<boolean>();
    const [processDataInDb, setProcessDataInDb] = useState<ProcessData>(null!);
    const [currentProcessData, setCurrentProcessData] = useState<ProcessData>(null!);
    const [processData, setProcessData] = useState<ProcessData>(null!);
    const [carId, setCarId] = useState(null!);
    const pushingChangesRef = useRef<boolean>(false);
    const processDataRef = useRef<ProcessData>(processData);
    processDataRef.current = processData;

    useEffect(() => {
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
        } catch (e) {
            setDataFromBackendLoaded(false);
        }
    }, [])

    async function handleChangeSomething(progressDataPropName: keyof ProcessData, newValue: any)
        : Promise<HandleChangeSomethingResult> {
        while (pushingChangesRef.current) {
            await delay(100);
        }
        pushingChangesRef.current = true;
        const newProcessData = {...processDataRef.current, [progressDataPropName]: newValue} as ProcessData;
        if (newProcessData.fuelType !== FuelType.Electric && newProcessData.engineCapacity <= 0) {
            newProcessData.engineCapacity = 1;
        }
        const result = await pushProcessData(carId,
            newProcessData.fuelType === FuelType.Electric ? {...newProcessData, engineCapacity: 0} :
                newProcessData);
        if (result.success) {
            setCurrentProcessData(newProcessData);
            setProcessData(newProcessData);
        } else {
            toast.error('Не удалось синхронизировать изменения.', {
                position: 'bottom-right',
                autoClose: defaultToastTime
            });
        }
        pushingChangesRef.current = false;
        return {
            pushResult: result,
            newProcessData: result.success ? newProcessData : undefined
        };
    }

    async function handleSaveChangesButton() {
        const handleChangeSomethingResult = await handleChangeSomething('brand', processData.brand);
        if (!handleChangeSomethingResult.pushResult.success) {
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

    async function handleReset({progressDataPropName, optionType}: {
        progressDataPropName: keyof ProcessData,
        optionType?: AdditionalCarOptionType
    })
        : Promise<HandleChangeSomethingResult> {
        if (progressDataPropName === 'additionalCarOptions') {
            let newAdditionalCarOptions = [...processData.additionalCarOptions]
                .filter(el => el.type !== optionType);

            for (const optionInDb of processDataInDb.additionalCarOptions) {
                if (optionInDb.type === optionType) {
                    newAdditionalCarOptions.push(optionInDb);
                    break;
                }
            }

            const result = await handleChangeSomething(progressDataPropName, newAdditionalCarOptions);
            return result;
        } else {
            const result = await handleChangeSomething(progressDataPropName, processDataInDb[progressDataPropName as keyof ProcessData]);
            return result;
        }
    }

    async function handleEditableChange(progressDataPropName: keyof ProcessData, newValue: EditableSupportedTypes)
        : Promise<EditableSupportedTypes | undefined> {
        const result = await handleChangeSomething(progressDataPropName, newValue);
        return result.newProcessData !== undefined ? result.newProcessData[progressDataPropName] as EditableSupportedTypes : undefined;
    }

    async function handleEditableReset(progressDataPropName: keyof ProcessData)
        : Promise<EditableSupportedTypes | undefined> {
        const result = await handleReset({progressDataPropName});
        return result.newProcessData !== undefined ? result.newProcessData[progressDataPropName] as EditableSupportedTypes : undefined;
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
                    imageUrl={processData.imageUrl !== '' ? processData.imageUrl : undefined}
                    edited={processData.imageUrl !== processDataInDb.imageUrl}
                    onReset={async() => {
                        await handleReset({progressDataPropName: 'imageUrl'})
                    }}
                    onChange={async imageUrl => {
                        await handleChangeSomething('imageUrl', imageUrl !== undefined ? imageUrl : '')
                    }}
                    onUploadFailed={() => toast.error('Не удалось загрузить изображение.', {
                        position: 'bottom-right',
                        autoClose: defaultToastTime
                    })}/>
            </div>

            <div className="text-center mt-2">
                <CatalogImagesCarouselEditor
                    imageUrls={processData.bigImageUrls}
                    edited={JSON.stringify(processData.bigImageUrls) !==
                        JSON.stringify(processDataInDb.bigImageUrls)}
                    onReset={async () => {
                        await handleReset({progressDataPropName: 'bigImageUrls'});
                    }}
                    onChange={async imageUrls => {
                        await handleChangeSomething('bigImageUrls', imageUrls);
                    }}
                    onUploadFailed={() => toast.error('Не удалось загрузить изображение(я).', {
                        position: 'bottom-right',
                        autoClose: defaultToastTime
                    })}/>
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
                                  onReset={() => handleEditableReset('brand')}
                                  onChange={newValue => handleEditableChange('brand', newValue)}/></td>
                </tr>
                <tr>
                    <td>Модель</td>
                    <td className={processData.model !== processDataInDb.model ? EDITED_CLASS : ''}>
                        <Editable initialValue={processData.model} type={'string'}
                                  edited={processData.model !== processDataInDb.model}
                                  onReset={() => handleEditableReset('model')}
                                  onChange={newValue => handleEditableChange('model', newValue)}/></td>
                </tr>
                <tr>
                    <td>Цвет</td>
                    <td className={processData.color !== processDataInDb.color ? EDITED_CLASS : ''}>
                        <Editable initialValue={processData.color} type={'string'}
                                  edited={processData.color !== processDataInDb.color}
                                  onReset={() => handleEditableReset('color')}
                                  onChange={newValue => handleEditableChange('color', newValue)}/></td>
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
                                onReset={() => handleEditableReset('engineCapacity')}
                                onChange={newValue => handleEditableChange('engineCapacity', newValue)}/></td>
                    </tr>
                )}
                <tr>
                    <td>Вид корпуса</td>
                    <td className={processData.corpusType !== processDataInDb.corpusType ? EDITED_CLASS : ''}>
                        <Editable initialValue={processData.corpusType} type={'CorpusType'}
                                  edited={processData.corpusType !== processDataInDb.corpusType}
                                  onReset={() => handleEditableReset('corpusType')}
                                  onChange={newValue => handleEditableChange('corpusType', newValue)}/></td>
                </tr>
                <tr>
                    <td>Вид топлива</td>
                    <td className={processData.fuelType !== processDataInDb.fuelType ? EDITED_CLASS : ''}>
                        <Editable initialValue={processData.fuelType} type={'FuelTypes'}
                                  edited={processData.fuelType !== processDataInDb.fuelType}
                                  onReset={() => handleEditableReset('fuelType')}
                                  onChange={newValue => handleEditableChange('fuelType', newValue)}/></td>
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
                            onReset={() => handleEditableReset('count')}
                            onChange={newValue => handleEditableChange('count', newValue)}/></td>
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
                            onReset={() => handleEditableReset('price')}
                            onChange={newValue => handleEditableChange('price', newValue)}/></td>
                </tr>
                </tbody>
            </table>
            <div className="mb-2">
                <AdditionalCarOptionsContainer
                    additionalCarOptions={processData.additionalCarOptions}
                    markAsEditedTypes={getMarkAsEditedTypes()}
                    // @ts-ignore
                    onReset={async (optionType) =>
                        await handleReset({progressDataPropName: 'additionalCarOptions', optionType})}
                    onChange={async optionsWithEnabled => {
                        await handleChangeSomething('additionalCarOptions',
                            optionsWithEnabled.filter(e => e.enabled)
                                .map(optionWithEnabled => optionWithEnabled.additionalCarOption));
                        return;
                    }}/>
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

function SaveChangesButton({onClick}: { onClick?: MouseEventHandler<HTMLButtonElement> }) {
    return (<button className={'btn btn-lg btn-outline-success w-100'} onClick={onClick}>Сохранить изменения</button>)
}
