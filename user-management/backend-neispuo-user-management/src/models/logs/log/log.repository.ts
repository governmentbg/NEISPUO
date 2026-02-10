import { Injectable } from '@nestjs/common';
import { AuditModuleEnum } from 'src/common/constants/enum/audit-module.enum';
import { LogLevelEnum } from 'src/common/constants/enum/log-level.enum';
import { LogEntity } from 'src/common/entities/log.entity';
import { LogMapper } from 'src/common/mappers/log.mapper';
import { Connection, getManager } from 'typeorm';

@Injectable()
export class LogRepository {
    entityManager = getManager();

    constructor(private connection: Connection) {}

    async insertLog(logDTO: LogEntity) {
        const { auditModuleId, message, messageTemplate, level, exception, logEvent } = logDTO;
        const result = await this.entityManager.query(
            `           
                INSERT
                INTO
                logs.Log (
                    AuditModuleId,
                    Message,
                    MessageTemplate,
                    Level,
                    Exception,
                    LogEvent,
                    Timestamp
                )
                OUTPUT Inserted.ID as id
                VALUES (@0,@1,@2,@3,@4,@5,GETUTCDATE());
            `,
            [auditModuleId, message, messageTemplate, level, exception, logEvent],
        );
        const transformedResult: LogEntity[] = LogMapper.transform(result);
        return transformedResult[0];
    }

    async getLastWeekLogs() {
        const result = await this.entityManager.query(
            `           
            SELECT
                *
            FROM
                logs.Log l
            WHERE
                AuditModuleId = ${AuditModuleEnum.USER_MANAGEMENT}
                AND [Level] = '${LogLevelEnum.ERROR}'
                AND l.TimeStamp > DATEADD(DAY, -7, GETUTCDATE())
            `,
        );
        const transformedResult: LogEntity[] = LogMapper.transform(result);
        return transformedResult;
    }
}
