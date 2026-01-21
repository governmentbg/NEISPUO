import { Injectable } from '@nestjs/common';
import { CronJobConfigResponseDTO } from 'src/common/dto/responses/cron-jobs-config-response.dto';
import { JobsMapper } from 'src/common/mappers/jobs.mapper';
import { Connection, EntityManager, getManager } from 'typeorm';

@Injectable()
export class JobsRepository {
    entityManager = getManager();

    constructor(private connection: Connection) {}

    async updateJob(dto: CronJobConfigResponseDTO, entityManager?: EntityManager) {
        const { jobID, name, cron, isRunning, isActive, markedForRestart } = dto;
        const manager = entityManager ? entityManager : this.entityManager;
        const jobConfigResult = await manager.query(
            `
        UPDATE
            azure_temp.CronJobConfig
        SET
            Name = @1,
            Cron = @2,
            IsRunning = @3,
            IsActive = @4,
            MarkedForRestart = @5
        OUTPUT
            INSERTED.JobID as jobID,
            INSERTED.Name as name,
            INSERTED.Cron as cron,
            INSERTED.IsRunning as isRunning,
            INSERTED.IsActive as isActive,
            INSERTED.MarkedForRestart as markedForRestart
        WHERE
            JobID = @0
        `,
            [jobID, name, cron, isRunning, isActive, markedForRestart],
        );
        const transformedResult: CronJobConfigResponseDTO[] = JobsMapper.transform(jobConfigResult);
        return transformedResult[0];
    }

    async getAllJobConfigs() {
        const jobConfigResult = await getManager().query(
            `
        SELECT
            JobID as jobID,
            Name as name,
            Cron as cron,
            IsRunning as isRunning,
            IsActive as isActive,
            MarkedForRestart as markedForRestart
        FROM
            azure_temp.CronJobConfig
        `,
        );
        const transformedResult: CronJobConfigResponseDTO[] = JobsMapper.transform(jobConfigResult);
        return transformedResult;
    }

    async getJobConfigByName(dto: CronJobConfigResponseDTO) {
        const { name } = dto;
        const jobConfigResult = await getManager().query(
            `
        SELECT
            JobID as jobID,
            Name as name,
            Cron as cron,
            IsRunning as isRunning,
            IsActive as isActive,
            MarkedForRestart as markedForRestart
        FROM
            azure_temp.CronJobConfig
        WHERE
            Name = @0
        `,
            [name],
        );
        const transformedResult: CronJobConfigResponseDTO[] = JobsMapper.transform(jobConfigResult);
        return transformedResult[0];
    }
}
