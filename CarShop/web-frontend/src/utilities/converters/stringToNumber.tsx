export function stringToNumber(value: string): number | undefined {
    if (typeof value !== 'string')
        return undefined;

    value = value.trim().replace(',', '.')
    if (value === '')
        return undefined;

    return Number(value);
}