
export type NumberValidatorOptions = {
    allowFloat?: boolean,
    minNumberValue?: number,
    minNumberValueExclusive?: boolean,
    maxNumberValue?: number,
    maxNumberValueExclusive?: boolean
}

export function validateNumber(value: number, options: NumberValidatorOptions = {}): boolean {
    return typeof value === 'number' &&
        !isNaN(value) &&
        (!options.allowFloat ? Number.isInteger(value) : true) &&
        (options.minNumberValue !== undefined ? // Валидация, если указано минимальное значение
            (options.minNumberValueExclusive ? value > options.minNumberValue : value >= options.minNumberValue) : true) &&
        (options.maxNumberValue !== undefined ? // Валидация, если указано максимальное значение
            (options.maxNumberValueExclusive ? value < options.maxNumberValue : value <= options.maxNumberValue) : true)
}

