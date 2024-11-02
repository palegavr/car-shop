import {AdditionalCarOption} from "@/types/types";
import {useEffect, useRef} from "react";
import {stringToNumber} from "@/utilities/converters/stringToNumber";
import {validateNumber} from "@/utilities/validators/numberValidator";

type Props = {
    additionalCarOption: AdditionalCarOption,
    onChange?: (newValue: AdditionalCarOption) => void
}

export default function AdditionalCarOptionInput({additionalCarOption, onChange}: Props) {
    const requiredCheckboxRef = useRef<HTMLInputElement>(null);
    const priceInputRef = useRef<HTMLInputElement>(null);

    if (priceInputRef.current !== null) {
        priceInputRef.current!.value = String(additionalCarOption.price);
    }
    useEffect(() => {
        priceInputRef.current!.value = String(additionalCarOption.price);
    }, []);

    function handleRequiredChange() {
        const newValue = {
            ...additionalCarOption,
            isRequired: requiredCheckboxRef.current!.checked
        };

        if (onChange)
            onChange(newValue);
    }

    function handlePriceInputBlur() {
        const price = stringToNumber(priceInputRef.current!.value);
        if (price !== undefined && validateNumber(price, {
            allowFloat: true,
            minNumberValue: 0
        })) {
            const newValue = {
                ...additionalCarOption,
                price: price
            };

            if (onChange) {
                onChange(newValue);
            }

            priceInputRef.current!.value = String(newValue.price);
        } else {
            priceInputRef.current!.value = String(additionalCarOption.price);
        }
    }

    return (
        <>
            <div>
                <div className="row row-cols-1 row-cols-md-2">
                    <div className="col d-block d-md-flex justify-content-center align-items-center">
                        <div className="form-check">
                            <input className="form-check-input" type="checkbox"
                                   checked={additionalCarOption.isRequired}
                                   ref={requiredCheckboxRef}
                                   onChange={handleRequiredChange}/>
                            <label className="form-check-label">
                                Обязательно
                            </label>
                        </div>
                    </div>
                    <div className="col">
                        <input type="text" className="form-control" placeholder="Цена"
                               ref={priceInputRef}
                               onBlur={handlePriceInputBlur}/>
                    </div>
                </div>
            </div>
        </>
    )
}