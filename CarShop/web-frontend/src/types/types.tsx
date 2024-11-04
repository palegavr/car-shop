import {hasFlag} from "@/utilities/hasFlag";

export type ProcessData = {
    brand: string,
    model: string,
    price: number,
    color: string,
    engineCapacity: number,
    corpusType: CorpusType,
    fuelType: FuelTypes,
    count: number,
    imageUrl: string,
    bigImageUrls: string[],
    additionalCarOptions: AdditionalCarOption[]
}

export enum CorpusType {
    Sedan = 0,
    Hatchback = 1
}
export function corpusTypeToString(corpusType: CorpusType): string {
    switch (corpusType) {
        case CorpusType.Sedan: return 'Седан';
        case CorpusType.Hatchback: return 'Хетчбек';
    }
}

export type FuelTypes = 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | 10 | 11 | 12 | 13 | 14 | 15;
export enum FuelType {
    Petrol = 1,
    Diesel = 2,
    Gas = 4,
    Electric = 8
}

export function fuelTypesToString(fuelTypes: FuelTypes, separator: string = ', '): string {
    let result: string[] = [];

    if (hasFlag(fuelTypes, FuelType.Petrol)){
        result.push('Бензин');
    }

    if (hasFlag(fuelTypes, FuelType.Diesel)){
        result.push('Дизель');
    }

    if (hasFlag(fuelTypes, FuelType.Gas)){
        result.push('Газ');
    }

    if (hasFlag(fuelTypes, FuelType.Electric)){
        result.push('Электрика');
    }

    return result.join(separator);
}

export type AdditionalCarOption = {
    type: AdditionalCarOptionType,
    price: number,
    isRequired: boolean,
}

export enum AdditionalCarOptionType {
    AirConditioner = 0,
    HeatedDriversSeat = 1,
    SeatHeightAdjustment = 2,
    DifferentCarColor = 3
}

export function additionalCarOptionTypeToString(type : AdditionalCarOptionType): string {
    switch (type) {
        case AdditionalCarOptionType.AirConditioner: return 'Кондиционер';
        case AdditionalCarOptionType.HeatedDriversSeat: return 'Подогрев сидения водителя';
        case AdditionalCarOptionType.SeatHeightAdjustment: return 'Регулировка сидения водителя по высоте';
        case AdditionalCarOptionType.DifferentCarColor: return 'Другой цвет авто';
    }
}