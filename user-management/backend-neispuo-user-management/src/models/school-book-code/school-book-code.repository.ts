import { Injectable } from '@nestjs/common';
import { SchoolBookCodeAssignResponseDTO } from 'src/common/dto/requests/school-books-code-assign-response.dto';
import { ParentChildAccessResponseDTO } from 'src/common/dto/responses/parent-child-access-response.dto';
import { ParentChildAccessMapper } from 'src/common/mappers/parent-child-access.mapper';
import { SchoolBookCodeMapper } from 'src/common/mappers/school-book-code.mapper';
import { Connection, EntityManager, getManager } from 'typeorm';

@Injectable()
export class SchoolBookCodeRepository {
    entityManager = getManager();

    constructor(private connection: Connection) {}

    async assignSchoolBookCode(dto: SchoolBookCodeAssignResponseDTO, entityManager?: EntityManager) {
        const { personID, code } = dto;
        const manager = entityManager ? entityManager : this.entityManager;
        const result = await manager.query(
            `
            UPDATE
                core.Person
            SET
                SchoolBooksCodesID = @1
            OUTPUT 
                INSERTED."SchoolBooksCodesID" AS "code",
                INSERTED."PersonID" AS "personID"
            WHERE
                PersonID = @0
            `,
            [personID, code],
        );
        const transformedResult: SchoolBookCodeAssignResponseDTO[] = SchoolBookCodeMapper.transform(result);
        return transformedResult[0];
    }

    async deleteChildCodeByPersonID(dto: SchoolBookCodeAssignResponseDTO, entityManager?: EntityManager) {
        const { personID } = dto;
        const manager = entityManager ? entityManager : this.entityManager;
        const result = await manager.query(
            `
            DELETE
            FROM
                core.ParentChildSchoolBookAccess
            OUTPUT 
                DELETED."ChildID" AS "personID",
                DELETED."ParentChildSchoolBookAccessID" AS "parentChildSchoolBookAccessID",
                DELETED."ParentID" AS "parentID"
            WHERE
                ChildID = @0
            `,
            [personID],
        );
        const transformedResult: ParentChildAccessResponseDTO[] = ParentChildAccessMapper.transform(result);
        return transformedResult;
    }
}
