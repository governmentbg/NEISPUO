import { Injectable } from '@nestjs/common';
import { PositionEnum } from 'src/common/constants/enum/position.enum';
import { EducationalStateDTO } from 'src/common/dto/responses/educational-state.dto';
import { EducationalStateMapper } from 'src/common/mappers/educational-state.mapper';
import { getManager } from 'typeorm';

@Injectable()
export class EducationalStateRepository {
    async getUserEducationalStatesByPersonID(educationalStateDTO: EducationalStateDTO) {
        const result = await getManager().query(
            `
            SELECT DISTINCT
                es.InstitutionID as institutionID,
                es.PositionID as positionID,
                es.PersonID as personID
            FROM
                core.EducationalState es
            WHERE 
                es.PersonID = @0
            `,
            [educationalStateDTO.personID],
        );
        const transformedResult: EducationalStateDTO[] = EducationalStateMapper.transform(result);
        return transformedResult;
    }

    async getTeacherEducationalStatesByPersonID(educationalStateDTO: EducationalStateDTO) {
        const result = await getManager().query(
            `
            SELECT DISTINCT
                es.InstitutionID as institutionID,
                es.PositionID as positionID,
                es.PersonID as personID
            FROM
                core.EducationalState es
            WHERE
                es.PersonID = @0
                AND es.PositionID = ${PositionEnum.EMPLOYEE}
            `,
            [educationalStateDTO.personID],
        );
        const transformedResult: EducationalStateDTO[] = EducationalStateMapper.transform(result);
        return transformedResult;
    }

    async getUserEducationalStatesByInstituionID(educationalStateDTO: EducationalStateDTO) {
        const result = await getManager().query(
            `
            SELECT DISTINCT
                es.InstitutionID as institutionID,
                es.PositionID as positionID,
                es.PersonID as personID
            FROM
                core.EducationalState es
            WHERE 
                es.InstitutionID = @0
            `,
            [educationalStateDTO.institutionID],
        );
        const transformedResult: EducationalStateDTO[] = EducationalStateMapper.transform(result);
        return transformedResult;
    }

    async getMissingEducationalStatesForStudentInAzureTemp() {
        const result = await getManager().query(
            `
                EXEC [azure_temp].[DAILY_PROCEDURE_STUDENT_SCHOOL_ENROLLMENT_CREATE]
            `,
        );
        const transformedResult: EducationalStateDTO[] = EducationalStateMapper.transform(result);
        return transformedResult;
    }

    async getMissingEducationalStatesForTeacherInAzureTemp() {
        const result = await getManager().query(
            `
                EXEC [azure_temp].[DAILY_PROCEDURE_TEACHER_SCHOOL_ENROLLMENT_CREATE]
            `,
        );
        const transformedResult: EducationalStateDTO[] = EducationalStateMapper.transform(result);
        return transformedResult;
    }
}
