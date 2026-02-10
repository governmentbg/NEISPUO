import { MigrationInterface, QueryRunner } from 'typeorm';

export class AlterRIDocumentTempTable11641482096693 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            ALTER TABLE reginst_basic.RIDocumentTemp
                ADD RIDocumentTempID_New int IDENTITY(1,1) NOT NULL;

            ALTER TABLE reginst_basic.RIDocumentTemp
                DROP CONSTRAINT [PK_RIDocumentTemp]; 

            ALTER TABLE reginst_basic.RIDocumentTemp 
                DROP COLUMN RIDocumentTempID;

            EXEC sp_rename 'reginst_basic.RIDocumentTemp.RIDocumentTempID_New', 'RIDocumentTempID', 'COLUMN';

            ALTER TABLE reginst_basic.RIDocumentTemp 
                ADD CONSTRAINT PK_RIDocumentTemp PRIMARY KEY CLUSTERED ([RIDocumentTempID] ASC)
            `,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            ALTER TABLE reginst_basic.RIDocumentTemp
                ADD RIDocumentTempID_Old int NOT NULL;

            ALTER TABLE reginst_basic.RIDocumentTemp
                DROP CONSTRAINT [PK_RIDocumentTemp]; 

            ALTER TABLE reginst_basic.RIDocumentTemp 
                DROP COLUMN RIDocumentTempID;

            EXEC sp_rename 'reginst_basic.RIDocumentTemp.RIDocumentTempID_New', 'RIDocumentTempID', 'COLUMN';

            ALTER TABLE reginst_basic.RIDocumentTemp 
                ADD CONSTRAINT PK_RIDocumentTemp PRIMARY KEY CLUSTERED ([RIDocumentTempID] ASC)
            
            `,
        );
    }
}
