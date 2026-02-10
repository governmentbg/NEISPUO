import { Injectable } from '@nestjs/common';
import { CONSTANTS } from 'src/common/constants/constants';
import { EventStatus } from 'src/common/constants/enum/event-status.enum';
import { InProcessing } from 'src/common/constants/enum/in-processing.enum';
import { IsAzureSynced } from 'src/common/constants/enum/is-azure-synced.enum';
import { IsAzureUser } from 'src/common/constants/enum/is-azure-user.enum';
import { IsForArchivation } from 'src/common/constants/enum/is-for-archivation.enum';
import { PositionEnum } from 'src/common/constants/enum/position.enum';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { SysUserTypeEnum } from 'src/common/constants/enum/sys-user-type.enum';
import { IAzureSyncUserSearchInput } from 'src/common/dto/azure-sync-user-search-input.dto';
import { AzureOrganizationsResponseDTO } from 'src/common/dto/responses/azure-organizations-response.dto';
import { AzureUsersResponseDTO } from 'src/common/dto/responses/azure-users-response.dto';
import { InstitutionResponseDTO } from 'src/common/dto/responses/institution-response.dto';
import { PersonResponseDTO } from 'src/common/dto/responses/person-response.dto';
import { SysUserSysRoleResponseDTO } from 'src/common/dto/responses/sys-user-sys-role.response.dto';
import { SysUserResponseDTO } from 'src/common/dto/responses/sys-user.response.dto';
import { SysUserCreateDTO } from 'src/common/dto/sys-user-create.dto';
import { SysUserSysRoleCreateDTO } from 'src/common/dto/sys-user-sys-role-create.dto';
import { AzureUsersMapper } from 'src/common/mappers/azure-users.mapper';
import { InstitutionMapper } from 'src/common/mappers/institution.mapper';
import { PersonMapper } from 'src/common/mappers/person.mapper';
import { SysUserSysRoleMapper } from 'src/common/mappers/sys-user-sys-role.mapper';
import { SysUserMapper } from 'src/common/mappers/sys-user.mapper';
import { Connection, EntityManager, getManager } from 'typeorm';

@Injectable()
export class UserRepository {
    constructor(private connection: Connection) {}

    entityManager = getManager();

    async createAzureUserProcedure(dto: AzureUsersResponseDTO) {
        let result;
        const { username, password, personID, rowID, azureID } = dto;
        try {
            await this.connection.transaction(async (manager) => {
                const stopJobResult = await manager.query(
                    `
                    UPDATE
                        azure_temp.Users
                    SET
                        UpdatedOn = GETUTCDATE(),
                        Status = ${EventStatus.SYNCHRONIZED},
                        IsForArchivation = ${IsForArchivation.YES},
                        InProcessing = ${InProcessing.NO}
                    OUTPUT
                        INSERTED.RowID as rowID
                    WHERE
                        RowID = @0;
                        `,
                    [rowID],
                );
                const transformedResult: AzureUsersResponseDTO[] = AzureUsersMapper.transform(stopJobResult);
                const jobRowID = transformedResult[0].rowID;

                const insertUserResult = await manager.query(
                    `
                    INSERT INTO
                        core.SysUser (
                            Username,
                            InitialPassword,
                            PersonID,
                            IsAzureUser,
                            IsAzureSynced
                        )
                    OUTPUT 
                        INSERTED.SysUserID as sysUserID,
                        INSERTED.Username as username,
                        INSERTED.InitialPassword as initialPassword,
                        INSERTED.PersonID as personID,
                        INSERTED.IsAzureUser as isAzureUser,
                        INSERTED.IsAzureSynced as isAzureSynced
                    VALUES (
                        @0,
                        @1,
                        @2,
                        @3,
                        @4
                    );
                    `,
                    [username, password, personID, IsAzureUser.YES, IsAzureSynced.YES],
                );
                const transformedInsertUserResult: SysUserResponseDTO[] = SysUserMapper.transform(insertUserResult);
                const insertedSysUserID = +transformedInsertUserResult[0].sysUserID;

                const updatePersonResult = await manager.query(
                    `
                    UPDATE 
                        core.Person 
                    SET
                        PublicEduNumber = @0,
                        AzureID = @1
                    OUTPUT 
                        INSERTED.PersonID as personID
                    WHERE
                        PersonID = @2
                        `,
                    [username.substring(0, username.indexOf('@')), azureID, personID],
                );
                const transformedPersonResult: PersonResponseDTO[] = PersonMapper.transform(updatePersonResult);
                const updatedPersonID = transformedPersonResult[0].personID;
                dto.sysUserID = insertedSysUserID;
                await this.entityManager.query(
                    `
                    DELETE FROM azure_temp.EntitiesInGeneration
                    WHERE 
                        1=1
                        AND Identifier = @0
                    `,
                    [personID.toString()],
                );
                if (jobRowID && insertedSysUserID && updatedPersonID) result = true;
            });
        } catch (error) {
            dto.errorMessage = error?.message;
        }
        return result;
    }

