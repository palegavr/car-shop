import {AdditionalCarOption, AdditionalCarOptionType, additionalCarOptionTypeToString} from "@/types/types";
import AdditionalCarOptionInput from "@/components/AdditionalCarOptionInput";
import React, {MouseEventHandler, useState} from "react";
import {EDITED_CLASS} from "@/constants";

type Props = {
    additionalCarOptions: AdditionalCarOption[],
    markAsEditedTypes?: AdditionalCarOptionType[],
    onChange?: (optionsWithEnabled: AdditionalCarOptionWithEnabled[]) => void,
    onReset?: (optionType: AdditionalCarOptionType) => AdditionalCarOption[]
}

type AdditionalCarOptionWithEnabled = {
    enabled: boolean,
    additionalCarOption: AdditionalCarOption
}

export default function AdditionalCarOptionsContainer({additionalCarOptions, onChange, markAsEditedTypes = [], onReset}: Props) {
    const [opened, setOpened] = useState<boolean>(false);
    const [currentOptions, setCurrentOptions]
        = useState<AdditionalCarOptionWithEnabled[]>(makeAdditionalCarOptionsWithEnabled(additionalCarOptions));

    function handleChangeOptionEnabled(optionType: AdditionalCarOptionType, enabled: boolean) {
        const newCurrentOptions = [...currentOptions];
        newCurrentOptions.find(e => e.additionalCarOption.type === optionType)!
            .enabled = enabled;

        setCurrentOptions(newCurrentOptions);
        if (onChange) {
            onChange(newCurrentOptions);
        }
    }

    function handleChangeOptionData(additionalCarOption: AdditionalCarOption) {
        const newCurrentOptions = [...currentOptions];
        const optionWithEnabled = newCurrentOptions.find(e => e.additionalCarOption.type === additionalCarOption.type)!;
        optionWithEnabled.additionalCarOption = additionalCarOption;

        setCurrentOptions(newCurrentOptions);
        if (onChange) {
            onChange(newCurrentOptions);
        }
    }

    function handleOpenButtonClick() {
        setOpened(!opened);
    }

    function handleReset(optionType: AdditionalCarOptionType) {
        if (onReset) {
            let optionsWithEnabled = makeAdditionalCarOptionsWithEnabled(onReset(optionType));
            setCurrentOptions(optionsWithEnabled.map(o => {o.enabled = false; return o;}));
            optionsWithEnabled = makeAdditionalCarOptionsWithEnabled(onReset(optionType));
            setTimeout(setCurrentOptions, 0, optionsWithEnabled);
            //setCurrentOptions(makeAdditionalCarOptionsWithEnabled(onReset(optionType)));
            // const scrollYPosition = window.pageYOffset;
            // setOpened(false);
            // setTimeout(() => {
            //     setOpened(true);
            //     setTimeout(() => window.scrollTo(0, scrollYPosition), 0);
            // }, 0);
        }
    }

    return (
        <div className="accordion fs-5">
            <div className="accordion-item">
                <h2 className="accordion-header">
                    <button className={`accordion-button ${!opened ? 'collapsed' : ''}`} type="button"
                            onClick={handleOpenButtonClick}>
                        <span className={'fs-5'}>Дополнительные опции</span>
                    </button>
                </h2>
                <div className={`accordion-collapse collapse ${opened ? 'show' : ''}`}>
                    <div className="accordion-body">
                        {Object.values(AdditionalCarOptionType)
                            .filter(e => typeof e === 'number')
                            .map(optionType => {
                                const optionWithEnabled = currentOptions
                                    .find(optionWithEnabled => optionWithEnabled.additionalCarOption.type === optionType)!;

                                return (
                                    <div key={optionType} className={`border border-1 rounded p-2 shadow-sm mb-3 ${markAsEditedTypes.includes(optionType) ? EDITED_CLASS : ''}`}>
                                        <div className="form-check form-switch">
                                            <input className="form-check-input" type="checkbox" role="button" checked={optionWithEnabled.enabled}
                                            onChange={(event) =>
                                                handleChangeOptionEnabled(optionType, event.currentTarget.checked)}/>
                                            <label
                                                className="form-check-label">{additionalCarOptionTypeToString(optionType)}</label>
                                            <span className="ms-2">
                                                <ResetChangesButton onClick={() => handleReset(optionType)}/>
                                            </span>
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

function ResetChangesButton({onClick} : {onClick?: MouseEventHandler<HTMLButtonElement>}) {
    return <button className={'btn btn-danger'} onClick={onClick}>R</button>
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
