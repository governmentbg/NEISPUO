import { Injectable } from '@nestjs/common';
import { ParentHasAccessEnum } from 'src/common/constants/enum/parent-has-access.enum';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { SysUserTypeEnum } from 'src/common/constants/enum/sys-user-type.enum';
import { ParentChildrenResponseDTO } from 'src/common/dto/responses/parent-children-response.dto';
import { PersonResponseDTO } from 'src/common/dto/responses/person-response.dto';
import { PersonEntity } from 'src/common/entities/person.entity';
import { PersonMapper } from 'src/common/mappers/person.mapper';
import { Connection, EntityManager, getManager } from 'typeorm';

@Injectable()
export class PersonRepository {
    entityManager = getManager();

    constructor(private connection: Connection) {}

    async createPerson(personRequestDTO: Partial<PersonEntity>, entityManager: EntityManager) {
        const manager = entityManager ? entityManager : this.entityManager;
        const result = await manager.query(
            `           
                  INSERT
                  INTO
                  core.Person (
                    FirstName,
                    MiddleName,
                    LastName,
                    PermanentAddress,
                    PermanentTownID,
                    CurrentAddress,
                    PublicEduNumber,
                    PersonalIDType,
                    NationalityID,
                    PersonalID,
                    BirthDate,
                    BirthPlaceTownID,
                    BirthPlaceCountry,
                    SysUserType,
                    Gender
                  )
                  OUTPUT Inserted.PersonID as personID
                  VALUES (@0,@1,@2,@3,@4,@5,@6,@7,@8,@9,@10,@11,@12,@13,@14);
              `,
            [
                personRequestDTO.firstName,
                personRequestDTO.middleName,
                personRequestDTO.lastName,
                personRequestDTO.permanentAddress,
                personRequestDTO.permanentTownID,
                personRequestDTO.currentAddress,
                personRequestDTO.publicEduNumber,
                personRequestDTO.personalIDType,
                personRequestDTO.nationalityID,
                personRequestDTO.personalID,
                personRequestDTO.birthDate,
                personRequestDTO.birthPlaceTownID,
                personRequestDTO.birthPlaceCountry,
                SysUserTypeEnum.PARENT,
                personRequestDTO.gender,
            ],
        );
        const transformedResult: PersonResponseDTO[] = PersonMapper.transform(result);
        return transformedResult[0];
    }

    async getPersonByPersonalIDAndChildSchoolCode(personalID: string, code: string): Promise<PersonEntity> {
        return (
            await this.entityManager.query(
                `
            SELECT
                "person"."PersonID" as personID,
                "person"."firstName" as firstName,
                "person"."middleName" as middleName,
                "person"."lastName" as lastName,
                "person"."BirthDate" as birthDate
            FROM "core"."Person" "person"
            WHERE SchoolBooksCodesID = @0
                AND PersonalID = @1
          `,
                [code, personalID],
            )
        )[0];
    }

    async getPersonByPersonID(personID: number) {
        const result = await getManager().query(
            `            
                SELECT
					p.PersonID as personID,
					p.AzureID as azureID,
					p.PersonalID as personalID,
					p.FirstName as firstName,
					p.MiddleName as middleName,
					p.LastName as lastName,
					p.PermanentAddress as permanentAddress,
					p.PermanentTownID as permanentTownID,
					p.CurrentAddress as currentAddress,
					p.CurrentTownID as currentTownID,
					p.PublicEduNumber as publicEduNumber,
					p.PersonalIDType as personalIDType,
					p.NationalityID as nationalityID,
					p.BirthDate as birthDate,
					p.BirthPlaceTownID as birthPlaceTownID,
					p.BirthPlaceCountry as birthPlaceCountry,
					p.Gender as gender,
					p.SchoolBooksCodesID as schoolBooksCodesID,
					p.BirthPlace as birthPlace,
					p.SysUserType as sysUserType
                FROM
                    core.person p
                WHERE
                    p.PersonID = @0
            `,
            [personID],
        );
        const transformedResult: PersonResponseDTO[] = PersonMapper.transform(result);
        return transformedResult[0];
    }