    async createAzureParentProcedure(dto: AzureUsersResponseDTO) {
        let result = false;
        const { rowID, password, personID, email, azureID } = dto;
        try {
            //TODO is there anything else to do after azure create for parents?
            await this.connection.transaction(async (manager) => {
                const stopJobResult = await manager.query(
                    `
                    UPDATE
                        azure_temp.Users
                    SET
                        UpdatedOn = GETUTCDATE(),
                        Status = ${EventStatus.SYNCHRONIZED},
                        IsForArchivation = ${IsForArchivation.YES},
                        InProcessing = ${InProcessing.NO}
                    OUTPUT 
                        INSERTED.RowID as rowID
                    WHERE
                        RowID = @0;
                        `,
                    [rowID],
                );
                const transformedResult: AzureUsersResponseDTO[] = AzureUsersMapper.transform(stopJobResult);
                const jobRowID = transformedResult[0]?.rowID;

                const updateCorePersonResult = await getManager().query(
                    `
                    UPDATE
                        core.Person
                    SET
                         AzureID = @0
                    OUTPUT
                        INSERTED."PersonID" AS "personID"
                    WHERE
                        PublicEduNumber = @1
                    `,
                    [azureID, email],
                );
                const transformedUpdateCorePersonResult: PersonResponseDTO[] =
                    PersonMapper.transform(updateCorePersonResult);
                const updatedPersonID = +transformedUpdateCorePersonResult[0].personID;
                // dto.sysUserID = insertedSysUserID;
                await this.entityManager.query(
                    `
                    DELETE FROM azure_temp.EntitiesInGeneration
                    WHERE 
                        1=1
                        AND Identifier = @0
                    `,
                    [email],
                );
                if (jobRowID && updatedPersonID) result = true;
            });
        } catch (error) {
            dto.errorMessage = error?.message;
        }
        return result;
    }

    async createAzureInstitutionUserProcedure(dto: AzureOrganizationsResponseDTO) {
        let result = false;
        const { username, password, organizationID, rowID, azureID, name } = dto;
        try {
            await this.connection.transaction(async (manager) => {
                const insertPersonResult = await manager.query(
                    `
                    INSERT 
                    INTO
                    core.Person (
                        FirstName,
                        MiddleName,
                        LastName,
                        SysUserType,
                        AzureID
                    ) 
                    OUTPUT
                        INSERTED.PersonID as personID
                    VALUES (
                        @0,
                        @1,
                        @2,
                        @3,
                        @4
                    );
                    `,
                    [
                        name,
                        '', // Empty string MiddleName
                        '', // Empty string LastName
                        SysUserTypeEnum.INSTITUTION,
                        azureID,
                    ],
                );
                const transformedInsertPersonResult: PersonResponseDTO[] = PersonMapper.transform(insertPersonResult);
                const insertedPersonID = +transformedInsertPersonResult[0].personID;

                const stopJobResult = await manager.query(
                    `
                    UPDATE
                        azure_temp.Organizations
                    SET
                        UpdatedOn = GETUTCDATE(),
                        Status = ${EventStatus.SYNCHRONIZED},
                        IsForArchivation = ${IsForArchivation.YES},
                        PersonID = @1,
                        InProcessing = ${InProcessing.NO}
                    OUTPUT
                        INSERTED.RowID as rowID
                    WHERE
                        RowID = @0;
                        `,
                    [rowID, insertedPersonID],
                );
                const transformedResult: AzureUsersResponseDTO[] = AzureUsersMapper.transform(stopJobResult);
                const jobRowID = transformedResult[0].rowID;

                const insertUserResult = await manager.query(
                    `
                    INSERT INTO
                        core.SysUser (
                            Username,
                            InitialPassword,
                            PersonID,
                            IsAzureUser,
                            IsAzureSynced
                        )
                    OUTPUT 
                        INSERTED.SysUserID as sysUserID,
                        INSERTED.Username as username,
                        INSERTED.InitialPassword as initialPassword,
                        INSERTED.PersonID as personID,
                        INSERTED.IsAzureUser as isAzureUser,
                        INSERTED.IsAzureSynced as isAzureSynced
                    VALUES (
                        @0,
                        @1,
                        @2,
                        @3,
                        @4
                    );
                    `,
                    [username, password, insertedPersonID, IsAzureUser.YES, IsAzureSynced.YES],
                );
                const transformedInsertUserResult: SysUserResponseDTO[] = SysUserMapper.transform(insertUserResult);
                const insertedSysUserID = +transformedInsertUserResult[0].sysUserID;
                const insertSysUserSysRoleResult = await manager.query(
                    `
                    INSERT INTO
                        core.SysUserSysRole (
                            SysUserID,
                            SysRoleID,
                            InstitutionID
                        )
                    OUTPUT
                        INSERTED.SysUserID as sysUserID,
                        INSERTED.SysRoleID as sysRoleID,
                        INSERTED.InstitutionID as institutionID,
                        INSERTED.BudgetingInstitutionID as budgetingInstitutionID,
                        INSERTED.MunicipalityID as municipalityID,
                        INSERTED.RegionID as regionID
                    VALUES (
                        @0,
                        @1,
                        @2
                    );
                    `,
                    [insertedSysUserID, RoleEnum.INSTITUTION, +organizationID],
                );
                const transformedInsertSysUserSysRoleResult: SysUserSysRoleResponseDTO[] =
                    SysUserSysRoleMapper.transform(insertSysUserSysRoleResult);
                const insertSysUserSysRoleID = transformedInsertSysUserSysRoleResult[0].sysUserID;
                if (jobRowID && insertedPersonID && insertedSysUserID && insertSysUserSysRoleID) result = true;
            });
        } catch (error) {
            dto.errorMessage = error?.message;
        }
        return result;
    }

