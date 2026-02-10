import { CONSTANTS } from "src/common/constants/constants";
import {MigrationInterface, QueryRunner} from "typeorm";

export class AddVariablesTable1678795240152 implements MigrationInterface {

    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            CREATE TABLE azure_temp.Variables (
                Name varchar(100)     NOT NULL,
                Value varchar(100)     NULL
            );
            `,
        );
        await queryRunner.query(
            `
            INSERT INTO  
                AZURE_TEMP.VARIABLES (NAME,VALUE) 
            VALUES
                (N'JOBS_TOP',N'${CONSTANTS.JOBS_VARIABLES_DEFAULT_TOP}');
            `,
        );
    }
    

    
    public async down(queryRunner: QueryRunner): Promise<void> { 
        await queryRunner.query(
            `
            DROP TABLE azure_temp.Variables;
            `,
        );
    }

}
