import { Injectable, Logger } from '@nestjs/common';
import { UserRoleType } from 'src/common/constants/enum/role-type-enum';
import { Connection, getManager } from 'typeorm';
import { WorkflowType } from '../../../common/constants/enum/workflow-type.enum';
import { FixAccountantRolesDTO } from './dto/fix-accountant-roles.dto';
import { FixAccountantRolesMapper } from './mappers/fix-accountant-roles.mapper';
import { AccountEnabledEnum } from 'src/common/constants/enum/account-enabled.enum';
import { EventStatus } from 'src/common/constants/enum/event-status.enum';
import { HasNeispuoAccess } from 'src/common/constants/enum/has-neispuo-access.enum';
import { RoleEnum } from 'src/common/constants/enum/role.enum';

@Injectable()
export class FixAccountantRolesRepository {
    entityManager = getManager();

    constructor(private connection: Connection) {}

    async isTableNameFree(providedTableName: string) {
        const tableNames = await this.entityManager.query(
            `
            SELECT STRING_AGG(CAST(LOWER(UniqueTableNames.TABLE_NAME) AS NVARCHAR(MAX)), ',') AS tableNames
            FROM (
                SELECT DISTINCT TABLE_NAME
                FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_TYPE = 'BASE TABLE'
            ) AS UniqueTableNames;  
            `,
        );
        const tableNamesArray = tableNames[0].tableNames.split(',');
        if (tableNamesArray.includes(providedTableName.toLowerCase())) {
            throw new Error('The provided table name already exists in the database.');
        }
    }

    async createTempTable(schemaName: string, tempTableName: string) {
        await this.entityManager.query(
            `
			CREATE TABLE ${schemaName}.${tempTableName} (
				RowID int IDENTITY(1,1) NOT NULL,
				WorkflowType int NOT NULL,
				UserID varchar(100) COLLATE Cyrillic_General_CI_AS NULL,
				FirstName varchar(255) COLLATE Cyrillic_General_CI_AS NULL,
				MiddleName varchar(255) COLLATE Cyrillic_General_CI_AS NULL,
				Surname varchar(255) COLLATE Cyrillic_General_CI_AS NULL,
				BirthDate datetime2 NULL,
                PersonID int NULL,
                Status int NOT NULL,
				HasNeispuoAccess int DEFAULT NULL NULL,
				AdditionalRole int DEFAULT NULL NULL,
				sisAccessSecondaryRole int DEFAULT NULL NULL,
				UserRole varchar(255) COLLATE Cyrillic_General_CI_AS NULL,
				AccountEnabled int NULL,
				AzureID varchar(100) COLLATE Cyrillic_General_CI_AS NULL,
				AssignedAccountantSchools nvarchar(255) COLLATE Cyrillic_General_CI_AS DEFAULT NULL NULL,
				
				CONSTRAINT PK__UsersTemp__1788CCAC998A9D4E PRIMARY KEY (RowID)
			);
			`,
        );
    }

    async getAccountantsForUpdate(schemaName: string) {
        const result = await getManager().query(
            `
            SELECT
        	DISTINCT
            	p.PersonID AS personID,
            	p.FirstName AS firstName,
            	p.MiddleName AS middleName,
            	p.LastName AS lastName,
            	p.BirthDate AS birthDate,
            	p.AzureID AS azureID,
            	p.PersonalID AS userID,
            	STRING_AGG(susr.InstitutionID,
            	',') AS institutionIDs
            FROM
            	core.Person p
            JOIN core.SysUser su
                        ON
            	p.PersonID = su.PersonID
            JOIN core.SysUserSysRole susr
                        ON
            	su.SysUserID = susr.SysUserID
            WHERE
            	susr.SysRoleID = 20
            	AND
            	su.DeletedOn IS NULL
            	AND
            	p.AzureID IS NOT NULL
				AND
				p.PersonalID IS NOT NULL
            GROUP BY
            	p.PersonID,
            	p.FirstName,
            	p.MiddleName,
            	p.LastName,
            	p.BirthDate,
            	p.AzureID,
				p.PersonalID
            `,
        );
        const transformedResult: FixAccountantRolesDTO[] = FixAccountantRolesMapper.transform(result);
        return transformedResult;
    }

    async bulkInsertAccountantsForUpdate(
        accountants: FixAccountantRolesDTO[],
        schemaName: string,
        tempTableName: string,
    ) {
        for (const accountant of accountants) {
            const query = `INSERT INTO "${schemaName}"."${tempTableName}" (
				WorkflowType,
                PersonID,
				UserID,
				FirstName,
				MiddleName,
				Surname,
				BirthDate,
				HasNeispuoAccess,
				AdditionalRole,
				sisAccessSecondaryRole,
				UserRole,
				AccountEnabled,
                Status,
				AzureID,
				AssignedAccountantSchools
			) VALUES (@0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10, @11, @12, @13, @14)`;
            try {
                await this.entityManager.query(query, [
                    WorkflowType.USER_UPDATE,
                    accountant.personID,
                    accountant.userID,
                    accountant.firstName,
                    accountant.middleName,
                    accountant.lastName,
                    new Date(accountant.birthDate).toISOString(),
                    HasNeispuoAccess.YES,
                    RoleEnum.TEACHER,
                    RoleEnum.ACCOUNTANT,
                    UserRoleType.TEACHER,
                    AccountEnabledEnum.ENABLED,
                    EventStatus.AWAITING_CREATION,
                    accountant.azureID,
                    accountant.institutionIDs,
                ]);
            } catch (error) {
                Logger.error(error);
            }
        }
    }
}
