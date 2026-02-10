import { MigrationInterface, QueryRunner } from 'typeorm';

export class UpdateRIInstitutionLatestActive1663311299983 implements MigrationInterface {
    name = 'UpdateRIInstitutionLatestActive1663311299983';

    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `ALTER VIEW [reginst_basic].[RIInstitutionLatestActive] AS WITH latest_active AS (
                SELECT
                ROW_NUMBER() OVER( PARTITION BY RIprocedure.InstitutionID order by RIprocedure.Ord desc) AS RowNumber,
                    RIinstitution.*,
                    ProcedureType.ProcedureTypeID AS _ProcedureTypeID
                FROM
                    reginst_basic.RIInstitution RIInstitution
                LEFT JOIN reginst_basic.RIProcedure RIProcedure ON
                    RIInstitution.RIprocedureID = RIProcedure.RIprocedureID
                LEFT JOIN reginst_nom.ProcedureType ProcedureType ON
                    RIProcedure.ProcedureTypeID = ProcedureType.ProcedureTypeID 
            )
            SELECT 
                *
            FROM
                latest_active
            WHERE
                RowNumber = 1
                and _ProcedureTypeID != 3
        `,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `ALTER VIEW [reginst_basic].[RIInstitutionLatestActive] AS WITH latest_active AS (
            SELECT
                ROW_NUMBER() OVER( PARTITION BY RIInstitution.InstitutionID ORDER BY RIInstitution.RIInstitutionID Desc, RIInstitution.ValidFrom Desc) AS RowNumber,
                RIinstitution.*,
                ProcedureType.ProcedureTypeID AS _ProcedureTypeID
            FROM
                reginst_basic.RIInstitution RIInstitution
            LEFT JOIN reginst_basic.RIProcedure RIProcedure ON
                RIInstitution.RIprocedureID = RIProcedure.RIprocedureID
            LEFT JOIN reginst_nom.ProcedureType ProcedureType ON
                RIProcedure.ProcedureTypeID = ProcedureType.ProcedureTypeID 
        )
        SELECT 
            *
        FROM
            latest_active
        WHERE
            RowNumber = 1
            and _ProcedureTypeID != 3
    `,
        );
    }
}
