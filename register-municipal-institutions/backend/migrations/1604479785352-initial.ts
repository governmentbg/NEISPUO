import { MigrationInterface, QueryRunner } from 'typeorm';

export class Initial1604479785352 implements MigrationInterface {
    name = 'Initial1604479785352';

    public async up(queryRunner: QueryRunner): Promise<any> {
        await queryRunner.query(
            'CREATE TABLE `file` (`id` varchar(36) NOT NULL, `name` varchar(255) NOT NULL, `fsName` varchar(255) NOT NULL, `mimeType` varchar(255) NOT NULL, `contentMd5` varchar(255) NOT NULL, `fileSize` int NOT NULL, `multerMetadata` json NULL, `createdAt` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6), `updatedAt` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6), `userId` varchar(36) NOT NULL, PRIMARY KEY (`id`)) ENGINE=InnoDB',
            undefined
        );
        await queryRunner.query(
            'CREATE TABLE `role` (`id` varchar(36) NOT NULL, `createdAt` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6), `updatedAt` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6), `roleName` varchar(255) NOT NULL, PRIMARY KEY (`id`)) ENGINE=InnoDB',
            undefined
        );
        await queryRunner.query(
            'CREATE TABLE `forgot_password` (`id` varchar(36) NOT NULL, `token` varchar(32) NOT NULL, `used` tinyint NOT NULL DEFAULT 0, `createdAt` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6), `updatedAt` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6), `userId` varchar(36) NOT NULL, PRIMARY KEY (`id`)) ENGINE=InnoDB',
            undefined
        );
        await queryRunner.query(
            'CREATE TABLE `failed_email_delivery` (`id` varchar(36) NOT NULL, `createdAt` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6), `updatedAt` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6), `sender` varchar(255) NOT NULL, `recipient` varchar(255) NOT NULL, `subject` varchar(255) NOT NULL, `body` varchar(255) NOT NULL, `error` text NOT NULL, `userId` varchar(36) NULL, PRIMARY KEY (`id`)) ENGINE=InnoDB',
            undefined
        );
        await queryRunner.query(
            "CREATE TABLE `user` (`id` varchar(36) NOT NULL, `password` varchar(255) NOT NULL, `email` varchar(255) NOT NULL, `firstName` varchar(255) NOT NULL, `middleName` varchar(255) NOT NULL, `lastName` varchar(255) NOT NULL, `fullName` varchar(1024) AS (concat(`firstName`,_utf8mb4' ',`middleName`,_utf8mb4' ',`lastName`)) VIRTUAL NOT NULL, `address` varchar(255) NOT NULL, `phone` varchar(255) NOT NULL, `emailVerified` tinyint NOT NULL DEFAULT 0, `emailVerificationToken` varchar(255) NOT NULL DEFAULT '', `createdAt` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6), `updatedAt` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6), `deletedAt` datetime NULL, UNIQUE INDEX `IDX_e12875dfb3b1d92d7d7c5377e2` (`email`), PRIMARY KEY (`id`)) ENGINE=InnoDB",
            undefined
        );
        await queryRunner.query(
            'CREATE TABLE `audit` (`id` int NOT NULL AUTO_INCREMENT, `createdAt` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6), `updatedAt` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6), `statusCode` int NOT NULL, `originalUrl` varchar(255) NOT NULL, `method` varchar(255) NOT NULL, `requestBody` longtext NOT NULL, `requestHeaders` longtext NOT NULL, `responseBody` longtext NOT NULL, `userId` varchar(36) NULL, PRIMARY KEY (`id`)) ENGINE=InnoDB',
            undefined
        );
        await queryRunner.query(
            'CREATE TABLE `setting` (`id` varchar(36) NOT NULL, `type` int NOT NULL, `value` json NOT NULL, `createdAt` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6), `updatedAt` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6), UNIQUE INDEX `IDX_7b4ebbcaa2308c27e982d8bf58` (`type`), PRIMARY KEY (`id`)) ENGINE=InnoDB',
            undefined
        );
        await queryRunner.query(
            'CREATE TABLE `user_roles_role` (`userId` varchar(36) NOT NULL, `roleId` varchar(36) NOT NULL, INDEX `IDX_5f9286e6c25594c6b88c108db7` (`userId`), INDEX `IDX_4be2f7adf862634f5f803d246b` (`roleId`), PRIMARY KEY (`userId`, `roleId`)) ENGINE=InnoDB',
            undefined
        );
        await queryRunner.query(
            'ALTER TABLE `file` ADD CONSTRAINT `FK_b2d8e683f020f61115edea206b3` FOREIGN KEY (`userId`) REFERENCES `user`(`id`) ON DELETE NO ACTION ON UPDATE NO ACTION',
            undefined
        );
        await queryRunner.query(
            'ALTER TABLE `forgot_password` ADD CONSTRAINT `FK_dba25590105b78ad1a6adfbc6ae` FOREIGN KEY (`userId`) REFERENCES `user`(`id`) ON DELETE NO ACTION ON UPDATE NO ACTION',
            undefined
        );
        await queryRunner.query(
            'ALTER TABLE `failed_email_delivery` ADD CONSTRAINT `FK_305e542502233f1ef9ff3c2becb` FOREIGN KEY (`userId`) REFERENCES `user`(`id`) ON DELETE NO ACTION ON UPDATE NO ACTION',
            undefined
        );
        await queryRunner.query(
            'ALTER TABLE `audit` ADD CONSTRAINT `FK_7ae389e858ad6f2c0c63112e387` FOREIGN KEY (`userId`) REFERENCES `user`(`id`) ON DELETE NO ACTION ON UPDATE NO ACTION',
            undefined
        );
        await queryRunner.query(
            'ALTER TABLE `user_roles_role` ADD CONSTRAINT `FK_5f9286e6c25594c6b88c108db77` FOREIGN KEY (`userId`) REFERENCES `user`(`id`) ON DELETE CASCADE ON UPDATE NO ACTION',
            undefined
        );
        await queryRunner.query(
            'ALTER TABLE `user_roles_role` ADD CONSTRAINT `FK_4be2f7adf862634f5f803d246b8` FOREIGN KEY (`roleId`) REFERENCES `role`(`id`) ON DELETE CASCADE ON UPDATE NO ACTION',
            undefined
        );
    }

