import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddTownMunicipalityAndRegion1604494972411 implements MigrationInterface {
    name = 'AddTownMunicipalityAndRegion1604494972411';

    public async up(queryRunner: QueryRunner): Promise<any> {
        await queryRunner.query(
            'CREATE TABLE `town` (`uuid` varchar(36) NOT NULL, `name` varchar(255) NOT NULL, `type` varchar(255) NOT NULL, `ekatte` varchar(255) NOT NULL, `createdAt` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6), `updatedAt` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6), `municipalityUuid` varchar(36) NULL, PRIMARY KEY (`uuid`)) ENGINE=InnoDB',
            undefined
        );
        await queryRunner.query(
            'CREATE TABLE `region` (`uuid` varchar(36) NOT NULL, `name` varchar(255) NOT NULL, `ekatte` varchar(255) NOT NULL, `createdAt` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6), `updatedAt` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6), PRIMARY KEY (`uuid`)) ENGINE=InnoDB',
            undefined
        );
        await queryRunner.query(
            'CREATE TABLE `municipality` (`uuid` varchar(36) NOT NULL, `name` varchar(255) NOT NULL, `ekatte` varchar(255) NOT NULL, `createdAt` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6), `updatedAt` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6), `regionUuid` varchar(36) NULL, PRIMARY KEY (`uuid`)) ENGINE=InnoDB',
            undefined
        );
        await queryRunner.query(
            "ALTER TABLE `user` CHANGE `fullName` `fullName` varchar(1024) AS (concat(`firstName`,_utf8mb4' ',`middleName`,_utf8mb4' ',`lastName`)) VIRTUAL NOT NULL",
            undefined
        );
        await queryRunner.query(
            'ALTER TABLE `town` ADD CONSTRAINT `FK_6bde1e88594b3bf4e740e3b1e3a` FOREIGN KEY (`municipalityUuid`) REFERENCES `municipality`(`uuid`) ON DELETE NO ACTION ON UPDATE NO ACTION',
            undefined
        );
        await queryRunner.query(
            'ALTER TABLE `municipality` ADD CONSTRAINT `FK_57e8b9b3d03fe0de998f37eaf5b` FOREIGN KEY (`regionUuid`) REFERENCES `region`(`uuid`) ON DELETE NO ACTION ON UPDATE NO ACTION',
            undefined
        );
    }

    public async down(queryRunner: QueryRunner): Promise<any> {
        await queryRunner.query(
            'ALTER TABLE `municipality` DROP FOREIGN KEY `FK_57e8b9b3d03fe0de998f37eaf5b`',
            undefined
        );
        await queryRunner.query(
            'ALTER TABLE `town` DROP FOREIGN KEY `FK_6bde1e88594b3bf4e740e3b1e3a`',
            undefined
        );
        await queryRunner.query(
            "ALTER TABLE `user` CHANGE `fullName` `fullName` varchar(1024) AS (concat(`firstName`,_utf8mb4' ',`middleName`,_utf8mb4' ',`lastName`)) VIRTUAL NOT NULL",
            undefined
        );
        await queryRunner.query('DROP TABLE `municipality`', undefined);
        await queryRunner.query('DROP TABLE `region`', undefined);
        await queryRunner.query('DROP TABLE `town`', undefined);
    }
}
