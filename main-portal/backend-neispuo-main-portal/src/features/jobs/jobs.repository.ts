import { Injectable } from '@nestjs/common';
import { CronJobConfigResponseDTO } from 'src/shared/dto/cron-jobs-config-response.dto';
import { JobsMapper } from 'src/shared/mappers/jobs.mapper';
import { Connection, getManager } from 'typeorm';

@Injectable()
export class JobsRepository {
  entityManager = getManager();

  constructor(private connection: Connection) {}

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
    const transformedResult: CronJobConfigResponseDTO[] = JobsMapper.transform(
      jobConfigResult,
    );
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
    const transformedResult: CronJobConfigResponseDTO[] = JobsMapper.transform(
      jobConfigResult,
    );
    return transformedResult[0];
  }
}
