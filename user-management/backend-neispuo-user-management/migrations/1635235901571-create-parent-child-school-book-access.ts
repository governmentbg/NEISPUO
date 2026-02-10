import { MigrationInterface, QueryRunner } from 'typeorm';

export class CreateParentChildSchoolBookAccess1635235901571 implements MigrationInterface {
    name = 'CreateParentChildSchoolBookAccess1635235901571';

    public async up(queryRunner: QueryRunner): Promise<void> {
        // revert changes to relative child
        await queryRunner.query('ALTER TABLE core.RelativeChild DROP CONSTRAINT HasAccessDefaultConstraint', undefined);
        await queryRunner.query('ALTER TABLE core.RelativeChild DROP COLUMN HasAccess', undefined);
        await queryRunner.query(
            `
        CREATE TABLE [core].[ParentChildSchoolBookAccess] (
            [ParentChildSchoolBookAccessID] [int] NOT NULL IDENTITY(1,1) ,
            [ChildID] [int] NOT NULL,
            [ParentID] [int] NOT NULL,
            [HasAccess] [int] DEFAULT 0,
        CONSTRAINT [PK_ParentChildSchoolBookAccess] PRIMARY KEY CLUSTERED 
        (
            [ParentChildSchoolBookAccessID] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
        ) ON [PRIMARY]
      `,
            undefined,
        );

        await queryRunner.query(
            `ALTER TABLE [core].[ParentChildSchoolBookAccess]  WITH CHECK ADD  CONSTRAINT [FK_ParentChildSchoolBookAccess_Parent_Person] FOREIGN KEY([ParentID])
        REFERENCES [core].[Person] ([PersonID])`,
            undefined,
        );
        await queryRunner.query(
            `ALTER TABLE [core].[ParentChildSchoolBookAccess]  WITH CHECK ADD  CONSTRAINT [FK_ParentChildSchoolBookAccess_Child_Person] FOREIGN KEY([ChildID])
        REFERENCES [core].[Person] ([PersonID])`,
            undefined,
        );
        await queryRunner.query(
            `ALTER TABLE [core].[ParentChildSchoolBookAccess]  ADD CONSTRAINT ChildIDParentIDUniqueConstraint UNIQUE(ParentID, ChildID);`,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            'ALTER TABLE core.RelativeChild ADD HasAccess INT CONSTRAINT HasAccessDefaultConstraint  DEFAULT 0',
            undefined,
        );
        await queryRunner.query(
            'ALTER TABLE core.ParentChildSchoolBookAccess DROP CONSTRAINT ChildIDParentIDUniqueConstraint',
            undefined,
        );
        await queryRunner.query(
            'ALTER TABLE core.ParentChildSchoolBookAccess DROP CONSTRAINT FK_ParentChildSchoolBookAccess_Child_Person',
            undefined,
        );
        await queryRunner.query(
            'ALTER TABLE core.ParentChildSchoolBookAccess DROP CONSTRAINT FK_ParentChildSchoolBookAccess_Parent_Person',
            undefined,
        );
        await queryRunner.query(`DROP TABLE [core].[ParentChildSchoolBookAccess]`, undefined);
    }
}
