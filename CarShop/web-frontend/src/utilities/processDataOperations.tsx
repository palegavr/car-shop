import {ProcessData} from "@/types/types";
import {camelToSnake, snakeToCamel} from "@/utilities/caseConverters";

export function parseJsonEncodedProcessData(encodedProcessData: string): ProcessData {
    let processData: ProcessData = JSON.parse(encodedProcessData);

    processData = Object.fromEntries(
        Object.entries(processData).map(([key, value]) => [snakeToCamel(key), value])
    ) as ProcessData;

    processData.additionalCarOptions = JSON.parse(processData.additionalCarOptions as unknown as string);

    return processData;
}

export function encodeProcessDataToJson(processData: ProcessData): string {
    let processDataForEncoding: ProcessData = JSON.parse(JSON.stringify(processData));

    // @ts-ignore
    processDataForEncoding.additionalCarOptions = JSON.stringify(processDataForEncoding.additionalCarOptions);

    processDataForEncoding = Object.fromEntries(
        Object.entries(processDataForEncoding).map(([key, value]) => [camelToSnake(key), value])
    ) as ProcessData;

    return JSON.stringify(processDataForEncoding);
}