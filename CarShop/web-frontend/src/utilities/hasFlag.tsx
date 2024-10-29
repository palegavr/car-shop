

export function hasFlag(bitField: number, flag: number): boolean {
    return (bitField & flag) === flag;
}