    async updateAzureUserStudentProcedure(dto: AzureUsersResponseDTO) {
        let result = false;
        const { rowID, personID } = dto;
        try {
            await this.connection.transaction(async (manager) => {
                const stopJobResult = await manager.query(
                    `
                    UPDATE
                        azure_temp.Users
                    SET
                        UpdatedOn = GETUTCDATE(),
                        Status = ${EventStatus.SYNCHRONIZED},
                        IsForArchivation = ${IsForArchivation.YES},
                        InProcessing = ${InProcessing.NO}
                    OUTPUT
                        INSERTED.RowID as rowID
                    WHERE
                        RowID = @0;
                        `,
                    [rowID],
                );
                const transformedResult: AzureUsersResponseDTO[] = AzureUsersMapper.transform(stopJobResult);
                const jobRowID = transformedResult[0].rowID;

                const updateSysUserResult = await manager.query(
                    `
                    UPDATE
                        core.SysUser
                    SET IsAzureSynced = @0
                    OUTPUT 
                        INSERTED."SysUserID" AS "sysUserID"
                    WHERE
                        PersonID = @1 
                    AND DeletedOn IS NULL
                    `,
                    [IsAzureSynced.YES, personID],
                );
                const transformedUpdatedUserResult: SysUserResponseDTO[] = SysUserMapper.transform(updateSysUserResult);
                const updatedSysUserID = +transformedUpdatedUserResult[0].sysUserID;
                dto.sysUserID = updatedSysUserID;
                if (jobRowID && updatedSysUserID) result = true;
            });
        } catch (error) {
            dto.errorMessage = error?.message;
        }
        return result;
    }

    async updateAzureUserTeacherProcedure(dto: AzureUsersResponseDTO) {
        let result = false;
        const { rowID, personID } = dto;
        try {
            await this.connection.transaction(async (manager) => {
                const stopJobResult = await manager.query(
                    `
                    UPDATE
                        azure_temp.Users
                    SET
                        UpdatedOn = GETUTCDATE(),
                        Status = ${EventStatus.SYNCHRONIZED},
                        IsForArchivation = ${IsForArchivation.YES},
                        InProcessing = ${InProcessing.NO}
                    OUTPUT
                        INSERTED.RowID as rowID
                    WHERE
                        RowID = @0;
                        `,
                    [rowID],
                );
                const transformedResult: AzureUsersResponseDTO[] = AzureUsersMapper.transform(stopJobResult);
                const jobRowID = transformedResult[0].rowID;

                const updateSysUserResult = await manager.query(
                    `
                    UPDATE
                        core.SysUser
                    SET IsAzureSynced = @0
                    OUTPUT 
                        INSERTED."SysUserID" AS "sysUserID"
                    WHERE
                        PersonID = @1
                        AND DeletedOn IS NULL
                    `,
                    [IsAzureSynced.YES, personID],
                );
                const transformedUpdatedUserResult: SysUserResponseDTO[] = SysUserMapper.transform(updateSysUserResult);
                const updatedSysUserID = +transformedUpdatedUserResult[0].sysUserID;
                dto.sysUserID = updatedSysUserID;
                if (jobRowID && updatedSysUserID) result = true;
            });
        } catch (error) {
            dto.errorMessage = error?.message;
        }
        return result;
    }

