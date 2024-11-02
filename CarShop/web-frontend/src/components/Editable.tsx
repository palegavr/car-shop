import {CorpusType, corpusTypeToString, FuelType, FuelTypes, fuelTypesToString} from "@/types/types";
import React, {CSSProperties, FormEvent, MouseEventHandler, useRef, useState} from "react";
import '@/utilities/extentions/stringExtentions'
import {hasFlag} from "@/utilities/hasFlag";
import {NumberValidatorOptions, validateNumber} from "@/utilities/validators/numberValidator";
import {stringToNumber} from "@/utilities/converters/stringToNumber";
import {backgroundImageStyle} from "@/utilities/backgroundImageStyle";

type Props = {
    displayIfNotEditFormat?: string,
    initialValue: EditableSupportedTypes,
    type: EditableType,
    edited?: boolean,
    // Для случая с числом
    numberValidationOptions?: NumberValidatorOptions,
    // События
    onChange?: (newValue: EditableSupportedTypes) => Promise<EditableSupportedTypes | undefined>,
    onReset?: () => Promise<EditableSupportedTypes | undefined>
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
    const [waitingAcceptChange, setWaitingAcceptChange] = useState<boolean>(false);

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

    async function handleApplyChanges() {
        const valueFromInput = getValueFromInput();
        if (typeof valueFromInput === 'undefined')
            return;

        if (isValueValid({value: valueFromInput})) {
            if (typeof onChange !== 'undefined') {
                setWaitingAcceptChange(true);
                setApplyChangesButtonEnabled(false);
                const result = await onChange(valueFromInput);
                if (result !== undefined) {
                    setCurrentValue(result);
                    setEditing(false);
                }
                setWaitingAcceptChange(false);
                setApplyChangesButtonEnabled(true);
            } else {
                setCurrentValue(valueFromInput);
                setEditing(false);
            }
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

    async function handleReset() {
        if (onReset) {
            setWaitingAcceptChange(true);
            const result = await onReset();
            if (result !== undefined) {
                setCurrentValue(result);
            }
            setWaitingAcceptChange(false);
        }
    }

    return (
        <>
            <div className={`d-flex align-items-center`}>
                {editing ? ( // Если режим редактирования ВКЛЮЧЕН
                    <>
                        {(type === 'string' || type === 'number') && ( // Тип: строка
                            <input type="text" ref={inputRef} onInput={handleChangeInputValue}

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
                        <ApplyChangesButton onClick={handleApplyChanges} enabled={applyChangesButtonEnabled}
                                            loading={waitingAcceptChange}/>
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

    function CancelChangesButton({onClick}: {
        onClick?: MouseEventHandler<HTMLButtonElement>
    }) {
        return <button className="btn btn-danger" disabled={waitingAcceptChange} onClick={onClick}>X</button>
    }

    function ResetChangesButton({onClick}: { onClick?: MouseEventHandler<HTMLButtonElement> }) {
        let buttonStyles: CSSProperties = {
                width: '30px',
                height: '30px'
        };

        if (!waitingAcceptChange) {
            buttonStyles = {...buttonStyles, ...backgroundImageStyle('/images/reset_icon_246246.svg')}
        }
        return <button className={'btn btn-danger'} disabled={waitingAcceptChange} onClick={onClick}
                style={buttonStyles}>
            {waitingAcceptChange && (
                <div style={{transform: 'translate(-5px, -5px)'}}>
                    <span className="spinner-grow spinner-grow-sm" aria-hidden="true"></span>
                    <span className="visually-hidden" role="status">Loading...</span>
                </div>
            )}
        </button>
    }

    function StartEditButton({onClick}: {
        onClick?: MouseEventHandler<HTMLButtonElement>
    }) {
        return <button className="btn btn-primary" disabled={waitingAcceptChange} onClick={onClick}
                       style={{
                           ...backgroundImageStyle('/images/edit_icon-icons.com_61193.svg'), ...{
                               width: '30px',
                               height: '30px'
                           }
                       }}></button>
    }
}

function ApplyChangesButton({onClick, enabled = true, loading = false}: {
    onClick?: MouseEventHandler<HTMLButtonElement>,
    enabled: boolean,
    loading: boolean
}) {
    return <button className={`btn btn-success ${enabled ? '' : 'disabled'}`} onClick={onClick}>
        {loading ? (
            <>
                <span className="spinner-grow spinner-grow-sm" aria-hidden="true"></span>
                <span className="visually-hidden" role="status">Loading...</span>
            </>
        ) : '+'}
    </button>
}
