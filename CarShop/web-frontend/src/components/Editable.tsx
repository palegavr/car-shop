import {CorpusType, corpusTypeToString, FuelType, FuelTypes, fuelTypesToString} from "@/types/types";
import React, {FormEvent, MouseEventHandler, useEffect, useRef, useState} from "react";
import '@/utilities/extentions/stringExtentions'
import {hasFlag} from "@/utilities/hasFlag";
import {NumberValidatorOptions, validateNumber} from "@/utilities/validators/numberValidator";
import {stringToNumber} from "@/utilities/converters/stringToNumber";
import {EDITED_CLASS} from "@/constants";
import {backgroundImageStyle} from "@/utilities/backgroundImageStyle";

type Props = {
    displayIfNotEditFormat?: string,
    initialValue: EditableSupportedTypes,
    type: EditableType,
    edited?: boolean,
    // Для случая с числом
    numberValidationOptions?: NumberValidatorOptions,
    // События
    onChange?: (newValue: EditableSupportedTypes) => void,
    onReset?: () => EditableSupportedTypes
}

type EditableType = 'string' | 'number' | 'FuelTypes' | 'CorpusType';
export type EditableSupportedTypes = string | number | FuelTypes | CorpusType;

export default function Editable(
    {
        displayIfNotEditFormat = '{0}', initialValue, type, numberValidationOptions, onChange, edited = false, onReset
    }: Props
) {
    const inputRef = useRef<HTMLInputElement>(null);
    const selectRef = useRef<HTMLSelectElement>(null);

    const [currentValue, setCurrentValue] = useState(initialValue);
    const [fuelTypeCheckboxesState, setFuelTypeCheckboxesState]
        = useState<FuelTypes | undefined>(type === 'FuelTypes' ? initialValue as FuelTypes : undefined);
    const [editing, setEditing] = useState(false);
    const [applyChangesButtonEnabled, setApplyChangesButtonEnabled] = useState(true);

    function handleStartEdit() {
        setEditing(true);
        setTimeout(() => {
            inputRef.current?.focus();
            selectRef.current?.focus();

            if (type === 'CorpusType') {
                selectRef.current!.value = currentValue as string;
                selectRef.current!.click();
            } else if (type === 'FuelTypes') {
            } else {
                inputRef.current!.value = type === 'number' ? currentValue.toString() : currentValue as string;
            }
        }, 0);
    }

    function handleCancelChangesButton() {
        if (type === 'FuelTypes')
            setFuelTypeCheckboxesState(currentValue as FuelTypes);

        setEditing(false);
    }

    function handleApplyChanges() {
        const valueFromInput = getValueFromInput();
        if (typeof valueFromInput === 'undefined')
            return;

        if (isValueValid({value: valueFromInput})) {
            setCurrentValue(valueFromInput);
            setEditing(false);
            if (typeof onChange !== 'undefined')
                setTimeout(onChange, 0, valueFromInput);
        }
    }

    function handleChangeInputValue(event: FormEvent<HTMLElement>) {
        const valueFromInput = getValueFromInput();
        let disableApplyChangesButton = false;

        if (typeof valueFromInput !== 'undefined') {
            if (!isValueValid({value: valueFromInput}))
                disableApplyChangesButton = true;
        } else {
            disableApplyChangesButton = true;
        }

        if (applyChangesButtonEnabled === disableApplyChangesButton) {
            setApplyChangesButtonEnabled(!disableApplyChangesButton);
        }
    }

    function handleBlurInput() {
        if (applyChangesButtonEnabled) {
            handleApplyChanges();
        }
    }

    function handleFuelTypeCheckboxChanged(fuelType: FuelType, checked: boolean) {
        let disableApplyChangesButton = false;

        // @ts-ignore
        const newCurrentValue = checked ? (fuelTypeCheckboxesState | fuelType) : (fuelTypeCheckboxesState & (~fuelType));
        if (newCurrentValue === 0)
            disableApplyChangesButton = true;

        setFuelTypeCheckboxesState(newCurrentValue as FuelTypes);

        if (applyChangesButtonEnabled === disableApplyChangesButton) {
            setApplyChangesButtonEnabled(!disableApplyChangesButton);
        }
    }

    function handleReset() {
        if (onReset) {
            setCurrentValue(onReset());
        }
    }

    return (
        <>
            <div className={`d-flex align-items-center`}>
                {editing ? ( // Если режим редактирования ВКЛЮЧЕН
                    <>
                        {(type === 'string' || type === 'number') && ( // Тип: строка
                            <input type="text" ref={inputRef} onInput={handleChangeInputValue}
                                   onBlur={handleBlurInput}
                                   onKeyDown={(event) => {
                                       if (event.key === 'Enter') handleApplyChanges()
                                   }}
                                   className={'form-control me-2'}/>
                        )}
                        {type === 'CorpusType' && (
                            <select className="form-select me-2" ref={selectRef} defaultValue={currentValue}
                                    onBlur={handleBlurInput} onInput={handleChangeInputValue}>
                                <option value="0">Седан</option>
                                <option value="1">Хэтчбек</option>
                            </select>
                        )}
                        {type === 'FuelTypes' && (
                            <div className={'d-block'}>
                                {Object.values(FuelType)
                                    .filter(value => typeof value === 'number')
                                    .map(fuelType => (
                                        <div className="form-check form-switch">
                                            <input className="form-check-input" type="checkbox"
                                                   role="button"
                                                   checked={hasFlag(fuelTypeCheckboxesState as FuelTypes, fuelType)}
                                                   onChange={(event) =>
                                                       handleFuelTypeCheckboxChanged(fuelType, event.currentTarget.checked)}/>
                                            <label className="form-check-label">{fuelTypesToString(fuelType)}</label>
                                        </div>
                                    ))}
                            </div>
                        )}

                        <div className="me-1">
                            <CancelChangesButton onClick={handleCancelChangesButton}/>
                        </div>
                        <ApplyChangesButton onClick={handleApplyChanges} enabled={applyChangesButtonEnabled}/>
                    </>
                ) : ( // Если режим редактирования ВЫКЛЮЧЕН
                    <div>
                        <span className="me-2">{displayIfNotEditFormat.format(
                            type === 'CorpusType' ? corpusTypeToString(currentValue as CorpusType) :
                                type === 'FuelTypes' ? fuelTypesToString(currentValue as FuelTypes) : currentValue)}</span>
                        <StartEditButton onClick={handleStartEdit}/>
                        {edited && (
                            <span className={'ms-1'}>
                                <ResetChangesButton onClick={handleReset}/>
                            </span>
                        )}
                    </div>
                )}
            </div>
        </>
    )

    function getValueFromInput(): EditableSupportedTypes | undefined {
        let value: EditableSupportedTypes =
            type === 'CorpusType' ? selectRef.current!.value :
                type === 'FuelTypes' ? '' : inputRef.current!.value.trim();

        if (type === 'FuelTypes') {
            // @ts-ignore
            return fuelTypeCheckboxesState === 0 ? undefined : fuelTypeCheckboxesState;
        }

        if (type === 'number')
            return stringToNumber(value);
        else if (type === 'CorpusType')
            value = Number(value);

        return value;
    }

    function isValueValid({value}: { value: EditableSupportedTypes }): boolean {
        if (type === 'string') {
            return (value as string).trim() !== '';
        } else if (type === 'number') {
            const valueAsNumber = (value as number);
            return validateNumber(valueAsNumber, numberValidationOptions);
            // !isNaN(valueAsNumber) &&
            //     (!allowFloat ? Number.isInteger(valueAsNumber) : true) &&
            //     (typeof minNumberValue !== 'undefined' ?
            //         (minNumberValueInclusive ? valueAsNumber >= minNumberValue : valueAsNumber > minNumberValue) : true) &&
            //     (typeof maxNumberValue !== 'undefined' ?
            //         (maxNumberValueInclusive ? valueAsNumber <= maxNumberValue : valueAsNumber < maxNumberValue) : true)
        } else if (type === 'CorpusType') {
            const valueAsCorpusType = value as CorpusType;
            return valueAsCorpusType === CorpusType.Sedan ||
                valueAsCorpusType === CorpusType.Hatchback;
        } else if (type === 'FuelTypes') {
            const valueAsFuelTypes: FuelTypes = value as FuelTypes;
            return valueAsFuelTypes >= 1 && valueAsFuelTypes <= 15
        }

        return false;
    }

}

function StartEditButton({onClick}: {
    onClick?: MouseEventHandler<HTMLButtonElement>
}) {
    return <button className="btn btn-primary" onClick={onClick}
                   style={{
                       ...backgroundImageStyle('/images/edit_icon-icons.com_61193.svg'), ...{
                           width: '30px',
                           height: '30px'
                       }
                   }}></button>
}

function ApplyChangesButton({onClick, enabled = true}: {
    onClick?: MouseEventHandler<HTMLButtonElement>,
    enabled: boolean
}) {
    return <button className={`btn btn-success ${enabled ? '' : 'disabled'}`} onClick={onClick}>+</button>
}

function CancelChangesButton({onClick}: {
    onClick?: MouseEventHandler<HTMLButtonElement>
}) {
    return <button className="btn btn-danger" onClick={onClick}>X</button>
}

function ResetChangesButton({onClick}: { onClick?: MouseEventHandler<HTMLButtonElement> }) {
    return <button className={'btn btn-danger'} onClick={onClick}
                   style={{
                       ...backgroundImageStyle('/images/reset_icon_246246.svg'), ...{
                           width: '30px',
                           height: '30px'
                       }
                   }}></button>
}