    async updateAzureParentProcedure(dto: AzureUsersResponseDTO) {
        let result = false;
        const { rowID, personID } = dto;
        try {
            //TODO is there anything else to do after azure create for parents?
            await this.connection.transaction(async (manager) => {
                const stopJobResult = await manager.query(
                    `
                    UPDATE
                        azure_temp.Users
                    SET
                        UpdatedOn = GETUTCDATE(),
                        Status = ${EventStatus.SYNCHRONIZED},
                        IsForArchivation = ${IsForArchivation.YES},
                        InProcessing = ${InProcessing.NO}
                    OUTPUT 
                        INSERTED.RowID as rowID
                    WHERE
                        RowID = @0;
                        `,
                    [rowID],
                );
                const transformedResult: AzureUsersResponseDTO[] = AzureUsersMapper.transform(stopJobResult);
                const jobRowID = transformedResult[0].rowID;

                const updateSysUserResult = await manager.query(
                    `
                    UPDATE
                        core.SysUser
                    SET IsAzureSynced = @0
                    OUTPUT 
                        INSERTED."SysUserID" AS "sysUserID"
                    WHERE
                        PersonID = @1
                        AND DeletedOn IS NULL
                    `,
                    [IsAzureSynced.YES, personID],
                );
                const transformedUpdatedUserResult: SysUserResponseDTO[] = SysUserMapper.transform(updateSysUserResult);
                const updatedSysUserID = transformedUpdatedUserResult[0].sysUserID;
                dto.sysUserID = updatedSysUserID;
                if (jobRowID && updatedSysUserID) result = true;
            });
        } catch (error) {
            dto.errorMessage = error?.message;
        }
        return result;
    }

    async updateAzureInstitutionUserProcedure(dto: AzureOrganizationsResponseDTO) {
        let result = false;
        const { rowID, personID } = dto;
        try {
            await this.connection.transaction(async (manager) => {
                const stopJobResult = await manager.query(
                    `
                    UPDATE
                        azure_temp.Organizations
                    SET
                        UpdatedOn = GETUTCDATE(),
                        Status = ${EventStatus.SYNCHRONIZED},
                        IsForArchivation = ${IsForArchivation.YES},
                        InProcessing = ${InProcessing.NO}
                    OUTPUT 
                        INSERTED.RowID as rowID
                    WHERE
                        RowID = @0;
                        `,
                    [rowID],
                );
                const transformedResult: AzureUsersResponseDTO[] = AzureUsersMapper.transform(stopJobResult);
                const jobRowID = transformedResult[0].rowID;

                const updateSysUserResult = await manager.query(
                    `
                    UPDATE
                        core.SysUser
                    SET IsAzureSynced = @0
                    OUTPUT 
                        INSERTED."SysUserID" AS "sysUserID"
                    WHERE
                        PersonID = @1
                        AND DeletedOn IS NULL
                    `,
                    [IsAzureSynced.YES, personID],
                );
                const transformedUpdatedUserResult: SysUserResponseDTO[] = SysUserMapper.transform(updateSysUserResult);
                const updatedSysUserID = transformedUpdatedUserResult[0].sysUserID;
                if (jobRowID && updatedSysUserID) result = true;
            });
        } catch (error) {
            dto.errorMessage = error?.message;
        }
        return result;
    }

    async deleteAzureUserTeacherProcedure(dto: AzureUsersResponseDTO) {
        const { rowID, personID } = dto;
        let result = false;
        try {
            await this.connection.transaction(async (manager) => {
                const stopJobResult = await manager.query(
                    `
                    UPDATE
                        azure_temp.Users
                    SET
                        UpdatedOn = GETUTCDATE(),
                        Status = ${EventStatus.SYNCHRONIZED},
                        IsForArchivation = ${IsForArchivation.YES},
                        InProcessing = ${InProcessing.NO}
                    OUTPUT 
                        INSERTED.RowID as rowID
                    WHERE
                        RowID = @0;
                        `,
                    [rowID],
                );
                const transformedResult: AzureUsersResponseDTO[] = AzureUsersMapper.transform(stopJobResult);
                const jobRowID = transformedResult[0].rowID;
                const updateSysUserResult = await manager.query(
                    `
                    UPDATE
                        core.SysUser
                    SET 
                        IsAzureSynced = @0,
                        DeletedOn = GETUTCDATE()
                    OUTPUT 
                        INSERTED."SysUserID" AS "sysUserID"
                    WHERE
                        PersonID = @1
                        AND DeletedOn IS NULL
                    `,
                    [IsAzureSynced.YES, personID],
                );
                const transformedUpdatedUserResult: SysUserResponseDTO[] = SysUserMapper.transform(updateSysUserResult);
                const updatedSysUserID = transformedUpdatedUserResult[0]?.sysUserID;
                dto.sysUserID = updatedSysUserID;
                const updatedPersonResult = await manager.query(
                    `
                    UPDATE
                        core.Person
                    SET 
                        AzureID = NULL
                    OUTPUT 
                        INSERTED."AzureID" AS "azureID"
                    WHERE
                        PersonID = @0
                    `,
                    [personID],
                );
                const transformedPersonResult: PersonResponseDTO[] = PersonMapper.transform(updatedPersonResult);
                if (jobRowID && transformedPersonResult && transformedPersonResult?.length > 0) result = true;
            });
        } catch (error) {
            dto.errorMessage = error?.message;
        }
        return result;
    }

