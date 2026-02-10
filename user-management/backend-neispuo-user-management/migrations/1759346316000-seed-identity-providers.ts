import { MigrationInterface, QueryRunner } from 'typeorm';

export class SeedIdentityProviders1759346316000 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            INSERT INTO core.IdentityProvider (Name)
            VALUES (N'AZURE'), (N'GOOGLE'), (N'FACEBOOK');
            `,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            DELETE FROM core.IdentityProvider WHERE Name IN (N'AZURE', N'GOOGLE', N'FACEBOOK');
            `,
        );
    }
}


