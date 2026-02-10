import { Injectable } from '@nestjs/common';
import { Paging } from 'src/common/dto/paging.dto';
import { SysUserResponseDTO } from 'src/common/dto/responses/sys-user.response.dto';
import { SysUserMapper } from 'src/common/mappers/sys-user.mapper';
import { Connection, getManager } from 'typeorm';

@Injectable()
export class SyncAzureEnrollmentsRepository {
    constructor(private connection: Connection) {}

    entityManager = getManager();

    async getStudentTeacherUsersCount() {
        const result = await getManager().query(
            `            
                SELECT
                    COUNT(DISTINCT su.SysUserID) as count
                FROM
                    core.SysUser su
                JOIN azure_temp.StudentTeacherUsers stu ON su.SysUserID = stu.SysUserID
                WHERE su.DeletedOn is NULL
                AND su.Username IS NOT NULL
                AND stu.PersonalID IS NOT NULL
            `,
        );

        return result[0].count;
    }

    async getPaginatedStudentTeacherUsers(paging: Paging) {
        const result = await this.entityManager.query(
            `
            SELECT DISTINCT
                stu.PersonID as "personID",
                stu.PersonalID as "personalID",
                su.Username as "username"
            FROM
                core.SysUser su
            JOIN azure_temp.StudentTeacherUsers stu ON su.SysUserID = stu.SysUserID
            WHERE su.DeletedOn is NULL
            AND su.Username IS NOT NULL
            AND stu.PersonalID IS NOT NULL
            ORDER BY
            "stu"."personID" ASC OFFSET ${paging.from} ROWS FETCH NEXT ${paging.numberOfElements} ROWS ONLY
            `,
        );

        const transformedResult: SysUserResponseDTO[] = SysUserMapper.transform(result);
        return transformedResult;
    }
}