    async deleteAzureUserStudentProcedure(dto: AzureUsersResponseDTO) {
        const { rowID, personID } = dto;
        let result = false;
        try {
            await this.connection.transaction(async (manager) => {
                const stopJobResult = await manager.query(
                    `
                    UPDATE
                        azure_temp.Users
                    SET
                        UpdatedOn = GETUTCDATE(),
                        Status = ${EventStatus.SYNCHRONIZED},
                        IsForArchivation = ${IsForArchivation.YES},
                        InProcessing = ${InProcessing.NO}
                    OUTPUT 
                        INSERTED.RowID as rowID
                    WHERE
                        RowID = @0;
                        `,
                    [rowID],
                );
                const transformedResult: AzureUsersResponseDTO[] = AzureUsersMapper.transform(stopJobResult);
                const jobRowID = transformedResult[0].rowID;

                const updateSysUserResult = await manager.query(
                    `
                    UPDATE
                        core.SysUser
                    SET 
                        IsAzureSynced = @0,
                        DeletedOn = GETUTCDATE()
                    OUTPUT 
                        INSERTED."SysUserID" AS "sysUserID"
                    WHERE
                        PersonID = @1
                        AND DeletedOn IS NULL
                    `,
                    [IsAzureSynced.YES, personID],
                );
                const transformedUpdatedUserResult: SysUserResponseDTO[] = SysUserMapper.transform(updateSysUserResult);
                const updatedSysUserID = +transformedUpdatedUserResult[0]?.sysUserID;
                dto.sysUserID = updatedSysUserID;

                const updatedPersonResult = await manager.query(
                    `
                    UPDATE
                        core.Person
                    SET 
                        AzureID = NULL
                    OUTPUT 
                        INSERTED."AzureID" AS "azureID"
                    WHERE
                        PersonID = @0
                    `,
                    [personID],
                );
                const transformedPersonResult: PersonResponseDTO[] = PersonMapper.transform(updatedPersonResult);
                if (jobRowID && transformedPersonResult && transformedPersonResult?.length > 0) result = true;
            });
        } catch (error) {
            dto.errorMessage = error?.message;
        }
        return result;
    }

    async deleteAzureParentProcedure(dto: AzureUsersResponseDTO) {
        let result = false;
        const { rowID, personID } = dto;
        try {
            //TODO is there anything else to do after azure create for parents?
            await this.connection.transaction(async (manager) => {
                const stopJobResult = await manager.query(
                    `
                    UPDATE
                        azure_temp.Users
                    SET
                        UpdatedOn = GETUTCDATE(),
                        Status = ${EventStatus.SYNCHRONIZED},
                        IsForArchivation = ${IsForArchivation.YES},
                        InProcessing = ${InProcessing.NO}
                    OUTPUT 
                        INSERTED.RowID as rowID
                    WHERE
                        RowID = @0;
                        `,
                    [rowID],
                );
                const transformedResult: AzureUsersResponseDTO[] = AzureUsersMapper.transform(stopJobResult);
                const jobRowID = transformedResult[0].rowID;
                const updateSysUserResult = await manager.query(
                    `
                    UPDATE
                        core.SysUser
                    SET 
                        IsAzureSynced = @0,
                        DeletedOn = GETUTCDATE()
                    OUTPUT 
                        INSERTED."SysUserID" AS "sysUserID"
                    WHERE
                        PersonID = @1
                        AND DeletedOn IS NULL
                    `,
                    [IsAzureSynced.YES, personID],
                );
                const transformedUpdatedUserResult: SysUserResponseDTO[] = SysUserMapper.transform(updateSysUserResult);
                const updatedSysUserID = +transformedUpdatedUserResult[0]?.sysUserID;
                dto.sysUserID = updatedSysUserID;
                const updatedPersonResult = await manager.query(
                    `
                    UPDATE
                        core.Person
                    SET 
                        AzureID = NULL
                    OUTPUT 
                        INSERTED."AzureID" AS "azureID"
                    WHERE
                        PersonID = @0
                    `,
                    [personID],
                );
                const transformedPersonResult: PersonResponseDTO[] = PersonMapper.transform(updatedPersonResult);
                if (jobRowID && transformedPersonResult && transformedPersonResult?.length > 0) result = true;
            });
        } catch (error) {
            dto.errorMessage = error?.message;
        }
        return result;
    }

