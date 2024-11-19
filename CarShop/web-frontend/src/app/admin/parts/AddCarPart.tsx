import {
    Autocomplete,
    Button,
    Container,
    FormControlLabel,
    FormGroup,
    Stack,
    Switch,
    TextField,
    Typography
} from "@mui/material";
import AdditionalCarOptionsContainer from "@/components/AdditionalCarOptionsContainer";
import {AdditionalCarOption, CorpusType, FuelType, FuelTypes, ProcessData} from "@/types/types";
import {useRef, useState} from "react";
import {validateNumber} from "@/utilities/validators/numberValidator";
import ImageUploader from "@/components/ImageUploader";
import {delay} from "@/utilities/delay";
import {addCarAsync} from "@/clients/backendСlient";
import {toast} from "react-toastify";

const corpusTypeDisplayName = {
    [CorpusType.Sedan]: 'Седан',
    [CorpusType.Hatchback]: 'Хэтчбек',
}

const defaultAutoCloseTime = 2000;

export default function AddCarPart() {
    const [addingCar, setAddingCar] = useState<boolean>(false);

    const brandInputRef = useRef<HTMLInputElement>(null);
    const [brandInputErrorMessage, setBrandInputErrorMessage] = useState<string>();

    const modelInputRef = useRef<HTMLInputElement>(null);
    const [modelInputErrorMessage, setModelInputErrorMessage] = useState<string>();


    const priceInputRef = useRef<HTMLInputElement>(null);
    const [priceInputErrorMessage, setPriceInputErrorMessage] = useState<string>();

    const colorInputRef = useRef<HTMLInputElement>(null);
    const [colorInputErrorMessage, setColorInputErrorMessage] = useState<string>();

    const corpusTypeInputRef = useRef<HTMLInputElement>(null);
    const [corpusTypeInputErrorMessage, setCorpusTypeInputErrorMessage] = useState<string>();


    const petrolInputRef = useRef<HTMLInputElement>(null);
    const dieselInputRef = useRef<HTMLInputElement>(null);
    const gasInputRef = useRef<HTMLInputElement>(null);
    const electricInputRef = useRef<HTMLInputElement>(null);
    const [fuelTypeState, setFuelTypeState] = useState<FuelTypes | 0>(0);
    const [fuelTypeInputErrorMessage, setFuelTypeInputErrorMessage] = useState<string>();

    const engineCapacityInputRef = useRef<HTMLInputElement>(null);
    const [engineCapacityInputErrorMessage, setEngineCapacityInputErrorMessage] = useState<string>();

    const countInputRef = useRef<HTMLInputElement>(null);
    const [countInputErrorMessage, setCountInputErrorMessage] = useState<string>();


    const [imageUrl, setImageUrl] = useState<string>();
    const [bigImageUrls, setBigImageUrls] = useState<string[]>([]);
    const [additionalCarOptions, setAdditionalCarOptions] = useState<AdditionalCarOption[]>([]);

    async function handleAddCar() {
        setAddingCar(true);

        let corpusTypeNumber;
        for (const [corpusType, displayName] of Object.entries(corpusTypeDisplayName)) {
            if (displayName === corpusTypeInputRef.current!.value) {
                corpusTypeNumber = Number(corpusType);
                break;
            }
        }

        const payload: ProcessData = {
            brand: brandInputRef.current!.value,
            model: modelInputRef.current!.value,
            price: Number(priceInputRef.current!.value),
            color: colorInputRef.current!.value,
            engineCapacity: Number(engineCapacityInputRef.current!.value),
            count: Number(countInputRef.current!.value),
            fuelType: getFuelType() as FuelTypes,
            corpusType: corpusTypeNumber as CorpusType,
            imageUrl: imageUrl !== undefined ? imageUrl : '',
            bigImageUrls: bigImageUrls,
            additionalCarOptions: additionalCarOptions
        }

        if (payload.fuelType === FuelType.Electric) {
            payload.engineCapacity = 0;
        }

        if (validateAll()) {
            const result = await addCarAsync(payload);
            if (result.success) {
                toast.success('Товар добавлен.', {
                    position: 'bottom-right',
                    autoClose: defaultAutoCloseTime,
                    onClose: props => window.location.href = `/catalog/${result.carId}`
                });
            } else {
                toast.error('Ошибка.', {
                    position: 'bottom-right',
                    autoClose: defaultAutoCloseTime,
                });
                setAddingCar(false);
            }
        } else {
            toast.error('Введите корректные данные.', {
                position: 'bottom-right',
                autoClose: defaultAutoCloseTime,
            });
            setAddingCar(false);
        }
    }

    return (
        <>
            <Container
                maxWidth={'sm'}>
                <Stack
                    spacing={2}
                    paddingBottom={2}>
                    <Typography
                        variant={'h5'}
                        align={'center'}>
                        Добавление товара</Typography>

                    <TextField variant={'standard'} label={'Марка'}
                               inputRef={brandInputRef}
                               error={brandInputErrorMessage !== undefined}
                               helperText={brandInputErrorMessage}
                               onChange={event => {
                                   validateBrand();
                               }}/>
                    <TextField variant={'standard'} label={'Модель'}
                               inputRef={modelInputRef}
                               error={modelInputErrorMessage !== undefined}
                               helperText={modelInputErrorMessage}
                               onChange={event => {
                                   validateModel();
                               }}/>
                    <TextField variant={'standard'} label={'Цена за стандартную комплектацию (грн)'}
                               inputRef={priceInputRef}
                               error={priceInputErrorMessage !== undefined}
                               helperText={priceInputErrorMessage}
                               onChange={event => {
                                   validatePrice();
                               }}/>
                    <TextField variant={'standard'} label={'Цвет'}
                               inputRef={colorInputRef}
                               error={colorInputErrorMessage !== undefined}
                               helperText={colorInputErrorMessage}
                               onChange={event => {
                                   validateColor();
                               }}/>
                    <Autocomplete
                        disablePortal
                        options={Object.values(corpusTypeDisplayName)}
                        renderInput={(params) => (
                            <TextField {...params} label="Вид корпуса"
                                       inputRef={corpusTypeInputRef}
                                       error={corpusTypeInputErrorMessage !== undefined}
                                       helperText={corpusTypeInputErrorMessage}/>
                        )}
                        onChange={async event => {
                            await delay(100);
                            validateCorpusType();
                        }}
                    />

                    <FormGroup
                        onChange={event => {
                            setFuelTypeState(getFuelType());
                            validateFuelType();
                        }}>
                        <Typography>
                            Вид топлива
                        </Typography>
                        <FormControlLabel control={<Switch inputRef={petrolInputRef}/>} label="Бензин"/>
                        <FormControlLabel control={<Switch inputRef={dieselInputRef}/>} label="Дизель"/>
                        <FormControlLabel control={<Switch inputRef={gasInputRef}/>} label="Газ"/>
                        <FormControlLabel control={<Switch inputRef={electricInputRef}/>} label="Электрика"/>
                        {fuelTypeInputErrorMessage !== undefined && (
                            <Typography color={'error'}>{fuelTypeInputErrorMessage}</Typography>
                        )}
                    </FormGroup>

                    <TextField variant={'standard'} label={'Объём двигателя (литры)'}
                               inputRef={engineCapacityInputRef}
                               sx={{
                                   display: (fuelTypeState > 0 && fuelTypeState !== FuelType.Electric) ? undefined : 'none',
                               }}
                               error={engineCapacityInputErrorMessage !== undefined}
                               helperText={engineCapacityInputErrorMessage}
                               onChange={event => {
                                   validateEngineCapacity();
                               }}/>

                    <TextField variant={'standard'} label={'Количество единиц товара на складе'}
                               inputRef={countInputRef}
                               error={countInputErrorMessage !== undefined}
                               helperText={countInputErrorMessage}
                               onChange={event => {
                                   validateCount();
                               }}/>

                    <Typography>
                        Картинка для <Typography fontWeight={'bold'} component={'span'}>/catalog</Typography>
                    </Typography>
                    <ImageUploader variant={'single'}
                                   allowedImageExtensions={['png', 'jpg', 'jpeg']}
                                   defaultImageUrls={[]}
                                   onChange={imageUrls => setImageUrl(imageUrls.length > 0 ? imageUrls[0] : undefined)}/>

                    <Typography>
                        Картинки для <Typography fontWeight={'bold'} component={'span'}>/catalog/{'{id}'}</Typography>
                    </Typography>
                    <ImageUploader variant={'multiple'}
                                   allowedImageExtensions={['png', 'jpg', 'jpeg']}
                                   defaultImageUrls={[]}
                                   onChange={imageUrls => setBigImageUrls(imageUrls)}/>

                    <AdditionalCarOptionsContainer
                        additionalCarOptions={additionalCarOptions}
                        showResetButton={false}
                        onChange={async optionsWithEnabled => {
                            const options = optionsWithEnabled.filter(e => e.enabled)
                                .map(optionWithEnabled => optionWithEnabled.additionalCarOption);
                            setAdditionalCarOptions(options);
                        }}/>

                    <Button
                        variant={'contained'}
                        onClick={handleAddCar}>
                        Добавить товар
                    </Button>
                </Stack>
            </Container>
        </>
    )

    function validateAll(): boolean {
        const isBrandValid = validateBrand();
        const isModelValid = validateModel();
        const isPriceValid = validatePrice();
        const isColorValid = validateColor();
        const isEngineCapacityValid = validateEngineCapacity();
        const isCountValid = validateCount();
        const isFuelTypeValid = validateFuelType();
        const isCorpusTypeValid = validateCorpusType();

        return (
            isBrandValid &&
            isModelValid &&
            isPriceValid &&
            isColorValid &&
            isEngineCapacityValid &&
            isCountValid &&
            isFuelTypeValid &&
            isCorpusTypeValid
        );
    }

    function validateBrand(): boolean {
        if (!validateStringNotEmpty(brandInputRef.current!.value)) {
            setBrandInputErrorMessage('Заполните');
            return false;
        } else {
            setBrandInputErrorMessage(undefined);
            return true;
        }
    }

    function validateModel(): boolean {
        const inputValue = modelInputRef.current!.value;
        if (!validateStringNotEmpty(inputValue)) {
            setModelInputErrorMessage('Заполните');
            return false;
        } else {
            setModelInputErrorMessage(undefined);
            return true;
        }
    }

    function validateColor(): boolean {
        const inputValue = colorInputRef.current!.value;
        if (!validateStringNotEmpty(inputValue)) {
            setColorInputErrorMessage('Заполните');
            return false;
        } else {
            setColorInputErrorMessage(undefined);
            return true;
        }
    }

    function validatePrice(): boolean {
        const inputValue = priceInputRef.current!.value;
        if (!validateStringNotEmpty(inputValue) ||
            !validateNumber(Number(inputValue), {
                allowFloat: true,
                minNumberValue: 0,
                minNumberValueExclusive: true
            })) {
            setPriceInputErrorMessage('Введите положительное число');
            return false;
        } else {
            setPriceInputErrorMessage(undefined);
            return true;
        }
    }

    function validateEngineCapacity(): boolean {
        const inputValue = engineCapacityInputRef.current!.value;
        if (fuelTypeState === FuelType.Electric) {
            return true;
        }
        if (!validateStringNotEmpty(inputValue) ||
            !validateNumber(Number(inputValue), {
                allowFloat: true,
                minNumberValue: 0,
                minNumberValueExclusive: true
            })) {
            setEngineCapacityInputErrorMessage('Введите положительное число');
            return false;
        } else {
            setEngineCapacityInputErrorMessage(undefined);
            return true;
        }
    }

    function validateCount(): boolean {
        const inputValue = countInputRef.current!.value;
        if (!validateStringNotEmpty(inputValue) ||
            !validateNumber(Number(inputValue), {
                minNumberValue: 0,
            })) {
            setCountInputErrorMessage('Введите положительное целое число');
            return false;
        } else {
            setCountInputErrorMessage(undefined);
            return true;
        }
    }

    function validateFuelType(): boolean {
        if (getFuelType() === 0) {
            setFuelTypeInputErrorMessage('Выберите минимум один тип топлива');
            return false;
        } else {
            setFuelTypeInputErrorMessage(undefined);
            return true;
        }
    }

    function validateCorpusType(): boolean {
        const inputValue = corpusTypeInputRef.current!.value;
        if (!validateStringNotEmpty(inputValue)) {
            setCorpusTypeInputErrorMessage('Выберите тип корпуса');
            return false;
        } else {
            setCorpusTypeInputErrorMessage(undefined);
            return true;
        }
    }

    function validateStringNotEmpty(value: string): boolean {
        return value.trim() !== '';
    }

    function getFuelType(): FuelTypes | 0 {
        let fuelType = 0;
        if (petrolInputRef.current!.checked) {
            fuelType |= FuelType.Petrol
        }
        if (dieselInputRef.current!.checked) {
            fuelType |= FuelType.Diesel
        }
        if (gasInputRef.current!.checked) {
            fuelType |= FuelType.Gas
        }
        if (electricInputRef.current!.checked) {
            fuelType |= FuelType.Electric
        }
        return fuelType as FuelTypes;
    }
}