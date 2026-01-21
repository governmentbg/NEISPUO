// mappers are used to convert one object to another

import { CronJobConfigResponseDTO } from '../dto/responses/cron-jobs-config-response.dto';

// in out case we will be ising them to convert the database response to an appropriate DTO
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