    async deleteAzureInstitutionUserProcedure(dto: AzureOrganizationsResponseDTO) {
        const { rowID, personID, organizationID } = dto;
        let result = false;
        try {
            await this.connection.transaction(async (manager) => {
                const stopJobResult = await manager.query(
                    `
                    UPDATE
                        azure_temp.Organizations
                    SET
                        UpdatedOn = GETUTCDATE(),
                        Status = ${EventStatus.SYNCHRONIZED},
                        IsForArchivation = ${IsForArchivation.YES},
                        InProcessing = ${InProcessing.NO}
                    OUTPUT 
                        INSERTED.RowID as rowID
                    WHERE
                        RowID = @0;
                        `,
                    [rowID],
                );
                const transformedResult: AzureUsersResponseDTO[] = AzureUsersMapper.transform(stopJobResult);
                const jobRowID = transformedResult[0].rowID;
                const updateSysUserResult = await manager.query(
                    `
                    UPDATE
                        core.SysUser
                    SET 
                        IsAzureSynced = @0,
                        DeletedOn = GETUTCDATE()
                    OUTPUT 
                        INSERTED."SysUserID" AS "sysUserID"
                    WHERE
                        PersonID = @1
                        AND DeletedOn IS NULL
                    `,
                    [IsAzureSynced.YES, personID],
                );
                const transformedUpdatedUserResult: SysUserResponseDTO[] = SysUserMapper.transform(updateSysUserResult);
                const updatedSysUserID = transformedUpdatedUserResult[0].sysUserID;

                const deleteSysUserSysRoleResult = await manager.query(
                    `
                    DELETE FROM 
                        core.SysUserSysRole
                    OUTPUT 
                        DELETED."SysUserID" AS "sysUserID"
                    WHERE 
                        InstitutionID = @0
                    `,
                    [organizationID],
                );

                const deleteInstitutionResult = await manager.query(
                    `
                    DELETE FROM 
                        core.Institution 
                    OUTPUT 
                        DELETED."InstitutionID" AS "institutionID"
                    WHERE 
                        InstitutionID = @0
                    `,
                    [organizationID],
                );
                const transformedDeleteInstitutionResult: InstitutionResponseDTO[] =
                    InstitutionMapper.transform(deleteInstitutionResult);
                const deletedInstitutionID = transformedDeleteInstitutionResult[0].institutionID;
                if (jobRowID && updatedSysUserID && deletedInstitutionID) result = true;
            });
        } catch (error) {
            dto.errorMessage = error?.message;
        }
        return result;
    }

    private getNonSyncedTeacherUserByInstitutionID(institutionID: number) {
        const query = `
        SELECT
            DISTINCT p.personID, p.personalID, p.validFrom
        FROM
            core.Person p
        join core.EducationalState es on
            p.PersonID = es.PersonID
        join core.Position pos on
            es.PositionID = pos.PositionID
        left join core.SysUser su on
            su.PersonID = p.PersonID
        WHERE
            su.SysUserID is NULL
            and pos.PositionID = ${PositionEnum.EMPLOYEE}
            and pos.SysRoleID = ${RoleEnum.TEACHER}
            and p.PersonalID is not NULL
            and su.DeletedOn is NULL
            and es.InstitutionID = ${institutionID}
        `;
        return getManager().query(query);
    }

    private getNonSyncedStudentUserByInstitutionID(institutionID: number) {
        const query = `
        SELECT
        DISTINCT per.personID, per.personalID, per.validFrom
        FROM
            core.Person per
            LEFT JOIN core.EducationalState es ON per.PersonID = es.PersonID
            LEFT JOIN core.Position pos ON es.PositionID = pos.PositionID
            LEFT JOIN core.SysUser su ON su.PersonID = per.PersonID
            LEFT JOIN azure_temp.Users atu ON atu.Identifier = per.PersonalID
            WHERE
                su.SysUserID IS NULL
            AND
                su.DeletedOn IS NULL
            AND 
                pos.PositionID IN (${PositionEnum.STUDENT_INSTITUTION}, \
                ${PositionEnum.STUDENT_OTHER_INSTITUTION}, \
                ${PositionEnum.STUDENT_PLR}, \
                ${PositionEnum.UNATTENTDING}, \
                ${PositionEnum.SPECIAL_STUDENT})
                AND 
                    (pos.SysRoleID = ${RoleEnum.STUDENT}
                OR (
                    pos.SysRoleID IS NULL
                    AND pos.PositionID = ${PositionEnum.SPECIAL_STUDENT})
                    )
                AND per.PersonalID IS not NULL
                AND su.DeletedOn IS NULL
                AND es.InstitutionID = ${institutionID}
            `;
        return getManager().query(query);
    }

    getNonSyncedAzureUser(dto: IAzureSyncUserSearchInput) {
        const { position, institutionID } = dto;
        if (position === 'teacher') {
            return this.getNonSyncedTeacherUserByInstitutionID(institutionID);
        } else if (position === 'student') {
            return this.getNonSyncedStudentUserByInstitutionID(institutionID);
        } else {
            throw new Error(`Invalid dto param supplied.`);
        }
    }

