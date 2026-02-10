import { CronJobConfigResponseDTO } from '../dto/cron-jobs-config-response.dto';

export class JobsMapper {
  static transform(personObjects: any[]) {
    const result: CronJobConfigResponseDTO[] = [];
    for (const jobsObject of personObjects) {
      const elementToBeInserted: CronJobConfigResponseDTO = {
        jobID: jobsObject.jobID,
        name: jobsObject.name,
        cron: jobsObject.cron,
        isRunning: jobsObject.isRunning,
        isActive: jobsObject.isActive,
        markedForRestart: jobsObject.markedForRestart,
      };
      result.push(elementToBeInserted);
    }
    return result;
  }
}
