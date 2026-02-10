import { MigrationInterface, QueryRunner } from 'typeorm';

export class ChangeUsernameGenQuery1665149934949 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            DROP FUNCTION [azure_temp].TRANSLATE_CYRILIC_TO_LATIN;
            `,
            undefined,
        );
        await queryRunner.query(
            `
            CREATE FUNCTION [azure_temp].TRANSLATE_CYRILIC_TO_LATIN(@string nvarchar(MAX))
                RETURNS TABLE
                AS RETURN(
                    SELECT
                    REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
                    REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
                    REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
                    REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
                    REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(TRIM(
                    @string),
                    N'@', N''),N'0', N''),N'1', N''),N'2', N''),N'3', N''),N'4', N''),N'5', N''),N'6', N''),N'7', N''),N'8', N''),N'9', N''),N'&', N''),N'=', N''),N'{', N''),N'}', N''),N'[', N''),N']', N''),N'<', N''),N'>', N''),N'\', N''),N'|', N''),N';', N''),N';', N''),N':', N''),N',', N''),N'_', N''),N')', N''),N'(', N''),N'^', N''),N'%', N''),N'$', N''),N'!', N''),N'#', N''),N'?', N''),N'*', N''),N'+', N''),N'/', N''),N'"', N''),N'   ', N' '),N'  ', N' '),N' - ', N'-'),N' -', N'-'),N'- ', N'-'),N' ', N'.'), N'ия', N'ia'), N'ый', N'y'), N'ЫЙ', N'Y'), N'а', N'a'), N'б', N'b'), N'в', N'v'), N'г', N'g'), N'д', N'd'), N'е', N'e'), N'ё', N'yo'), N'ж', N'zh'), N'з', N'z'), N'и', N'i'), N'й', N'i'), N'к', N'k'), N'л', N'l'), N'м', N'm'), N'н', N'n'), N'о', N'o'), N'п', N'p'), N'р', N'r'), N'с', N's'), N'т', N't'), N'у', N'u'), N'ф', N'f'), N'х', N'h'), N'ц', N'ts'), N'ч', N'ch'), N'ш', N'sh'), N'щ', N'shch'), N'ъ', N'a'), N'ы', N'yi'), N'ь', N'i'), N'э', N'e'), N'ю', N'iu'), N'я', N'ia'), N'А', N'A'), N'Б', N'B'), N'В', N'V'), N'Г', N'G'), N'Д', N'D'), N'Е', N'E'), N'Ё', N'YO'), N'Ж', N'ZH'), N'З', N'Z'), N'И', N'I'), N'Й', N'I'), N'К', N'K'), N'Л', N'L'), N'М', N'M'), N'Н', N'N'), N'О', N'O'), N'П', N'P'), N'Р', N'R'), N'С', N'S'), N'Т', N'T'), N'У', N'U'), N'Ф', N'F'), N'Х', N'H'), N'Ц', N'TS'), N'Ч', N'CH'), N'Ш', N'SH'), N'Щ', N'SHCH'), N'Ъ', N'A'), N'Ы', N'YE'), N'Ь', N'I'), N'Э', N'E'), N'Ю', N'IU'), N'Я', N'IA') AS RESULT
                );
            `,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            DROP FUNCTION [azure_temp].TRANSLATE_CYRILIC_TO_LATIN;
            `,
            undefined,
        );
        await queryRunner.query(
            `
            CREATE FUNCTION [azure_temp].TRANSLATE_CYRILIC_TO_LATIN(@string nvarchar(MAX))
            RETURNS TABLE
            AS RETURN(
            SELECT
                REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(@string, N'ия', N'ia'), N'ый', N'y'), N'ЫЙ', N'Y'), N'а', N'a'), N'б', N'b'), N'в', N'v'), N'г', N'g'), N'д', N'd'), N'е', N'e'), N'ё', N'yo'), N'ж', N'zh'), N'з', N'z'), N'и', N'i'), N'й', N'i'), N'к', N'k'), N'л', N'l'), N'м', N'm'), N'н', N'n'), N'о', N'o'), N'п', N'p'), N'р', N'r'), N'с', N's'), N'т', N't'), N'у', N'u'), N'ф', N'f'), N'х', N'h'), N'ц', N'ts'), N'ч', N'ch'), N'ш', N'sh'), N'щ', N'shch'), N'ъ', N'a'), N'ы', N'yi'), N'ь', N'i'), N'э', N'e'), N'ю', N'iu'), N'я', N'ia'), N'А', N'A'), N'Б', N'B'), N'В', N'V'), N'Г', N'G'), N'Д', N'D'), N'Е', N'E'), N'Ё', N'YO'), N'Ж', N'ZH'), N'З', N'Z'), N'И', N'I'), N'Й', N'I'), N'К', N'K'), N'Л', N'L'), N'М', N'M'), N'Н', N'N'), N'О', N'O'), N'П', N'P'), N'Р', N'R'), N'С', N'S'), N'Т', N'T'), N'У', N'U'), N'Ф', N'F'), N'Х', N'H'), N'Ц', N'TS'), N'Ч', N'CH'), N'Ш', N'SH'), N'Щ', N'SHCH'), N'Ъ', N'A'), N'Ы', N'YE'), N'Ь', N'I'), N'Э', N'E'), N'Ю', N'IU'), N'Я', N'IA') AS RESULT
            );
            `,
            undefined,
        );
    }
}
