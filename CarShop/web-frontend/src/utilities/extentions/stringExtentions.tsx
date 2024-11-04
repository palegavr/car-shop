export {}

declare global {
    interface String {
        format: (...args: any[]) => string;
    }
}

String.prototype.format = function () {
    const args = arguments;
    return this.replace(/{([0-9]+)}/g, function (match, index) {
        return typeof args[index] == 'undefined' ? match : args[index];
    });
};