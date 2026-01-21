import { MigrationInterface, QueryRunner } from 'typeorm';

export class CreateRIDocumentTempTable11641482096692 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            CREATE TABLE [reginst_basic].[RIDocumentTemp](
                [RIDocumentTempID] [int] NOT NULL,
                [BlobId] [int] NOT NULL,
                [MunicipalityID] [int] NOT NULL,
                CONSTRAINT [PK_RIDocumentTemp] PRIMARY KEY CLUSTERED 
               (
                   [RIDocumentTempID] ASC
               )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
   
            ) ON [PRIMARY]
            `,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `
            DROP TABLE [reginst_basic].[RIDocumentTemp];
            `,
        );
    }
}
