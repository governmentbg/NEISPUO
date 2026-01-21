import { MigrationInterface, QueryRunner } from 'typeorm';

export class UpdateRIInstitutionsView1694613055338 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `ALTER VIEW [reginst_basic].[RIInstitutionLatestActive] AS WITH latest_active AS (

    SELECT

        ROW_NUMBER() OVER (PARTITION BY RIprocedure.InstitutionID ORDER BY RIprocedure.Ord DESC) AS RowNumber,

        RIinstitution.*,

        ProcedureType.ProcedureTypeID AS _ProcedureTypeID,

        TransformType.TransformTypeID AS _TransformTypeID

    FROM

        reginst_basic.RIInstitution RIInstitution

    LEFT JOIN reginst_basic.RIProcedure RIProcedure ON

        RIInstitution.RIprocedureID = RIProcedure.RIprocedureID

    LEFT JOIN reginst_nom.ProcedureType ProcedureType ON

        RIProcedure.ProcedureTypeID = ProcedureType.ProcedureTypeID

    LEFT JOIN reginst_nom.TransformType TransformType ON

        RIProcedure.TransformTypeID = TransformType.TransformTypeID

)      
    SELECT * FROM
      latest_active
        WHERE _ProcedureTypeID !=3 AND
          RowNumber=1 AND
          _TransformTypeID NOT IN (1, 11, 12, 13, 14, 9)     
   `
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `ALTER VIEW [reginst_basic].[RIInstitutionLatestActive] AS WITH latest_active AS (

    SELECT

        ROW_NUMBER() OVER (PARTITION BY RIprocedure.InstitutionID ORDER BY RIprocedure.Ord DESC) AS RowNumber,

        RIinstitution.*,

        ProcedureType.ProcedureTypeID AS _ProcedureTypeID,

        TransformType.TransformTypeID AS _TransformTypeID

    FROM

        reginst_basic.RIInstitution RIInstitution

    LEFT JOIN reginst_basic.RIProcedure RIProcedure ON

        RIInstitution.RIprocedureID = RIProcedure.RIprocedureID

    LEFT JOIN reginst_nom.ProcedureType ProcedureType ON

        RIProcedure.ProcedureTypeID = ProcedureType.ProcedureTypeID

    LEFT JOIN reginst_nom.TransformType TransformType ON

        RIProcedure.TransformTypeID = TransformType.TransformTypeID

)

    SELECT * FROM
      latest_active
        WHERE
         RowNumber = 1
         AND ((_ProcedureTypeID !=3) OR _TransformTypeID NOT IN (1, 11, 12, 13, 14, 9))
    `
        );
    }
}
