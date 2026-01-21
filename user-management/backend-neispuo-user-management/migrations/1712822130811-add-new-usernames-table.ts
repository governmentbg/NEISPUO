import {MigrationInterface, QueryRunner} from "typeorm";

export class AddNewUsernamesTable1712822130811 implements MigrationInterface {

    public async up(queryRunner: QueryRunner): Promise<void> {
        const queryStr = `
            CREATE TABLE azure_temp.NewUsernames (
                old_username varchar(100) COLLATE Cyrillic_General_CI_AS NOT NULL,
                person_id varchar(100) COLLATE Cyrillic_General_CI_AS NULL,
                azure_id varchar(100) COLLATE Cyrillic_General_CI_AS NULL,
                new_username varchar(100) COLLATE Cyrillic_General_CI_AS NULL,
                message varchar(100) COLLATE Cyrillic_General_CI_AS NULL,
                new_edu varchar(100) COLLATE Cyrillic_General_CI_AS NULL,
                old_edu varchar(100) COLLATE Cyrillic_General_CI_AS NULL,
                CONSTRAINT NewUsernames_PK PRIMARY KEY (old_username)
            );`

        await queryRunner.query(queryStr);
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        const queryStr = `DROP TABLE azure_temp.NewUsernames;`
        await queryRunner.query(queryStr);
    }

}
