import { Injectable } from '@nestjs/common';
import { ParentHasAccessEnum } from 'src/common/constants/enum/parent-has-access.enum';
import { UserRoleType } from 'src/common/constants/enum/role-type-enum';
import { ParentChildAccessResponseDTO } from 'src/common/dto/responses/parent-child-access-response.dto';
import { ParentResponseDTO } from 'src/common/dto/responses/parent-response.dto';
import { RelativesAccessResponseDTO } from 'src/common/dto/responses/relatives-access-response.dto';
import { ParentChildAccessMapper } from 'src/common/mappers/parent-child-access.mapper';
import { ParentMapper } from 'src/common/mappers/parent.mapper';
import { RelativesAccessMapper } from 'src/common/mappers/relatives-access.mapper';
import { Connection, EntityManager, getManager } from 'typeorm';
import { ParentChildSchoolBookAccessesFindManyRequestDto } from '../../common/dto/requests/pcsba-find-many-request.dto';

@Injectable()
export class ParentAccessRepository {
    entityManager = getManager();

    constructor(private connection: Connection) {}

    async getPersonByParentID(parentID: number) {
        const result = await this.entityManager.query(
            `
            SELECT
                PersonID as parentID
            FROM
                "core"."Person" "p"
            WHERE
                "p"."PersonID" = @0
            `,
            [parentID],
        );
        const transformedResult: ParentResponseDTO[] = ParentMapper.transform(result);
        return transformedResult[0];
    }

    async changeAccess(parentID: number) {
        const result = await this.entityManager.query(
            `
            UPDATE
                core.ParentChildSchoolBookAccess
            SET
                HasAccess = CASE  
                    WHEN HasAccess = ${ParentHasAccessEnum.NO} THEN ${ParentHasAccessEnum.YES}
                    ELSE ${ParentHasAccessEnum.NO}
                END
            OUTPUT 
                INSERTED."ParentID" AS "parentID",
                INSERTED."HasAccess" AS "hasAccess",
                INSERTED."ChildID" AS "childID"
            WHERE
                ParentID = @0
            `,
            [parentID],
        );
        const transformedResult: ParentChildAccessResponseDTO[] = ParentChildAccessMapper.transform(result);
        return transformedResult;
    }

    async createParentAccess(parentID: number, childID: number, entityManager: EntityManager) {
        const manager = entityManager ? entityManager : this.entityManager;
        const result = await manager.query(
            `           
                  INSERT
                  INTO
                  core.ParentChildSchoolBookAccess (
                    ChildID,
                    ParentID,
                    HasAccess
                  )
                  OUTPUT Inserted.ParentChildSchoolBookAccessID as parentChildSchoolBookAccessID
                  VALUES (@0,@1,@2);
              `,
            [childID, parentID, ParentHasAccessEnum.YES],
        );
        const transformedResult: ParentChildAccessResponseDTO[] = ParentChildAccessMapper.transform(result);
        return transformedResult[0];
    }

    async upsertParentAccess(parentID: number, childID: number, hasAccess: number) {
        const result = await this.entityManager.query(
            `           
      MERGE core.ParentChildSchoolBookAccess AS TARGET
        USING ( SELECT @0 as ParentID, @1 as ChildID, @2 as HasAccess) AS SOURCE ON
        (TARGET.ParentID = SOURCE.ParentID
          AND TARGET.ChildID = SOURCE.ChildID)
      WHEN MATCHED THEN
        UPDATE
        SET
          TARGET.HasAccess = SOURCE.HasAccess
      WHEN NOT MATCHED BY TARGET THEN
        INSERT
          (ParentID,
          ChildID,
          HasAccess)
        VALUES (SOURCE.ParentID,
        SOURCE.ChildID,
        SOURCE.HasAccess)
      OUTPUT
        $action as action,
        INSERTED.ParentChildSchoolBookAccessID AS parentChildSchoolBookAccessID,
        INSERTED.HasAccess as hasAccess,
        INSERTED.ChildID as childID,
        INSERTED.ParentID as parentID;
              `,
            [parentID, childID, hasAccess],
        );
        const transformedResult: ParentChildAccessResponseDTO[] = ParentChildAccessMapper.transform(result);
        return transformedResult[0];
    }

    async getParentIDsByChildID(personID: number) {
        const result = await getManager().query(
            `            
        SELECT 
          pcsba.ParentID as parentID
        FROM
          core.ParentChildSchoolBookAccess pcsba
        WHERE pcsba.ChildID = @0
        `,
            [personID],
        );
        const transformedResult: ParentResponseDTO[] = ParentMapper.transform(result);
        return transformedResult;
    }

    async getParentChildSchoolBookAccesses(query: ParentChildSchoolBookAccessesFindManyRequestDto) {
        const { personID, userRoleType } = query;

        let whereClause = '';
        let personIdSelect = '';
        let personJoin = '';

        if (userRoleType === UserRoleType.STUDENT) {
            whereClause = 'pcsba.ChildID = @0';
            personIdSelect = 'pcsba.ParentID AS "personID"';
            personJoin = 'LEFT JOIN core.Person as person ON pcsba.ParentID = person.PersonID';
        } else if (userRoleType === UserRoleType.PARENT) {
            whereClause = 'pcsba.ParentID = @0';
            personIdSelect = 'pcsba.ChildID AS "personID"';
            personJoin = 'LEFT JOIN core.Person as person ON pcsba.ChildID = person.PersonID';
        } else {
            return [];
        }

        const result = await getManager().query(
            `            
                SELECT
                    pcsba.ParentChildSchoolBookAccessID AS "parentChildSchoolBookAccessID",
                    su.Username AS "username",
                    CONCAT(
                        TRIM(person.FirstName),
                        ' ',
                        ISNULL(NULLIF(TRIM(person.MiddleName), '') + ' ', ''),
                        TRIM(person.LastName)
                    ) AS "fullName",
                    ${personIdSelect},
                    pcsba.HasAccess AS "hasAccess"
                FROM core.ParentChildSchoolBookAccess AS pcsba
                ${personJoin}
                LEFT JOIN core.SysUser AS su
                    ON su.PersonID = person.PersonID
                    AND su.DeletedOn IS NULL
                WHERE ${whereClause}
            `,
            [personID],
        );

        const transformedResult: RelativesAccessResponseDTO[] = RelativesAccessMapper.transform(result);
        return transformedResult;
    }

    async deleteParentChildSchoolBookAccess(parentChildSchoolBookAccessID: number) {
        const result = await getManager().query(
            `
                DELETE FROM core.ParentChildSchoolBookAccess
                OUTPUT
                    DELETED.ParentChildSchoolBookAccessID AS "parentChildSchoolBookAccessID"
                WHERE ParentChildSchoolBookAccessID = @0;
            `,
            [parentChildSchoolBookAccessID],
        );

        const processedCount = result?.length || 0;

        return {
            processedCount,
        };
    }

    async hasParentChildSchoolBookAccessRecord(parentID: number, childID: number) {
        const result = await getManager().query(
            `            
        SELECT 
          pcsba.ParentChildSchoolBookAccessID as parentChildSchoolBookAccessID
        FROM
          core.ParentChildSchoolBookAccess pcsba
        WHERE pcsba.ParentID = @0
        AND pcsba.ChildID = @1
        `,
            [parentID, childID],
        );
        return result.length > 0;
    }
}