    async getNonSyncedUsersPersonalIDsCount() {
        const result = await getManager().query(
            `
            SELECT
                COUNT( DISTINCT PersonalID ) as count
            FROM
                core.Person p
            join core.EducationalState es on
                p.PersonID = es.PersonID
            join core.Position pos on
                es.PositionID = pos.PositionID
            left join core.SysUser su on
                su.PersonID = p.PersonID
            WHERE
                su.SysUserID is NULL
                and pos.PositionID IN (${PositionEnum.EMPLOYEE}, ${PositionEnum.STUDENT_INSTITUTION}, ${PositionEnum.STUDENT_OTHER_INSTITUTION}, ${PositionEnum.STUDENT_PLR}, ${PositionEnum.UNATTENTDING}, ${PositionEnum.SPECIAL_STUDENT})
                and (pos.SysRoleID IN (${RoleEnum.STUDENT}, ${RoleEnum.TEACHER})
                    OR (pos.SysRoleID is NULL
                        and pos.PositionID = ${PositionEnum.SPECIAL_STUDENT}))
                and p.PersonalID is not NULL
                and p.PublicEduNumber is NULL
                and su.DeletedOn is NULL
            `,
        );
        return result[0].count;
    }

    async getUnemployedTeachersForDelete() {
        const result = await getManager().query(
            `
            SELECT
                DISTINCT PersonalID
            FROM
                core.Person p
            JOIN core.EducationalStateHistory esh on
                p.PersonID = esh.PersonID
            LEFT JOIN core.EducationalState es on
                p.PersonID = es.PersonID
            WHERE
                es.EducationalStateID IS NULL AND 
                esh.PositionID = @0 AND 
                esh.ValidTo <= DATEADD(year, -1, GETUTCDATE())
            `,
            [PositionEnum.EMPLOYEE],
        );
        return result;
    }

    async getUnattendingStudentsForDelete() {
        const result = await getManager().query(
            `
            SELECT
                DISTINCT PersonalID
            FROM
                core.Person p
            join core.EducationalState es on
                p.PersonID = es.PersonID
            WHERE
                es.PositionID = @0
                AND es.ValidFrom <= DATEADD(year, -1, GETUTCDATE())
            `,
            [PositionEnum.UNATTENTDING],
        );
        return result;
    }

    async getSysUserBySysUserID(sysUserID: number) {
        const result = await getManager().query(
            `
            SELECT
                su.Username AS username
            FROM
                core.SysUser su
            WHERE
                su.SysUserID = @0 AND
                su.DeletedOn is NULL
            `,
            [sysUserID],
        );
        const transformedResult: SysUserResponseDTO[] = SysUserMapper.transform(result);
        return transformedResult[0];
    }

    async getSysUserByUsername(username: string) {
        const result = await getManager().query(
            `
            SELECT
                su.Username AS username
            FROM
                core.SysUser su
            WHERE
                su.Username = @0 AND
                su.DeletedOn is NULL
            `,
            [username],
        );
        const transformedResult: SysUserResponseDTO[] = SysUserMapper.transform(result);
        return transformedResult[0];
    }

    async getSysUserByPersonID(personID: number) {
        const result = await getManager().query(
            `
            SELECT
                su.Username AS username,
                su.SysUserID AS sysUserID,
                su.IsAzureUser AS isAzureUser,
                su.PersonID AS personID
            FROM
                core.SysUser su
            WHERE
                su.PersonID = @0 AND
                su.DeletedOn is NULL
            `,
            [personID],
        );
        const transformedResult: SysUserResponseDTO[] = SysUserMapper.transform(result);
        return transformedResult[0];
    }

    async getSysUsersByPersonID(personID: number) {
        const result = await getManager().query(
            `
            SELECT
                su.SysUserID AS sysUserID,
                su.Username AS username,
                su.IsAzureUser AS isAzureUser,
                su.PersonID AS personID,
                su.isAzureSynced,
                su.DeletedOn AS deletedOn
            FROM
                core.SysUser su
            WHERE
                su.PersonID = @0
            `,
            [personID],
        );
        const transformedResult: SysUserResponseDTO[] = SysUserMapper.transform(result);
        return transformedResult;
    }

    async deleteSysUserByPersonID(personID: number, entityManager: EntityManager) {
        const manager = entityManager ? entityManager : this.entityManager;
        const updateSysUserResult = await manager.query(
            `
        UPDATE
            core.SysUser
        SET
            DeletedOn = GETUTCDATE()
        OUTPUT 
            INSERTED."SysUserID" AS "sysUserID"
        WHERE
            1=1
            AND PersonID = @0
            AND DeletedOn IS NULL
        `,
            [personID],
        );
        const transformedResult: SysUserResponseDTO[] = SysUserMapper.transform(updateSysUserResult);
        return transformedResult[0];
    }

