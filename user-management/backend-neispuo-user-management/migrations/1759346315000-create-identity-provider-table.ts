import { MigrationInterface, QueryRunner } from 'typeorm';

export class CreateIdentityProviderTable1759346315000 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            CREATE TABLE core.IdentityProvider (
                IdentityProviderID int IDENTITY(1,1) NOT NULL,
                Name nvarchar(255) COLLATE Cyrillic_General_CI_AS NOT NULL,
                CONSTRAINT PK_IdentityProvider PRIMARY KEY CLUSTERED (IdentityProviderID ASC)
            );

            CREATE UNIQUE INDEX UX_IdentityProvider_Name ON core.IdentityProvider (Name);
            `,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            ` 
            DROP TABLE core.IdentityProvider;
            `,
        );
    }
}
