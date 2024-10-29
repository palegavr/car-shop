
export function snakeToCamel(snakeStr: string) {
    return snakeStr
        .toLowerCase()
        .replace(/_./g, (match) => match.charAt(1).toUpperCase());
}

export function camelToSnake(camelStr: string): string {
    return camelStr
        .replace(/([a-z])([A-Z])/g, '$1_$2') // Вставляем "_" перед заглавной буквой
        .toLowerCase(); // Преобразуем всю строку в нижний регистр
}