    async deleteAzureIDByPersonID(personID: number, entityManager: EntityManager) {
        const manager = entityManager ? entityManager : this.entityManager;
        const updateSysUserResult = await manager.query(
            `
        UPDATE
            core.Person
        SET
            AzureID = NULL
        OUTPUT 
            INSERTED."PersonID" AS "personID"
        WHERE
            1=1
            AND PersonID = @0
        `,
            [personID],
        );
        const transformedResult: SysUserResponseDTO[] = SysUserMapper.transform(updateSysUserResult);
        return transformedResult[0];
    }

    async createSysUser(createUserDTO: SysUserCreateDTO, entityManager?: EntityManager) {
        const manager = entityManager ? entityManager : this.entityManager;
        const result = await manager.query(
            `            
            INSERT INTO 
            core.SysUser (
                    Username,
                    PersonID,
                    IsAzureUser,
                    IsAzureSynced
                )
            OUTPUT 
                INSERTED.SysUserID as sysUserID,
                INSERTED.Username as username,
                INSERTED.PersonID as personID
            VALUES (
                @0,
                @1,
                @2,
                @3
            );
            `,
            [createUserDTO.username, createUserDTO.personID, IsAzureUser.YES, IsAzureSynced.YES],
        );
        const transformedResult: SysUserResponseDTO[] = SysUserMapper.transform(result);
        return transformedResult[0];
    }

    async createSysUserSysRole(dto: SysUserSysRoleCreateDTO, entityManager: EntityManager) {
        const manager = entityManager ? entityManager : this.entityManager;
        const insertSysUserSysRoleResult = await manager.query(
            `
            INSERT INTO
                core.SysUserSysRole (
                    SysUserID,
                    SysRoleID
                )
            OUTPUT
                INSERTED.SysUserID as sysUserID,
                INSERTED.SysRoleID as sysRoleID,
                INSERTED.InstitutionID as institutionID,
                INSERTED.BudgetingInstitutionID as budgetingInstitutionID,
                INSERTED.MunicipalityID as municipalityID,
                INSERTED.RegionID as regionID
            VALUES (
                @0,
                @1
            );
            `,
            [dto.sysUserID, dto.sysRoleID],
        );
        const transformedInsertSysUserSysRoleResult: SysUserSysRoleResponseDTO[] =
            SysUserSysRoleMapper.transform(insertSysUserSysRoleResult);
        return transformedInsertSysUserSysRoleResult[0];
    }

    async findParents(personID: number | null, email: string | null) {
        let whereClause = '';
        let params: any[] = [];

        if (personID) {
            whereClause = 'p.PersonID = @0';
            params = [personID];
        } else if (email) {
            whereClause = "su.Username LIKE '%' + @0 + '%'";
            params = [email];
        } else {
            return [];
        }

        const query = `
            SELECT TOP ${CONSTANTS.DB_QUERY_SEARCH_USERS_TOP_VALUE}
                su.Username as username,
                CONCAT(
                    TRIM(p.FirstName), 
                    ' ', 
                    ISNULL(NULLIF(TRIM(p.MiddleName), '') + ' ', ''), 
                    TRIM(p.LastName)
                ) as fullName,
                p.PersonID as personID
            FROM core.Person p
                JOIN core.SysUser su 
                    ON su.PersonID = p.PersonID 
                    AND su.DeletedOn IS NULL
                JOIN core.SysUserSysRole sur 
                    ON sur.SysUserID = su.SysUserID
            WHERE sur.SysRoleID = ${RoleEnum.PARENT}
                AND ${whereClause}
        `;

        return this.entityManager.query(query, params);
    }

    async findStudents(personID: number | null, email: string | null) {
        let whereClause = '';
        let params: any[] = [];

        if (personID) {
            whereClause = 'person.PersonID = @0';
            params = [personID];
        } else if (email) {
            whereClause = "su.Username LIKE '%' + @0 + '%'";
            params = [email];
        } else {
            return [];
        }

        const query = `
            SELECT TOP ${CONSTANTS.DB_QUERY_SEARCH_USERS_TOP_VALUE}
                su.Username as username,
                CONCAT(
                    TRIM(person.FirstName), 
                    ' ', 
                    ISNULL(NULLIF(TRIM(person.MiddleName), '') + ' ', ''), 
                    TRIM(person.LastName)
                ) as fullName,
                person.PersonID as personID
            FROM core.Person person
                LEFT JOIN core.SysUser su 
                    ON su.PersonID = person.PersonID 
                    AND su.DeletedOn IS NULL
                LEFT JOIN core.EducationalState edu 
                    ON edu.personID = person.PersonID
                LEFT JOIN core.Position pos 
                    ON pos.positionID = edu.positionID
                LEFT JOIN core.sysRole rol 
                    ON rol.sysRoleID = pos.sysRoleID
            WHERE edu.PositionID NOT IN (2, 9)
                AND ${whereClause}
        `;

        return this.entityManager.query(query, params);
    }
}
