import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddSysUserIdentityProviderFk1759346317000 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            ALTER TABLE core.SysUser ADD IdentityProviderID int NULL;
            ALTER TABLE core.SysUser ADD IdentityProviderUserID nvarchar(255) COLLATE Cyrillic_General_CI_AS NULL;

            ALTER TABLE core.SysUser 
            ADD CONSTRAINT FK_SysUser_IdentityProvider FOREIGN KEY (IdentityProviderID)
            REFERENCES core.IdentityProvider(IdentityProviderID);
            `,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            ALTER TABLE core.SysUser DROP CONSTRAINT FK_SysUser_IdentityProvider;

            ALTER TABLE core.SysUser DROP COLUMN IdentityProviderUserID;
            ALTER TABLE core.SysUser DROP COLUMN IdentityProviderID;
            `,
        );
    }
}