    public async down(queryRunner: QueryRunner): Promise<any> {
        await queryRunner.query(
            'ALTER TABLE `user_roles_role` DROP FOREIGN KEY `FK_4be2f7adf862634f5f803d246b8`',
            undefined
        );
        await queryRunner.query(
            'ALTER TABLE `user_roles_role` DROP FOREIGN KEY `FK_5f9286e6c25594c6b88c108db77`',
            undefined
        );
        await queryRunner.query(
            'ALTER TABLE `audit` DROP FOREIGN KEY `FK_7ae389e858ad6f2c0c63112e387`',
            undefined
        );
        await queryRunner.query(
            'ALTER TABLE `failed_email_delivery` DROP FOREIGN KEY `FK_305e542502233f1ef9ff3c2becb`',
            undefined
        );
        await queryRunner.query(
            'ALTER TABLE `forgot_password` DROP FOREIGN KEY `FK_dba25590105b78ad1a6adfbc6ae`',
            undefined
        );
        await queryRunner.query(
            'ALTER TABLE `file` DROP FOREIGN KEY `FK_b2d8e683f020f61115edea206b3`',
            undefined
        );
        await queryRunner.query(
            'DROP INDEX `IDX_4be2f7adf862634f5f803d246b` ON `user_roles_role`',
            undefined
        );
        await queryRunner.query(
            'DROP INDEX `IDX_5f9286e6c25594c6b88c108db7` ON `user_roles_role`',
            undefined
        );
        await queryRunner.query('DROP TABLE `user_roles_role`', undefined);
        await queryRunner.query(
            'DROP INDEX `IDX_7b4ebbcaa2308c27e982d8bf58` ON `setting`',
            undefined
        );
        await queryRunner.query('DROP TABLE `setting`', undefined);
        await queryRunner.query('DROP TABLE `audit`', undefined);
        await queryRunner.query('DROP INDEX `IDX_e12875dfb3b1d92d7d7c5377e2` ON `user`', undefined);
        await queryRunner.query('DROP TABLE `user`', undefined);
        await queryRunner.query('DROP TABLE `failed_email_delivery`', undefined);
        await queryRunner.query('DROP TABLE `forgot_password`', undefined);
        await queryRunner.query('DROP TABLE `role`', undefined);
        await queryRunner.query('DROP TABLE `file`', undefined);
    }
}
