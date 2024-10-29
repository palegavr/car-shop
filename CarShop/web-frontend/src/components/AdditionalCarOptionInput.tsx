import {AdditionalCarOption} from "@/types/types";
import {useEffect, useRef, useState} from "react";
import {stringToNumber} from "@/utilities/converters/stringToNumber";
import {validateNumber} from "@/utilities/validators/numberValidator";

type Props = {
    additionalCarOption: AdditionalCarOption,
    onChange?: (newValue: AdditionalCarOption) => void
}

export default function AdditionalCarOptionInput({additionalCarOption, onChange}: Props) {
    const [currentOptionState, setCurrentOptionState]
        = useState<AdditionalCarOption>({...additionalCarOption})
    const requiredCheckboxRef = useRef<HTMLInputElement>(null);
    const priceInputRef = useRef<HTMLInputElement>(null);

    useEffect(() => {
        priceInputRef.current!.value = String(currentOptionState.price);
    }, []);

    function handleRequiredChange() {
        const newValue = {
            ...currentOptionState,
            isRequired: requiredCheckboxRef.current!.checked
        };

        setCurrentOptionState(newValue);
        if (onChange)
            onChange(newValue);
    }

    function handlePriceInput() {
        const price = stringToNumber(priceInputRef.current!.value);
        if (price !== undefined && validateNumber(price, {
            allowFloat: true,
            minNumberValue: 0
        })) {
            const newValue = {
                ...currentOptionState,
                price: price
            };

            setCurrentOptionState(newValue);
        }
    }

    function handlePriceInputBlur() {
        priceInputRef.current!.value = String(currentOptionState.price);
        if (onChange)
            onChange(currentOptionState);
    }

    return (
        <>
            <div>
                <div className="row row-cols-1 row-cols-md-2">
                    <div className="col d-block d-md-flex justify-content-center align-items-center">
                        <div className="form-check">
                            <input className="form-check-input" type="checkbox"
                                   checked={currentOptionState.isRequired}
                                   ref={requiredCheckboxRef}
                                   onChange={handleRequiredChange}/>
                            <label className="form-check-label">
                                Обязательно
                            </label>
                        </div>
                    </div>
                    <div className="col">
                        <input type="text" className="form-control" placeholder="Цена"
                               ref={priceInputRef} onInput={handlePriceInput}
                               onBlur={handlePriceInputBlur}/>
                    </div>
                </div>
            </div>
        </>
    )
}