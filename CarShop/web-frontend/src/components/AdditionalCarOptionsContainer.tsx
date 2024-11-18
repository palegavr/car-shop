import {AdditionalCarOption, AdditionalCarOptionType, additionalCarOptionTypeToString} from "@/types/types";
import AdditionalCarOptionInput from "@/components/AdditionalCarOptionInput";
import React, {CSSProperties, MouseEventHandler, useState} from "react";
import {EDITED_CLASS} from "@/constants";
import {backgroundImageStyle} from "@/utilities/backgroundImageStyle";

type Props = {
    additionalCarOptions: AdditionalCarOption[],
    markAsEditedTypes?: AdditionalCarOptionType[],
    showResetButton?: boolean
    onChange?: (optionsWithEnabled: AdditionalCarOptionWithEnabled[]) => Promise<void>,
    onReset?: (optionType: AdditionalCarOptionType) => Promise<void>
}

type AdditionalCarOptionWithEnabled = {
    enabled: boolean,
    additionalCarOption: AdditionalCarOption
}

export default function AdditionalCarOptionsContainer({
                                                          additionalCarOptions,
                                                          onChange,
                                                          markAsEditedTypes = [],
                                                          showResetButton = true,
                                                          onReset
                                                      }: Props) {
    const [opened, setOpened] = useState<boolean>(false);
    const currentOptionsWithEnabled = makeAdditionalCarOptionsWithEnabled(additionalCarOptions);
    const [waitingAcceptChange, setWaitingAcceptChange] = useState<boolean>(false);

    async function handleChangeOptionEnabled(optionType: AdditionalCarOptionType, enabled: boolean) {
        const newCurrentOptionsWithEnabled = [...currentOptionsWithEnabled];
        newCurrentOptionsWithEnabled.find(e => e.additionalCarOption.type === optionType)!
            .enabled = enabled;

        //setCurrentOptions(newCurrentOptions);
        if (onChange) {
            setWaitingAcceptChange(true);
            await onChange(newCurrentOptionsWithEnabled);
            setWaitingAcceptChange(false);
        }
    }

    async function handleChangeOptionData(additionalCarOption: AdditionalCarOption) {
        const newCurrentOptionsWithEnabled = [...currentOptionsWithEnabled];
        const optionWithEnabled = newCurrentOptionsWithEnabled.find(e => e.additionalCarOption.type === additionalCarOption.type)!;
        optionWithEnabled.additionalCarOption = additionalCarOption;

        //setCurrentOptions(newCurrentOptions);
        if (onChange) {
            setWaitingAcceptChange(true);
            await onChange(newCurrentOptionsWithEnabled);
            setWaitingAcceptChange(false);
        }
    }

    function handleOpenButtonClick() {
        setOpened(!opened);
    }

    async function handleReset(optionType: AdditionalCarOptionType) {
        if (onReset) {
            setWaitingAcceptChange(true);
            await onReset(optionType);
            setWaitingAcceptChange(false);
        }
    }

    return (
        <div className="accordion fs-5">
            <div className="accordion-item">
                <h2 className="accordion-header">
                    <button className={`accordion-button ${!opened ? 'collapsed' : ''}`} type="button"
                            onClick={handleOpenButtonClick}>
                        <span className={'fs-5'}>Дополнительные опции</span>
                        {waitingAcceptChange && (
                            <div className="spinner-border spinner-border-sm ms-2" role="status">
                                <span className="visually-hidden">Loading...</span>
                            </div>
                        )}
                    </button>
                </h2>
                <div className={`accordion-collapse collapse ${opened ? 'show' : ''}`}>
                    <div className="accordion-body"
                         style={waitingAcceptChange ? {pointerEvents: 'none', opacity: '0.7'} : {}}>
                        {Object.values(AdditionalCarOptionType)
                            .filter(e => typeof e === 'number')
                            .map(optionType => {
                                const optionWithEnabled = currentOptionsWithEnabled
                                    .find(optionWithEnabled => optionWithEnabled.additionalCarOption.type === optionType)!;

                                return (
                                    <div key={optionType}
                                         className={`border border-1 rounded p-2 shadow-sm mb-3 ${markAsEditedTypes.includes(optionType) ? EDITED_CLASS : ''}`}>
                                        <div className="form-check form-switch">
                                            <input className="form-check-input" type="checkbox" role="button"
                                                   checked={optionWithEnabled.enabled}
                                                   onChange={(event) =>
                                                       handleChangeOptionEnabled(optionType, event.currentTarget.checked)}/>
                                            <label
                                                className="form-check-label">{additionalCarOptionTypeToString(optionType)}</label>
                                            {showResetButton && (
                                                <span className="ms-2">
                                                    <ResetChangesButton onClick={() => handleReset(optionType)}/>
                                                </span>
                                            )}
                                        </div>
                                        {optionWithEnabled.enabled && (
                                            <AdditionalCarOptionInput
                                                additionalCarOption={optionWithEnabled.additionalCarOption}
                                                onChange={handleChangeOptionData}/>
                                        )}
                                    </div>
                                )
                            })
                        }
                    </div>
                </div>
            </div>
        </div>
    )
}

function ResetChangesButton({onClick}: { onClick?: MouseEventHandler<HTMLButtonElement> }) {
    let buttonStyles: CSSProperties = {
        width: '30px',
        height: '30px'
    };

    return <button className={'btn btn-danger'}
                   style={
                       {
                           ...backgroundImageStyle('/images/reset_icon_246246.svg'),
                           ...buttonStyles
                       }}
                   onClick={onClick}></button>
}

function makeAdditionalCarOptionsWithEnabled(additionalCarOptions: AdditionalCarOption[]): AdditionalCarOptionWithEnabled[] {
    return Object.values(AdditionalCarOptionType)
        .filter(e => typeof e === 'number')
        .map(optionType => {
            const carOption = additionalCarOptions.find(option => option.type === optionType);
            return {
                enabled: typeof carOption !== 'undefined',
                additionalCarOption: carOption ? {...carOption} : {
                    type: optionType,
                    price: 0,
                    isRequired: false
                }
            }
        });
}
