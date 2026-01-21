import { SysRoleEnum } from 'src/shared/enums/role.enum';
import { SysSchemaEnum } from 'src/shared/enums/schemas.enum';
import { MigrationInterface, QueryRunner } from 'typeorm';

export class renameHelpdeskDBViewCreation1666262505635
  implements MigrationInterface
{
  public async up(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(`
        INSERT INTO "reporting"."SchemaRoleAccess"("SchemaName", "AllowedSysRole")
        VALUES
        ('${SysSchemaEnum.R_HELPDESK}', ${SysRoleEnum.MON_ADMIN});`);

    await queryRunner.query(
      `CREATE VIEW "reporting"."R_Helpdesk"
            AS SELECT 
            "i"."Id" AS "IssueID",
            "i"."Title" AS "IssueTitle",
            "i"."Description" AS "IssueDescription",
            "i"."CategoryId" AS "IssueCategoryId",
            "i"."SubcategoryId" AS "IssueSubcategoryId",
            "i"."PriorityId" AS "IssuePriorityId",
            "i"."StatusId" AS "IssueStatusId",
            "i"."IsEscalated" AS "IssueIsEscalated",
            "i"."SchoolYear" AS "IssueSchoolYear",
            "i"."ResolveDate" AS "IssueResolveDate",
            "i"."CreateDate" AS "IssueCreateDate",
            "i"."ModifyDate" AS "IssueModifyDate",
            "i"."IsLevel3Support" AS "IssueIsLevel3Support",
            "i"."Phone" AS "IssuePhone",
            "s"."Name" AS "StatusName",
            "s"."Code" AS "StatusCode",
            "p"."Name" AS "PriorityName",
            "p"."Code" AS "PriorityCode",
            "c"."Name" AS "CategoryName",
            "c"."Code" AS "CategoryCode",
            "sc"."Name" AS "SubCategoryName",
            "sc"."Code" AS "SubCategoryCode",
            "su"."Username" AS "IssueSubmitterUsername",
            "sr"."Name" AS "IssueSubmitterRole",
            "au"."Username" AS "IssueAssignedToUsername",
            "cu"."Username" AS "IssueCreatedByUsername",
            "mu"."username" AS "IssueModifiedByUsername",
            "i"."InstitutionId" AS "InstitutionId",
            "isy"."Name" AS "InsitutionName"
            FROM "helpdesk"."Issue" "i" 
            LEFT JOIN "helpdesk"."Status" "s" ON "i"."StatusId" = "s"."Id" 
            LEFT JOIN "helpdesk"."Priority" "p" ON "i"."PriorityId" = "p"."Id"
            LEFT JOIN "helpdesk"."Category" "c" ON "i"."CategoryId" = "c"."Id" 
            LEFT JOIN "helpdesk"."Category" "sc" ON "i"."SubcategoryId" = "sc"."Id"
            LEFT JOIN "core"."SysUser" "su" ON "i"."SubmitterSysUserId" = "su"."SysUserID" 
            LEFT JOIN "core"."SysRole" "sr" ON "i"."SubmitterSysRoleId " = "sr"."SysRoleID" 
            LEFT JOIN "core"."SysUser" "au" ON "i"."AssignedToSysUserId" = "au"."SysUserID"
            LEFT JOIN "core"."SysUser" "cu" ON "i"."CreatedBySysUserID" = "cu"."SysUserID" 
            LEFT JOIN "core"."SysUser" "mu" ON "i"."CreatedBySysUserID" = "mu"."SysUserID" 
            LEFT JOIN "core"."InstitutionSchoolYear" "isy" ON "i"."InstitutionId" = "isy"."InstitutionId" AND "i"."SchoolYear" = "isy"."SchoolYear" 
            `,
    );
  }

  public async down(queryRunner: QueryRunner): Promise<void> {}
}
