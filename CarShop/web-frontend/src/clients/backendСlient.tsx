import {ProcessData} from "@/types/types";
import {encodeProcessDataToJson} from "@/utilities/processDataOperations";


type UploadImageResult = {
    success: boolean,
    publicImageUrls?: string[]
}
export async function uploadImageAsync(files: File[]): Promise<UploadImageResult> {
    const formData = new FormData();
    files.forEach(file => formData.append('images', file));
    try {
        const response = await fetch('/api/admin/uploadimage', {
            method: 'POST',
            body: formData,
            credentials: 'same-origin',
        })

        if (response.ok) {
            return {success: true, publicImageUrls: (await response.json()) as string[]}
        } else {
            return {success: false}
        }
    } catch (error) {
        return {success: false}
    }
}

type ApplyChangesResult = {
    success: boolean,
}
export async function applyChangesAsync(carId: number): Promise<ApplyChangesResult> {
    try {
        const response = await fetch(`/api/admin/editcar/${carId}/applychanges`, {
            method: 'POST',
            credentials: 'same-origin',
        });

        return {success: response.ok}
    } catch (error) {
        return {success: false}
    }
}

type PushProcessDataResult = {
    success: boolean,
}
export async function pushProcessData(carId: number, processData: ProcessData): Promise<PushProcessDataResult> {
    try {
        const response = await fetch(`/api/admin/editcar/${carId}/process`, {
            method: 'POST',
            credentials: 'same-origin',
            headers: {
                'Content-Type': 'application/json'
            },
            body: encodeProcessDataToJson(processData)
        });

        return {success: response.ok}
    } catch (error) {
        return {success: false}
    }
}