    async getPersonByPersonalID(personalID: string) {
        const result = await getManager().query(
            `            
                SELECT
                    p.PersonID as personID,
                    p.PersonalID as personalID
                FROM
                    core.person p
                WHERE
                    p.PersonalID = @0
            `,
            [personalID],
        );
        const transformedResult: PersonResponseDTO[] = PersonMapper.transform(result);
        return transformedResult[0];
    }

    async getPersonByInstitutionID(institutionID: number) {
        const result = await getManager().query(
            `            
            SELECT
                p.PersonID as personID,
                p.PersonalID as personalID
            FROM
                core.Institution ins		                
            LEFT JOIN core.SysUserSysRole susr ON
                susr.InstitutionID = ins.InstitutionID
            LEFT JOIN core.SysUser u ON
                u.SysUserID= susr.SysUserID
            LEFT JOIN core.Person p ON
                p.personID = u.personID
            LEFT JOIN core.sysRole r ON
                r.sysRoleID = susr.sysRoleID
            WHERE
                ins.InstitutionID = @0 AND
                r.sysRoleId = ${RoleEnum.INSTITUTION} AND
                u.DeletedOn is NULL
            `,
            [institutionID],
        );
        const transformedResult: PersonResponseDTO[] = PersonMapper.transform(result);
        return transformedResult[0];
    }

    async getPersonChildren(parentID: number): Promise<ParentChildrenResponseDTO[]> {
        return getManager().query(
            `            
                SELECT
                    p.PersonID as personID,
                    p.FirstName as firstName,
                    p.MiddleName as middleName,
                    p.LastName as lastName,
                    p.PersonalID as personalID,
                    p.SchoolBooksCodesID as schoolBooksCodes
                FROM
                    core.person p
                JOIN core.ParentChildSchoolBookAccess acc ON p.PersonID = acc.ChildID
                WHERE
                    acc.ParentID = @0
                AND acc.HasAccess = ${ParentHasAccessEnum.YES}
        `,
            [parentID],
        );
    }

    async getPersonByUserName(username: string) {
        const result = await getManager().query(
            `            
                SELECT
                    p.PersonID as personID,
                    p.PersonalID as personalID
                FROM
                    core.person p
                JOIN core.SysUser s ON s.PersonID = p.PersonID
                WHERE
                    s.Username = @0 AND
                    s.DeletedOn is NULL
            `,
            [username],
        );
        const transformedResult: PersonResponseDTO[] = PersonMapper.transform(result);
        return transformedResult[0];
    }

    async updatePublicEduNumberByPersonID(dto: PersonResponseDTO) {
        const { personID, publicEduNumber } = dto;
        const updateSysUserResult = await getManager().query(
            `
        UPDATE
            core.Person
        SET
            PublicEduNumber = @1
        OUTPUT 
            INSERTED."PersonID" AS "personID"
        WHERE
            PersonID = @0
        `,
            [personID, publicEduNumber],
        );
        const transformedResult: PersonResponseDTO[] = PersonMapper.transform(updateSysUserResult);
        return transformedResult[0];
    }

    async updateAzureIDByPersonID(dto: PersonResponseDTO) {
        const { personID, azureID } = dto;
        const updateSysUserResult = await getManager().query(
            `
        UPDATE
            core.Person
        SET
            AzureID = @1
        OUTPUT 
            INSERTED."PersonID" AS "personID"
        WHERE
            PersonID = @0
        `,
            [personID, azureID],
        );
        const transformedResult: PersonResponseDTO[] = PersonMapper.transform(updateSysUserResult);
        return transformedResult[0];
    }

    async updateAzureIDByPublicEduNumber(dto: PersonResponseDTO) {
        const { publicEduNumber, azureID } = dto;
        const updateSysUserResult = await getManager().query(
            `
        UPDATE
            core.Person
        SET
            AzureID = @1
        OUTPUT 
            INSERTED."PersonID" AS "personID"
        WHERE
            PublicEduNumber = @0
        `,
            [publicEduNumber, azureID],
        );
        const transformedResult: PersonResponseDTO[] = PersonMapper.transform(updateSysUserResult);
        return transformedResult[0];
    }
}
