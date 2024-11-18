

export enum CopyResult {
    Success,
    Fail
}

export async function copyToClipboard(text: string): Promise<CopyResult> {
    try {
        await navigator.clipboard.writeText(text);
        return CopyResult.Success;
    } catch {
        return CopyResult.Fail;
    }
}
